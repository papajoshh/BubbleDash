using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [Header("Score UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI coinText;
    
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
            scoreManager.OnDistanceChanged += UpdateDistanceUI;
        }
        
        if (gameManager != null)
        {
            gameManager.OnGameStart += OnGameStart;
            gameManager.OnGamePause += OnGamePause;
            gameManager.OnGameResume += OnGameResume;
            gameManager.OnGameRestart += OnGameStart;
        }
        
        // Subscribe to coin events
        if (CoinSystem.Instance != null)
        {
            CoinSystem.Instance.OnCoinsChanged += UpdateCoinUI;
            UpdateCoinUI(CoinSystem.Instance.GetCurrentCoins());
        }
        
        // Setup buttons
        if (resumeButton != null)
            resumeButton.onClick.AddListener(() => gameManager.ResumeGame());
            
        if (pauseQuitButton != null)
            pauseQuitButton.onClick.AddListener(() => gameManager.QuitGame());
            
        // Initialize UI
        UpdateScoreUI(0);
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
    
    void UpdateCoinUI(int coins)
    {
        if (coinText != null)
            coinText.text = $"Coins: {coins}";
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
        if (pausePanel != null) pausePanel.SetActive(false);
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
    
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged -= UpdateScoreUI;
            scoreManager.OnDistanceChanged -= UpdateDistanceUI;
        }
        
        if (gameManager != null)
        {
            gameManager.OnGameStart -= OnGameStart;
            gameManager.OnGamePause -= OnGamePause;
            gameManager.OnGameResume -= OnGameResume;
            gameManager.OnGameRestart -= OnGameStart;
        }
        
        if (CoinSystem.Instance != null)
        {
            CoinSystem.Instance.OnCoinsChanged -= UpdateCoinUI;
        }
    }
    
}