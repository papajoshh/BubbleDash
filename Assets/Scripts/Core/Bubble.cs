using UnityEngine;

public class Bubble : MonoBehaviour
{
    [Header("Bubble Properties")]
    public BubbleColor bubbleColor = BubbleColor.Red;
    public BubbleType bubbleType = BubbleType.Normal;
    
    [Header("Physics")]
    public float maxLifetime = 10f;
    public float stopVelocityThreshold = 0.5f;
    
    [Header("Visual")]
    public Renderer bubbleRenderer;
    
    private Rigidbody2D rb;
    private bool hasCollided = false;
    private float lifetime = 0f;
    
    // Color materials (assign these in Unity or load from Resources)
    [Header("Materials")]
    public Material redMaterial;
    public Material blueMaterial;
    public Material greenMaterial;
    public Material yellowMaterial;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        
        if (bubbleRenderer == null)
        {
            bubbleRenderer = GetComponent<Renderer>();
        }
        
        // Register with BubbleManager
        if (BubbleManager.Instance != null)
        {
            BubbleManager.Instance.RegisterBubble(this);
        }
        
        // Set visual appearance
        UpdateAppearance();
        
        // Add collider if not present
        if (GetComponent<Collider2D>() == null)
        {
            CircleCollider2D col = gameObject.AddComponent<CircleCollider2D>();
            col.isTrigger = false; // For physics collisions
        }
    }
    
    void Update()
    {
        lifetime += Time.deltaTime;
        
        // Check if bubble should be destroyed
        if (lifetime > maxLifetime)
        {
            DestroyBubble();
            return;
        }
        
        // Check if bubble has stopped moving
        if (rb != null && rb.velocity.magnitude < stopVelocityThreshold && !hasCollided)
        {
            OnBubbleSettled();
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasCollided) return;
        
        Bubble otherBubble = collision.gameObject.GetComponent<Bubble>();
        if (otherBubble != null)
        {
            OnBubbleCollision(otherBubble);
        }
        else
        {
            // Hit wall or obstacle
            OnWallCollision();
        }
    }
    
    void OnBubbleCollision(Bubble otherBubble)
    {
        hasCollided = true;
        
        // Stop physics movement
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }
        
        // Check for matches
        if (BubbleManager.Instance != null)
        {
            BubbleManager.Instance.CheckForMatches(this);
        }
    }
    
    void OnWallCollision()
    {
        // Bubble hit a wall, maybe bounce or stick depending on game design
        hasCollided = true;
        
        if (rb != null)
        {
            rb.velocity *= 0.5f; // Reduce velocity on wall hit
        }
    }
    
    void OnBubbleSettled()
    {
        hasCollided = true;
        
        // Make bubble static
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        
        // Check for matches
        if (BubbleManager.Instance != null)
        {
            BubbleManager.Instance.CheckForMatches(this);
        }
    }
    
    void UpdateAppearance()
    {
        if (bubbleRenderer == null) return;
        
        Material materialToUse = null;
        
        switch (bubbleColor)
        {
            case BubbleColor.Red:
                materialToUse = redMaterial;
                break;
            case BubbleColor.Blue:
                materialToUse = blueMaterial;
                break;
            case BubbleColor.Green:
                materialToUse = greenMaterial;
                break;
            case BubbleColor.Yellow:
                materialToUse = yellowMaterial;
                break;
        }
        
        if (materialToUse != null)
        {
            bubbleRenderer.material = materialToUse;
        }
        else
        {
            // Fallback: create simple colored material
            Material fallbackMat = new Material(Shader.Find("Standard"));
            fallbackMat.color = GetColorFromEnum(bubbleColor);
            bubbleRenderer.material = fallbackMat;
        }
    }
    
    Color GetColorFromEnum(BubbleColor color)
    {
        switch (color)
        {
            case BubbleColor.Red: return Color.red;
            case BubbleColor.Blue: return Color.blue;
            case BubbleColor.Green: return Color.green;
            case BubbleColor.Yellow: return Color.yellow;
            default: return Color.white;
        }
    }
    
    public void SetColor(BubbleColor newColor)
    {
        bubbleColor = newColor;
        UpdateAppearance();
    }
    
    public void DestroyBubble()
    {
        // Unregister from manager
        if (BubbleManager.Instance != null)
        {
            BubbleManager.Instance.UnregisterBubble(this);
        }
        
        Destroy(gameObject);
    }
    
    void OnDestroy()
    {
        // Ensure we're unregistered
        if (BubbleManager.Instance != null)
        {
            BubbleManager.Instance.UnregisterBubble(this);
        }
    }
}

public enum BubbleColor
{
    Red,
    Blue,
    Green,
    Yellow
}

public enum BubbleType
{
    Normal,
    Rainbow,    // Matches any color
    Bomb,       // Destroys area
    Multiplier  // Score bonus
}