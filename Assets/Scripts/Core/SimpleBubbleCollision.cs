using UnityEngine;

public class SimpleBubbleCollision : MonoBehaviour
{
    private Bubble myBubble;
    private bool hasExploded = false;
    
    void Start()
    {
        myBubble = GetComponent<Bubble>();
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Prevent multiple explosions
        if (hasExploded) return;
        
        // Check if we hit another bubble
        Bubble otherBubble = collision.gameObject.GetComponent<Bubble>();
        if (otherBubble == null) return;
        
        // Create collision point for effects
        Vector3 collisionPoint = collision.contacts[0].point;
        
        // Check if same color
        if (myBubble != null && otherBubble.bubbleColor == myBubble.bubbleColor)
        {
            // SAME COLOR - Successful hit!
            hasExploded = true;
            otherBubble.GetComponent<SimpleBubbleCollision>()?.SetExploded();
            
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
            
            // Destroy both bubbles
            Destroy(otherBubble.gameObject);
            Destroy(gameObject);
        }
        else
        {
            // DIFFERENT COLOR - Miss!
            hasExploded = true;
            
            // Visual effects - subtle puff
            if (SimpleEffects.Instance != null)
            {
                // Gray/white puff effect for miss
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
    
    public void SetExploded()
    {
        hasExploded = true;
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