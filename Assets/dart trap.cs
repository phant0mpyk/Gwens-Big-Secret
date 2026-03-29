using UnityEngine;

public class DartTrap2D : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float detectionRange = 8f;
    public float fireCooldown = 2f;
    public LayerMask detectionLayer;

    private float nextFireTime;

    void Update()
    {
        // Shoot the ray in the direction the firePoint is facing (Right)
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right, detectionRange, detectionLayer);

        // Visualize the ray in the Scene View
        Debug.DrawRay(firePoint.position, firePoint.right * detectionRange, Color.red);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player") && Time.time >= nextFireTime)
            {
                FireArrow();
                nextFireTime = Time.time + fireCooldown;
            }
        }
    }

    void FireArrow()
    {
        if (arrowPrefab != null)
        {
            Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
        }
    }
}