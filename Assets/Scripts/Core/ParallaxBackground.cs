using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Parallax Settings")]
    public Transform cameraTransform;
    public float parallaxSpeedX = 0.5f; // 0 = no movement, 1 = moves with camera
    public float parallaxSpeedY = 0f;   // Usually 0 for endless runners
    
    [Header("Debug")]
    public bool showDebugInfo = false;
    
    [Header("Infinite Scrolling")]
    public bool infiniteScrollX = true;
    public float backgroundWidth = 20f; // Width of your background sprite
    
    [Header("Auto Setup")]
    public bool autoFindCamera = true;
    public bool autoCalculateWidth = true;
    
    [Header("Camera Settings")]
    public float cameraWidth = 10f; // Half of camera view width
    
    private Vector3 startCameraPos;
    private Vector3 startBackgroundPos;
    private Vector3 originalBackgroundPos; // The actual initial position
    private float textureUnitSizeX;
    
    void Start()
    {
        // Auto find camera
        if (autoFindCamera && cameraTransform == null)
        {
            cameraTransform = Camera.main?.transform;
            if (cameraTransform == null)
                cameraTransform = FindObjectOfType<CameraFollow>()?.transform;
        }
        
        // Auto calculate width from sprite
        if (autoCalculateWidth)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                backgroundWidth = sr.bounds.size.x;
            }
        }
        
        // Store initial positions
        if (cameraTransform != null)
        {
            startCameraPos = cameraTransform.position;
        }
        startBackgroundPos = transform.position;
        originalBackgroundPos = transform.position; // Save the actual starting position
        
        // Calculate texture unit size for infinite scrolling
        textureUnitSizeX = backgroundWidth;
    }
    
    void Update()
    {
        if (cameraTransform == null) return;
        
        // Calculate camera movement
        Vector3 cameraDelta = cameraTransform.position - startCameraPos;
        
        // Apply parallax movement
        Vector3 backgroundTarget = startBackgroundPos + new Vector3(
            cameraDelta.x * parallaxSpeedX,
            cameraDelta.y * parallaxSpeedY,
            0
        );
        
        // Apply the movement
        transform.position = backgroundTarget;
        
        // Debug info
        if (showDebugInfo)
        {
            Debug.Log($"{gameObject.name}: Speed={parallaxSpeedX}, Camera={cameraDelta.x}, BG Move={cameraDelta.x * parallaxSpeedX}");
        }
        
        // Handle infinite scrolling - more aggressive detection
        if (infiniteScrollX && textureUnitSizeX > 0)
        {
            float cameraX = cameraTransform.position.x;
            float backgroundX = transform.position.x;
            float spriteRightEdge = backgroundX + (textureUnitSizeX * 0.5f); // Right edge of visible sprite
            float spriteLeftEdge = backgroundX - (textureUnitSizeX * 0.5f);  // Left edge of visible sprite
            
            // Get camera bounds
            float cameraLeftEdge = cameraX - cameraWidth;
            float cameraRightEdge = cameraX + cameraWidth;
            
            // If sprite right edge is approaching camera right edge, move sprite forward
            // This ensures there's always sprite visible on the right side
            if (spriteRightEdge < cameraRightEdge + (textureUnitSizeX * 0.1f)) // Trigger when 10% of texture ahead
            {
                // Calculate new position to place sprite seamlessly to the right
                float newX = spriteRightEdge; // Position so left edge starts where right edge was
                transform.position = new Vector3(newX, transform.position.y, transform.position.z);
                
                // Update start position to maintain parallax continuity
                startBackgroundPos.x = newX - (cameraDelta.x * parallaxSpeedX);
                
                if (showDebugInfo)
                {
                    Debug.Log($"{gameObject.name}: Need more sprite on right - moved from {backgroundX:F1} to {newX:F1}");
                }
            }
            // If sprite is too far ahead (moving backwards), reposition behind
            else if (spriteLeftEdge > cameraRightEdge + textureUnitSizeX)
            {
                float newX = backgroundX - textureUnitSizeX;
                transform.position = new Vector3(newX, transform.position.y, transform.position.z);
                startBackgroundPos.x = newX - (cameraDelta.x * parallaxSpeedX);
                
                if (showDebugInfo)
                {
                    Debug.Log($"{gameObject.name}: Sprite too far ahead - moved back from {backgroundX:F1} to {newX:F1}");
                }
            }
        }
    }
    
    // Public method to manually set parallax speed
    public void SetParallaxSpeed(float speedX, float speedY = 0f)
    {
        parallaxSpeedX = speedX;
        parallaxSpeedY = speedY;
    }
    
    // Method to reset position (useful for game restart)
    public void ResetPosition()
    {
        // Reset the background to its original position
        transform.position = originalBackgroundPos;
        
        // Reset the reference positions
        if (cameraTransform != null)
        {
            startCameraPos = cameraTransform.position;
        }
        startBackgroundPos = originalBackgroundPos;
    }
}