using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class FadeTransition : MonoBehaviour
{
    public static FadeTransition Instance { get; private set; }
    
    [Header("Fade Settings")]
    public Image fadeImage;
    public float fadeDuration = 0.5f;
    public Color fadeColor = Color.black;
    
    private bool isFading = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Ensure fade image covers full screen
            if (fadeImage != null)
            {
                fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
                fadeImage.raycastTarget = false; // Don't block UI interactions when transparent
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Make sure fade panel is on top
        if (fadeImage != null)
        {
            Canvas canvas = fadeImage.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = 1000; // Very high to be on top of everything
            }
        }
    }
    
    public void FadeToBlackAndExecute(Action onFadeComplete, Action onFadeBackComplete = null)
    {
        if (isFading) return;
        
        isFading = true;
        
        if (fadeImage != null)
        {
            fadeImage.raycastTarget = true; // Block interactions during fade
            
            // Fade to black
            fadeImage.DOFade(1f, fadeDuration)
                .SetUpdate(true) // Work with timeScale = 0
                .OnComplete(() => {
                    // Execute the action (like resetting game or going to menu)
                    onFadeComplete?.Invoke();
                    
                    // Wait a frame then fade back
                    DOVirtual.DelayedCall(0.1f, () => {
                        FadeFromBlack(onFadeBackComplete);
                    }).SetUpdate(true);
                });
        }
        else
        {
            // No fade image, just execute immediately
            onFadeComplete?.Invoke();
            onFadeBackComplete?.Invoke();
            isFading = false;
        }
    }
    
    public void FadeFromBlack(Action onComplete = null)
    {
        if (fadeImage != null)
        {
            fadeImage.DOFade(0f, fadeDuration)
                .SetUpdate(true)
                .OnComplete(() => {
                    fadeImage.raycastTarget = false; // Re-enable UI interactions
                    isFading = false;
                    onComplete?.Invoke();
                });
        }
        else
        {
            isFading = false;
            onComplete?.Invoke();
        }
    }
    
    public void FadeToBlack(Action onComplete = null)
    {
        if (isFading) return;
        
        isFading = true;
        
        if (fadeImage != null)
        {
            fadeImage.raycastTarget = true;
            fadeImage.DOFade(1f, fadeDuration)
                .SetUpdate(true)
                .OnComplete(() => {
                    onComplete?.Invoke();
                });
        }
        else
        {
            isFading = false;
            onComplete?.Invoke();
        }
    }
    
    public void SetFadeColor(Color color)
    {
        fadeColor = color;
        if (fadeImage != null)
        {
            Color currentColor = fadeImage.color;
            fadeImage.color = new Color(color.r, color.g, color.b, currentColor.a);
        }
    }
    
    public bool IsFading() => isFading;
    
    // Immediate fade without animation (for scene transitions)
    public void SetFadeAlpha(float alpha)
    {
        if (fadeImage != null)
        {
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            fadeImage.raycastTarget = alpha > 0f;
        }
    }
}