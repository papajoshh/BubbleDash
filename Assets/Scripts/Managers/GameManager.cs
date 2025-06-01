using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game State")]
    public GameState currentState = GameState.Menu;
    
    [Header("Player Reference")]
    public PlayerController player;
    public MomentumSystem momentumSystem;
    
    [Header("Game Settings")]
    public bool autoStartGame = true;
    
    // Events
    public System.Action OnGameStart;
    public System.Action OnGamePause;
    public System.Action OnGameResume;
    public System.Action OnGameOver;
    public System.Action OnGameOverTimer; // New: Game over by timer
    public System.Action OnGameRestart;
    
    void Awake()
    {
        // Singleton pattern
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
        // Find player components if not assigned
        if (player == null)
        {
            player = FindObjectOfType<PlayerController>();
        }
        
        if (momentumSystem == null)
        {
            momentumSystem = FindObjectOfType<MomentumSystem>();
        }
        if (UpgradeSystem.Instance != null)
            UpgradeSystem.Instance.ApplyStartingUpgrades();
        
        // Auto start if enabled, otherwise stay in menu
        if (autoStartGame)
        {
            StartGame();
        }
        else
        {
            currentState = GameState.Menu;
            Time.timeScale = 1f; // Ensure time isn't paused in menu
            Debug.Log("Game Manager in Menu state - use StartGame() to begin");
        }
    }
    
    void Update()
    {
        HandleInput();
    }
    
    void HandleInput()
    {
        // Pause/Resume with Escape or back button
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Playing)
            {
                PauseGame();
            }
            else if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
        }
        
        // Restart with R key (debug)
        if (Input.GetKeyDown(KeyCode.R) && currentState == GameState.GameOver)
        {
            RestartGame();
        }
    }
    
    public void StartGame()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1f;
        
        // Initialize game systems
        if (player != null)
        {
            player.Restart();
        }
        
        if (momentumSystem != null)
        {
            momentumSystem.ResetMomentum();
        }
        
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
        }
        
        // Apply starting upgrades each game
        if (UpgradeSystem.Instance != null)
            UpgradeSystem.Instance.ApplyStartingUpgrades();
        
        // Timer system removed - to be implemented later
        
        OnGameStart?.Invoke();
        Debug.Log("Game Started!");
    }
    
    public void PauseGame()
    {
        if (currentState != GameState.Playing) return;
        
        currentState = GameState.Paused;
        Time.timeScale = 0f;
        
        OnGamePause?.Invoke();
        Debug.Log("Game Paused");
    }
    
    // Pause game without triggering UI events (legacy - now only used for special cases)
    public void PauseGameSilent()
    {
        if (currentState != GameState.Playing) return;
        
        currentState = GameState.Paused;
        Time.timeScale = 0f;
        
        Debug.Log("Game Paused (Silent)");
    }
    
    public void ResumeGame()
    {
        if (currentState != GameState.Paused) return;
        
        currentState = GameState.Playing;
        Time.timeScale = 1f;
        
        OnGameResume?.Invoke();
        Debug.Log("Game Resumed");
    }
    
    public void GameOver()
    {
        if (currentState == GameState.GameOver) return;
        
        currentState = GameState.GameOver;
        
        // Stop player but don't reset position yet
        if (player != null)
        {
            player.Die();
        }
        
        // Save high score if applicable
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SaveHighScore();
        }
        
        OnGameOver?.Invoke();
    }
    
    public void RestartGame()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1f;
        
        // Reset all systems
        ResetAllSystems();
        
        // Apply starting upgrades each game restart
        if (UpgradeSystem.Instance != null)
            UpgradeSystem.Instance.ApplyStartingUpgrades();
        
        // Timer system removed - to be implemented later
        
        OnGameRestart?.Invoke();
    }
    
    // New: Game Over specifically triggered by timer expiring
    public void TriggerGameOverTimer()
    {
        if (currentState == GameState.GameOver) return;
        
        currentState = GameState.GameOver;
        
        // Stop player but don't reset position yet
        if (player != null)
        {
            player.Die();
        }
        
        // Save high score if applicable
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SaveHighScore();
        }
        
        OnGameOverTimer?.Invoke();
    }
    
    public void GoToMainMenu()
    {
        currentState = GameState.Menu;
        Time.timeScale = 1f;
        
        // Reset all systems for clean menu state
        ResetAllSystems();
    }
    
    void ResetAllSystems()
    {
        // Reset all systems (extracted from RestartGame for reuse)
        if (player != null)
        {
            player.Restart();
        }
        
        if (momentumSystem != null)
        {
            momentumSystem.ResetMomentum();
        }
        
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
        }
        
        // Reset camera position
        CameraFollow cameraFollow = FindObjectOfType<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.SnapToTarget();
        }
        
        // Reset all parallax background systems
        ParallaxBackground[] parallaxBackgrounds = FindObjectsOfType<ParallaxBackground>();
        foreach (var parallax in parallaxBackgrounds)
        {
            parallax.ResetPosition();
        }
        
        ParallaxSeamless[] parallaxSeamless = FindObjectsOfType<ParallaxSeamless>();
        foreach (var parallax in parallaxSeamless)
        {
            parallax.ResetPosition();
        }
        
        ParallaxSeamlessDouble[] parallaxSeamlessDouble = FindObjectsOfType<ParallaxSeamlessDouble>();
        foreach (var parallax in parallaxSeamlessDouble)
        {
            parallax.ResetPosition();
        }
        
        FollowCamera[] followCameras = FindObjectsOfType<FollowCamera>();
        foreach (var followCam in followCameras)
        {
            followCam.ResetPosition();
        }
        
        // Timer reset removed - to be implemented later
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
    
    // State checkers
    public bool IsPlaying() => currentState == GameState.Playing;
    public bool IsPaused() => currentState == GameState.Paused;
    public bool IsGameOver() => currentState == GameState.GameOver;
    public GameState GetGameState() => currentState;
}

public enum GameState
{
    Menu,
    Playing,
    Paused,
    GameOver
}