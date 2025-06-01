using UnityEngine;

public class CoinBubble : MonoBehaviour, IBubble
{
    [Header("Bubble Properties")]
    public BubbleColor bubbleColor = BubbleColor.Yellow;
    
    // IBubble implementation
    public BubbleColor GetBubbleColor()
    {
        return bubbleColor;
    }
    public bool randomizeColor = true;
    
    [Header("Coin Settings")]
    public int baseCoinValue = 1;
    
    [Header("Visual")]
    public SpriteRenderer bubbleRenderer;
    
    [Header("Sprites")]
    public Sprite redCoinBubbleSprite;
    public Sprite blueCoinBubbleSprite;
    public Sprite greenCoinBubbleSprite;
    public Sprite yellowCoinBubbleSprite;
    
    [Header("Effects")]
    public GameObject coinPopEffectPrefab;
    public float bobSpeed = 2f;
    public float bobHeight = 0.1f;
    
    private Vector3 startPosition;
    private float bobTime = 0f;
    
    void Start()
    {
        if (bubbleRenderer == null)
        {
            bubbleRenderer = GetComponent<SpriteRenderer>();
        }
        
        // Randomize color if enabled
        if (randomizeColor)
        {
            bubbleColor = (BubbleColor)Random.Range(0, 4); // 0-3 for Red, Blue, Green, Yellow
        }
        
        // Set coin bubble appearance
        UpdateAppearance();
        
        // Store start position for bobbing animation
        startPosition = transform.localPosition;
        
        // Ensure we have a collider
        if (GetComponent<Collider2D>() == null)
        {
            CircleCollider2D col = gameObject.AddComponent<CircleCollider2D>();
            col.isTrigger = false;
        }
        
        // Make sure we DON'T have a Rigidbody2D (static target)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Destroy(rb);
        }
        
        // Add random starting offset for bob animation
        bobTime = Random.Range(0f, Mathf.PI * 2f);
    }
    
    void Update()
    {
        // Gentle bobbing animation
        if (bobHeight > 0)
        {
            bobTime += Time.deltaTime * bobSpeed;
            float yOffset = Mathf.Sin(bobTime) * bobHeight;
            transform.localPosition = startPosition + new Vector3(0, yOffset, 0);
        }
        
        // Check for auto-pop upgrade
        CheckAutoPop();
    }
    
    void CheckAutoPop()
    {
        // Check if auto-pop upgrade is unlocked
        if (PlayerPrefs.GetInt("AutoPopUnlocked", 0) == 0) return;
        
        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        
        Vector3 playerPos = player.transform.position;
        Vector3 myPos = transform.position;
        
        // Check if player is directly below this coin bubble
        float horizontalDistance = Mathf.Abs(playerPos.x - myPos.x);
        float verticalDistance = myPos.y - playerPos.y;
        
        // Player must be below bubble (verticalDistance > 0) and close enough
        if (horizontalDistance < 0.5f && verticalDistance > 0 && verticalDistance < 1.5f)
        {
            // Auto-pop! Player is directly below
            PopCoinBubble();
            
            // Visual effect for auto-pop
            if (SimpleEffects.Instance != null)
            {
                SimpleEffects.Instance.ShowScorePopup(transform.position, baseCoinValue, new Color(1f, 0.8f, 0f));
            }
        }
    }
    
    void UpdateAppearance()
    {
        if (bubbleRenderer == null) return;
        
        Sprite spriteToUse = null;
        
        // Select sprite based on color
        switch (bubbleColor)
        {
            case BubbleColor.Red:
                spriteToUse = redCoinBubbleSprite;
                break;
            case BubbleColor.Blue:
                spriteToUse = blueCoinBubbleSprite;
                break;
            case BubbleColor.Green:
                spriteToUse = greenCoinBubbleSprite;
                break;
            case BubbleColor.Yellow:
                spriteToUse = yellowCoinBubbleSprite;
                break;
        }
        
        if (spriteToUse != null)
        {
            bubbleRenderer.sprite = spriteToUse;
            bubbleRenderer.color = Color.white; // No tint if using specific sprites
        }
        else
        {
            // Fallback: use color tint on base sprite
            bubbleRenderer.color = GetColorForBubble(bubbleColor);
        }
    }
    
    Color GetColorForBubble(BubbleColor color)
    {
        // Return golden-tinted versions of each color
        switch (color)
        {
            case BubbleColor.Red:
                return new Color(1f, 0.6f, 0.4f); // Golden red
            case BubbleColor.Blue:
                return new Color(0.4f, 0.6f, 1f); // Golden blue
            case BubbleColor.Green:
                return new Color(0.6f, 1f, 0.4f); // Golden green
            case BubbleColor.Yellow:
                return new Color(1f, 0.9f, 0.2f); // Golden yellow
            default:
                return new Color(1f, 0.8f, 0f); // Default gold
        }
    }
    
    public void PopCoinBubble()
    {
        // Calculate total coins with upgrades
        int goldenTouchLevel = PlayerPrefs.GetInt("GoldenTouchLevel", 0);
        int totalCoins = baseCoinValue + goldenTouchLevel;
        
        // Award coins
        if (CoinSystem.Instance != null)
        {
            CoinSystem.Instance.AddCoins(totalCoins);
        }
        
        // Visual effects
        if (SimpleEffects.Instance != null)
        {
            Color goldColor = new Color(1f, 0.8f, 0f, 1f);
            SimpleEffects.Instance.PlayBubblePop(transform.position, goldColor);
            SimpleEffects.Instance.ShowScorePopup(transform.position, totalCoins, Color.yellow);
        }
        
        // Sound effect
        if (SimpleSoundManager.Instance != null)
        {
            SimpleSoundManager.Instance.PlayCoinCollect();
        }
        
        // Also add to score
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnBubbleHit(1);
        }
        
        // Update momentum (counts as successful hit)
        MomentumSystem momentum = FindObjectOfType<MomentumSystem>();
        if (momentum != null)
        {
            momentum.OnBubbleHit();
        }
        
        Debug.Log($"Coin bubble popped! Awarded {totalCoins} coins");
        
        // Destroy the bubble
        Destroy(gameObject);
    }
    
    // All collision handling is now done by SimpleBubbleCollision on the shooting bubble
    
    // Visual indicator in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}