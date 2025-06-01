using UnityEngine;
using System.Collections.Generic;

public class BubbleManager : MonoBehaviour
{
    public static BubbleManager Instance { get; private set; }
    
    // Simplified: Only tracks shooting bubbles, no collision logic
    private List<ShootingBubble> allBubbles = new List<ShootingBubble>();
    
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
    
    public void RegisterBubble(ShootingBubble bubble)
    {
        if (!allBubbles.Contains(bubble))
        {
            allBubbles.Add(bubble);
        }
    }
    
    public void UnregisterBubble(ShootingBubble bubble)
    {
        allBubbles.Remove(bubble);
    }
    
    // Get count of active bubbles (useful for debugging)
    public int GetActiveBubbleCount()
    {
        return allBubbles.Count;
    }
    
    // Clear all bubbles (useful for game reset)
    public void ClearAllBubbles()
    {
        foreach (var bubble in allBubbles)
        {
            if (bubble != null)
            {
                Destroy(bubble.gameObject);
            }
        }
        allBubbles.Clear();
    }
}