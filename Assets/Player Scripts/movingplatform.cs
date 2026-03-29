using UnityEngine;
using System.Collections;

public class DudukPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 raisedOffset = new Vector3(0, 5f, 0); // How far up it goes
    public float moveSpeed = 3f;
    public float activeDuration = 5f; // How long it stays up

    private Vector3 loweredPos;
    private Vector3 raisedPos;
    private bool isActive = false;

    void Start()
    {
        loweredPos = transform.position;
        raisedPos = loweredPos + raisedOffset;
    }

    // Subscribe to the Donkey's broadcast when this object exists
    void OnEnable() => DonkeyCrankMovement.OnDudukPlayed += ActivatePlatform;
    void OnDisable() => DonkeyCrankMovement.OnDudukPlayed -= ActivatePlatform;

    void ActivatePlatform()
    {
        if (!isActive) StartCoroutine(MoveRoutine());
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
}