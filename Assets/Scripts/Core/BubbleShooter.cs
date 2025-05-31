using UnityEngine;

public class BubbleShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bubblePrefab;
    public Transform shootPoint;
    public float shootForce = 25f; // Aumentado de 10 a 25
    public float bubbleScale = 0.5f; // Escala para burbujas más pequeñas
    public float shootCooldown = 0.2f; // Time between shots
    public LineRenderer trajectoryLine;
    
    [Header("Aiming")]
    public int trajectoryPoints = 30;
    public float trajectoryTimeStep = 0.1f;
    public LayerMask obstacleLayer = 1;
    
    [Header("Bubble Queue")]
    public BubbleColor nextBubbleColor;
    
    [Header("Player Visual Feedback")]
    public SpriteRenderer playerSpriteRenderer; // Reference to player's sprite renderer
    public Sprite playerRedSprite;
    public Sprite playerBlueSprite;
    public Sprite playerGreenSprite;
    public Sprite playerYellowSprite;
    
    private Camera mainCamera;
    private bool isAiming = false;
    private Vector2 aimDirection;
    private bool canShoot = true;
    private GameObject playerObject;
    private Collider2D playerCollider;
    
    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
        
        // Setup trajectory line
        if (trajectoryLine == null)
        {
            trajectoryLine = gameObject.AddComponent<LineRenderer>();
        }
        
        trajectoryLine.material = new Material(Shader.Find("Sprites/Default"));
        // Corrección: usar startColor/endColor en lugar de color
        trajectoryLine.startColor = Color.white;
        trajectoryLine.endColor = Color.white;
        // Corrección: usar startWidth/endWidth en lugar de width
        trajectoryLine.startWidth = 0.05f;
        trajectoryLine.endWidth = 0.05f;
        trajectoryLine.enabled = false;
        
        // Find player reference
        playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            PlayerController pc = FindObjectOfType<PlayerController>();
            if (pc != null) playerObject = pc.gameObject;
        }
        
        if (playerObject != null)
        {
            playerCollider = playerObject.GetComponent<Collider2D>();
            
            // Get player sprite renderer if not assigned
            if (playerSpriteRenderer == null)
            {
                playerSpriteRenderer = playerObject.GetComponent<SpriteRenderer>();
                if (playerSpriteRenderer == null)
                {
                    playerSpriteRenderer = playerObject.GetComponentInChildren<SpriteRenderer>();
                }
            }
        }
        
        // Initialize with random bubble color
        GenerateNextBubble();
    }
    
    void Update()
    {
        HandleInput();
    }
    
    void HandleInput()
    {
        if (!canShoot) return;
        
        // Mouse/Touch input for aiming
        if (Input.GetMouseButtonDown(0))
        {
            StartAiming();
        }
        else if (Input.GetMouseButton(0) && isAiming)
        {
            UpdateAim();
        }
        else if (Input.GetMouseButtonUp(0) && isAiming)
        {
            Shoot();
            StopAiming();
        }
        
        // Touch input for mobile
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    StartAiming();
                    break;
                case TouchPhase.Moved:
                    if (isAiming) UpdateAim();
                    break;
                case TouchPhase.Ended:
                    if (isAiming)
                    {
                        Shoot();
                        StopAiming();
                    }
                    break;
            }
        }
    }
    
    void StartAiming()
    {
        isAiming = true;
        trajectoryLine.enabled = true;
    }
    
    void UpdateAim()
    {
        Vector3 mousePos = GetInputPosition();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;
        
        aimDirection = (worldPos - shootPoint.position).normalized;
        
        // Update trajectory visualization
        ShowTrajectory();
    }
    
    void StopAiming()
    {
        isAiming = false;
        trajectoryLine.enabled = false;
    }
    
    void Shoot()
    {
        if (bubblePrefab == null || shootPoint == null || !canShoot) return;
        
        canShoot = false;
        
        // Instantiate bubble
        GameObject bubble = Instantiate(bubblePrefab, shootPoint.position, Quaternion.identity);
        
        // Apply scale to make bubble smaller
        bubble.transform.localScale = Vector3.one * bubbleScale;
        
        // Set bubble color
        Bubble bubbleComponent = bubble.GetComponent<Bubble>();
        if (bubbleComponent != null)
        {
            bubbleComponent.SetColor(nextBubbleColor);
        }
        
        // Add physics
        Rigidbody2D bubbleRb = bubble.GetComponent<Rigidbody2D>();
        if (bubbleRb == null)
        {
            bubbleRb = bubble.AddComponent<Rigidbody2D>();
        }
        
        // Configure physics for linear movement (no gravity)
        bubbleRb.mass = 0.5f;
        bubbleRb.gravityScale = 0f; // NO GRAVITY - linear movement
        bubbleRb.drag = 0f; // NO DRAG - constant velocity
        bubbleRb.angularDrag = 0f; // No rotation drag
        
        // Set velocity directly for consistent speed
        bubbleRb.velocity = aimDirection * shootForce;
        
        // Ignore collision with player
        if (playerCollider != null)
        {
            Collider2D bubbleCollider = bubble.GetComponent<Collider2D>();
            if (bubbleCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, bubbleCollider);
            }
        }
        
        // Play shoot sound
        if (SimpleSoundManager.Instance != null)
        {
            SimpleSoundManager.Instance.PlayBubbleShoot();
        }
        
        // Notify momentum system
        MomentumSystem momentum = GetComponent<MomentumSystem>();
        if (momentum != null)
        {
            momentum.OnBubbleShot();
        }
        
        // Prepare next bubble
        Invoke(nameof(EnableShooting), shootCooldown);
        GenerateNextBubble();
    }
    
    void ShowTrajectory()
    {
        // Simple straight line trajectory (no gravity)
        Vector3 startPos = shootPoint.position;
        Vector3 endPos = startPos + (Vector3)(aimDirection * shootForce * 0.5f); // Show partial trajectory
        
        trajectoryLine.positionCount = 2;
        trajectoryLine.SetPosition(0, startPos);
        trajectoryLine.SetPosition(1, endPos);
    }
    
    Vector3 GetInputPosition()
    {
        if (Input.touchCount > 0)
        {
            return Input.GetTouch(0).position;
        }
        return Input.mousePosition;
    }
    
    void GenerateNextBubble()
    {
        // Random next bubble color
        nextBubbleColor = (BubbleColor)Random.Range(0, 4);
        
        // Update player color to match next bubble
        UpdatePlayerColor();
    }
    
    void UpdatePlayerColor()
    {
        if (playerSpriteRenderer == null) return;
        
        // Set player sprite to match the next bubble color
        Sprite newSprite = GetPlayerSprite(nextBubbleColor);
        if (newSprite != null)
        {
            playerSpriteRenderer.sprite = newSprite;
            playerSpriteRenderer.color = Color.white; // Reset to white to show sprite colors properly
        }
        else
        {
            // Fallback: if no sprite assigned, use color tinting
            playerSpriteRenderer.color = GetBubbleColor(nextBubbleColor);
        }
    }
    
    Sprite GetPlayerSprite(BubbleColor bubbleColor)
    {
        switch (bubbleColor)
        {
            case BubbleColor.Red:
                return playerRedSprite;
            case BubbleColor.Blue:
                return playerBlueSprite;
            case BubbleColor.Green:
                return playerGreenSprite;
            case BubbleColor.Yellow:
                return playerYellowSprite;
            default:
                return null;
        }
    }
    
    Color GetBubbleColor(BubbleColor bubbleColor)
    {
        switch (bubbleColor)
        {
            case BubbleColor.Red:
                return Color.red;
            case BubbleColor.Blue:
                return new Color(0.2f, 0.4f, 1f); // Nice blue
            case BubbleColor.Green:
                return new Color(0.2f, 0.8f, 0.2f); // Bright green
            case BubbleColor.Yellow:
                return new Color(1f, 0.9f, 0.2f); // Golden yellow
            default:
                return Color.white;
        }
    }
    
    void EnableShooting()
    {
        canShoot = true;
    }
}