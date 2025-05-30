using UnityEngine;
using System.Collections.Generic;

public class BubbleManager : MonoBehaviour
{
    public static BubbleManager Instance { get; private set; }
    
    [Header("Bubble Settings")]
    public LayerMask bubbleLayer = 1;
    public float detectionRadius = 0.6f;
    public int minMatchCount = 3;
    
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
        foreach (Bubble bubble in bubblesToDestroy)
        {
            if (bubble != null)
            {
                // Spawn pop effect
                if (popEffect != null)
                {
                    Instantiate(popEffect, bubble.transform.position, Quaternion.identity);
                }
                
                // Play pop sound
                if (popSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(popSound);
                }
                
                // Remove from tracking
                UnregisterBubble(bubble);
                
                // Destroy the bubble
                Destroy(bubble.gameObject);
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