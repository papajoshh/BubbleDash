using UnityEngine;
using System.Collections.Generic;

public class ObstacleGenerator : MonoBehaviour
{
    [Header("Obstacle Prefabs")]
    public GameObject[] obstaclePrefabs;
    public float minDistanceBetweenObstacles = 5f;
    public float maxDistanceBetweenObstacles = 8f;
    
    [Header("Spawn Settings")]
    public Transform spawnPoint;
    public float spawnAheadDistance = 20f;
    public int maxObstaclesActive = 10;
    
    [Header("Difficulty")]
    public float startingDifficulty = 0f;
    public float difficultyIncreaseRate = 0.1f;
    public float maxDifficulty = 1f;
    private float currentDifficulty = 0f;
    
    [Header("References")]
    public Transform player;
    
    private List<GameObject> activeObstacles = new List<GameObject>();
    private float lastSpawnX = 0f;
    private float nextSpawnDistance;
    
    void Start()
    {
        // Find references if not assigned
        if (player == null)
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
                player = playerController.transform;
        }
        
        if (spawnPoint == null)
            spawnPoint = transform;
        
        // Initialize
        currentDifficulty = startingDifficulty;
        nextSpawnDistance = Random.Range(minDistanceBetweenObstacles, maxDistanceBetweenObstacles);
        lastSpawnX = player.position.x + spawnAheadDistance;
        
        // Subscribe to game events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStart += OnGameStart;
            GameManager.Instance.OnGameRestart += OnGameStart;
            GameManager.Instance.OnGameOver += OnGameOver;
        }
        
        // Spawn initial obstacles
        SpawnInitialObstacles();
    }
    
    void Update()
    {
        if (player == null || !GameManager.Instance.IsPlaying()) return;
        
        // Check if we need to spawn new obstacles
        float playerX = player.position.x;
        float spawnX = playerX + spawnAheadDistance;
        
        if (spawnX >= lastSpawnX + nextSpawnDistance)
        {
            SpawnObstacle(spawnX);
        }
        
        // Clean up obstacles behind player
        CleanupObstacles(playerX);
        
        // Update difficulty
        UpdateDifficulty();
    }
    
    void SpawnObstacle(float xPosition)
    {
        if (obstaclePrefabs.Length == 0)
        {
            Debug.LogWarning("No obstacle prefabs assigned!");
            return;
        }
        
        // Choose obstacle based on difficulty
        GameObject prefabToSpawn = ChooseObstaclePrefab();
        
        // Random Y offset for variety
        float yOffset = Random.Range(-1f, 1f);
        Vector3 spawnPosition = new Vector3(xPosition, yOffset, 0);
        
        // Spawn the obstacle
        GameObject obstacle = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        activeObstacles.Add(obstacle);
        
        // Spawn coins near obstacles
        if (CoinSystem.Instance != null)
        {
            // Spawn coins offset from obstacle
            Vector3 coinPosition = spawnPosition + Vector3.right * 2f;
            CoinSystem.Instance.SpawnCoinsAtPosition(coinPosition, 1f);
        }
        
        // Update spawn tracking
        lastSpawnX = xPosition;
        
        // Calculate next spawn distance based on difficulty
        float difficultyFactor = 1f - (currentDifficulty * 0.3f); // Higher difficulty = closer obstacles
        nextSpawnDistance = Random.Range(
            minDistanceBetweenObstacles * difficultyFactor,
            maxDistanceBetweenObstacles * difficultyFactor
        );
        
        Debug.Log($"Spawned obstacle at X: {xPosition}, Next in: {nextSpawnDistance}m");
    }
    
    GameObject ChooseObstaclePrefab()
    {
        // As difficulty increases, favor harder patterns
        if (obstaclePrefabs.Length == 1)
            return obstaclePrefabs[0];
        
        // Simple selection based on difficulty
        // Easy obstacles (index 0) more common at low difficulty
        // Hard obstacles (higher indices) more common at high difficulty
        
        float random = Random.value;
        int index = 0;
        
        if (currentDifficulty < 0.3f)
        {
            // Mostly easy obstacles
            index = random < 0.8f ? 0 : Random.Range(1, obstaclePrefabs.Length);
        }
        else if (currentDifficulty < 0.7f)
        {
            // Mix of all obstacles
            index = Random.Range(0, obstaclePrefabs.Length);
        }
        else
        {
            // Mostly harder obstacles
            index = random < 0.8f ? Random.Range(1, obstaclePrefabs.Length) : 0;
        }
        
        return obstaclePrefabs[Mathf.Clamp(index, 0, obstaclePrefabs.Length - 1)];
    }
    
    void SpawnInitialObstacles()
    {
        // Spawn a few obstacles ahead of the player at start
        float currentX = lastSpawnX;
        for (int i = 0; i < 3; i++)
        {
            SpawnObstacle(currentX);
            currentX += nextSpawnDistance;
        }
    }
    
    void CleanupObstacles(float playerX)
    {
        // Remove obstacles that are too far behind the player
        float cleanupDistance = 10f;
        
        for (int i = activeObstacles.Count - 1; i >= 0; i--)
        {
            if (activeObstacles[i] == null)
            {
                activeObstacles.RemoveAt(i);
                continue;
            }
            
            if (activeObstacles[i].transform.position.x < playerX - cleanupDistance)
            {
                Destroy(activeObstacles[i]);
                activeObstacles.RemoveAt(i);
            }
        }
        
        // Also limit max active obstacles
        while (activeObstacles.Count > maxObstaclesActive)
        {
            if (activeObstacles[0] != null)
                Destroy(activeObstacles[0]);
            activeObstacles.RemoveAt(0);
        }
    }
    
    void UpdateDifficulty()
    {
        // Increase difficulty over time
        currentDifficulty = Mathf.Clamp01(currentDifficulty + difficultyIncreaseRate * Time.deltaTime / 60f);
        
        // Could also increase based on score
        if (ScoreManager.Instance != null)
        {
            int score = ScoreManager.Instance.GetCurrentScore();
            float scoreDifficulty = score / 10000f; // Every 10k points = 1.0 difficulty
            currentDifficulty = Mathf.Max(currentDifficulty, scoreDifficulty);
        }
        
        currentDifficulty = Mathf.Clamp01(currentDifficulty);
    }
    
    void OnGameStart()
    {
        // Clear all obstacles
        foreach (var obstacle in activeObstacles)
        {
            if (obstacle != null)
                Destroy(obstacle);
        }
        activeObstacles.Clear();
        
        // Reset difficulty
        currentDifficulty = startingDifficulty;
        
        // Reset spawn position
        if (player != null)
        {
            lastSpawnX = player.position.x + spawnAheadDistance;
            SpawnInitialObstacles();
        }
    }
    
    void OnGameOver()
    {
        // Could do something special on game over
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStart -= OnGameStart;
            GameManager.Instance.OnGameRestart -= OnGameStart;
            GameManager.Instance.OnGameOver -= OnGameOver;
        }
    }
    
    // Public methods for external control
    public float GetCurrentDifficulty() => currentDifficulty;
    
    public void SetDifficulty(float difficulty)
    {
        currentDifficulty = Mathf.Clamp01(difficulty);
    }
}