using UnityEngine;

public class HouseMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get input from WASD or Arrow Keys
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Normalize movement so walking diagonally isn't faster
        movement = movement.normalized;
    }

    void FixedUpdate()
    {
        // Apply the movement to the physics body
        rb.linearVelocity = movement * moveSpeed; 
    }
}