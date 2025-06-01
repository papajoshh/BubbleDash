using UnityEngine;

public class SimpleBubbleCollision : MonoBehaviour
{
    private ShootingBubble myBubble;
    private bool hasExploded = false;
    
    void Start()
    {
        myBubble = GetComponent<ShootingBubble>();
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Prevent multiple explosions
        if (hasExploded) return;
        
        // Check if we hit any bubble type (using IBubble interface)
        IBubble otherBubble = collision.gameObject.GetComponent<IBubble>();
        if (otherBubble == null) 
        {
            // Hit a wall or obstacle - destroy the bubble
            OnWallCollision(collision);
            return;
        }
        
        // Now handle specific bubble types
        StaticBubble staticBubble = otherBubble as StaticBubble;
        CoinBubble coinBubble = otherBubble as CoinBubble;
        
        // Create collision point for effects
        Vector3 collisionPoint = collision.contacts[0].point;
        
        // Handle StaticBubble collision
        if (staticBubble != null)
        {
            HandleStaticBubbleCollision(staticBubble, collisionPoint);
            return;
        }
        
        // Handle CoinBubble collision
        if (coinBubble != null)
        {
            HandleCoinBubbleCollision(coinBubble, collisionPoint);
            return;
        }
    }
    
    public void SetExploded()
    {
        hasExploded = true;
    }
    
    void HandleStaticBubbleCollision(StaticBubble staticBubble, Vector3 collisionPoint)
    {
        hasExploded = true;
        
        // Check if same color
        if (myBubble != null && staticBubble.bubbleColor == myBubble.bubbleColor)
        {
            // SAME COLOR - Successful hit!
            // Get bubble color for effects
            Color effectColor = GetBubbleColor(myBubble.bubbleColor);
            
            // Visual effects - colorful explosion
            if (SimpleEffects.Instance != null)
            {
                SimpleEffects.Instance.PlayBubblePop(collisionPoint, effectColor);
                SimpleEffects.Instance.ShowComboText(collisionPoint, 1);
            }
            
            // Sound effect - satisfying pop
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
            
            // Update run statistics
            if (RunStatsManager.Instance != null)
            {
                RunStatsManager.Instance.OnBubblesPopped(2); // Both bubbles pop
            }
            
            // Destroy both bubbles
            Destroy(staticBubble.gameObject);
            Destroy(gameObject);
        }
        else
        {
            // DIFFERENT COLOR - Miss!
            // Visual effects - subtle puff
            if (SimpleEffects.Instance != null)
            {
                SimpleEffects.Instance.PlayMissEffect(collisionPoint);
            }
            
            // Sound effect - soft puff
            if (SimpleSoundManager.Instance != null)
            {
                SimpleSoundManager.Instance.PlayBubbleMiss();
            }
            
            // Just destroy the projectile bubble (no score, no momentum penalty)
            Destroy(gameObject);
        }
    }
    
    void HandleCoinBubbleCollision(CoinBubble coinBubble, Vector3 collisionPoint)
    {
        hasExploded = true;
        
        // Check if colors match or if player has Bubble Breaker upgrade
        bool canBreak = false;
        
        if (myBubble != null && coinBubble.bubbleColor == myBubble.bubbleColor)
        {
            canBreak = true;
        }
        
        // Bubble Breaker upgrade - VIP feature
        bool hasBubbleBreaker = PlayerPrefs.GetInt("BubbleBreakerUnlocked", 0) == 1;
        if (hasBubbleBreaker)
        {
            canBreak = true; // VIP players can use ANY color
        }
        
        if (canBreak)
        {
            // Pop the coin bubble!
            coinBubble.PopCoinBubble();
            
            // Destroy the shooting bubble
            Destroy(gameObject);
        }
        else
        {
            // Wrong color - miss
            if (SimpleEffects.Instance != null)
            {
                SimpleEffects.Instance.PlayMissEffect(collisionPoint);
            }
            
            if (SimpleSoundManager.Instance != null)
            {
                SimpleSoundManager.Instance.PlayBubbleMiss();
            }
            
            // Just destroy the projectile bubble
            Destroy(gameObject);
        }
    }
    
    void OnWallCollision(Collision2D collision)
    {
        hasExploded = true;
        
        // Create small puff effect
        if (SimpleEffects.Instance != null)
        {
            Vector3 collisionPoint = collision.contacts[0].point;
            SimpleEffects.Instance.PlayMissEffect(collisionPoint);
        }
        
        // Play miss sound
        if (SimpleSoundManager.Instance != null)
        {
            SimpleSoundManager.Instance.PlayBubbleMiss();
        }
        
        // Destroy the bubble
        Destroy(gameObject);
    }
    
    Color GetBubbleColor(BubbleColor bubbleColor)
    {
        switch (bubbleColor)
        {
            case BubbleColor.Red:
                return Color.red;
            case BubbleColor.Blue:
                return new Color(0.2f, 0.4f, 1f);
            case BubbleColor.Green:
                return new Color(0.2f, 0.8f, 0.2f);
            case BubbleColor.Yellow:
                return new Color(1f, 0.9f, 0.2f);
            default:
                return Color.white;
        }
    }
}