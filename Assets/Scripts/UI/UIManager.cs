using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Score UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI comboText;
    
    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI newHighScoreText;
    public Button restartButton;
    public Button quitButton;
    public Button doubleCoinsButton;
    public TextMeshProUGUI coinsEarnedText;
    
    [Header("Pause UI")]
    public GameObject pausePanel;
    public Button resumeButton;
    public Button pauseQuitButton;
    
    [Header("HUD Elements")]
    public GameObject hudPanel;
    public Image speedIndicator;
    public Gradient speedGradient;
    
    private ScoreManager scoreManager;
    private GameManager gameManager;
    private MomentumSystem momentumSystem;
    
    void Start()
    {
        // Get references
        scoreManager = ScoreManager.Instance;
        gameManager = GameManager.Instance;
        momentumSystem = FindObjectOfType<MomentumSystem>();
        
        // Subscribe to events
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged += UpdateScoreUI;
            scoreManager.OnNewHighScore += OnNewHighScore;
            scoreManager.OnDistanceChanged += UpdateDistanceUI;
        }
        
        if (gameManager != null)
        {
            gameManager.OnGameStart += OnGameStart;
            gameManager.OnGameOver += OnGameOver;
            gameManager.OnGamePause += OnGamePause;
            gameManager.OnGameResume += OnGameResume;
            gameManager.OnGameRestart += OnGameStart; // Use same method for restart
        }
        
        // Setup buttons
        if (restartButton != null)
            restartButton.onClick.AddListener(() => {
                gameManager.RestartGame();
                // Show interstitial ad if appropriate
                if (AdManager.Instance != null && AdManager.Instance.ShouldShowInterstitial())
                {
                    AdManager.Instance.ShowInterstitialAd();
                }
            });
            
        if (quitButton != null)
            quitButton.onClick.AddListener(() => gameManager.QuitGame());
            
        if (resumeButton != null)
            resumeButton.onClick.AddListener(() => gameManager.ResumeGame());
            
        if (pauseQuitButton != null)
            pauseQuitButton.onClick.AddListener(() => gameManager.QuitGame());
            
        if (doubleCoinsButton != null)
            doubleCoinsButton.onClick.AddListener(OnDoubleCoinsClicked);
        
        // Initialize UI
        UpdateScoreUI(0);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
    }
    
    void Update()
    {
        UpdateComboUI();
        UpdateSpeedIndicator();
    }
    
    void UpdateScoreUI(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score:N0}";
            
        if (highScoreText != null && scoreManager != null)
            highScoreText.text = $"Best: {scoreManager.GetHighScore():N0}";
    }
    
    void UpdateDistanceUI(int distance)
    {
        if (distanceText != null)
            distanceText.text = $"{distance}m";
    }
    
    void UpdateComboUI()
    {
        if (comboText != null && momentumSystem != null)
        {
            int combo = momentumSystem.GetComboCount();
            if (combo > 0)
            {
                comboText.text = $"Combo x{combo}";
                comboText.gameObject.SetActive(true);
            }
            else
            {
                comboText.gameObject.SetActive(false);
            }
        }
    }
    
    void UpdateSpeedIndicator()
    {
        if (speedIndicator != null && momentumSystem != null)
        {
            float speedPercent = (momentumSystem.GetSpeedMultiplier() - 1f) / 2f; // Normalize to 0-1
            speedIndicator.fillAmount = speedPercent;
            
            if (speedGradient != null)
                speedIndicator.color = speedGradient.Evaluate(speedPercent);
        }
    }
    
    void OnGameStart()
    {
        if (hudPanel != null) hudPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        
        // Reset double coins button for next game
        if (doubleCoinsButton != null)
            doubleCoinsButton.gameObject.SetActive(true);
        
        // Hide new high score text
        if (newHighScoreText != null)
            newHighScoreText.gameObject.SetActive(false);
    }
    
    void OnGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            if (finalScoreText != null && scoreManager != null)
                finalScoreText.text = $"Final Score: {scoreManager.GetCurrentScore():N0}";
                
            // Calculate coins earned (simplified: 1 coin per 100 points)
            int coinsEarned = scoreManager.GetCurrentScore() / 100;
            if (coinsEarnedText != null)
                coinsEarnedText.text = $"Coins Earned: {coinsEarned}";
                
            // Show double coins button if ad is available
            if (doubleCoinsButton != null && AdManager.Instance != null)
            {
                doubleCoinsButton.gameObject.SetActive(AdManager.Instance.IsRewardedAdReady());
            }
        }
    }
    
    void OnGamePause()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true);
    }
    
    void OnGameResume()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }
    
    void OnNewHighScore(int score)
    {
        if (newHighScoreText != null)
        {
            newHighScoreText.text = "NEW HIGH SCORE!";
            newHighScoreText.gameObject.SetActive(true);
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged -= UpdateScoreUI;
            scoreManager.OnNewHighScore -= OnNewHighScore;
            scoreManager.OnDistanceChanged -= UpdateDistanceUI;
        }
        
        if (gameManager != null)
        {
            gameManager.OnGameStart -= OnGameStart;
            gameManager.OnGameOver -= OnGameOver;
            gameManager.OnGamePause -= OnGamePause;
            gameManager.OnGameResume -= OnGameResume;
            gameManager.OnGameRestart -= OnGameStart;
        }
    }
    
    void OnDoubleCoinsClicked()
    {
        if (AdManager.Instance != null)
        {
            AdManager.Instance.ShowRewardedAd((success) => {
                if (success)
                {
                    // Double the coins
                    int coinsEarned = scoreManager.GetCurrentScore() / 100;
                    int bonusCoins = coinsEarned;
                    
                    // Update display
                    if (coinsEarnedText != null)
                        coinsEarnedText.text = $"Coins Earned: {coinsEarned * 2} (Doubled!)";
                    
                    // TODO: Add coins to player's currency system
                    PlayerPrefs.SetInt("TotalCoins", PlayerPrefs.GetInt("TotalCoins", 0) + bonusCoins);
                    PlayerPrefs.Save();
                    
                    // Disable button after use
                    if (doubleCoinsButton != null)
                        doubleCoinsButton.gameObject.SetActive(false);
                }
            });
        }
    }
}