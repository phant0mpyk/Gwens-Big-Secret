using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DonkeyCrankMovement : MonoBehaviour
{
    [Header("Attachments")]
    public Animator anim;
    public SpriteRenderer donkeySprite;
    public Transform carrotPivot;
    public Transform stickTip;
    public Transform carrotObject;
    public LineRenderer ropeRenderer;

    [Header("Audio Settings")]
    public AudioClip jumpSound;
    public AudioClip dudukSound;
    public AudioClip deathSound;       // <--- NEW: Slot for falling/death sound
    [Range(0, 1)] public float sfxVolume = 1f;
    private AudioSource audioSource;

    [Header("Movement Settings")]
    public Vector3 pivotOffset = new Vector3(0, 1.5f, 0);
    public float mouseSensitivity = 5f;
    public float maxSpeed = 10f;
    public float acceleration = 50f;
    public float airAcceleration = 10f;
    public float brakeDeceleration = 10f;
    public bool hasDuduk = false;

    [Header("Jump Charge Settings")]
    public float minJumpForce = 5f;
    public float maxJumpForce = 20f;
    public float maxChargeTime = 1.0f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Status Effects")]
    public bool isInverted = false;
    private float invertTimer = 0f;

    [Header("Scare Settings")]
    public float scareSpeed = 15f;
    public float scareDuration = 2f;
    private bool isScared = false;
    private float scareTimer = 0f;
    private float scareDirection = 1f;
    public static event Action OnDudukPlayed;

    [Header("Respawn Settings")]
    public Vector3 currentRespawnPoint;
    public bool isDead = false;
    public float respawnDelay = 1.0f;

    private Rigidbody2D rb;
    private float currentAngle = 90f;
    private float targetMoveSpeed;
    private bool isGrounded;
    private float jumpChargeTimer = 0f;

    private float currentGroundFriction = 0f;
    private bool isOnCustomMaterial = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        Cursor.lockState = CursorLockMode.Locked;
        if (ropeRenderer != null) ropeRenderer.positionCount = 2;

        currentRespawnPoint = transform.position;
    }

    void Update()
    {
        if (isDead) return;

        // --- STATUS EFFECT TIMERS ---
        if (isInverted)
        {
            invertTimer -= Time.deltaTime;
            if (invertTimer <= 0) isInverted = false;
        }

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

            if (isOnCustomMaterial && currentGroundFriction >= 1.0f) finalJumpForce *= 0.5f;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, finalJumpForce);

            PlaySFX(jumpSound);

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
                Vector2 directionToCarrot = (Vector2)carrotObject.position - (Vector2)carrotPivot.position;
                float carrotAngleRad = Mathf.Atan2(directionToCarrot.y, directionToCarrot.x);
                targetMoveSpeed = Mathf.Cos(carrotAngleRad) * maxSpeed;

                if (isInverted) targetMoveSpeed *= -1f;
            }
        }

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
            else anim.speed = 1.0f;
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

        // --- DUDUK LOGIC ---
        if (hasDuduk == true && Input.GetKeyDown(KeyCode.E))
        {
            OnDudukPlayed?.Invoke();
            PlaySFX(dudukSound);
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, sfxVolume);
        }
    }

    void FixedUpdate()
    {
        float activeAccel;
        float activeDecel = brakeDeceleration;
        float activeTargetSpeed = targetMoveSpeed;

        if (isGrounded)
        {
            activeAccel = acceleration;
            if (isOnCustomMaterial)
            {
                if (currentGroundFriction <= 0.1f)
                {
                    activeAccel = acceleration * 0.2f;
                    activeDecel = 2f;
                }
                else if (currentGroundFriction >= 1.0f)
                {
                    activeAccel = acceleration * 0.7f;
                    activeDecel = brakeDeceleration * 3f;
                    activeTargetSpeed *= 0.6f;
                }
            }
        }
        else
        {
            activeAccel = airAcceleration;
            activeDecel = airAcceleration;
        }

        float currentXVel = rb.linearVelocity.x;
        bool isSlowingDown = Mathf.Abs(currentXVel) > Mathf.Abs(activeTargetSpeed);
        float chosenStep = isSlowingDown ? activeDecel : activeAccel;
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
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        rb.linearVelocity = Vector2.zero;

        // --- PLAY DEATH/FALLING SOUND ---
        PlaySFX(deathSound);

        if (anim != null) anim.speed = 0;
        Invoke("Respawn", respawnDelay);
    }

    private void Respawn()
    {
        transform.position = currentRespawnPoint;
        isDead = false;
        isScared = false;
        scareTimer = 0;
        rb.linearVelocity = Vector2.zero;
        if (anim != null) anim.speed = 1f;
    }

    public void SpookDonkey(Vector3 snakePosition)
    {
        isScared = true;
        scareTimer = scareDuration;
        scareDirection = (transform.position.x < snakePosition.x) ? -1f : 1f;
    }

    public void UpdateRespawnPoint(Vector3 newPoint) => currentRespawnPoint = newPoint;
    public void InvertMovement(float duration) { isInverted = true; invertTimer = duration; }
}