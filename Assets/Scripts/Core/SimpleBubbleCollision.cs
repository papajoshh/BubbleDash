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
        
        // Check if same color
        if (myBubble != null && otherBubble.bubbleColor == myBubble.bubbleColor)
        {
            hasExploded = true;
            otherBubble.GetComponent<SimpleBubbleCollision>()?.SetExploded();
            
            // Create explosion effect at collision point
            Vector3 explosionPos = collision.contacts[0].point;
            
            // Visual effects
            if (SimpleEffects.Instance != null)
            {
                // Get the actual bubble color
                Color effectColor = Color.white;
                if (myBubble != null)
                {
                    switch (myBubble.bubbleColor)
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
                ScoreManager.Instance.OnBubbleHit(1); // Simple hit, no combo
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
    }
    
    public void SetExploded()
    {
        hasExploded = true;
    }
}