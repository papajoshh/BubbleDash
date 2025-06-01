using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class SimpleEffects : MonoBehaviour
{
    public static SimpleEffects Instance { get; private set; }
    
    [Header("Bubble Pop Effect")]
    public float popScaleMultiplier = 1.5f;
    public float popDuration = 0.3f;
    public AnimationCurve popCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    
    [Header("Combo Text Effect")]
    public GameObject comboTextPrefab; // TextMeshPro prefab
    public float comboTextDuration = 1f;
    public float comboTextRiseSpeed = 2f;
    public AnimationCurve comboTextCurve;
    
    [Header("Screen Shake")]
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.2f;
    
    [Header("Color Flash")]
    public float flashDuration = 0.1f;
    
    private Camera mainCamera;
    private CameraFollow cameraFollow;
    private Coroutine currentShakeCoroutine;
    
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
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            cameraFollow = mainCamera.GetComponent<CameraFollow>();
        }
            
        // Create default combo text prefab if not assigned
        if (comboTextPrefab == null)
        {
            CreateDefaultComboTextPrefab();
        }
    }
    
    void CreateDefaultComboTextPrefab()
    {
        GameObject textObj = new GameObject("ComboText");
        TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
        tmp.fontSize = 36;
        tmp.color = Color.yellow;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.sortingOrder = 10;
        
        comboTextPrefab = textObj;
        textObj.SetActive(false);
    }
    
    // Bubble pop effect using scale animation
    public void PlayBubblePop(Vector3 position, Color bubbleColor)
    {
        // Create a temporary object for the effect
        GameObject popEffect = new GameObject("PopEffect");
        popEffect.transform.position = position;
        
        // Add a sprite renderer for visual
        SpriteRenderer sr = popEffect.AddComponent<SpriteRenderer>();
        sr.sprite = GetCircleSprite(); // Helper method to get/create circle sprite
        sr.color = bubbleColor;
        sr.sortingOrder = 5;
        
        // Initial scale
        popEffect.transform.localScale = Vector3.one * 0.5f;
        
        // Scale up animation
        popEffect.transform.DOScale(Vector3.one * (0.5f + popScaleMultiplier), popDuration)
            .SetEase(popCurve);
        
        // Fade out animation
        sr.DOFade(0f, popDuration)
            .OnComplete(() => Destroy(popEffect));
    }
    
    // Show combo text that rises and fades
    public void ShowComboText(Vector3 position, int comboCount)
    {
        if (comboTextPrefab == null) return;
        
        GameObject comboObj = Instantiate(comboTextPrefab, position, Quaternion.identity);
        comboObj.SetActive(true);
        
        TextMeshPro tmp = comboObj.GetComponent<TextMeshPro>();
        if (tmp != null)
        {
            tmp.text = $"COMBO x{comboCount}!";
            
            // Color based on combo size
            if (comboCount >= 10)
                tmp.color = Color.red;
            else if (comboCount >= 5)
                tmp.color = new Color(1f, 0.5f, 0f); // Orange
            else
                tmp.color = Color.yellow;
        }
        
        // Move up animation
        comboObj.transform.DOMoveY(position.y + comboTextRiseSpeed, comboTextDuration)
            .SetEase(Ease.OutQuad);
        
        // Scale animation with curve
        if (comboTextCurve != null && comboTextCurve.keys.Length > 0)
        {
            // Use DOTween's custom ease with AnimationCurve
            comboObj.transform.DOScale(1f, comboTextDuration)
                .SetEase(comboTextCurve);
        }
        else
        {
            // Default bounce scale
            comboObj.transform.DOScale(1.2f, comboTextDuration * 0.3f)
                .SetLoops(2, LoopType.Yoyo);
        }
        
        // Fade out in last 30%
        if (tmp != null)
        {
            Sequence fadeSequence = DOTween.Sequence();
            fadeSequence.AppendInterval(comboTextDuration * 0.7f);
            fadeSequence.Append(tmp.DOFade(0f, comboTextDuration * 0.3f));
        }
        
        // Destroy after all animations
        Destroy(comboObj, comboTextDuration + 0.1f);
    }
    
    // Screen shake for impacts
    public void ShakeScreen(float intensity = -1f, float duration = -1f)
    {
        if (mainCamera == null) return;
        
        if (intensity < 0) intensity = shakeIntensity;
        if (duration < 0) duration = shakeDuration;
        
        // Stop any existing shake
        if (currentShakeCoroutine != null)
        {
            StopCoroutine(currentShakeCoroutine);
            currentShakeCoroutine = null;
        }
        
        // If we have CameraFollow, temporarily disable it during shake
        if (cameraFollow != null)
        {
            currentShakeCoroutine = StartCoroutine(ShakeWithCameraFollow(intensity, duration));
        }
        else
        {
            // Fallback to simple shake
            mainCamera.transform.DOKill();
            mainCamera.transform.DOShakePosition(duration, intensity, 10, 90, false, true);
        }
    }
    
    IEnumerator ShakeWithCameraFollow(float intensity, float duration)
    {
        // Temporarily disable camera follow
        bool wasFollowingX = cameraFollow.followX;
        bool wasFollowingY = cameraFollow.followY;
        cameraFollow.followX = false;
        cameraFollow.followY = false;
        
        // Do the shake
        mainCamera.transform.DOShakePosition(duration, intensity, 10, 90, false, true);
        
        // Wait for shake to complete
        yield return new WaitForSeconds(duration);
        
        // Re-enable camera follow
        cameraFollow.followX = wasFollowingX;
        cameraFollow.followY = wasFollowingY;
        
        // Snap back to correct position
        cameraFollow.SnapToTarget();
        
        currentShakeCoroutine = null;
    }
    
    // Flash sprite color
    public void FlashSprite(SpriteRenderer sprite, Color flashColor, float duration = -1f)
    {
        if (sprite == null) return;
        if (duration < 0) duration = flashDuration;
        
        Color originalColor = sprite.color;
        
        // Create a sequence for the flash effect
        Sequence flashSequence = DOTween.Sequence();
        flashSequence.Append(sprite.DOColor(flashColor, 0f));
        flashSequence.AppendInterval(duration);
        flashSequence.Append(sprite.DOColor(originalColor, 0f));
    }
    
    // Score popup for coins/points
    public void ShowScorePopup(Vector3 position, int score, Color color)
    {
        GameObject scoreObj = new GameObject("ScorePopup");
        scoreObj.transform.position = position;
        
        TextMeshPro tmp = scoreObj.AddComponent<TextMeshPro>();
        tmp.text = $"+{score}";
        tmp.fontSize = 24;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.sortingOrder = 10;
        
        float duration = 1f;
        
        // Float up animation
        scoreObj.transform.DOMoveY(position.y + 1f, duration)
            .SetEase(Ease.OutQuad);
        
        // Fade out animation
        tmp.DOFade(0f, duration)
            .OnComplete(() => Destroy(scoreObj));
        
        // Optional: Add a subtle scale effect
        scoreObj.transform.localScale = Vector3.zero;
        scoreObj.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
    }
    
    // Miss effect - subtle puff for wrong color hits
    public void PlayMissEffect(Vector3 position)
    {
        GameObject missEffect = new GameObject("MissEffect");
        missEffect.transform.position = position;
        
        // Add sprite renderer
        SpriteRenderer sr = missEffect.AddComponent<SpriteRenderer>();
        sr.sprite = GetCircleSprite();
        sr.color = new Color(0.8f, 0.8f, 0.8f, 0.5f); // Light gray, semi-transparent
        sr.sortingOrder = 5;
        
        // Initial scale
        missEffect.transform.localScale = Vector3.one * 0.3f;
        
        float duration = 0.2f; // Shorter than pop
        
        // Subtle scale up
        missEffect.transform.DOScale(Vector3.one * 0.36f, duration)
            .SetEase(Ease.OutQuad);
        
        // Quick fade out
        sr.DOFade(0f, duration)
            .SetEase(Ease.InQuad)
            .OnComplete(() => Destroy(missEffect));
    }
    
    // Helper to get/create a circle sprite
    Sprite GetCircleSprite()
    {
        // Try to find existing circle sprite
        Sprite[] sprites = Resources.FindObjectsOfTypeAll<Sprite>();
        foreach (var sprite in sprites)
        {
            if (sprite.name.ToLower().Contains("circle") || 
                sprite.name.ToLower().Contains("bubble"))
            {
                return sprite;
            }
        }
        
        // Fallback: create a simple square
        Texture2D tex = new Texture2D(32, 32);
        Color[] pixels = new Color[32 * 32];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        tex.SetPixels(pixels);
        tex.Apply();
        
        return Sprite.Create(tex, new Rect(0, 0, 32, 32), Vector2.one * 0.5f);
    }
}