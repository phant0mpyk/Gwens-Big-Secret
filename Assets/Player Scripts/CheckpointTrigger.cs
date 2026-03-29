using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [Header("Visuals")]
    public Sprite activeSprite; // The art to show when touched
    
    private bool isActivated = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Grab the SpriteRenderer component so we can change the image later
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the donkey walks through, and it isn't activated yet
        if (!isActivated && collision.CompareTag("Player"))
        {
            DonkeyCrankMovement donkey = collision.GetComponent<DonkeyCrankMovement>();
            if (donkey != null)
            {
                // 1. Save the respawn point
                donkey.UpdateRespawnPoint(transform.position);
                
                // 2. Lock it so it doesn't trigger again
                isActivated = true;
                
                // 3. Swap the sprite to your "On" art!
                if (activeSprite != null)
                {
                    spriteRenderer.sprite = activeSprite;
                }
            }
        }
    }
}