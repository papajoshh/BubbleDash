using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MainMenuUI : MonoBehaviour
{
    [Header("Main Menu Panel")]
    public GameObject mainMenuPanel;
    
    [Header("Menu Buttons")]
    public Button startGameButton;
    public Button upgradesButton;
    public Button settingsButton;
    public Button quitButton;
    
    [Header("Info Display")]
    public TextMeshProUGUI totalCoinsText;
    public TextMeshProUGUI gameVersionText;
    public TextMeshProUGUI highScoreDisplayText;
    
    [Header("Title Elements")]
    public TextMeshProUGUI gameTitleText;
    public Image logoImage;
    
    [Header("Animation Settings")]
    public float buttonAnimationDelay = 0.1f;
    public float titleAnimationDuration = 1f;
    
    void Awake()
    {
        // Setup button listeners
        if (startGameButton != null)
            startGameButton.onClick.AddListener(StartGame);
            
        if (upgradesButton != null)
            upgradesButton.onClick.AddListener(OpenUpgrades);
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }
    
    void Start()
    {
        // Initialize display
        UpdateCoinDisplay();
        UpdateHighScoreDisplay();
        UpdateVersionText();
        
        // Subscribe to coin changes
        if (CoinSystem.Instance != null)
        {
            CoinSystem.Instance.OnCoinsChanged += UpdateCoinDisplay;
        }
        
        // Show main menu by default
        ShowMainMenu();
        
        // Start entrance animations
        AnimateMenuEntrance();
    }
    
    void UpdateCoinDisplay()
    {
        if (totalCoinsText != null && CoinSystem.Instance != null)
        {
            int coins = CoinSystem.Instance.GetCurrentCoins();
            totalCoinsText.text = $"💰 Coins: {coins:N0}";
        }
    }
    
    void UpdateCoinDisplay(int coins)
    {
        if (totalCoinsText != null)
        {
            totalCoinsText.text = $"💰 Coins: {coins:N0}";
            
            // Pulse animation when coins change
            totalCoinsText.transform.DOScale(1.1f, 0.2f)
                .OnComplete(() => totalCoinsText.transform.DOScale(1f, 0.1f));
        }
    }
    
    void UpdateHighScoreDisplay()
    {
        if (highScoreDisplayText != null && ScoreManager.Instance != null)
        {
            int highScore = ScoreManager.Instance.GetHighScore();
            if (highScore > 0)
            {
                highScoreDisplayText.text = $"🏆 Best Score: {highScore:N0}";
                highScoreDisplayText.gameObject.SetActive(true);
            }
            else
            {
                highScoreDisplayText.gameObject.SetActive(false);
            }
        }
    }
    
    void UpdateVersionText()
    {
        if (gameVersionText != null)
        {
            gameVersionText.text = $"v{Application.version}";
        }
    }
    
    void AnimateMenuEntrance()
    {
        if (gameTitleText != null)
        {
            // Title slide down and fade in
            gameTitleText.transform.DOMoveY(gameTitleText.transform.position.y + 100f, 0f);
            gameTitleText.DOFade(0f, 0f);
            
            gameTitleText.transform.DOMoveY(gameTitleText.transform.position.y - 100f, titleAnimationDuration)
                .SetEase(Ease.OutBounce);
            gameTitleText.DOFade(1f, titleAnimationDuration * 0.7f);
        }
        
        if (logoImage != null)
        {
            // Logo scale in
            logoImage.transform.localScale = Vector3.zero;
            logoImage.transform.DOScale(1f, titleAnimationDuration * 0.8f)
                .SetDelay(0.2f)
                .SetEase(Ease.OutBack);
        }
        
        // Animate buttons in sequence
        AnimateButtons();
    }
    
    void AnimateButtons()
    {
        Button[] buttons = { startGameButton, upgradesButton, settingsButton, quitButton };
        
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                // Start invisible and scaled down
                buttons[i].transform.localScale = Vector3.zero;
                
                // Animate in with delay
                float delay = (i + 1) * buttonAnimationDelay + titleAnimationDuration * 0.5f;
                buttons[i].transform.DOScale(1f, 0.3f)
                    .SetDelay(delay)
                    .SetEase(Ease.OutBack);
            }
        }
        
        // Animate info texts
        if (totalCoinsText != null)
        {
            totalCoinsText.DOFade(0f, 0f);
            totalCoinsText.DOFade(1f, 0.5f).SetDelay(titleAnimationDuration + 0.3f);
        }
        
        if (highScoreDisplayText != null)
        {
            highScoreDisplayText.DOFade(0f, 0f);
            highScoreDisplayText.DOFade(1f, 0.5f).SetDelay(titleAnimationDuration + 0.4f);
        }
    }
    
    // Button handlers
    void StartGame()
    {
        // Play sound
        if (SimpleSoundManager.Instance != null)
        {
            SimpleSoundManager.Instance.PlayButtonClick();
        }
        
        // Hide main menu
        HideMainMenu();
        
        // Start game
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
        
        Debug.Log("Starting new game from main menu");
    }
    
    void OpenUpgrades()
    {
        // Play sound
        if (SimpleSoundManager.Instance != null)
        {
            SimpleSoundManager.Instance.PlayButtonClick();
        }
        
        // Open upgrade menu
        UpgradeUI upgradeUI = FindObjectOfType<UpgradeUI>(true);
        if (upgradeUI != null)
        {
            upgradeUI.OpenUpgradeMenuFromMainMenu();
        }
        
        Debug.Log("Opening upgrades from main menu");
    }
    
    void OpenSettings()
    {
        // Play sound
        if (SimpleSoundManager.Instance != null)
        {
            SimpleSoundManager.Instance.PlayButtonClick();
        }
        
        // TODO: Implement settings menu
        Debug.Log("Settings menu not implemented yet");
        
        // Placeholder: Could open a settings panel here
        // SettingsUI settingsUI = FindObjectOfType<SettingsUI>(true);
        // if (settingsUI != null)
        //     settingsUI.OpenSettings();
    }
    
    void QuitGame()
    {
        // Play sound
        if (SimpleSoundManager.Instance != null)
        {
            SimpleSoundManager.Instance.PlayButtonClick();
        }
        
        Debug.Log("Quitting game from main menu");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    // Menu visibility
    public void ShowMainMenu()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
            
            // Update displays when showing
            UpdateCoinDisplay();
            UpdateHighScoreDisplay();
        }
        
        // Set game state to menu
        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentState = GameState.Menu;
        }
    }
    
    public void HideMainMenu()
    {
        if (mainMenuPanel != null)
        {
            // Animate out
            mainMenuPanel.transform.DOScale(0.9f, 0.2f)
                .SetEase(Ease.InQuad)
                .OnComplete(() => {
                    mainMenuPanel.SetActive(false);
                    mainMenuPanel.transform.localScale = Vector3.one; // Reset scale
                });
        }
    }
    
    // Called when returning from game to main menu
    public void ReturnToMainMenu()
    {
        ShowMainMenu();
        
        // Refresh data
        UpdateCoinDisplay();
        UpdateHighScoreDisplay();
        
        // Reset game state
        Time.timeScale = 1f;
        
        Debug.Log("Returned to main menu");
    }
    
    // Public methods for external calls
    public void OnGameOverReturnToMenu()
    {
        ReturnToMainMenu();
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (CoinSystem.Instance != null)
        {
            CoinSystem.Instance.OnCoinsChanged -= UpdateCoinDisplay;
        }
    }
}