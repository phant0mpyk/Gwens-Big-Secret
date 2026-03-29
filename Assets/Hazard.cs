using UnityEngine;

public class Hazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DonkeyCrankMovement donkey = collision.GetComponent<DonkeyCrankMovement>();
            if (donkey != null)
            {
                donkey.Die(); // Execute the donkey!
            }
        }
    }
}