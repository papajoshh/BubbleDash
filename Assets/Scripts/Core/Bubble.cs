using UnityEngine;

public class Bubble : MonoBehaviour
{
    [Header("Bubble Properties")]
    public BubbleColor bubbleColor = BubbleColor.Red;
    public BubbleType bubbleType = BubbleType.Normal;
    
    [Header("Physics")]
    public float maxLifetime = 10f;
    public float stopVelocityThreshold = 0.5f;
    public float maxDistanceBehindPlayer = 15f; // Destroy if too far behind
    
    [Header("Visual")]
    public SpriteRenderer bubbleRenderer;
    
    private Rigidbody2D rb;
    private bool hasCollided = false;
    private float lifetime = 0f;
    
    // Color sprites (assign these in Unity or load from Resources)
    [Header("Sprites")]
    public Sprite redBubbleSprite;
    public Sprite blueBubbleSprite;
    public Sprite greenBubbleSprite;
    public Sprite yellowBubbleSprite;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Don't add Rigidbody2D automatically - let BubbleShooter handle it
        
        if (bubbleRenderer == null)
        {
            bubbleRenderer = GetComponent<SpriteRenderer>();
        }
        
        // Register with BubbleManager
        if (BubbleManager.Instance != null)
        {
            BubbleManager.Instance.RegisterBubble(this);
        }
        
        // Set visual appearance
        UpdateAppearance();
        
        // Add collider if not present
        if (GetComponent<Collider2D>() == null)
        {
            CircleCollider2D col = gameObject.AddComponent<CircleCollider2D>();
            col.isTrigger = false; // For physics collisions
        }
    }
    
    void Update()
    {
        lifetime += Time.deltaTime;
        
        // Check if bubble should be destroyed
        if (lifetime > maxLifetime)
        {
            DestroyBubble();
            return;
        }
        
        // Check if bubble is too far behind player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distanceBehind = player.transform.position.x - transform.position.x;
            if (distanceBehind > maxDistanceBehindPlayer)
            {
                DestroyBubble();
                return;
            }
        }
        
        // For endless runner, bubbles shouldn't stop - they should keep moving or be destroyed
        // Remove the velocity check since we want bubbles to keep moving
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasCollided) return;
        
        Bubble otherBubble = collision.gameObject.GetComponent<Bubble>();
        if (otherBubble != null)
        {
            OnBubbleCollision(otherBubble);
        }
        else
        {
            // Hit wall or obstacle
            OnWallCollision();
        }
    }
    
    void OnBubbleCollision(Bubble otherBubble)
    {
        hasCollided = true;
        
        // Stop physics movement
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }
        
        // Check for matches
        if (BubbleManager.Instance != null)
        {
            BubbleManager.Instance.CheckForMatches(this);
        }
    }
    
    void OnWallCollision()
    {
        // In endless runner, bubbles bounce off walls but don't stick
        // WallBounce component handles the actual bouncing
        
        // Just mark that we've hit something
        hasCollided = true;
        
        // Don't reduce velocity here - let WallBounce handle it
        // This prevents double velocity reduction
    }
    
    void OnBubbleSettled()
    {
        // In endless runner, bubbles shouldn't settle - they should be destroyed
        // This method is kept for compatibility but bubbles are destroyed instead
        hasCollided = true;
        
        // Destroy bubble that has settled (stopped moving)
        DestroyBubble();
    }
    
    void UpdateAppearance()
    {
        if (bubbleRenderer == null) return;
        
        Sprite spriteToUse = null;
        
        switch (bubbleColor)
        {
            case BubbleColor.Red:
                spriteToUse = redBubbleSprite;
                break;
            case BubbleColor.Blue:
                spriteToUse = blueBubbleSprite;
                break;
            case BubbleColor.Green:
                spriteToUse = greenBubbleSprite;
                break;
            case BubbleColor.Yellow:
                spriteToUse = yellowBubbleSprite;
                break;
        }
        
        if (spriteToUse != null)
        {
            bubbleRenderer.sprite = spriteToUse;
        }
        else
        {
            // Fallback: just set color tint
            bubbleRenderer.color = GetColorFromEnum(bubbleColor);
        }
    }
    
    Color GetColorFromEnum(BubbleColor color)
    {
        switch (color)
        {
            case BubbleColor.Red: return Color.red;
            case BubbleColor.Blue: return Color.blue;
            case BubbleColor.Green: return Color.green;
            case BubbleColor.Yellow: return Color.yellow;
            default: return Color.white;
        }
    }
    
    public void SetColor(BubbleColor newColor)
    {
        bubbleColor = newColor;
        UpdateAppearance();
    }
    
    public void DestroyBubble()
    {
        // Unregister from manager
        if (BubbleManager.Instance != null)
        {
            BubbleManager.Instance.UnregisterBubble(this);
        }
        
        Destroy(gameObject);
    }
    
    void OnDestroy()
    {
        // Ensure we're unregistered
        if (BubbleManager.Instance != null)
        {
            BubbleManager.Instance.UnregisterBubble(this);
        }
    }
}

public enum BubbleColor
{
    Red,
    Blue,
    Green,
    Yellow
}

public enum BubbleType
{
    Normal,
    Rainbow,    // Matches any color
    Bomb,       // Destroys area
    Multiplier  // Score bonus
}