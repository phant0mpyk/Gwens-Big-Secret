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
    public float brakeDeceleration = 10f;

    [Header("Jump Charge Settings")]
    public float minJumpForce = 5f;
    public float maxJumpForce = 20f;
    public float maxChargeTime = 1.0f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Scare Settings")]
    public float scareSpeed = 15f;
    public float scareDuration = 2f;
    private bool isScared = false;
    private float scareTimer = 0f;
    private float scareDirection = 1f;

    private Rigidbody2D rb;
    private float currentAngle = 90f;
    private float targetMoveSpeed;
    private bool isGrounded;
    private float jumpChargeTimer = 0f;

    // --- ICE & MUD VARIABLES ---
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
        // 1. Stick Logic
        float mouseX = Input.GetAxis("Mouse X");
        currentAngle -= mouseX * mouseSensitivity;
        currentAngle = Mathf.Clamp(currentAngle, 0f, 180f);
        carrotPivot.position = transform.position + pivotOffset;
        carrotPivot.rotation = Quaternion.Euler(0, 0, currentAngle - 90f);

        // 2. Ground Detection
        Collider2D groundHit = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isGrounded = groundHit != null;

        if (isGrounded && groundHit != null && groundHit.sharedMaterial != null)
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
        if (isGrounded && Input.GetButton("Jump"))
        {
            targetMoveSpeed = 0;
            jumpChargeTimer += Time.deltaTime;
            jumpChargeTimer = Mathf.Min(jumpChargeTimer, maxChargeTime);

            if (anim != null) anim.SetBool("isCharging", true);
        }
        else if (isGrounded && Input.GetButtonUp("Jump"))
        {
            float chargePct = jumpChargeTimer / maxChargeTime;
            float finalJumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargePct);

            // --- MUD JUMP PENALTY ---
            if (isOnCustomMaterial && currentGroundFriction >= 1.0f)
            {
                finalJumpForce *= 0.5f;
            }

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, finalJumpForce);

            jumpChargeTimer = 0;
            if (anim != null)
            {
                anim.SetBool("isCharging", false);
                anim.SetTrigger("doJump");
            }
        }
        else if (!isScared)
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

        // Scare Logic
        if (isScared)
        {
            scareTimer -= Time.deltaTime;
            targetMoveSpeed = scareDirection * scareSpeed;
            if (scareTimer <= 0) isScared = false;
        }

        // 4. Animation & Sprite Logic
        if (anim != null)
        {
            float currentHorizontalSpeed = Mathf.Abs(rb.linearVelocity.x);
            anim.SetFloat("Speed", currentHorizontalSpeed);
            anim.SetBool("isGrounded", isGrounded);

            if (isGrounded && !Input.GetButton("Jump"))
            {
                float animationSpeedMultiplier = currentHorizontalSpeed / 5f;
                anim.speed = Mathf.Clamp(animationSpeedMultiplier, 0.5f, 2.0f);
            }
            else
            {
                anim.speed = 1.0f;
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
        float activeAccel = acceleration;
        float activeDecel = brakeDeceleration;
        float activeTargetSpeed = targetMoveSpeed;

        if (isOnCustomMaterial)
        {
            if (currentGroundFriction <= 0.1f) // ICE 
            {
                activeAccel = acceleration * 0.2f;
                activeDecel = 2f; // Very low decel = sliding
            }
            else if (currentGroundFriction >= 1.0f) // MUD
            {
                activeAccel = acceleration * 0.3f; // Hard to speed up
                activeDecel = brakeDeceleration * 30f; // Stop instantly (sticky)
                activeTargetSpeed *= 0.3f; // Much slower max speed
            }
        }

        // Logic to determine if we are accelerating or braking
        float currentXVel = rb.linearVelocity.x;
        bool isBraking = (activeTargetSpeed == 0) || (Mathf.Sign(currentXVel) != Mathf.Sign(activeTargetSpeed) && activeTargetSpeed != 0);

        // If the current speed is higher than the target speed (like entering mud), use deceleration
        if (Mathf.Abs(currentXVel) > Mathf.Abs(activeTargetSpeed)) isBraking = true;

        float chosenStep = isBraking ? activeDecel : activeAccel;

        float velocityX = Mathf.MoveTowards(currentXVel, activeTargetSpeed, chosenStep * Time.fixedDeltaTime);
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

    public void SpookDonkey(Vector3 snakePosition)
    {
        isScared = true;
        scareTimer = scareDuration;
        scareDirection = (transform.position.x < snakePosition.x) ? -1f : 1f;
    }
}