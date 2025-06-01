using UnityEngine;

public class ParallaxSeamless : MonoBehaviour
{
    [Header("Parallax Settings")]
    public Transform cameraTransform;
    public float parallaxSpeedX = 0.5f;
    
    [Header("Sprite Settings")]
    public Sprite backgroundSprite;
    public float spriteWidth = 10f; // Width of one sprite
    public Vector2 spriteSize = new Vector2(10, 5);
    public int sortingOrder = -10;
    
    [Header("Auto Setup")]
    public bool autoFindCamera = true;
    public bool autoCalculateWidth = true;
    
    private GameObject sprite1;
    private GameObject sprite2;
    private SpriteRenderer renderer1;
    private SpriteRenderer renderer2;
    
    private Vector3 startCameraPos;
    private float sprite1StartX;
    private float sprite2StartX;
    
    void Start()
    {
        // Auto find camera
        if (autoFindCamera && cameraTransform == null)
        {
            cameraTransform = Camera.main?.transform;
            if (cameraTransform == null)
                cameraTransform = FindObjectOfType<CameraFollow>()?.transform;
        }
        
        // Auto calculate sprite width
        if (autoCalculateWidth && backgroundSprite != null)
        {
            // Calculate based on sprite bounds and scale
            Bounds bounds = backgroundSprite.bounds;
            spriteWidth = bounds.size.x * spriteSize.x / backgroundSprite.bounds.size.x;
        }
        
        // Store initial camera position
        if (cameraTransform != null)
        {
            startCameraPos = cameraTransform.position;
        }
        
        CreateDoubleSprites();
    }
    
    void CreateDoubleSprites()
    {
        // Create first sprite
        sprite1 = new GameObject(gameObject.name + "_Sprite1");
        sprite1.transform.SetParent(transform);
        renderer1 = sprite1.AddComponent<SpriteRenderer>();
        
        // Create second sprite
        sprite2 = new GameObject(gameObject.name + "_Sprite2");
        sprite2.transform.SetParent(transform);
        renderer2 = sprite2.AddComponent<SpriteRenderer>();
        
        // Configure both renderers
        ConfigureRenderer(renderer1);
        ConfigureRenderer(renderer2);
        
        // Position sprites side by side
        sprite1StartX = transform.position.x;
        sprite2StartX = transform.position.x + spriteWidth;
        
        sprite1.transform.position = new Vector3(sprite1StartX, transform.position.y, transform.position.z);
        sprite2.transform.position = new Vector3(sprite2StartX, transform.position.y, transform.position.z);
    }
    
    void ConfigureRenderer(SpriteRenderer renderer)
    {
        renderer.sprite = backgroundSprite;
        renderer.drawMode = SpriteDrawMode.Tiled;
        renderer.size = spriteSize;
        renderer.sortingOrder = sortingOrder;
    }
    
    void Update()
    {
        if (cameraTransform == null) return;
        
        // Calculate camera movement since start
        Vector3 cameraDelta = cameraTransform.position - startCameraPos;
        float parallaxOffset = cameraDelta.x * parallaxSpeedX;
        
        // Update sprite positions
        float newSprite1X = sprite1StartX + parallaxOffset;
        float newSprite2X = sprite2StartX + parallaxOffset;
        
        sprite1.transform.position = new Vector3(newSprite1X, sprite1.transform.position.y, sprite1.transform.position.z);
        sprite2.transform.position = new Vector3(newSprite2X, sprite2.transform.position.y, sprite2.transform.position.z);
        
        // Handle infinite scrolling - check if sprites moved too far behind camera
        float cameraX = cameraTransform.position.x;
        
        // If sprite1 is too far behind, move it ahead of sprite2
        if (newSprite1X + spriteWidth < cameraX - spriteWidth)
        {
            sprite1StartX = sprite2StartX + spriteWidth;
            // Swap references so sprite1 becomes the one ahead
            SwapSprites();
        }
        // If sprite2 is too far behind, move it ahead of sprite1
        else if (newSprite2X + spriteWidth < cameraX - spriteWidth)
        {
            sprite2StartX = sprite1StartX + spriteWidth;
            // Swap references so sprite2 becomes the one ahead
            SwapSprites();
        }
    }
    
    void SwapSprites()
    {
        // Swap the start positions
        float tempStartX = sprite1StartX;
        sprite1StartX = sprite2StartX;
        sprite2StartX = tempStartX;
        
        // Swap the GameObjects
        GameObject tempSprite = sprite1;
        sprite1 = sprite2;
        sprite2 = tempSprite;
        
        // Swap the renderers
        SpriteRenderer tempRenderer = renderer1;
        renderer1 = renderer2;
        renderer2 = tempRenderer;
    }
    
    // Public methods
    public void SetParallaxSpeed(float speed)
    {
        parallaxSpeedX = speed;
    }
    
    public void SetSpriteSize(Vector2 size)
    {
        spriteSize = size;
        if (renderer1 != null) renderer1.size = size;
        if (renderer2 != null) renderer2.size = size;
    }
    
    public void ResetPosition()
    {
        if (cameraTransform == null) return;
        
        // Reset camera position reference
        startCameraPos = cameraTransform.position;
        
        // Reset sprite positions to their initial arrangement
        sprite1StartX = transform.position.x;
        sprite2StartX = sprite1StartX + spriteWidth;
        
        if (sprite1 != null)
            sprite1.transform.position = new Vector3(sprite1StartX, sprite1.transform.position.y, sprite1.transform.position.z);
        if (sprite2 != null)
            sprite2.transform.position = new Vector3(sprite2StartX, sprite2.transform.position.y, sprite2.transform.position.z);
            
    }
}