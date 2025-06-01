using UnityEngine;

public class ParallaxSeamlessDouble : MonoBehaviour
{
    [Header("Parallax Settings")]
    public Transform cameraTransform;
    public float parallaxSpeedX = 0.5f;
    
    [Header("Sprite Settings")]
    public Sprite backgroundSprite;
    public Vector2 spriteSize = new Vector2(30, 10);
    public int sortingOrder = -10;
    public Color spriteColor = Color.white;
    
    [Header("Auto Setup")]
    public bool autoFindCamera = true;
    
    [Header("Debug")]
    public bool showDebugInfo = false;
    
    private GameObject spriteA;
    private GameObject spriteB;
    private SpriteRenderer rendererA;
    private SpriteRenderer rendererB;
    
    private Vector3 startCameraPos;
    private float spriteWidth;
    private Vector3 originalPosition;
    
    void Start()
    {
        // Auto find camera
        if (autoFindCamera && cameraTransform == null)
        {
            cameraTransform = Camera.main?.transform;
            if (cameraTransform == null)
                cameraTransform = FindObjectOfType<CameraFollow>()?.transform;
        }
        
        // Store initial camera position
        if (cameraTransform != null)
        {
            startCameraPos = cameraTransform.position;
        }
        
        // Store original position
        originalPosition = transform.position;
        
        spriteWidth = spriteSize.x;
        CreateDoubleSprites();
    }
    
    void CreateDoubleSprites()
    {
        // Create sprite A
        spriteA = new GameObject(gameObject.name + "_A");
        spriteA.transform.SetParent(transform);
        rendererA = spriteA.AddComponent<SpriteRenderer>();
        ConfigureRenderer(rendererA);
        
        // Create sprite B
        spriteB = new GameObject(gameObject.name + "_B");
        spriteB.transform.SetParent(transform);
        rendererB = spriteB.AddComponent<SpriteRenderer>();
        ConfigureRenderer(rendererB);
        
        // Position sprites side by side initially
        Vector3 basePos = transform.position;
        spriteA.transform.position = basePos;
        spriteB.transform.position = new Vector3(basePos.x + spriteWidth, basePos.y, basePos.z);
        
        if (showDebugInfo)
        {
            Debug.Log($"{gameObject.name}: Created double sprites at {basePos.x} and {basePos.x + spriteWidth}");
        }
    }
    
    void ConfigureRenderer(SpriteRenderer renderer)
    {
        renderer.sprite = backgroundSprite;
        renderer.drawMode = SpriteDrawMode.Tiled;
        renderer.size = spriteSize;
        renderer.sortingOrder = sortingOrder;
        renderer.color = spriteColor;
    }
    
    void Update()
    {
        if (cameraTransform == null) return;
        
        // Calculate parallax movement from camera
        Vector3 cameraDelta = cameraTransform.position - startCameraPos;
        float parallaxOffset = cameraDelta.x * parallaxSpeedX;
        
        // Update both sprites with parallax movement
        Vector3 basePos = transform.position;
        spriteA.transform.position = new Vector3(basePos.x + parallaxOffset, basePos.y, basePos.z);
        spriteB.transform.position = new Vector3(basePos.x + parallaxOffset + spriteWidth, basePos.y, basePos.z);
        
        // Handle infinite scrolling
        HandleInfiniteScroll();
    }
    
    void HandleInfiniteScroll()
    {
        float cameraX = cameraTransform.position.x;
        float cameraLeftEdge = cameraX - 15f; // Behind camera threshold
        
        Vector3 posA = spriteA.transform.position;
        Vector3 posB = spriteB.transform.position;
        
        // If sprite A is too far behind, move it ahead of sprite B
        if (posA.x + spriteWidth < cameraLeftEdge)
        {
            spriteA.transform.position = new Vector3(posB.x + spriteWidth, posA.y, posA.z);
            if (showDebugInfo)
            {
                Debug.Log($"{gameObject.name}: Moved sprite A from {posA.x:F1} to {posB.x + spriteWidth:F1}");
            }
        }
        // If sprite B is too far behind, move it ahead of sprite A
        else if (posB.x + spriteWidth < cameraLeftEdge)
        {
            spriteB.transform.position = new Vector3(posA.x + spriteWidth, posB.y, posB.z);
            if (showDebugInfo)
            {
                Debug.Log($"{gameObject.name}: Moved sprite B from {posB.x:F1} to {posA.x + spriteWidth:F1}");
            }
        }
    }
    
    
    // Public methods
    public void SetParallaxSpeed(float speed)
    {
        parallaxSpeedX = speed;
    }
    
    public void SetSpriteColor(Color color)
    {
        spriteColor = color;
        if (rendererA != null) rendererA.color = color;
        if (rendererB != null) rendererB.color = color;
    }
    
    public void SetSpriteSize(Vector2 size)
    {
        spriteSize = size;
        spriteWidth = size.x;
        if (rendererA != null) rendererA.size = size;
        if (rendererB != null) rendererB.size = size;
    }
    
    public void ResetPosition()
    {
        if (cameraTransform == null) return;
        
        // Reset transform to original position
        transform.position = originalPosition;
        
        // Reset camera position reference
        startCameraPos = cameraTransform.position;
        
        // Reset sprite positions
        if (spriteA != null)
            spriteA.transform.position = originalPosition;
        if (spriteB != null)
            spriteB.transform.position = new Vector3(originalPosition.x + spriteWidth, originalPosition.y, originalPosition.z);
    }
}