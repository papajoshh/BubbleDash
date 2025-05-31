using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseSpeed = 3f;
    public float currentSpeed { get; private set; }
    
    [Header("Boundaries")]
    public Vector2 verticalBounds = new Vector2(-4.5f, 2f); // Ajustado para nueva posición
    public float forwardBoundary = 20f;
    
    [Header("Starting Position")]
    public Vector3 fixedStartPosition = new Vector3(-7f, -2.5f, 0f);
    public bool useFixedStartPosition = true;
    
    public bool isAlive { get; private set; } = true;
    
    private Vector3 startPosition;
    
    [Header("Collision")]
    public LayerMask obstacleLayer = -1; // All layers by default
    
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
        
        // Only move if game is actually playing
        if (GameManager.Instance != null && GameManager.Instance.GetGameState() != GameState.Playing)
        {
            return;
        }
        
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
        if (!isAlive) return;
        
        // Check for static bubbles
        StaticBubble staticBubble = other.GetComponent<StaticBubble>();
        if (staticBubble != null)
        {
            Debug.Log("Player hit a static bubble (trigger) - Game Over!");
            Die();
            return;
        }
        
        if (other.CompareTag("Obstacle"))
        {
            Die();
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isAlive) return;
        
        // Check if we hit a static bubble
        StaticBubble staticBubble = collision.gameObject.GetComponent<StaticBubble>();
        if (staticBubble != null)
        {
            Debug.Log("Player hit a static bubble (collision) - Game Over!");
            Die();
            return;
        }
        
        // Check by tag as backup
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Player hit an obstacle - Game Over!");
            Die();
        }
    }
    
    public void Die()
    {
        if (!isAlive) return;
        
        isAlive = false;
        currentSpeed = 0f;
        
        // Visual feedback
        if (SimpleEffects.Instance != null)
        {
            SimpleEffects.Instance.PlayBubblePop(transform.position, Color.red);
            SimpleEffects.Instance.ShakeScreen(0.3f, 0.5f);
        }
        
        // Sound feedback
        if (SimpleSoundManager.Instance != null)
        {
            SimpleSoundManager.Instance.PlayGameOver();
        }
        
        // Trigger game over
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        
        Debug.Log("Player died!");
    }
}