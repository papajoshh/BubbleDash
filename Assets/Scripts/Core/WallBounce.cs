using UnityEngine;

public class WallBounce : MonoBehaviour
{
    [Header("Bounce Settings")]
    public float bounciness = 0.8f; // How much velocity to keep after bounce (0-1)
    public bool bounceHorizontally = false;
    public bool bounceVertically = true;
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if it's a bubble
        ShootingBubble bubble = collision.gameObject.GetComponent<ShootingBubble>();
        if (bubble == null) return;
        
        // Get the bubble's Rigidbody2D
        Rigidbody2D bubbleRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (bubbleRb == null) return;
        
        // Get current velocity
        Vector2 velocity = bubbleRb.velocity;
        
        // Calculate bounce based on wall type
        if (bounceVertically)
        {
            velocity.y = -velocity.y * bounciness;
        }
        
        if (bounceHorizontally)
        {
            velocity.x = -velocity.x * bounciness;
        }
        
        // Apply the new velocity
        bubbleRb.velocity = velocity;
        
        // Optional: Add a small random factor to prevent infinite bounces
        float randomFactor = Random.Range(-0.5f, 0.5f);
        bubbleRb.velocity += new Vector2(randomFactor, 0);
    }
}