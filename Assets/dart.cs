using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Arrow2D : MonoBehaviour
{
    public float speed = 15f;
    public float knockbackForce = 30f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Use transform.right because in 2D, 'Right' is the red arrow (forward)
        rb.linearVelocity = transform.right * speed;

        // This stops the arrow from spinning like a crazy top
        rb.freezeRotation = true;

        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                // Push player away from the arrow
                Vector2 pushDir = (collision.transform.position - transform.position).normalized;
                playerRb.AddForce(new Vector2(pushDir.x, 0.5f) * knockbackForce, ForceMode2D.Impulse);
            }
        }
        Destroy(gameObject);
    }
}