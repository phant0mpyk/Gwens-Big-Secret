using System.Collections;
using UnityEngine;

public class RetroCeilingTrap : MonoBehaviour
{
    [Header("Art & Hitboxes")]
    public SpriteRenderer spikeRenderer;
    public Sprite retractedSprite;
    public Sprite extendedSprite;
    public Collider2D lethalCollider; // The hitbox that kills the donkey

    [Header("Timing")]
    public float warningDelay = 0.2f; // The "Click" before it fires
    public float extendedDuration = 1.0f; // How long it stays out

    private bool isTriggered = false;

    void Start()
    {
        // Make sure it starts hidden and safe!
        spikeRenderer.sprite = retractedSprite;
        lethalCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Donkey walked into the sensor!
        if (!isTriggered && collision.CompareTag("Player"))
        {
            StartCoroutine(FireTrapSequence());
        }
    }

    private IEnumerator FireTrapSequence()
    {
        isTriggered = true;

        // 1. THE WARNING
        // Wait a fraction of a second so the player realizes they messed up
        yield return new WaitForSeconds(warningDelay);

        // 2. THE SNAP!
        // Instantly swap the art and turn on the death hitbox
        spikeRenderer.sprite = extendedSprite;
        lethalCollider.enabled = true;

        // 3. STAY EXTENDED
        yield return new WaitForSeconds(extendedDuration);

        // 4. RETRACT & RESET
        // Swap the art back to hidden, and turn the death hitbox off
        spikeRenderer.sprite = retractedSprite;
        lethalCollider.enabled = false;
        
        isTriggered = false; // Ready to trap again!
    }
}