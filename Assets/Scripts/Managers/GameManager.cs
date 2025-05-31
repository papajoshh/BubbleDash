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
        
        // Auto start if enabled
        if (autoStartGame)
        {
            StartGame();
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
        
        // Stop player
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
        Debug.Log("Game Over!");
    }
    
    public void RestartGame()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1f;
        
        // Reset all systems
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
        
        OnGameRestart?.Invoke();
        Debug.Log("Game Restarted!");
    }
    
    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
    
    // State checkers
    public bool IsPlaying() => currentState == GameState.Playing;
    public bool IsPaused() => currentState == GameState.Paused;
    public bool IsGameOver() => currentState == GameState.GameOver;
}

public enum GameState
{
    Menu,
    Playing,
    Paused,
    GameOver
}