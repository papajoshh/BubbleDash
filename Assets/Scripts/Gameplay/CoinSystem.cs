using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class CoinSystem : MonoBehaviour
{
    public static CoinSystem Instance { get; private set; }
    
    public int currentCoins { get; private set; }
    public int totalCoinsEarned { get; private set; }
    private int coinsThisRun = 0;
    
    [Header("Coin Spawning")]
    public GameObject coinPrefab; // Legacy floating coins
    public GameObject coinBubblePrefab; // New coin bubbles
    public Transform coinSpawnPoint;
    public float coinSpawnChance = 0.3f; // 30% chance per obstacle
    public int minCoinsPerSpawn = 1;
    public int maxCoinsPerSpawn = 3;
    public float coinSpacing = 1f;
    public bool useFloatingCoins = false; // Toggle between old and new system
    
    [Header("Coin Patterns")]
    public bool usePatterns = true;
    
    // Events
    public Action<int> OnCoinsChanged;
    public Action<int> OnCoinsCollected;
    
    private const string COINS_KEY = "TotalCoins";
    private const string TOTAL_EARNED_KEY = "TotalCoinsEarned";
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        // Load saved coins
        LoadCoins();
        
        // Subscribe to game events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver += SaveCoins;
            GameManager.Instance.OnGameStart += ResetRunCoins;
            GameManager.Instance.OnGameRestart += ResetRunCoins;
        }
    }
    
    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        
        currentCoins += amount;
        totalCoinsEarned += amount;
        coinsThisRun += amount;
        
        OnCoinsChanged?.Invoke(currentCoins);
        OnCoinsCollected?.Invoke(amount);
        
        // Add to score
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddBonusScore(amount * 10); // 10 points per coin
        }
        
        Debug.Log($"Collected {amount} coins! Total: {currentCoins}");
    }
    
    public bool SpendCoins(int amount)
    {
        if (amount <= 0 || currentCoins < amount)
            return false;
        
        currentCoins -= amount;
        OnCoinsChanged?.Invoke(currentCoins);
        
        SaveCoins();
        return true;
    }
    
    public void SpawnCoinsAtPosition(Vector3 position, float spreadY = 0f)
    {
        GameObject prefabToUse = useFloatingCoins ? coinPrefab : coinBubblePrefab;
        
        if (prefabToUse == null) 
        {
            Debug.LogWarning($"No coin prefab assigned! useFloatingCoins={useFloatingCoins}");
            return;
        }
        
        // Check spawn chance with upgrade bonus
        float upgradeBonus = PlayerPrefs.GetFloat("CoinSpawnRateBonus", 0f);
        float totalSpawnChance = Mathf.Min(coinSpawnChance + upgradeBonus, 0.3f); // Cap at 30%
        
        if (Random.value > totalSpawnChance) return;
        
        int coinsToSpawn = Random.Range(minCoinsPerSpawn, maxCoinsPerSpawn + 1);
        
        if (usePatterns && coinsToSpawn > 1)
        {
            SpawnCoinPattern(position, coinsToSpawn);
        }
        else
        {
            // Simple spawn
            for (int i = 0; i < coinsToSpawn; i++)
            {
                Vector3 spawnPos = position + Vector3.up * (i * coinSpacing);
                if (spreadY > 0)
                {
                    spawnPos.y += Random.Range(-spreadY, spreadY);
                }
                
                // Check if position is clear before spawning
                if (IsPositionClear(spawnPos))
                {
                    Instantiate(prefabToUse, spawnPos, Quaternion.identity);
                }
            }
        }
    }
    
    void SpawnCoinPattern(Vector3 centerPosition, int coinCount)
    {
        GameObject prefabToUse = useFloatingCoins ? coinPrefab : coinBubblePrefab;
        if (prefabToUse == null) return;
        
        // Different patterns based on coin count
        switch (coinCount)
        {
            case 2:
                // Horizontal line
                TrySpawnCoin(prefabToUse, centerPosition + Vector3.left * 0.5f);
                TrySpawnCoin(prefabToUse, centerPosition + Vector3.right * 0.5f);
                break;
                
            case 3:
                // Triangle
                TrySpawnCoin(prefabToUse, centerPosition);
                TrySpawnCoin(prefabToUse, centerPosition + Vector3.left * 0.5f + Vector3.down * 0.5f);
                TrySpawnCoin(prefabToUse, centerPosition + Vector3.right * 0.5f + Vector3.down * 0.5f);
                break;
                
            case 4:
                // Square
                TrySpawnCoin(prefabToUse, centerPosition + new Vector3(-0.5f, 0.5f));
                TrySpawnCoin(prefabToUse, centerPosition + new Vector3(0.5f, 0.5f));
                TrySpawnCoin(prefabToUse, centerPosition + new Vector3(-0.5f, -0.5f));
                TrySpawnCoin(prefabToUse, centerPosition + new Vector3(0.5f, -0.5f));
                break;
                
            case 5:
                // Cross
                TrySpawnCoin(prefabToUse, centerPosition);
                TrySpawnCoin(prefabToUse, centerPosition + Vector3.up * coinSpacing);
                TrySpawnCoin(prefabToUse, centerPosition + Vector3.down * coinSpacing);
                TrySpawnCoin(prefabToUse, centerPosition + Vector3.left * coinSpacing);
                TrySpawnCoin(prefabToUse, centerPosition + Vector3.right * coinSpacing);
                break;
                
            default:
                // Line
                for (int i = 0; i < coinCount; i++)
                {
                    float offset = (i - coinCount / 2f) * coinSpacing;
                    TrySpawnCoin(prefabToUse, centerPosition + Vector3.right * offset);
                }
                break;
        }
    }
    
    public void SaveCoins()
    {
        PlayerPrefs.SetInt(COINS_KEY, currentCoins);
        PlayerPrefs.SetInt(TOTAL_EARNED_KEY, totalCoinsEarned);
        PlayerPrefs.Save();
        
        Debug.Log($"Coins saved: {currentCoins}");
    }
    
    void LoadCoins()
    {
        currentCoins = PlayerPrefs.GetInt(COINS_KEY, 0);
        totalCoinsEarned = PlayerPrefs.GetInt(TOTAL_EARNED_KEY, 0);
        
        OnCoinsChanged?.Invoke(currentCoins);
        
        Debug.Log($"Coins loaded: {currentCoins}");
    }
    
    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver -= SaveCoins;
            GameManager.Instance.OnGameStart -= ResetRunCoins;
            GameManager.Instance.OnGameRestart -= ResetRunCoins;
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            SaveCoins();
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            SaveCoins();
    }
    
    // Public getters
    public int GetCurrentCoins() => currentCoins;
    public int GetTotalEarned() => totalCoinsEarned;
    public bool CanAfford(int amount) => currentCoins >= amount;
    public int GetCoinsThisRun() => coinsThisRun;
    
    // Reset coins for new run
    public void ResetRunCoins()
    {
        coinsThisRun = 0;
    }
    
    // Helper method to check if a position is clear of other objects
    bool IsPositionClear(Vector3 position, float checkRadius = 0.4f)
    {
        // Check for any colliders in the area (bubbles, obstacles, other coins)
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, checkRadius);
        
        // If we find any colliders, the position is not clear
        foreach (var collider in colliders)
        {
            // Ignore triggers (like coin collection areas)
            if (collider.isTrigger) continue;
            
            // Check if it's a bubble, obstacle, or another coin
            if (collider.GetComponent<StaticBubble>() != null ||
                collider.GetComponent<CoinBubble>() != null ||
                collider.GetComponent<Coin>() != null ||
                collider.CompareTag("Obstacle"))
            {
                return false;
            }
        }
        
        return true;
    }
    
    // Helper method to spawn coin only if position is clear
    void TrySpawnCoin(GameObject prefab, Vector3 position)
    {
        if (IsPositionClear(position))
        {
            Instantiate(prefab, position, Quaternion.identity);
        }
        else
        {
            // Try to find a nearby clear position
            Vector3 alternativePos = FindClearPositionNearby(position);
            if (alternativePos != Vector3.zero)
            {
                Instantiate(prefab, alternativePos, Quaternion.identity);
            }
        }
    }
    
    // Find a clear position near the original one
    Vector3 FindClearPositionNearby(Vector3 originalPos, float searchRadius = 2f)
    {
        // Try different positions in a circle around the original
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f * Mathf.Deg2Rad;
            Vector3 testPos = originalPos + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 0.5f;
            
            if (IsPositionClear(testPos))
            {
                return testPos;
            }
        }
        
        // If no clear position found, return zero (skip spawning)
        return Vector3.zero;
    }
}