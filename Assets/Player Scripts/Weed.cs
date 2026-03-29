using UnityEngine;

public class Weed : MonoBehaviour
{
    [Header("Effect Settings")]
    public float invertDuration = 10f;
    
    [Header("Visuals")]
    public GameObject badPoofParticle; // Optional: a puff of purple smoke!

    [Header("Epic Visuals")]
    public float floatAmplitude = 0.25f; // How high it bobs
    public float floatFrequency = 2f;    // How fast it bobs
    
    [Header("Collection Effects")]
    public GameObject collectParticlePrefab; // The *POOF* when grabbed

    private Vector3 startPos;

    private void Start()
    {
        // Remember where we placed it in the level so it bobs around that point
        startPos = transform.position;
    }

    void Update()
    {
        // The magic floating math!
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {   
            CollectYogurt();
            DonkeyCrankMovement donkey = collision.GetComponent<DonkeyCrankMovement>();
            if (donkey != null)
            {
                // Poison the donkey!
                donkey.InvertMovement(invertDuration);
                
                // Spawn the particle effect if you have one
                if (badPoofParticle != null)
                {
                    Instantiate(badPoofParticle, transform.position, Quaternion.identity);
                }

                // Delete the collectible from the screen
                Destroy(gameObject);
            }
        }
    }

    void CollectYogurt()
    {
        // 1. Spawn the epic sparkles (if we assigned a prefab)
        if (collectParticlePrefab != null)
        {
            Instantiate(collectParticlePrefab, transform.position, Quaternion.identity);
        }

        // 2. Tell the game we got it (You can link your score UI here later!)
        Debug.Log("EPIC YOGURT OBTAINED!");

        // 3. Destroy the yogurt cup so it disappears from the level
        Destroy(gameObject);

    }
}