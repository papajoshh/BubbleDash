using UnityEngine;

public class StaticBubble : MonoBehaviour
{
    [Header("Bubble Properties")]
    public BubbleColor bubbleColor = BubbleColor.Red;
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
        
        // Check if hit by a projectile bubble
        Bubble projectileBubble = collision.gameObject.GetComponent<Bubble>();
        SimpleBubbleCollision simpleBubble = collision.gameObject.GetComponent<SimpleBubbleCollision>();
        
        if (projectileBubble != null || simpleBubble != null)
        {
            // Get the color of the projectile
            BubbleColor projectileColor = BubbleColor.Red;
            if (projectileBubble != null)
                projectileColor = projectileBubble.bubbleColor;
            else if (simpleBubble != null)
            {
                Bubble b = simpleBubble.GetComponent<Bubble>();
                if (b != null) projectileColor = b.bubbleColor;
            }
            
            // Check if same color
            if (projectileColor == bubbleColor)
            {
                // Create explosion effect
                Vector3 explosionPos = collision.contacts[0].point;
                
                // Visual effects
                if (SimpleEffects.Instance != null)
                {
                    // Get the actual bubble color
                    Color effectColor = Color.white;
                    switch (bubbleColor)
                    {
                        case BubbleColor.Red:
                            effectColor = Color.red;
                            break;
                        case BubbleColor.Blue:
                            effectColor = new Color(0.2f, 0.4f, 1f);
                            break;
                        case BubbleColor.Green:
                            effectColor = new Color(0.2f, 0.8f, 0.2f);
                            break;
                        case BubbleColor.Yellow:
                            effectColor = new Color(1f, 0.9f, 0.2f);
                            break;
                    }
                    SimpleEffects.Instance.PlayBubblePop(explosionPos, effectColor);
                    SimpleEffects.Instance.ShowComboText(explosionPos, 1);
                }
                
                // Sound effect
                if (SimpleSoundManager.Instance != null)
                {
                    SimpleSoundManager.Instance.PlayBubblePop();
                }
                
                // Update score
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.OnBubbleHit(1);
                }
                
                // Update momentum
                MomentumSystem momentum = FindObjectOfType<MomentumSystem>();
                if (momentum != null)
                {
                    momentum.OnBubbleHit();
                }
                
                // Destroy the projectile
                Destroy(collision.gameObject);
                
                // Destroy this static bubble
                Destroy(gameObject);
            }
        }
    }
}