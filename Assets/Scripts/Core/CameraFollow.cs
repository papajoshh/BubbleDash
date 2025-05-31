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
    
    [Header("Boundaries")]
    public bool useBoundaries = true;
    public float minX = float.NegativeInfinity;
    public float maxX = float.PositiveInfinity;
    public float minY = -5f;
    public float maxY = 5f;
    
    [Header("Look Ahead")]
    public float lookAheadDistance = 2f; // Ver un poco adelante del player
    
    private Vector3 velocity = Vector3.zero;
    
    void Start()
    {
        // Find player if not assigned
        if (target == null)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
                target = player.transform;
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
        
        // Smooth follow
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            desiredPosition, 
            ref velocity, 
            1f / smoothSpeed
        );
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