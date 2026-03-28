using UnityEngine;

public class SnakeHazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Did the donkey just walk into our Danger Zone?
        if (collision.CompareTag("Player"))
        {
            DonkeyCrankMovement donkey = collision.GetComponent<DonkeyCrankMovement>();
            if (donkey != null)
            {
                donkey.SpookDonkey(transform.position); // BOO!
            }
        }
    }
}