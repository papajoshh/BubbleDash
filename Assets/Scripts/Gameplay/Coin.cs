using UnityEngine;
using DG.Tweening;

public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    public int coinValue = 1;
    public float rotationSpeed = 180f; // Degrees per second
    public float bobSpeed = 2f;
    public float bobHeight = 0.2f;
    
    [Header("Collection")]
    public float magnetRange = 3f;
    public float magnetSpeed = 8f;
    public bool canBeCollected = true;
    
    private float effectiveMagnetRange;
    
    [Header("Lifetime")]
    public float lifetime = 20f; // Destroy after X seconds if not collected
    
    private Transform player;
    private Vector3 startPosition;
    private bool isBeingCollected = false;
    private float currentLifetime = 0f;
    
    void Start()
    {
        startPosition = transform.position;
        
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            PlayerController pc = FindObjectOfType<PlayerController>();
            if (pc != null)
            {
                player = pc.transform;
            }
        }
        
        // Add collider if not present
        if (GetComponent<Collider2D>() == null)
        {
            CircleCollider2D col = gameObject.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.3f;
        }
        
        // Apply magnet upgrade bonus
        float magnetBonus = PlayerPrefs.GetFloat("CoinMagnetBonus", 0f);
        effectiveMagnetRange = magnetRange + magnetBonus;
        
        // Start animations
        StartAnimations();
    }
    
    void StartAnimations()
    {
        // Rotation animation using DOTween
        transform.DORotate(new Vector3(0, 360, 0), 360f / rotationSpeed, RotateMode.LocalAxisAdd)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
        
        // Bobbing animation using DOTween
        transform.DOMoveY(startPosition.y + bobHeight, 1f / bobSpeed)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
        
        // Initial spawn animation
        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }
    
    void Update()
    {
        if (!canBeCollected) return;
        
        // Lifetime check
        currentLifetime += Time.deltaTime;
        if (currentLifetime > lifetime)
        {
            // Fade out before destroying
            transform.DOScale(0f, 0.3f).SetEase(Ease.InBack)
                .OnComplete(() => Destroy(gameObject));
            canBeCollected = false;
            return;
        }
        
        // Magnetic attraction to player with DOTween
        if (player != null && !isBeingCollected)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            
            if (distance < effectiveMagnetRange)
            {
                isBeingCollected = true;
                Debug.Log("Magnet activated - isBeingCollected set to true");
                
                // Kill existing animations
                transform.DOKill();
                
                // Magnetic collection animation
                float magnetTime = distance / (magnetSpeed * 2f);
                magnetTime = Mathf.Clamp(magnetTime, 0.2f, 0.5f);
                
                transform.DOMove(player.position, magnetTime)
                    .SetEase(Ease.InExpo)
                    .OnComplete(CollectCoin);
                
                // Scale effect during collection
                transform.DOScale(0.7f, magnetTime * 0.5f).SetEase(Ease.InQuad);
            }
        }
    }
    
    void CollectCoin()
    {
        if (CoinSystem.Instance != null)
        {
            CoinSystem.Instance.AddCoins(coinValue);
            Debug.Log($"Coin collected! Total coins: {CoinSystem.Instance.GetCurrentCoins()}");
        }
        else
        {
            Debug.LogError("CoinSystem.Instance is null!");
        }
        
        // NEW: Notify Energy and Objective systems
        if (EnergyManager.Instance != null)
        {
            EnergyManager.Instance.OnCoinCollected();
        }
        
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.OnCoinCollected();
        }
        
        // Visual effect
        if (SimpleEffects.Instance != null)
        {
            SimpleEffects.Instance.ShowScorePopup(transform.position, coinValue, Color.yellow);
        }
        
        // Sound effect
        if (SimpleSoundManager.Instance != null)
        {
            SimpleSoundManager.Instance.PlayCoinCollect();
        }
        
        // Collection animation
        transform.DOKill();
        transform.DOScale(0f, 0.2f).SetEase(Ease.InBack);
        transform.DORotate(new Vector3(0, 0, 360), 0.2f, RotateMode.FastBeyond360);
        
        // Destroy after animation
        Destroy(gameObject, 0.25f);
    }
    
    // Called when coin goes off screen or too far behind
    void OnBecameInvisible()
    {
        // Give it some extra time in case it comes back on screen
        Invoke(nameof(CheckIfStillInvisible), 2f);
    }
    
    void OnDestroy()
    {
        // Clean up any running tweens
        transform.DOKill();
    }
    
    void CheckIfStillInvisible()
    {
        if (player != null)
        {
            // If coin is too far behind player, destroy it
            float distanceBehind = player.position.x - transform.position.x;
            if (distanceBehind > 10f)
            {
                Destroy(gameObject);
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Show magnet range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, effectiveMagnetRange);
    }
}