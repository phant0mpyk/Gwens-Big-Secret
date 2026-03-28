using UnityEngine;

public class JumpKingCamera : MonoBehaviour
{
    private Vector3 targetPos;
    private float moveSpeed;
    private bool isMoving = false;

    void Start()
    {
        // Initialize the camera at its starting position
        targetPos = transform.position;
    }

    void Update()
    {
        if (isMoving)
        {
            // Smoothly slide to the next screen
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);

            // Stop moving once we are close enough to the target
            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                transform.position = targetPos;
                isMoving = false;
            }
        }
    }

    public void SetNewTarget(Vector3 newPos, float speed)
    {
        targetPos = newPos;
        moveSpeed = speed;
        isMoving = true;
    }
}