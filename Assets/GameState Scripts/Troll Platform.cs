using UnityEngine;

public class BetrayalPlatform : MonoBehaviour
{
    [Header("Settings")]
    public int myTrollLevel = 0;
    public float spinSpeed = 400f;
    public float deathYThreshold = -5f;

    [Header("Timing")]
    public float delayBeforeSpin = 2.0f;
    public float spinDuration = 2.0f;

    private bool isPlayerOnTop = false;
    private bool hasContributedToRage = false;
    private float standingTimer = 0f;
    private float spinTimer = 0f;
    private bool hasFinishedSpinning = false;
    private Transform playerTransform;

    // NEW: Variable to store the "Safe" rotation
    private Quaternion originalRotation;

    void Start()
    {
        // Store the rotation exactly as it is in the editor
        originalRotation = transform.rotation;
    }

    void Update()
    {
        // 1. Is the game angry enough?
        if (TrollGlobal.Level >= myTrollLevel && isPlayerOnTop)
        {
            if (!hasFinishedSpinning)
            {
                standingTimer += Time.deltaTime;

                // 2. Start Spinning
                if (standingTimer >= delayBeforeSpin)
                {
                    transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
                    spinTimer += Time.deltaTime;

                    // 3. Stop and RESET rotation
                    if (spinTimer >= spinDuration)
                    {
                        hasFinishedSpinning = true;
                        transform.rotation = originalRotation; // SNAP back to normal
                        Debug.Log("Platform reset to original rotation.");
                    }
                }
            }

            // 4. Check for the fall
            if (playerTransform != null && playerTransform.position.y < deathYThreshold && !hasContributedToRage)
            {
                TrollGlobal.Level++;
                hasContributedToRage = true;
                Debug.Log("Rage Level Increased! Now: " + TrollGlobal.Level);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnTop = true;
            playerTransform = collision.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Reset logic: if they jump off before the spin happens, reset the timer
            if (collision.transform.position.y > transform.position.y && !hasFinishedSpinning && spinTimer <= 0)
            {
                isPlayerOnTop = false;
                standingTimer = 0f;
            }

            // Note: We don't set isPlayerOnTop = false if they fall (Y < platform Y)
            // so that the deathYThreshold check can still run.
        }
    }
}