using UnityEngine;

public class DudukCollectible : MonoBehaviour
{
    [Header("Epic Visuals")]
    public float floatAmplitude = 0.25f; // How high it bobs
    public float floatFrequency = 2f;    // How fast it bobs
    public bool dudukObtained = false;

    [Header("Collection Effects")]
    public GameObject collectParticlePrefab; // The *POOF* when grabbed

    private Vector3 startPos;

    void Start()
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
            // Get the movement script from the object that hit the trigger
            DonkeyCrankMovement movement = collision.GetComponent<DonkeyCrankMovement>();

            if (movement != null)
            {
                movement.hasDuduk = true; // Set the bool on the DONKEY
                CollectYogurt(); // Now destroy the collectible
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
        Debug.Log("EPIC DUDUK OBTAINED!");
        dudukObtained = true;

        // 3. Destroy the yogurt cup so it disappears from the level
        Destroy(gameObject);
    }
}