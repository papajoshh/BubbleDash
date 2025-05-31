using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    public int value = 1;
    public float rotationSpeed = 100f;
    
    [Header("Collection Effects")]
    public float collectAnimationDuration = 0.5f;
    public AnimationCurve collectCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private bool isCollected = false;
    private float collectTime = 0f;
    private Vector3 originalScale;
    
    void Start()
    {
        originalScale = transform.localScale;
    }
    
    void Update()
    {
        if (!isCollected)
        {
            // Rotate the coin
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }
        else
        {
            // Collection animation
            collectTime += Time.deltaTime;
            float progress = collectTime / collectAnimationDuration;
            
            if (progress >= 1f)
            {
                // Destroy after animation
                Destroy(gameObject);
            }
            else
            {
                // Scale up and fade
                float curveValue = collectCurve.Evaluate(progress);
                transform.localScale = originalScale * (1f + curveValue);
                
                // Optional: Move up
                transform.position += Vector3.up * Time.deltaTime * 2f;
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;
        
        // Check if player collected the coin
        if (other.CompareTag("Player"))
        {
            CollectCoin();
        }
    }
    
    void CollectCoin()
    {
        isCollected = true;
        
        // Add coins to the system
        if (CoinSystem.Instance != null)
        {
            CoinSystem.Instance.AddCoins(value);
        }
        
        // Show score popup
        if (SimpleEffects.Instance != null)
        {
            SimpleEffects.Instance.ShowScorePopup(transform.position, value * 10, Color.yellow);
        }
        
        // Play collection sound
        if (SimpleSoundManager.Instance != null)
        {
            SimpleSoundManager.Instance.PlayCoinCollect();
        }
        
        // Disable collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        
        // Start collection animation (handled in Update)
    }
}