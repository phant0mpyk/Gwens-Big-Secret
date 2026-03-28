using UnityEngine;

[RequireComponent(typeof(Camera))]
public class RetroAspectEnforcer : MonoBehaviour
{
    void Start()
    {
        // 4:3 Aspect Ratio
        float targetAspect = 4.0f / 3.0f; 
        
        // Determine the actual screen ratio
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Camera cam = GetComponent<Camera>();

        // If current screen is wider than 4:3, add black bars on the sides
        if (scaleHeight >= 1.0f) 
        {
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            cam.rect = rect;
        }
        else // If screen is taller (rare for PC, but handles it), add bars top/bottom
        {
            Rect rect = cam.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            cam.rect = rect;
        }
    }
}