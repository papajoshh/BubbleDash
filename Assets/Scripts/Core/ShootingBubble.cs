using UnityEngine;

public class ShootingBubble : MonoBehaviour, IBubble
{
    [Header("Bubble Properties")]
    public BubbleColor bubbleColor = BubbleColor.Red;
    
    // IBubble implementation
    public BubbleColor GetBubbleColor()
    {
        return bubbleColor;
    }
    public BubbleType bubbleType = BubbleType.Normal;
    
    [Header("Physics")]
    public float maxLifetime = 10f;
    public float stopVelocityThreshold = 0.5f;
    public float maxDistanceBehindPlayer = 15f; // Destroy if too far behind
    
    [Header("Visual")]
    public SpriteRenderer bubbleRenderer;
    
    private float lifetime = 0f;
    
    // Color sprites - Only 4 colors exist in the game
    [Header("Sprites")]
    public Sprite redBubbleSprite;
    public Sprite blueBubbleSprite;
    public Sprite greenBubbleSprite;
    public Sprite yellowBubbleSprite;
    
    void Start()
    {
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
    }
    
    void UpdateAppearance()
    {
        if (bubbleRenderer == null) return;
        
        Sprite spriteToUse = null;
        
        // Normal color-based bubbles (coin bubbles are handled by CoinBubble class)
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
            bubbleRenderer.color = Color.white; // No tint, use sprite's original colors
        }
        else
        {
            // Fallback: if no sprite assigned, use color tint
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
    Multiplier, // Score bonus
    CoinBubble  // Gives coins when popped
}