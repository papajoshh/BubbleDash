using UnityEngine;
using System.Collections;
using TMPro;

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
    private Vector3 originalCameraPosition;
    
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
            originalCameraPosition = mainCamera.transform.position;
            
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
        
        // Start the pop animation
        StartCoroutine(PopAnimation(popEffect));
    }
    
    IEnumerator PopAnimation(GameObject popObject)
    {
        Transform trans = popObject.transform;
        SpriteRenderer sr = popObject.GetComponent<SpriteRenderer>();
        
        float elapsed = 0f;
        Vector3 originalScale = Vector3.one * 0.5f; // Start from bubble size
        
        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / popDuration;
            
            // Scale up and fade out
            float curveValue = popCurve.Evaluate(progress);
            trans.localScale = originalScale * (1f + curveValue * popScaleMultiplier);
            
            // Fade alpha
            Color color = sr.color;
            color.a = 1f - progress;
            sr.color = color;
            
            yield return null;
        }
        
        Destroy(popObject);
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
        
        StartCoroutine(ComboTextAnimation(comboObj));
    }
    
    IEnumerator ComboTextAnimation(GameObject textObj)
    {
        TextMeshPro tmp = textObj.GetComponent<TextMeshPro>();
        Vector3 startPos = textObj.transform.position;
        
        float elapsed = 0f;
        
        while (elapsed < comboTextDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / comboTextDuration;
            
            // Move up
            textObj.transform.position = startPos + Vector3.up * (comboTextRiseSpeed * progress);
            
            // Scale bounce
            if (comboTextCurve != null && comboTextCurve.keys.Length > 0)
            {
                float scale = comboTextCurve.Evaluate(progress);
                textObj.transform.localScale = Vector3.one * scale;
            }
            
            // Fade out in last 30%
            if (progress > 0.7f)
            {
                float fadeProgress = (progress - 0.7f) / 0.3f;
                Color color = tmp.color;
                color.a = 1f - fadeProgress;
                tmp.color = color;
            }
            
            yield return null;
        }
        
        Destroy(textObj);
    }
    
    // Screen shake for impacts
    public void ShakeScreen(float intensity = -1f, float duration = -1f)
    {
        if (mainCamera == null) return;
        
        if (intensity < 0) intensity = shakeIntensity;
        if (duration < 0) duration = shakeDuration;
        
        StartCoroutine(ScreenShakeCoroutine(intensity, duration));
    }
    
    IEnumerator ScreenShakeCoroutine(float intensity, float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            
            // Reduce intensity over time
            float currentIntensity = intensity * (1f - progress);
            
            // Random shake
            float x = Random.Range(-1f, 1f) * currentIntensity;
            float y = Random.Range(-1f, 1f) * currentIntensity;
            
            mainCamera.transform.position = originalCameraPosition + new Vector3(x, y, 0);
            
            yield return null;
        }
        
        // Reset position
        mainCamera.transform.position = originalCameraPosition;
    }
    
    // Flash sprite color
    public void FlashSprite(SpriteRenderer sprite, Color flashColor, float duration = -1f)
    {
        if (sprite == null) return;
        if (duration < 0) duration = flashDuration;
        
        StartCoroutine(FlashCoroutine(sprite, flashColor, duration));
    }
    
    IEnumerator FlashCoroutine(SpriteRenderer sprite, Color flashColor, float duration)
    {
        Color originalColor = sprite.color;
        sprite.color = flashColor;
        
        yield return new WaitForSeconds(duration);
        
        sprite.color = originalColor;
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
        
        StartCoroutine(ScorePopupAnimation(scoreObj));
    }
    
    IEnumerator ScorePopupAnimation(GameObject scoreObj)
    {
        Vector3 startPos = scoreObj.transform.position;
        TextMeshPro tmp = scoreObj.GetComponent<TextMeshPro>();
        
        float elapsed = 0f;
        float duration = 1f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            
            // Float up
            scoreObj.transform.position = startPos + Vector3.up * progress;
            
            // Fade out
            Color color = tmp.color;
            color.a = 1f - progress;
            tmp.color = color;
            
            yield return null;
        }
        
        Destroy(scoreObj);
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