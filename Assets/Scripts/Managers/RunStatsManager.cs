using UnityEngine;

public class RunStatsManager : MonoBehaviour
{
    public static RunStatsManager Instance { get; private set; }
    
    // Run statistics
    private int bubblesPopped = 0;
    private int maxCombo = 0;
    private float maxDistance = 0f;
    private float startPosition = 0f;
    private bool hasRecordedStart = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        // Subscribe to game events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStart += ResetStats;
            GameManager.Instance.OnGameRestart += ResetStats;
        }
    }
    
    void Update()
    {
        // Track distance continuously
        if (GameManager.Instance != null && GameManager.Instance.IsPlaying())
        {
            TrackDistance();
        }
    }
    
    void TrackDistance()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            // Record start position on first frame of gameplay
            if (!hasRecordedStart)
            {
                startPosition = player.transform.position.x;
                hasRecordedStart = true;
            }
            
            // Calculate distance from start
            float currentDistance = player.transform.position.x - startPosition;
            if (currentDistance > maxDistance)
            {
                maxDistance = currentDistance;
            }
        }
    }
    
    public void ResetStats()
    {
        bubblesPopped = 0;
        maxCombo = 0;
        maxDistance = 0f;
        startPosition = 0f;
        hasRecordedStart = false;
    }
    
    public void OnBubblePopped()
    {
        bubblesPopped++;
        Debug.Log($"RunStatsManager: Single bubble popped. Total: {bubblesPopped}");
    }
    
    public void OnBubblesPopped(int count)
    {
        bubblesPopped += count;
        Debug.Log($"RunStatsManager: {count} bubbles popped. Total: {bubblesPopped}");
    }
    
    public void OnComboAchieved(int comboCount)
    {
        if (comboCount > maxCombo)
        {
            maxCombo = comboCount;
        }
    }
    
    // Getters
    public int GetBubblesPopped() => bubblesPopped;
    public int GetMaxCombo() => maxCombo;
    public float GetMaxDistance() => maxDistance;
    public string GetFormattedDistance() => $"{maxDistance:F1}m";
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStart -= ResetStats;
            GameManager.Instance.OnGameRestart -= ResetStats;
        }
    }
}