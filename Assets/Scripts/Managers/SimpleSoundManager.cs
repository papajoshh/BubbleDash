using UnityEngine;
using System.Collections.Generic;

public class SimpleSoundManager : MonoBehaviour
{
    public static SimpleSoundManager Instance { get; private set; }
    
    [Header("Sound Effects")]
    public AudioClip bubbleShoot;
    public AudioClip bubblePop;
    public AudioClip coinCollect;
    public AudioClip gameOver;
    public AudioClip buttonClick;
    public AudioClip comboSound;
    
    [Header("Background Music")]
    public AudioClip backgroundMusic;
    public bool playMusicOnStart = true;
    public float musicVolume = 0.5f;
    
    [Header("Audio Sources")]
    public AudioSource effectsSource;
    public AudioSource musicSource;
    
    [Header("Settings")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    [Range(0f, 1f)]
    public float effectsVolume = 1f;
    public bool soundEnabled = true;
    
    private Dictionary<string, float> lastPlayedTimes = new Dictionary<string, float>();
    private const float MIN_SOUND_INTERVAL = 0.05f; // Prevent sound spam
    
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
        // Create audio sources if not assigned
        if (effectsSource == null)
        {
            effectsSource = gameObject.AddComponent<AudioSource>();
            effectsSource.playOnAwake = false;
        }
        
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
            musicSource.volume = musicVolume;
        }
        
        // Load settings
        LoadSoundSettings();
        
        // Play background music
        if (playMusicOnStart && backgroundMusic != null)
        {
            PlayMusic(backgroundMusic);
        }
    }
    
    // Main play method with anti-spam
    void PlaySound(AudioClip clip, float volumeScale = 1f, string soundKey = "")
    {
        if (!soundEnabled || clip == null || effectsSource == null) return;
        
        // Anti-spam check
        if (!string.IsNullOrEmpty(soundKey))
        {
            if (lastPlayedTimes.ContainsKey(soundKey))
            {
                if (Time.time - lastPlayedTimes[soundKey] < MIN_SOUND_INTERVAL)
                    return;
            }
            lastPlayedTimes[soundKey] = Time.time;
        }
        
        float finalVolume = masterVolume * effectsVolume * volumeScale;
        effectsSource.PlayOneShot(clip, finalVolume);
    }
    
    // Specific sound methods
    public void PlayBubbleShoot()
    {
        PlaySound(bubbleShoot, 0.8f, "shoot");
    }
    
    public void PlayBubblePop()
    {
        PlaySound(bubblePop, 1f, "pop");
    }
    
    public void PlayBubbleMiss()
    {
        // Soft puff sound for misses - using buttonClick as placeholder or no sound
        PlaySound(buttonClick, 0.3f);
    }
    
    public void PlayCoinCollect()
    {
        PlaySound(coinCollect, 1f);
    }
    
    public void PlayGameOver()
    {
        PlaySound(gameOver, 1f);
        
        // Optional: Stop or fade music on game over
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.volume *= 0.3f; // Reduce music volume
        }
    }
    
    public void PlayButtonClick()
    {
        PlaySound(buttonClick, 0.7f);
    }
    
    public void PlayComboSound(int comboLevel = 1)
    {
        // Play with higher pitch for bigger combos
        if (comboSound != null && effectsSource != null)
        {
            float pitch = 1f + (comboLevel - 1) * 0.1f;
            pitch = Mathf.Clamp(pitch, 1f, 2f);
            
            effectsSource.pitch = pitch;
            PlaySound(comboSound, 1f);
            effectsSource.pitch = 1f; // Reset pitch
        }
    }
    
    // Music control
    public void PlayMusic(AudioClip music)
    {
        if (musicSource == null || music == null) return;
        
        musicSource.clip = music;
        musicSource.volume = musicVolume * masterVolume;
        musicSource.Play();
    }
    
    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }
    
    public void PauseMusic()
    {
        if (musicSource != null)
            musicSource.Pause();
    }
    
    public void ResumeMusic()
    {
        if (musicSource != null)
            musicSource.UnPause();
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
            musicSource.volume = musicVolume * masterVolume;
        SaveSoundSettings();
    }
    
    // Settings
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
        SaveSoundSettings();
    }
    
    public void SetEffectsVolume(float volume)
    {
        effectsVolume = Mathf.Clamp01(volume);
        SaveSoundSettings();
    }
    
    public void ToggleSound()
    {
        soundEnabled = !soundEnabled;
        
        if (!soundEnabled && musicSource != null)
            musicSource.mute = true;
        else if (soundEnabled && musicSource != null)
            musicSource.mute = false;
            
        SaveSoundSettings();
    }
    
    void UpdateAllVolumes()
    {
        if (musicSource != null)
            musicSource.volume = musicVolume * masterVolume;
    }
    
    // Save/Load settings
    void SaveSoundSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("EffectsVolume", effectsVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetInt("SoundEnabled", soundEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    void LoadSoundSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        effectsVolume = PlayerPrefs.GetFloat("EffectsVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        soundEnabled = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
        
        UpdateAllVolumes();
    }
    
    // Integration with game events
    void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver += PlayGameOver;
            GameManager.Instance.OnGamePause += PauseMusic;
            GameManager.Instance.OnGameResume += ResumeMusic;
        }
    }
    
    void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver -= PlayGameOver;
            GameManager.Instance.OnGamePause -= PauseMusic;
            GameManager.Instance.OnGameResume -= ResumeMusic;
        }
    }
}