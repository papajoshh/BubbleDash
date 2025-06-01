using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    
    public int currentScore { get; private set; }
    public int highScore { get; private set; }
    public int distanceScore { get; private set; }
    public int bubbleScore { get; private set; }
    
    [Header("Score Values")]
    public int pointsPerMeter = 1;
    public int pointsPerBubble = 10;
    public int comboMultiplier = 5;
    
    [Header("Distance Tracking")]
    public Transform player;
    private Vector3 startPosition;
    private float lastDistance = 0f;
    
    // Events
    public System.Action<int> OnScoreChanged;
    public System.Action<int> OnNewHighScore;
    public System.Action<int> OnDistanceChanged;
    
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
        // Find player if not assigned
        if (player == null)
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                player = playerController.transform;
            }
        }
        
        // Load saved high score
        LoadHighScore();
        
        // Initialize
        ResetScore();
    }
    
    void FixedUpdate() // Changed to FixedUpdate for less frequent updates
    {
        UpdateDistanceScore();
    }
    
    void UpdateDistanceScore()
    {
        if (player == null || !GameManager.Instance.IsPlaying()) return;
        
        float currentDistance = Vector3.Distance(startPosition, player.position);
        float distanceTraveled = currentDistance - lastDistance;
        
        if (distanceTraveled > 0.1f) // Update every 10cm to avoid micro-updates
        {
            int distancePoints = Mathf.FloorToInt(distanceTraveled * pointsPerMeter);
            AddDistanceScore(distancePoints);
            lastDistance = currentDistance;
        }
    }
    
    public void ResetScore()
    {
        currentScore = 0;
        distanceScore = 0;
        bubbleScore = 0;
        
        if (player != null)
        {
            startPosition = player.position;
            lastDistance = 0f;
        }
        
        OnScoreChanged?.Invoke(currentScore);
    }
    
    public void AddDistanceScore(int points)
    {
        distanceScore += points;
        currentScore += points;
        
        OnScoreChanged?.Invoke(currentScore);
        OnDistanceChanged?.Invoke(Mathf.FloorToInt(GetDistanceTraveled()));
    }
    
    public void OnBubbleHit(int comboCount)
    {
        int basePoints = pointsPerBubble;
        int comboBonus = (comboCount - 1) * comboMultiplier;
        int totalPoints = basePoints + comboBonus;
        
        bubbleScore += totalPoints;
        currentScore += totalPoints;
        
        OnScoreChanged?.Invoke(currentScore);
        
        Debug.Log($"Bubble hit! +{totalPoints} points (combo: {comboCount})");
    }
    
    public void SaveHighScore()
    {
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            
            OnNewHighScore?.Invoke(highScore);
            Debug.Log($"New High Score: {highScore}!");
        }
    }
    
    void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }
    
    // Public getters
    public float GetDistanceTraveled()
    {
        if (player == null) return 0f;
        return Vector3.Distance(startPosition, player.position);
    }
    
    public int GetCurrentScore() => currentScore;
    public int GetScore() => currentScore; // Alias for GameOverUI
    public int GetHighScore() => highScore;
    public int GetDistanceScore() => distanceScore;
    public int GetBubbleScore() => bubbleScore;
    public bool IsNewHighScore() => currentScore > highScore;
    public int GetDistance() => Mathf.FloorToInt(GetDistanceTraveled());
    
    // Method to add bonus score (like from coins)
    public void AddBonusScore(int points)
    {
        currentScore += points;
        OnScoreChanged?.Invoke(currentScore);
    }
    
    // Score formatting for UI
    public string GetScoreText()
    {
        return currentScore.ToString("N0");
    }
    
    public string GetDistanceText()
    {
        return $"{GetDistanceTraveled():F0}m";
    }
}