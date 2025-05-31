using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform cameraTransform;
    public bool followX = true;
    public bool followY = false;
    public Vector3 offset = Vector3.zero;
    
    [Header("Auto Setup")]
    public bool autoFindCamera = true;
    
    private Vector3 initialPosition;
    private Vector3 lastCameraPosition;
    
    void Start()
    {
        // Store initial position
        initialPosition = transform.position;
        
        // Auto find camera
        if (autoFindCamera && cameraTransform == null)
        {
            cameraTransform = Camera.main?.transform;
            if (cameraTransform == null)
            {
                CameraFollow cameraFollow = FindObjectOfType<CameraFollow>();
                if (cameraFollow != null)
                    cameraTransform = cameraFollow.transform;
            }
        }
        
        // Calculate offset from camera if not set
        if (cameraTransform != null && offset == Vector3.zero)
        {
            offset = transform.position - cameraTransform.position;
        }
    }
    
    void LateUpdate()
    {
        if (cameraTransform == null) return;
        
        // Only update if camera has moved significantly
        if (Vector3.Distance(cameraTransform.position, lastCameraPosition) < 0.01f) return;
        
        Vector3 targetPosition = transform.position;
        
        if (followX)
        {
            targetPosition.x = cameraTransform.position.x + offset.x;
        }
        
        if (followY)
        {
            targetPosition.y = cameraTransform.position.y + offset.y;
        }
        
        transform.position = targetPosition;
        lastCameraPosition = cameraTransform.position;
    }
    
    // Method to manually set the camera to follow
    public void SetCameraToFollow(Transform newCamera)
    {
        cameraTransform = newCamera;
        if (newCamera != null)
        {
            offset = transform.position - newCamera.position;
        }
    }
    
    // Method to reset to initial position
    public void ResetPosition()
    {
        transform.position = initialPosition;
    }
}