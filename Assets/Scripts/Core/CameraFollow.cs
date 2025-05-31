using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // El player
    
    [Header("Follow Settings")]
    public Vector3 offset = new Vector3(0, 0, -10);
    public float smoothSpeed = 5f;
    public bool followX = true;
    public bool followY = false; // Normalmente NO en endless runners
    public bool adaptiveSmoothness = true; // Adjust smoothness based on speed
    
    [Header("Boundaries")]
    public bool useBoundaries = true;
    public float minX = float.NegativeInfinity;
    public float maxX = float.PositiveInfinity;
    public float minY = -5f;
    public float maxY = 5f;
    
    [Header("Look Ahead")]
    public float lookAheadDistance = 2f; // Ver un poco adelante del player
    
    [Header("Dynamic Zoom")]
    public bool dynamicZoom = true;
    public float baseOrthoSize = 5f; // Normal camera size
    public float maxOrthoSize = 7f; // Camera size at max speed (zoomed out)
    public float zoomSmoothSpeed = 2f; // How fast zoom changes
    
    private Vector3 velocity = Vector3.zero;
    private PlayerController playerController;
    private float lastPlayerSpeed = 0f;
    private Camera cam;
    private float currentOrthoSize;
    
    void Start()
    {
        // Find player if not assigned
        if (target == null)
        {
            playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
                target = playerController.transform;
        }
        else
        {
            // Try to get PlayerController from target
            playerController = target.GetComponent<PlayerController>();
        }
        
        // Get camera component
        cam = GetComponent<Camera>();
        if (cam != null && cam.orthographic)
        {
            currentOrthoSize = cam.orthographicSize;
            if (baseOrthoSize == 0) baseOrthoSize = currentOrthoSize;
        }
        
        // Set initial position
        if (target != null)
        {
            Vector3 desiredPosition = CalculateDesiredPosition();
            transform.position = desiredPosition;
        }
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        Vector3 desiredPosition = CalculateDesiredPosition();
        
        // Simple smooth follow - always consistent
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            desiredPosition, 
            ref velocity, 
            1f / smoothSpeed
        );
        
        // Update zoom based on speed
        UpdateDynamicZoom();
    }
    
    void UpdateDynamicZoom()
    {
        if (!dynamicZoom || cam == null || !cam.orthographic || playerController == null) return;
        
        // Calculate speed ratio (0 to 1)
        float speedRatio = (playerController.currentSpeed - playerController.baseSpeed) / 
                          (playerController.baseSpeed * 2f); // Assuming max speed is 3x base
        speedRatio = Mathf.Clamp01(speedRatio);
        
        // Calculate target orthographic size
        float targetOrthoSize = Mathf.Lerp(baseOrthoSize, maxOrthoSize, speedRatio);
        
        // Smooth zoom transition
        currentOrthoSize = Mathf.Lerp(currentOrthoSize, targetOrthoSize, Time.deltaTime * zoomSmoothSpeed);
        cam.orthographicSize = currentOrthoSize;
    }
    
    Vector3 CalculateDesiredPosition()
    {
        Vector3 targetPos = target.position;
        
        // Add look ahead
        targetPos.x += lookAheadDistance;
        
        // Apply offset
        Vector3 desiredPos = targetPos + offset;
        
        // Apply follow settings
        if (!followX)
            desiredPos.x = transform.position.x;
        if (!followY)
            desiredPos.y = offset.y; // Keep original Y + offset
            
        // Apply boundaries
        if (useBoundaries)
        {
            desiredPos.x = Mathf.Clamp(desiredPos.x, minX, maxX);
            desiredPos.y = Mathf.Clamp(desiredPos.y, minY, maxY);
        }
        
        return desiredPos;
    }
    
    // Public method to shake camera (for effects)
    public void Shake(float intensity, float duration)
    {
        if (SimpleEffects.Instance != null)
        {
            SimpleEffects.Instance.ShakeScreen(intensity, duration);
        }
    }
    
    // Method to instantly snap to target (for restarts)
    public void SnapToTarget()
    {
        if (target != null)
        {
            transform.position = CalculateDesiredPosition();
        }
    }
}