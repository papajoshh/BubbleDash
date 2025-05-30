using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseSpeed = 3f;
    public float currentSpeed { get; private set; }
    
    [Header("Boundaries")]
    public Vector2 verticalBounds = new Vector2(-4f, 4f);
    public float forwardBoundary = 20f;
    
    [Header("Starting Position")]
    public Vector3 fixedStartPosition = new Vector3(-7f, 0f, 0f);
    public bool useFixedStartPosition = true;
    
    public bool isAlive { get; private set; } = true;
    
    private Vector3 startPosition;
    
    void Start()
    {
        // Force starting position to ensure consistency between Editor and Build
        if (useFixedStartPosition)
        {
            transform.position = fixedStartPosition;
            startPosition = fixedStartPosition;
        }
        else
        {
            startPosition = transform.position;
        }
        
        currentSpeed = baseSpeed;
        isAlive = true;
        
        Debug.Log($"Player starting at position: {transform.position}");
    }
    
    void Update()
    {
        if (!isAlive) return;
        
        MoveForward();
        ClampVerticalPosition();
    }
    
    void MoveForward()
    {
        transform.Translate(Vector3.right * currentSpeed * Time.deltaTime);
    }
    
    void ClampVerticalPosition()
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, verticalBounds.x, verticalBounds.y);
        transform.position = pos;
    }
    
    public void SetSpeed(float speed)
    {
        currentSpeed = speed;
    }
    
    public void Die()
    {
        isAlive = false;
        Debug.Log("Player died!");
        
        // Notify GameManager if exists
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }
    
    public void Restart()
    {
        // Always use the same starting position for consistency
        if (useFixedStartPosition)
        {
            transform.position = fixedStartPosition;
        }
        else
        {
            transform.position = startPosition;
        }
        
        currentSpeed = baseSpeed;
        isAlive = true;
        
        Debug.Log($"Player restarted at position: {transform.position}");
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Die();
        }
    }
}