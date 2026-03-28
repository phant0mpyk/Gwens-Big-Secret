using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public Vector3 cameraTargetPosition; // The X, Y, Z coordinates the camera should move to
    public float transitionSpeed = 5f;   // How fast the camera slides to the new spot

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Find the main camera and tell it where to go
            JumpKingCamera cam = Camera.main.GetComponent<JumpKingCamera>();
            if (cam != null)
            {
                cam.SetNewTarget(cameraTargetPosition, transitionSpeed);
            }
        }
    }
}