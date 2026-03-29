using UnityEngine;
using System.Collections;

public class DudukPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 raisedOffset = new Vector3(0, 5f, 0);
    public float moveSpeed = 3f;
    public float activeDuration = 5f;

    [Header("Proximity Settings")]
    public Transform playerTransform; // Drag the Player object here in Inspector
    public float triggerDistance = 10f; // Only activates if player is within 10 units

    private Vector3 loweredPos;
    private Vector3 raisedPos;
    private bool isActive = false;

    void Start()
    {
        loweredPos = transform.position;
        raisedPos = loweredPos + raisedOffset;

        // Auto-find player if not assigned (optional)
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }
    }

    void OnEnable() => DonkeyCrankMovement.OnDudukPlayed += TryActivate;
    void OnDisable() => DonkeyCrankMovement.OnDudukPlayed -= TryActivate;

    void TryActivate()
    {
        // Only run if not already moving
        if (isActive) return;

        // Check distance between player and platform
        float dist = Vector3.Distance(transform.position, playerTransform.position);

        if (dist <= triggerDistance)
        {
            StartCoroutine(MoveRoutine());
        }
    }

    IEnumerator MoveRoutine()
    {
        isActive = true;

        // 1. Move UP
        while (Vector3.Distance(transform.position, raisedPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, raisedPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // 2. Wait at the top
        yield return new WaitForSeconds(activeDuration);

        // 3. Move DOWN
        while (Vector3.Distance(transform.position, loweredPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, loweredPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        isActive = false;
    }

    // Visual aid in the Editor to see the activation range
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
    }
}