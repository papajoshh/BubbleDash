using UnityEngine;

public class BubbleEndlessRunner : MonoBehaviour
{
    [Header("Bubble Settings for Endless Runner")]
    public float destroyDistanceBehindPlayer = 15f;
    public float bounceForce = 5f;
    public bool destroyOnSettle = true;
    
    private Transform playerTransform;
    private Rigidbody2D rb;
    private float initialX;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialX = transform.position.x;
        
        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            // Try to find by PlayerController
            PlayerController pc = FindObjectOfType<PlayerController>();
            if (pc != null)
            {
                playerTransform = pc.transform;
            }
        }
    }
    
    void Update()
    {
        // Check if bubble is too far behind
        if (playerTransform != null)
        {
            float distanceBehind = playerTransform.position.x - transform.position.x;
            if (distanceBehind > destroyDistanceBehindPlayer)
            {
                Destroy(gameObject);
            }
        }
        
        // Destroy if moving too slowly (settled)
        if (destroyOnSettle && rb != null && rb.velocity.magnitude < 0.5f)
        {
            // Give it a moment to see if it starts moving again
            Invoke(nameof(CheckIfStillSlow), 0.5f);
        }
    }
    
    void CheckIfStillSlow()
    {
        if (rb != null && rb.velocity.magnitude < 0.5f)
        {
            Destroy(gameObject);
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Add small upward force on wall collision to keep bubbles moving
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.name.Contains("Wall"))
        {
            if (rb != null)
            {
                rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
            }
        }
    }
}