using UnityEngine;

public class DonkeyCrankMovement : MonoBehaviour
{
    [Header("Attachments")]
    public Transform carrotPivot;
    public Transform stickTip;
    public Transform carrotObject;
    public LineRenderer ropeRenderer;

    [Header("Movement Settings")]
    public Vector3 pivotOffset = new Vector3(0, 1.5f, 0);
    public float mouseSensitivity = 5f;
    public float maxSpeed = 10f;
    public float acceleration = 50f;

    [Header("Jump Settings")]
    public float jumpForce = 12f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private float currentAngle = 90f;
    private float targetMoveSpeed;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Cursor.lockState = CursorLockMode.Locked;

        if (ropeRenderer != null) ropeRenderer.positionCount = 2;
    }

    void Update()
    {
        // 1. Stick & Carrot Logic
        float mouseX = Input.GetAxis("Mouse X");
        currentAngle -= mouseX * mouseSensitivity;
        currentAngle = Mathf.Clamp(currentAngle, 0f, 180f);

        carrotPivot.position = transform.position + pivotOffset;
        carrotPivot.rotation = Quaternion.Euler(0, 0, currentAngle - 90f);

        // 2. Jumping
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (carrotObject != null)
        {
            // OBSERVE: Calculate speed based on carrot distance from pivot
            Vector2 directionToCarrot = carrotObject.position - carrotPivot.position;
            float carrotAngleRad = Mathf.Atan2(directionToCarrot.y, directionToCarrot.x);
            targetMoveSpeed = Mathf.Cos(carrotAngleRad) * maxSpeed;

            carrotObject.rotation = Quaternion.identity;

            // DRAW: Set rope positions (REVERTED to your preferred simple mapping)
            if (ropeRenderer != null && stickTip != null)
            {
                ropeRenderer.SetPosition(0, stickTip.position);
                ropeRenderer.SetPosition(1, carrotObject.position);
            }
        }
    }

    void FixedUpdate()
    {
        // Apply velocity only on X, preserving the current Y (falling/jumping)
        float velocityX = Mathf.MoveTowards(rb.linearVelocity.x, targetMoveSpeed, acceleration * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(velocityX, rb.linearVelocity.y);
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (carrotPivot != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 center = transform.position + pivotOffset;
            float radius = 2f;
            for (int i = 0; i <= 180; i += 10)
            {
                float rad = i * Mathf.Deg2Rad;
                Vector3 pos = center + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;
                Gizmos.DrawSphere(pos, 0.05f);
            }
        }
    }
}