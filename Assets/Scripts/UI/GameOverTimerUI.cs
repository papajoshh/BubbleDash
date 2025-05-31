using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameOverTimerUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject gameOverTimerPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI runTimeText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI coinsEarnedText;
    public TextMeshProUGUI highScoreText;
    
    [Header("Buttons")]
    public Button playAgainButton;
    public Button mainMenuButton;
    
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
        
        // Hide panel initially
        if (gameOverTimerPanel != null)
        {
            gameOverTimerPanel.SetActive(false);
        }
    }
    
    void Start()
    {
        // Subscribe to game over timer event
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOverTimer += ShowGameOverTimer;
        }
    }
    
    void ShowGameOverTimer()
    {
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
            
            // Start entrance animation
            StartCoroutine(AnimateGameOverScreen());
        }
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
        // These would ideally come from a RunStatsManager, but for now we'll use placeholders
        // or derive from existing systems
        
        if (bubblesPopedText != null)
        {
            // Could track this in BubbleManager
            bubblesPopedText.text = "Bubbles Popped: --";
        }
        
        if (maxComboText != null)
        {
            // Could track this in MomentumSystem
            if (MomentumSystem.Instance != null)
            {
                maxComboText.text = $"Max Combo: {MomentumSystem.Instance.GetComboCount()}";
            }
            else
            {
                maxComboText.text = "Max Combo: --";
            }
        }
        
        if (distanceTraveledText != null)
        {
            // Could track this in PlayerController
            distanceTraveledText.text = "Distance: --";
        }
    }
    
    System.Collections.IEnumerator AnimateGameOverScreen()
    {
        // Initial state - everything invisible
        SetAllElementsAlpha(0f);
        
        // Wait initial delay
        yield return new WaitForSecondsRealtime(animationDelay);
        
        // Animate title
        if (titleText != null)
        {
            titleText.DOFade(1f, 0.5f).SetUpdate(true);
            titleText.transform.DOScale(1.2f, 0.3f).SetUpdate(true)
                .OnComplete(() => titleText.transform.DOScale(1f, 0.2f).SetUpdate(true));
        }
        
        yield return new WaitForSecondsRealtime(elementDelay);
        
        // Animate stats
        AnimateElement(runTimeText);
        yield return new WaitForSecondsRealtime(elementDelay);
        
        AnimateElement(finalScoreText);
        yield return new WaitForSecondsRealtime(elementDelay);
        
        AnimateElement(coinsEarnedText);
        yield return new WaitForSecondsRealtime(elementDelay);
        
        AnimateElement(highScoreText);
        yield return new WaitForSecondsRealtime(elementDelay);
        
        // Animate detailed stats
        if (statsContainer != null)
        {
            AnimateElement(bubblesPopedText);
            yield return new WaitForSecondsRealtime(elementDelay * 0.5f);
            
            AnimateElement(maxComboText);
            yield return new WaitForSecondsRealtime(elementDelay * 0.5f);
            
            AnimateElement(distanceTraveledText);
            yield return new WaitForSecondsRealtime(elementDelay);
        }
        
        // Animate buttons
        AnimateElement(playAgainButton?.GetComponent<TextMeshProUGUI>());
        yield return new WaitForSecondsRealtime(elementDelay * 0.5f);
        
        AnimateElement(mainMenuButton?.GetComponent<TextMeshProUGUI>());
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
    
    // Button handlers
    void PlayAgain()
    {
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
        
        // Play sound
        if (SimpleSoundManager.Instance != null)
        {
            SimpleSoundManager.Instance.PlayButtonClick();
        }
    }
    
    
    void GoToMainMenu()
    {
        // Hide panel
        if (gameOverTimerPanel != null)
        {
            gameOverTimerPanel.SetActive(false);
        }
        
        // Resume time and go to menu state
        Time.timeScale = 1f;
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentState = GameState.Menu;
        }
        
        // Play sound
        if (SimpleSoundManager.Instance != null)
        {
            SimpleSoundManager.Instance.PlayButtonClick();
        }
        
        Debug.Log("Returned to Main Menu");
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