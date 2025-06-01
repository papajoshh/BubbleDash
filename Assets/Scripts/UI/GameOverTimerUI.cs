using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameOverTimerUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject gameOverTimerPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText; // "TIME'S UP!" or "COLLISION!"
    public TextMeshProUGUI runTimeText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI coinsEarnedText;
    public TextMeshProUGUI highScoreText;
    
    [Header("Buttons")]
    public Button playAgainButton;
    public Button mainMenuButton;
    public Button doubleCoinsButton;
    
    [Header("Run Stats")]
    public GameObject statsContainer;
    public TextMeshProUGUI bubblesPopedText;
    public TextMeshProUGUI maxComboText;
    public TextMeshProUGUI distanceTraveledText;
    
    [Header("Animation")]
    public float animationDelay = 1f;
    public float elementDelay = 0.2f;
    
    private int coinsEarnedThisRun = 0;
    
    void Awake()
    {
        // Setup button listeners
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(PlayAgain);
            
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
            
        if (doubleCoinsButton != null)
            doubleCoinsButton.onClick.AddListener(OnDoubleCoinsClicked);
        
    }
    
    void Start()
    {
        SubscribeToEvents();
        // Hide panel initially
        if (gameOverTimerPanel != null)
        {
            gameOverTimerPanel.SetActive(false);
        }
    }
    
    void SubscribeToEvents()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOverTimer += () => ShowGameOver("TIME'S UP!", "Timer expired");
            GameManager.Instance.OnGameOver += () => ShowGameOver("GAME OVER!", "Collision detected");
        }
        else
        {
            Debug.LogError("GameOverTimerUI: GameManager.Instance is null after delay!");
        }
    }
    
    void ShowGameOver(string mainTitle, string reason)
    {
        
        // Set titles based on game over reason
        if (titleText != null)
        {
            titleText.text = mainTitle;
        }
        
        if (subtitleText != null)
        {
            subtitleText.text = reason;
        }
        
        // Collect run statistics
        CollectRunStats();
        
        // Show panel
        if (gameOverTimerPanel != null)
        {
            gameOverTimerPanel.SetActive(true);
            
            // Pause game
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PauseGameSilent();
            }
            
            // Start entrance animation with DOTween (works with timeScale = 0)
            AnimateGameOverScreen();
        }
    }
    
    // Legacy method name for compatibility
    void ShowGameOverTimer()
    {
        ShowGameOver("TIME'S UP!", "Timer expired");
    }
    
    void CollectRunStats()
    {
        // Get run time from timer
        float runTime = 0f;
        if (TimerManager.Instance != null)
        {
            runTime = TimerManager.Instance.GetTimeElapsed();
        }
        
        // Get final score
        int finalScore = 0;
        if (ScoreManager.Instance != null)
        {
            finalScore = ScoreManager.Instance.GetCurrentScore();
        }
        
        // Calculate coins earned (base calculation - could be enhanced)
        coinsEarnedThisRun = Mathf.FloorToInt(finalScore * 0.1f); // 10% of score as coins
        coinsEarnedThisRun = Mathf.Max(coinsEarnedThisRun, 5); // Minimum 5 coins per run
        
        // Add coins to player
        if (CoinSystem.Instance != null)
        {
            CoinSystem.Instance.AddCoins(coinsEarnedThisRun);
        }
        
        // Update UI texts
        UpdateStatTexts(runTime, finalScore);
    }
    
    void UpdateStatTexts(float runTime, int finalScore)
    {
        // Title
        if (titleText != null)
        {
            titleText.text = "TIME'S UP!";
        }
        
        // Run time
        if (runTimeText != null)
        {
            int minutes = Mathf.FloorToInt(runTime / 60f);
            int seconds = Mathf.FloorToInt(runTime % 60f);
            runTimeText.text = $"Run Time: {minutes:00}:{seconds:00}";
        }
        
        // Final score
        if (finalScoreText != null)
        {
            finalScoreText.text = $"Final Score: {finalScore:N0}";
        }
        
        // Coins earned
        if (coinsEarnedText != null)
        {
            coinsEarnedText.text = $"Coins Earned: +{coinsEarnedThisRun}";
        }
        
        // High score
        if (highScoreText != null && ScoreManager.Instance != null)
        {
            int highScore = ScoreManager.Instance.GetHighScore();
            bool isNewHighScore = finalScore > highScore;
            
            if (isNewHighScore)
            {
                highScoreText.text = "NEW HIGH SCORE!";
                highScoreText.color = Color.yellow;
            }
            else
            {
                highScoreText.text = $"Best: {highScore:N0}";
                highScoreText.color = Color.white;
            }
        }
        
        // Additional stats
        UpdateDetailedStats();
    }
    
    void UpdateDetailedStats()
    {
        if (RunStatsManager.Instance != null)
        {
            Debug.Log($"RunStatsManager found. Bubbles: {RunStatsManager.Instance.GetBubblesPopped()}, Combo: {RunStatsManager.Instance.GetMaxCombo()}, Distance: {RunStatsManager.Instance.GetFormattedDistance()}");
            
            if (bubblesPopedText != null)
            {
                bubblesPopedText.text = $"Bubbles Popped: {RunStatsManager.Instance.GetBubblesPopped()}";
            }
            
            if (maxComboText != null)
            {
                maxComboText.text = $"Max Combo: {RunStatsManager.Instance.GetMaxCombo()}";
            }
            
            if (distanceTraveledText != null)
            {
                distanceTraveledText.text = $"Distance: {RunStatsManager.Instance.GetFormattedDistance()}";
            }
        }
        else
        {
            Debug.LogWarning("RunStatsManager.Instance is null! Make sure to add RunStatsManager to the scene.");
            
            // Fallback if RunStatsManager doesn't exist
            if (bubblesPopedText != null)
            {
                bubblesPopedText.text = "Bubbles Popped: --";
            }
            
            if (maxComboText != null)
            {
                maxComboText.text = "Max Combo: --";
            }
            
            if (distanceTraveledText != null)
            {
                distanceTraveledText.text = "Distance: --";
            }
        }
    }
    
    void AnimateGameOverScreen()
    {
        // Initial state - everything invisible
        SetAllElementsAlpha(0f);
        
        // Create DOTween sequence that works with timeScale = 0
        var sequence = DOTween.Sequence().SetUpdate(true);
        
        // Initial delay
        sequence.AppendInterval(animationDelay);
        
        // Animate title
        if (titleText != null)
        {
            sequence.AppendCallback(() => {
                titleText.DOFade(1f, 0.5f).SetUpdate(true);
                titleText.transform.DOScale(1.2f, 0.3f).SetUpdate(true)
                    .OnComplete(() => titleText.transform.DOScale(1f, 0.2f).SetUpdate(true));
            });
            sequence.AppendInterval(elementDelay);
        }
        
        // Animate subtitle
        if (subtitleText != null)
        {
            sequence.AppendCallback(() => AnimateElement(subtitleText));
            sequence.AppendInterval(elementDelay);
        }
        
        // Animate stats
        sequence.AppendCallback(() => AnimateElement(runTimeText));
        sequence.AppendInterval(elementDelay);
        
        sequence.AppendCallback(() => AnimateElement(finalScoreText));
        sequence.AppendInterval(elementDelay);
        
        sequence.AppendCallback(() => AnimateElement(coinsEarnedText));
        sequence.AppendInterval(elementDelay);
        
        sequence.AppendCallback(() => AnimateElement(highScoreText));
        sequence.AppendInterval(elementDelay);
        
        // Animate detailed stats
        if (statsContainer != null)
        {
            sequence.AppendCallback(() => AnimateElement(bubblesPopedText));
            sequence.AppendInterval(elementDelay * 0.5f);
            
            sequence.AppendCallback(() => AnimateElement(maxComboText));
            sequence.AppendInterval(elementDelay * 0.5f);
            
            sequence.AppendCallback(() => AnimateElement(distanceTraveledText));
            sequence.AppendInterval(elementDelay);
        }
        
        // Animate buttons (both text and background)
        sequence.AppendCallback(() => {
            AnimateButton(playAgainButton);
        });
        sequence.AppendInterval(elementDelay * 0.5f);
        
        sequence.AppendCallback(() => {
            AnimateButton(mainMenuButton);
        });
        
        // Show double coins button if available
        if (doubleCoinsButton != null)
        {
            sequence.AppendInterval(elementDelay * 0.5f);
            sequence.AppendCallback(() => {
                AnimateButton(doubleCoinsButton);
            });
        }
    }
    
    void SetAllElementsAlpha(float alpha)
    {
        TextMeshProUGUI[] texts = gameOverTimerPanel.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var text in texts)
        {
            if (text != null)
            {
                Color color = text.color;
                color.a = alpha;
                text.color = color;
            }
        }
        
        Image[] images = gameOverTimerPanel.GetComponentsInChildren<Image>();
        foreach (var image in images)
        {
            if (image != null && image != gameOverTimerPanel.GetComponent<Image>()) // Don't fade background
            {
                Color color = image.color;
                color.a = alpha;
                image.color = color;
            }
        }
    }
    
    void AnimateElement(TextMeshProUGUI element)
    {
        if (element == null) return;
        
        element.DOFade(1f, 0.3f).SetUpdate(true);
        element.transform.localScale = Vector3.zero;
        element.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);
    }
    
    void AnimateButton(Button button)
    {
        if (button == null) return;
        
        // Animate button background image
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.DOFade(1f, 0.3f).SetUpdate(true);
        }
        
        // Animate button text
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            AnimateElement(buttonText);
        }
        
        // Animate entire button transform
        button.transform.localScale = Vector3.zero;
        button.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);
    }
    
    // Button handlers
    void PlayAgain()
    {
        // Play sound
        if (SimpleSoundManager.Instance != null)
        {
            SimpleSoundManager.Instance.PlayButtonClick();
        }
        
        // Use fade transition to smoothly restart
        if (FadeTransition.Instance != null)
        {
            FadeTransition.Instance.FadeToBlackAndExecute(
                onFadeComplete: () => {
                    // Hide panel
                    if (gameOverTimerPanel != null)
                    {
                        gameOverTimerPanel.SetActive(false);
                    }
                    
                    // Restart game
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.RestartGame();
                    }
                }
            );
        }
        else
        {
            // Fallback without fade
            if (gameOverTimerPanel != null)
            {
                gameOverTimerPanel.SetActive(false);
            }
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartGame();
            }
        }
    }
    
    
    void GoToMainMenu()
    {
        // Play sound
        if (SimpleSoundManager.Instance != null)
        {
            SimpleSoundManager.Instance.PlayButtonClick();
        }
        
        // Use fade transition to smoothly go to menu
        if (FadeTransition.Instance != null)
        {
            FadeTransition.Instance.FadeToBlackAndExecute(
                onFadeComplete: () => {
                    // Hide panel
                    if (gameOverTimerPanel != null)
                    {
                        gameOverTimerPanel.SetActive(false);
                    }
                    
                    // Go to main menu (this will reset systems)
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.GoToMainMenu();
                    }
                    
                    // Show main menu UI
                    if (MainMenuUI.Instance != null)
                    {
                        MainMenuUI.Instance.ShowMainMenu();
                    }
                }
            );
        }
        else
        {
            // Fallback without fade
            if (gameOverTimerPanel != null)
            {
                gameOverTimerPanel.SetActive(false);
            }
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GoToMainMenu();
            }
            
            if (MainMenuUI.Instance != null)
            {
                MainMenuUI.Instance.ShowMainMenu();
            }
        }
    }
    
    void OnDoubleCoinsClicked()
    {
        // For now, simulate ad success. Later integrate with real AdManager
        bool adSuccess = true; // TODO: Replace with AdManager.Instance.ShowRewardedAd()
        
        if (adSuccess)
        {
            // Double the coins earned this run
            int bonusCoins = coinsEarnedThisRun;
            
            if (CoinSystem.Instance != null)
            {
                CoinSystem.Instance.AddCoins(bonusCoins);
                Debug.Log($"Double coins bonus: +{bonusCoins} coins!");
            }
            
            // Update display
            if (coinsEarnedText != null)
            {
                int totalCoins = coinsEarnedThisRun * 2;
                coinsEarnedText.text = $"Coins Earned: {totalCoins} (Doubled!)";
                
                // Animate the text
                coinsEarnedText.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 5, 0.5f).SetUpdate(true);
                coinsEarnedText.DOColor(Color.yellow, 0.3f).SetUpdate(true)
                    .OnComplete(() => coinsEarnedText.DOColor(Color.white, 0.3f).SetUpdate(true));
            }
            
            // Disable button after use
            if (doubleCoinsButton != null)
            {
                doubleCoinsButton.gameObject.SetActive(false);
            }
            
            // Play success sound
            if (SimpleSoundManager.Instance != null)
            {
                SimpleSoundManager.Instance.PlayCoinCollect();
            }
        }
        else
        {
            Debug.Log("Ad failed to show or was skipped");
            // Could show a "Ad failed" message here
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOverTimer -= ShowGameOverTimer;
        }
    }
}