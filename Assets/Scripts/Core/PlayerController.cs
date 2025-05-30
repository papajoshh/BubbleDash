using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseSpeed = 3f;
    public float currentSpeed { get; private set; }
    
    [Header("Boundaries")]
    public Vector2 verticalBounds = new Vector2(-4f, 4f);
    public float forwardBoundary = 20f;
    
    public bool isAlive { get; private set; } = true;
    
    private Vector3 startPosition;
    
    void Start()
    {
        startPosition = transform.position;
        currentSpeed = baseSpeed;
        isAlive = true;
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
        transform.position = startPosition;
        currentSpeed = baseSpeed;
        isAlive = true;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Die();
        }
    }
}