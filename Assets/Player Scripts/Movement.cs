using UnityEngine;

public class DonkeyCrankMovement : MonoBehaviour
{
    [Header("Attachments")]
    public Animator anim;
    public SpriteRenderer donkeySprite;
    public Transform carrotPivot;
    public Transform stickTip;
    public Transform carrotObject;
    public LineRenderer ropeRenderer;

    [Header("Movement Settings")]
    public Vector3 pivotOffset = new Vector3(0, 1.5f, 0);
    public float mouseSensitivity = 5f;
    public float maxSpeed = 10f;
    public float acceleration = 50f;
    public float brakeDeceleration = 100f; // Fast stop when braking

    [Header("Jump Charge Settings")]
    public float minJumpForce = 5f;
    public float maxJumpForce = 20f;
    public float maxChargeTime = 1.0f; // Seconds to full power
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private float currentAngle = 90f;
    private float targetMoveSpeed;
    private bool isGrounded;
    private float jumpChargeTimer = 0f;

    // --- NEW VARIABLES FOR ICE & MUD ---
    private float currentGroundFriction = 0f;
    private bool isOnCustomMaterial = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Cursor.lockState = CursorLockMode.Locked;
        if (ropeRenderer != null) ropeRenderer.positionCount = 2;
    }

    void Update()
    {
        // 1. Stick Logic (Always active)
        float mouseX = Input.GetAxis("Mouse X");
        currentAngle -= mouseX * mouseSensitivity;
        currentAngle = Mathf.Clamp(currentAngle, 0f, 180f);
        carrotPivot.position = transform.position + pivotOffset;
        carrotPivot.rotation = Quaternion.Euler(0, 0, currentAngle - 90f);

        // 2. Ground Detection
        // 2. Ground Detection
        Collider2D groundHit = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isGrounded = groundHit != null;

        if (isGrounded && groundHit.sharedMaterial != null)
        {
            currentGroundFriction = groundHit.sharedMaterial.friction;
            isOnCustomMaterial = true;
        }
        else
        {
            currentGroundFriction = 0f;
            isOnCustomMaterial = false;
        }
        // 3. BRAKE & JUMP LOGIC
        // If holding Space while on ground: Stop and Charge
        if (isGrounded && Input.GetButton("Jump"))
        {
            targetMoveSpeed = 0;
            jumpChargeTimer += Time.deltaTime;
            jumpChargeTimer = Mathf.Min(jumpChargeTimer, maxChargeTime);

            if (anim != null) anim.SetBool("isCharging", true); // Optional parameter
        }
        // If let go of Space: Release Jump
        else if (isGrounded && Input.GetButtonUp("Jump"))
        {
            float chargePct = jumpChargeTimer / maxChargeTime;
            float finalJumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargePct);

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, finalJumpForce);

            jumpChargeTimer = 0; // Reset
            if (anim != null)
            {
                anim.SetBool("isCharging", false);
                anim.SetTrigger("doJump"); // Trigger jump animation on release
            }
        }
        // Normal movement
        else
        {
            jumpChargeTimer = 0;
            if (anim != null) anim.SetBool("isCharging", false);

            if (carrotObject != null)
            {
                Vector2 directionToCarrot = carrotObject.position - carrotPivot.position;
                float carrotAngleRad = Mathf.Atan2(directionToCarrot.y, directionToCarrot.x);
                targetMoveSpeed = Mathf.Cos(carrotAngleRad) * maxSpeed;
            }
        }

        // 4. Animation & Sprite Logic
        if (anim != null)
        {
            float currentHorizontalSpeed = Mathf.Abs(rb.linearVelocity.x);
            anim.SetFloat("Speed", currentHorizontalSpeed);
            anim.SetBool("isGrounded", isGrounded);

            // Playback speed logic
            if (isGrounded && !Input.GetButton("Jump"))
            {
                float animationSpeedMultiplier = currentHorizontalSpeed / 5f;
                anim.speed = Mathf.Clamp(animationSpeedMultiplier, 0.5f, 2.0f);
            }
            else
            {
                anim.speed = 1.0f; // Reset speed for Jump/Idle/Charge
            }
        }

        if (donkeySprite != null && Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            donkeySprite.flipX = rb.linearVelocity.x < 0;
        }

        // 5. Rope Visuals
        if (carrotObject != null)
        {
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
       // --- NEW SLIDING LOGIC IN FIXED UPDATE ---
        float activeAccel = acceleration;
        float activeDecel = brakeDeceleration;
        float activeTargetSpeed = targetMoveSpeed;

        if (isOnCustomMaterial)
        {
            if (currentGroundFriction <= 0.1f) // ICE 
            {
                activeAccel = acceleration * 0.2f; // Takes longer to speed up on ice
                activeDecel = 2f;                  // Barely stops at all, causing a slide!
            }
            else if (currentGroundFriction >= 1.0f) // MUD
            {
                activeAccel = acceleration * 0.4f; // Sluggish to start moving
                activeTargetSpeed *= 0.5f;         // Max speed is cut in half
                // Decel stays high so they stop instantly in the mud
            }
        }

        // Calculate final movement
        float currentAccel = (activeTargetSpeed == 0) ? activeDecel : activeAccel;
        float velocityX = Mathf.MoveTowards(rb.linearVelocity.x, activeTargetSpeed, currentAccel * Time.fixedDeltaTime);
        
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