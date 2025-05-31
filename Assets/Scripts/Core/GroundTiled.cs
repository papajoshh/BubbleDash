using UnityEngine;

public class GroundTiled : MonoBehaviour
{
    [Header("Tiled Ground Settings")]
    public float groundY = -4.5f;
    public float groundWidth = 200f; // Ancho total del suelo
    public float groundHeight = 2f; // Alto del suelo
    
    [Header("Visual Settings")]
    public Color groundColor = new Color(0.6f, 0.4f, 0.2f, 1f); // Marrón por defecto
    
    [Header("References")]
    public Transform player;
    public Sprite groundSprite; // El sprite del tile
    
    private SpriteRenderer groundRenderer;
    private BoxCollider2D groundCollider;
    
    void Start()
    {
        // Find player if not assigned
        if (player == null)
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
                player = playerController.transform;
        }
        
        CreateTiledGround();
    }
    
    void CreateTiledGround()
    {
        // Create the main ground object
        GameObject ground = new GameObject("TiledGround");
        ground.transform.SetParent(transform);
        ground.transform.position = new Vector3(0, groundY, 0);
        
        // Add SpriteRenderer with tiled mode
        groundRenderer = ground.AddComponent<SpriteRenderer>();
        
        // Use assigned sprite or try to find tile sprite
        if (groundSprite != null)
        {
            groundRenderer.sprite = groundSprite;
        }
        else
        {
            groundRenderer.sprite = FindTileSprite();
        }
        
        if (groundRenderer.sprite != null)
        {
            // Configure for tiling
            groundRenderer.drawMode = SpriteDrawMode.Tiled;
            groundRenderer.size = new Vector2(groundWidth, groundHeight);
            groundRenderer.sortingOrder = -1;
            
            // Apply color tint
            groundRenderer.color = groundColor;
            
            // Add collider for the entire ground
            groundCollider = ground.AddComponent<BoxCollider2D>();
            groundCollider.size = new Vector2(groundWidth, groundHeight);
            groundCollider.isTrigger = false; // So bubbles can bounce
        }
        else
        {
            Debug.LogWarning("No ground sprite found! Please assign groundSprite in GroundTiled component.");
            CreateFallbackGround(ground);
        }
    }
    
    void CreateFallbackGround(GameObject ground)
    {
        // Create a simple colored ground as fallback
        groundRenderer = ground.GetComponent<SpriteRenderer>();
        if (groundRenderer == null)
            groundRenderer = ground.AddComponent<SpriteRenderer>();
            
        // Create simple texture
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, new Color(0.4f, 0.8f, 0.4f)); // Green color
        tex.Apply();
        
        Sprite simpleSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.one * 0.5f);
        groundRenderer.sprite = simpleSprite;
        groundRenderer.drawMode = SpriteDrawMode.Tiled;
        groundRenderer.size = new Vector2(groundWidth, groundHeight);
        groundRenderer.sortingOrder = -1;
        
        // Apply color tint
        groundRenderer.color = groundColor;
        
        // Add collider
        groundCollider = ground.AddComponent<BoxCollider2D>();
        groundCollider.size = new Vector2(groundWidth, groundHeight);
        groundCollider.isTrigger = false;
    }
    
    Sprite FindTileSprite()
    {
        // Try to find a suitable tile sprite
        Sprite[] allSprites = Resources.FindObjectsOfTypeAll<Sprite>();
        foreach (var sprite in allSprites)
        {
            string spriteName = sprite.name.ToLower();
            if (spriteName.Contains("tile") && 
                !spriteName.Contains("background") &&
                !spriteName.Contains("coin") &&
                !spriteName.Contains("bubble"))
            {
                return sprite;
            }
        }
        return null;
    }
    
    void Update()
    {
        // REMOVED: Don't move with player - let it stay behind like endless runner
        // Ground should remain static in world space
    }
    
    // Public method to change ground color
    public void SetGroundColor(Color newColor)
    {
        groundColor = newColor;
        if (groundRenderer != null)
        {
            groundRenderer.color = groundColor;
        }
    }
    
    // Preset colors for easy use
    public void SetBrownGround()
    {
        SetGroundColor(new Color(0.6f, 0.4f, 0.2f, 1f)); // Marrón
    }
    
    public void SetGreenGround()
    {
        SetGroundColor(new Color(0.4f, 0.8f, 0.4f, 1f)); // Verde
    }
    
    public void SetGrayGround()
    {
        SetGroundColor(new Color(0.5f, 0.5f, 0.5f, 1f)); // Gris
    }
}