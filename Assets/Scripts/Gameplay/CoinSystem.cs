using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class CoinSystem : MonoBehaviour
{
    public static CoinSystem Instance { get; private set; }
    
    public int currentCoins { get; private set; }
    public int totalCoinsEarned { get; private set; }
    
    [Header("Coin Spawning")]
    public GameObject coinPrefab;
    public Transform coinSpawnPoint;
    public float coinSpawnChance = 0.3f; // 30% chance per obstacle
    public int minCoinsPerSpawn = 1;
    public int maxCoinsPerSpawn = 3;
    public float coinSpacing = 1f;
    
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
        }
    }
    
    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        
        currentCoins += amount;
        totalCoinsEarned += amount;
        
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
        if (coinPrefab == null) return;
        
        // Check spawn chance
        if (Random.value > coinSpawnChance) return;
        
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
                
                Instantiate(coinPrefab, spawnPos, Quaternion.identity);
            }
        }
    }
    
    void SpawnCoinPattern(Vector3 centerPosition, int coinCount)
    {
        // Different patterns based on coin count
        switch (coinCount)
        {
            case 2:
                // Horizontal line
                Instantiate(coinPrefab, centerPosition + Vector3.left * 0.5f, Quaternion.identity);
                Instantiate(coinPrefab, centerPosition + Vector3.right * 0.5f, Quaternion.identity);
                break;
                
            case 3:
                // Triangle
                Instantiate(coinPrefab, centerPosition, Quaternion.identity);
                Instantiate(coinPrefab, centerPosition + Vector3.left * 0.5f + Vector3.down * 0.5f, Quaternion.identity);
                Instantiate(coinPrefab, centerPosition + Vector3.right * 0.5f + Vector3.down * 0.5f, Quaternion.identity);
                break;
                
            case 4:
                // Square
                Instantiate(coinPrefab, centerPosition + new Vector3(-0.5f, 0.5f), Quaternion.identity);
                Instantiate(coinPrefab, centerPosition + new Vector3(0.5f, 0.5f), Quaternion.identity);
                Instantiate(coinPrefab, centerPosition + new Vector3(-0.5f, -0.5f), Quaternion.identity);
                Instantiate(coinPrefab, centerPosition + new Vector3(0.5f, -0.5f), Quaternion.identity);
                break;
                
            case 5:
                // Cross
                Instantiate(coinPrefab, centerPosition, Quaternion.identity);
                Instantiate(coinPrefab, centerPosition + Vector3.up * coinSpacing, Quaternion.identity);
                Instantiate(coinPrefab, centerPosition + Vector3.down * coinSpacing, Quaternion.identity);
                Instantiate(coinPrefab, centerPosition + Vector3.left * coinSpacing, Quaternion.identity);
                Instantiate(coinPrefab, centerPosition + Vector3.right * coinSpacing, Quaternion.identity);
                break;
                
            default:
                // Line
                for (int i = 0; i < coinCount; i++)
                {
                    float offset = (i - coinCount / 2f) * coinSpacing;
                    Instantiate(coinPrefab, centerPosition + Vector3.right * offset, Quaternion.identity);
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
}