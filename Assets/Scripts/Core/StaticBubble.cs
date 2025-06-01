using UnityEngine;

public class StaticBubble : MonoBehaviour, IBubble
{
    [Header("Bubble Properties")]
    public BubbleColor bubbleColor = BubbleColor.Red;
    
    // IBubble implementation
    public BubbleColor GetBubbleColor()
    {
        return bubbleColor;
    }
    public bool randomizeColor = true; // Randomize color on start
    
    [Header("Visual")]
    public SpriteRenderer bubbleRenderer;
    
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
        
        // Randomize color if enabled
        if (randomizeColor)
        {
            bubbleColor = (BubbleColor)Random.Range(0, 4);
        }
        
        // Update visual appearance
        UpdateAppearance();
        
        // Ensure we have a collider but NO rigidbody
        if (GetComponent<Collider2D>() == null)
        {
            CircleCollider2D col = gameObject.AddComponent<CircleCollider2D>();
            col.isTrigger = false;
        }
        
        // Make sure we DON'T have a Rigidbody2D
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Destroy(rb);
        }
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
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if hit by player - instant death
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null || collision.gameObject.CompareTag("Player"))
        {
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
            
            // Kill the player
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
            return;
        }
        
        // All bubble-to-bubble collision is now handled by SimpleBubbleCollision
        // on the shooting bubble, so we don't need to handle it here
    }
}