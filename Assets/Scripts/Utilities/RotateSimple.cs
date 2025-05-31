using UnityEngine;

public class RotateSimple : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 100f;
    public Vector3 rotationAxis = Vector3.forward; // Z-axis for 2D
    public bool randomizeStartRotation = true;
    
    [Header("Advanced Options")]
    public bool useUnscaledTime = false; // For UI elements that shouldn't be affected by Time.timeScale
    public bool oscillate = false;
    public float oscillationAngle = 45f;
    public float oscillationSpeed = 1f;
    
    private float oscillationTimer = 0f;
    private Quaternion startRotation;
    
    void Start()
    {
        startRotation = transform.rotation;
        
        if (randomizeStartRotation)
        {
            // Start at random rotation
            transform.Rotate(rotationAxis * Random.Range(0f, 360f));
        }
    }
    
    void Update()
    {
        float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        
        if (oscillate)
        {
            // Oscillate back and forth
            oscillationTimer += deltaTime * oscillationSpeed;
            float angle = Mathf.Sin(oscillationTimer) * oscillationAngle;
            transform.rotation = startRotation * Quaternion.AngleAxis(angle, rotationAxis);
        }
        else
        {
            // Continuous rotation
            transform.Rotate(rotationAxis * rotationSpeed * deltaTime);
        }
    }
}