using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameOverUI : MonoBehaviour
{
    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    
    [Header("Text Elements")]
    public TextMeshProUGUI gameOverTitle;
    public TextMeshProUGUI gameOverReason;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI coinsCollectedText;
    public TextMeshProUGUI waveReachedText;
    public TextMeshProUGUI objectivesCompletedText;
    
    [Header("Buttons")]
    public Button restartButton;
    public Button mainMenuButton;
    
    [Header("Animation")]
    public float animationDuration = 0.5f;
    public float delayBetweenElements = 0.1f;
    
    private int totalObjectivesCompleted = 0;

    void Start()
    {
        // Subscribe to game events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver += ShowGameOver;
        }
        
        // Subscribe to energy events
        if (EnergyManager.Instance != null)
        {
            EnergyManager.OnEnergyEmpty += OnEnergyDepleted;
        }
        
        // Subscribe to objective events to track completions
        ObjectiveManager.OnObjectiveCompleted += OnObjectiveCompleted;
        
        // Setup button listeners
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);
            
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        
        // Hide panel at start
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
    
    void OnObjectiveCompleted(ObjectiveManager.Objective objective)
    {
        totalObjectivesCompleted++;
    }
    
    void OnEnergyDepleted()
    {
        // Update the reason text when energy runs out
        if (gameOverReason != null)
        {
            gameOverReason.text = "ENERGY DEPLETED!";
            gameOverReason.color = new Color(1f, 0.3f, 0.3f); // Red color
        }
    }
    
    void ShowGameOver()
    {
        if (gameOverPanel == null) return;
        
        // Reset for new game over
        totalObjectivesCompleted = 0;
        
        // Activate panel
        gameOverPanel.SetActive(true);
        
        // Pause game silently (without showing pause menu)
        if (GameManager.Instance != null)
        {
            Time.timeScale = 0f; // Pause the game
        }
        
        // Update all statistics
        UpdateGameOverStats();
        
        // Animate panel appearance
        AnimateGameOverPanel();
        
        // Play game over sound
        if (SimpleSoundManager.Instance != null)
        {
            SimpleSoundManager.Instance.PlayGameOver();
        }
    }
    
    void UpdateGameOverStats()
    {
        // Title
        if (gameOverTitle != null)
        {
            gameOverTitle.text = "GAME OVER";
        }
        
        // Score
        if (finalScoreText != null && ScoreManager.Instance != null)
        {
            int finalScore = ScoreManager.Instance.GetScore();
            finalScoreText.text = $"Score: {finalScore:N0}";
        }
        
        // High Score
        if (highScoreText != null && ScoreManager.Instance != null)
        {
            int highScore = ScoreManager.Instance.GetHighScore();
            highScoreText.text = $"Best: {highScore:N0}";
            
            // Highlight if new high score
            if (ScoreManager.Instance.IsNewHighScore())
            {
                highScoreText.text = "NEW BEST!";
                highScoreText.color = Color.yellow;
            }
        }
        
        // Distance
        if (distanceText != null && ScoreManager.Instance != null)
        {
            int distance = ScoreManager.Instance.GetDistance();
            distanceText.text = $"Distance: {distance}m";
        }
        
        // Coins collected this run
        if (coinsCollectedText != null && CoinSystem.Instance != null)
        {
            int coinsThisRun = CoinSystem.Instance.GetCoinsThisRun();
            coinsCollectedText.text = $"Coins: +{coinsThisRun}";
        }
        
        // Wave reached
        if (waveReachedText != null && WaveManager.Instance != null)
        {
            int waveIndex = WaveManager.Instance.GetCurrentWaveIndex();
            string waveName = WaveManager.Instance.GetCurrentWaveName();
            waveReachedText.text = $"Wave {waveIndex + 1}: {waveName}";
        }
        
        // Objectives completed
        if (objectivesCompletedText != null)
        {
            objectivesCompletedText.text = $"Objectives: {totalObjectivesCompleted}";
        }
    }
    
    void AnimateGameOverPanel()
    {
        if (gameOverPanel == null) return;
        
        // Scale animation for panel
        gameOverPanel.transform.localScale = Vector3.zero;
        gameOverPanel.transform.DOScale(1f, animationDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true); // Work with timeScale = 0
        
        // Fade in elements one by one
        CanvasGroup canvasGroup = gameOverPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameOverPanel.AddComponent<CanvasGroup>();
        }
        
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, animationDuration)
            .SetUpdate(true); // Work with timeScale = 0
        
        // Animate text elements appearing
        AnimateTextElement(gameOverTitle, 0f);
        AnimateTextElement(gameOverReason, delayBetweenElements);
        AnimateTextElement(finalScoreText, delayBetweenElements * 2);
        AnimateTextElement(highScoreText, delayBetweenElements * 3);
        AnimateTextElement(distanceText, delayBetweenElements * 4);
        AnimateTextElement(coinsCollectedText, delayBetweenElements * 5);
        AnimateTextElement(waveReachedText, delayBetweenElements * 6);
        AnimateTextElement(objectivesCompletedText, delayBetweenElements * 7);
        
        // Animate buttons
        AnimateButton(restartButton, delayBetweenElements * 8);
        AnimateButton(mainMenuButton, delayBetweenElements * 9);
    }
    
    void AnimateTextElement(TextMeshProUGUI textElement, float delay)
    {
        if (textElement == null) return;
        
        textElement.transform.localScale = Vector3.zero;
        textElement.transform.DOScale(1f, 0.3f)
            .SetDelay(delay)
            .SetEase(Ease.OutBack)
            .SetUpdate(true); // Work with timeScale = 0
    }
    
    void AnimateButton(Button button, float delay)
    {
        if (button == null) return;
        
        button.transform.localScale = Vector3.zero;
        button.transform.DOScale(1f, 0.3f)
            .SetDelay(delay)
            .SetEase(Ease.OutBack)
            .SetUpdate(true); // Work with timeScale = 0
    }
    
    void OnRestartClicked()
    {
        // Play button sound
        if (SimpleSoundManager.Instance != null)
        {
            SimpleSoundManager.Instance.PlayButtonClick();
        }
        
        // Use fade transition if available, otherwise immediate restart
        FadeTransition fadeTransition = FindObjectOfType<FadeTransition>();
        if (fadeTransition != null)
        {
            fadeTransition.FadeToBlackAndExecute(() => {
                // Hide panel and restart
                HidePanelAndRestart();
            });
        }
        else
        {
            // Immediate restart without fade
            HidePanelAndRestart();
        }
    }
    
    void HidePanelAndRestart()
    {
        // Hide panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        // Resume time scale and restart
        Time.timeScale = 1f;
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }
    
    void OnMainMenuClicked()
    {
        // Play button sound
        if (SimpleSoundManager.Instance != null)
        {
            SimpleSoundManager.Instance.PlayButtonClick();
        }
        
        // Use fade transition if available
        FadeTransition fadeTransition = FindObjectOfType<FadeTransition>();
        if (fadeTransition != null)
        {
            fadeTransition.FadeToBlackAndExecute(() => {
                // Hide panel and go to main menu
                HidePanelAndGoToMainMenu();
            });
        }
        else
        {
            // Immediate transition without fade
            HidePanelAndGoToMainMenu();
        }
    }
    
    void HidePanelAndGoToMainMenu()
    {
        // Hide panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        // Go to main menu
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToMainMenu();
        }
        
        // Show main menu UI if available
        MainMenuUI mainMenuUI = MainMenuUI.Instance;
        if (mainMenuUI != null)
        {
            Debug.Log("MainMenuUI found, activating...");
            
            // Try to show main menu
            mainMenuUI.gameObject.SetActive(true);
            
            // If it has a ShowMainMenu method, call it
            var showMainMenuMethod = mainMenuUI.GetType().GetMethod("ShowMainMenu");
            if (showMainMenuMethod != null)
            {
                Debug.Log("Calling ShowMainMenu method...");
                showMainMenuMethod.Invoke(mainMenuUI, null);
            }
            else
            {
                Debug.Log("ShowMainMenu method not found on MainMenuUI");
            }
        }
        else
        {
            Debug.LogWarning("MainMenuUI not found in scene!");
        }
    }
    void OnDestroy()
    {
        // Unsubscribe from events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver -= ShowGameOver;
        }
        
        if (EnergyManager.Instance != null)
        {
            EnergyManager.OnEnergyEmpty -= OnEnergyDepleted;
        }
        
        ObjectiveManager.OnObjectiveCompleted -= OnObjectiveCompleted;
        
        // Clean up button listeners
        if (restartButton != null)
            restartButton.onClick.RemoveListener(OnRestartClicked);
            
        if (mainMenuButton != null)
            mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
    }
}