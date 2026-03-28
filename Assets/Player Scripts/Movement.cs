using UnityEngine;

public class DonkeyCrankMovement : MonoBehaviour
{
    [Header("Attachments")]
    public Animator anim;
    public SpriteRenderer donkeySprite; // Drag the donkey's SpriteRenderer here
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

        // 2. Ground Detection
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 3. Animation Logic
        if (anim != null)
        {
            float currentHorizontalSpeed = Mathf.Abs(rb.linearVelocity.x);

            // 1. Tell the animator how fast we are moving for transitions
            anim.SetFloat("Speed", currentHorizontalSpeed);
            anim.SetBool("isGrounded", isGrounded);

            // 2. Adjust the Animation Playback Speed
            // We divide by a 'base' speed so at 5 units/sec, he walks at 1x speed.
            // Adjust the '5f' until the hooves stop sliding on the floor.
            float animationSpeedMultiplier = currentHorizontalSpeed / 5f;

            // Clamp it so he doesn't move at 0 speed or lightning speed
            anim.speed = Mathf.Clamp(animationSpeedMultiplier, 0.5f, 2.0f);
        }

        // 4. Sprite Flipping (Face the direction of movement)
        if (donkeySprite != null && Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            // If moving right (positive x), flipX = false. If left, flipX = true.
            // Note: This depends on which way your original sprite faces.
            donkeySprite.flipX = rb.linearVelocity.x < 0;
        }

        // 5. Jumping
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // 6. Physics/Rope Visuals
        if (carrotObject != null)
        {
            Vector2 directionToCarrot = carrotObject.position - carrotPivot.position;
            float carrotAngleRad = Mathf.Atan2(directionToCarrot.y, directionToCarrot.x);
            targetMoveSpeed = Mathf.Cos(carrotAngleRad) * maxSpeed;

            carrotObject.rotation = Quaternion.identity;

            if (ropeRenderer != null && stickTip != null)
            {
                ropeRenderer.SetPosition(0, stickTip.position);
                ropeRenderer.SetPosition(1, carrotObject.position);
            }
        }
    }

    void FixedUpdate()
    {
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