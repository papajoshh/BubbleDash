using UnityEngine;
using System.Collections.Generic;

public class BubbleManager : MonoBehaviour
{
    public static BubbleManager Instance { get; private set; }
    
    [Header("Bubble Settings")]
    public LayerMask bubbleLayer = 1;
    public float detectionRadius = 0.6f;
    public int minMatchCount = 1; // Explode on any same-color contact
    
    [Header("Effects")]
    public GameObject popEffect;
    public AudioClip popSound;
    
    private List<Bubble> allBubbles = new List<Bubble>();
    private AudioSource audioSource;
    
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
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    public void RegisterBubble(Bubble bubble)
    {
        if (!allBubbles.Contains(bubble))
        {
            allBubbles.Add(bubble);
        }
    }
    
    public void UnregisterBubble(Bubble bubble)
    {
        allBubbles.Remove(bubble);
    }
    
    public void CheckForMatches(Bubble startBubble)
    {
        if (startBubble == null) return;
        
        List<Bubble> matchedBubbles = new List<Bubble>();
        List<Bubble> visited = new List<Bubble>();
        
        // Find all connected bubbles of same color
        FindConnectedBubbles(startBubble, matchedBubbles, visited);
        
        // If we have enough matches, destroy them
        if (matchedBubbles.Count >= minMatchCount)
        {
            DestroyBubbles(matchedBubbles);
            
            // Notify momentum system of successful hit
            MomentumSystem momentum = FindObjectOfType<MomentumSystem>();
            if (momentum != null)
            {
                momentum.OnBubbleHit();
            }
        }
        else
        {
            // Notify momentum system of miss
            MomentumSystem momentum = FindObjectOfType<MomentumSystem>();
            if (momentum != null)
            {
                momentum.OnBubbleMiss();
            }
        }
    }
    
    void FindConnectedBubbles(Bubble bubble, List<Bubble> matched, List<Bubble> visited)
    {
        if (bubble == null || visited.Contains(bubble)) return;
        
        visited.Add(bubble);
        matched.Add(bubble);
        
        // Find nearby bubbles of same color
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(
            bubble.transform.position, 
            detectionRadius, 
            bubbleLayer
        );
        
        foreach (Collider2D col in nearbyColliders)
        {
            Bubble nearbyBubble = col.GetComponent<Bubble>();
            if (nearbyBubble != null && 
                nearbyBubble != bubble && 
                nearbyBubble.bubbleColor == bubble.bubbleColor &&
                !visited.Contains(nearbyBubble))
            {
                FindConnectedBubbles(nearbyBubble, matched, visited);
            }
        }
    }
    
    void DestroyBubbles(List<Bubble> bubblesToDestroy)
    {
        // Calculate center position for combo text
        Vector3 centerPos = Vector3.zero;
        Color bubbleColor = Color.white;
        
        foreach (Bubble bubble in bubblesToDestroy)
        {
            if (bubble != null)
            {
                centerPos += bubble.transform.position;
                
                // Get the actual bubble color from the bubble component
                Color effectColor = Color.white;
                switch (bubble.bubbleColor)
                {
                    case BubbleColor.Red:
                        effectColor = Color.red;
                        break;
                    case BubbleColor.Blue:
                        effectColor = new Color(0.2f, 0.4f, 1f);
                        break;
                    case BubbleColor.Green:
                        effectColor = new Color(0.2f, 0.8f, 0.2f);
                        break;
                    case BubbleColor.Yellow:
                        effectColor = new Color(1f, 0.9f, 0.2f);
                        break;
                }
                
                // Play pop effect using SimpleEffects
                if (SimpleEffects.Instance != null)
                {
                    SimpleEffects.Instance.PlayBubblePop(bubble.transform.position, effectColor);
                }
                
                // Spawn pop effect prefab if assigned
                if (popEffect != null)
                {
                    Instantiate(popEffect, bubble.transform.position, Quaternion.identity);
                }
                
                // Play pop sound using SimpleSoundManager
                if (SimpleSoundManager.Instance != null)
                {
                    SimpleSoundManager.Instance.PlayBubblePop();
                }
                // Or use local sound if assigned
                else if (popSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(popSound);
                }
                
                // Remove from tracking
                UnregisterBubble(bubble);
                
                // Destroy the bubble
                Destroy(bubble.gameObject);
            }
        }
        
        // Show combo text if we destroyed multiple bubbles
        if (bubblesToDestroy.Count >= 3 && SimpleEffects.Instance != null)
        {
            centerPos /= bubblesToDestroy.Count;
            
            // Get current combo from momentum system
            int comboCount = 1;
            MomentumSystem momentum = FindObjectOfType<MomentumSystem>();
            if (momentum != null)
                comboCount = momentum.GetComboCount();
                
            SimpleEffects.Instance.ShowComboText(centerPos, comboCount);
            
            // Play combo sound
            if (SimpleSoundManager.Instance != null)
            {
                SimpleSoundManager.Instance.PlayComboSound(bubblesToDestroy.Count);
            }
            
            // Small screen shake for big combos
            if (bubblesToDestroy.Count >= 5)
            {
                SimpleEffects.Instance.ShakeScreen(0.1f, 0.2f);
            }
        }
        
        Debug.Log($"Destroyed {bubblesToDestroy.Count} bubbles!");
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw detection radius for debugging
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}