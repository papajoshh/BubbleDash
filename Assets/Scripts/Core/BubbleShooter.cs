using UnityEngine;
using UnityEngine.UI;

public class BubbleShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bubblePrefab;
    public Transform shootPoint;
    public float shootForce = 25f; // Aumentado de 10 a 25
    public float bubbleScale = 0.5f; // Escala para burbujas más pequeñas
    public LineRenderer trajectoryLine;
    
    [Header("Aiming")]
    public int trajectoryPoints = 30;
    public float trajectoryTimeStep = 0.1f;
    public LayerMask obstacleLayer = 1;
    
    [Header("Bubble Queue")]
    public BubbleColor nextBubbleColor;
    public UnityEngine.UI.Image nextBubbleUIImage; // Para UI Canvas
    
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
        
        // Configure physics for better shooting
        bubbleRb.mass = 0.5f; // Lighter mass for better flight
        bubbleRb.gravityScale = 0.8f; // Slightly less gravity
        bubbleRb.drag = 0.5f; // Some air resistance
        
        // Apply force
        bubbleRb.AddForce(aimDirection * shootForce, ForceMode2D.Impulse);
        
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
        Invoke(nameof(EnableShooting), 0.5f);
        GenerateNextBubble();
    }
    
    void ShowTrajectory()
    {
        Vector3[] points = new Vector3[trajectoryPoints];
        Vector3 startPos = shootPoint.position;
        Vector3 velocity = aimDirection * shootForce;
        
        for (int i = 0; i < trajectoryPoints; i++)
        {
            float time = i * trajectoryTimeStep;
            // Corrección: convertir Physics2D.gravity a Vector3
            Vector3 gravity = new Vector3(Physics2D.gravity.x, Physics2D.gravity.y, 0);
            points[i] = startPos + velocity * time + 0.5f * gravity * time * time;
        }
        
        trajectoryLine.positionCount = trajectoryPoints;
        trajectoryLine.SetPositions(points);
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
        
        // Update UI Image preview with actual bubble sprite
        if (nextBubbleUIImage != null)
        {
            // Get bubble sprites from prefab
            Bubble tempBubble = bubblePrefab?.GetComponent<Bubble>();
            if (tempBubble != null)
            {
                Sprite bubbleSprite = null;
                Color tintColor = Color.white; // Default white = no tint
                
                switch (nextBubbleColor)
                {
                    case BubbleColor.Red:
                        bubbleSprite = tempBubble.redBubbleSprite;
                        if (bubbleSprite == null) tintColor = Color.red;
                        break;
                    case BubbleColor.Blue:
                        bubbleSprite = tempBubble.blueBubbleSprite;
                        if (bubbleSprite == null) tintColor = new Color(0.2f, 0.4f, 1f);
                        break;
                    case BubbleColor.Green:
                        bubbleSprite = tempBubble.greenBubbleSprite;
                        if (bubbleSprite == null) tintColor = new Color(0.2f, 0.8f, 0.2f);
                        break;
                    case BubbleColor.Yellow:
                        bubbleSprite = tempBubble.yellowBubbleSprite;
                        if (bubbleSprite == null) tintColor = new Color(1f, 0.9f, 0.2f);
                        break;
                }
                
                // Apply sprite if available
                if (bubbleSprite != null)
                {
                    nextBubbleUIImage.sprite = bubbleSprite;
                    nextBubbleUIImage.color = Color.white; // No tint when using colored sprite
                }
                else
                {
                    // Fallback: use any available sprite and tint it
                    if (tempBubble.redBubbleSprite != null)
                        nextBubbleUIImage.sprite = tempBubble.redBubbleSprite;
                    else if (tempBubble.blueBubbleSprite != null)
                        nextBubbleUIImage.sprite = tempBubble.blueBubbleSprite;
                    else if (tempBubble.greenBubbleSprite != null)
                        nextBubbleUIImage.sprite = tempBubble.greenBubbleSprite;
                    else if (tempBubble.yellowBubbleSprite != null)
                        nextBubbleUIImage.sprite = tempBubble.yellowBubbleSprite;
                        
                    nextBubbleUIImage.color = tintColor;
                }
            }
            else
            {
                // No bubble component, just use color
                switch (nextBubbleColor)
                {
                    case BubbleColor.Red:
                        nextBubbleUIImage.color = Color.red;
                        break;
                    case BubbleColor.Blue:
                        nextBubbleUIImage.color = new Color(0.2f, 0.4f, 1f);
                        break;
                    case BubbleColor.Green:
                        nextBubbleUIImage.color = new Color(0.2f, 0.8f, 0.2f);
                        break;
                    case BubbleColor.Yellow:
                        nextBubbleUIImage.color = new Color(1f, 0.9f, 0.2f);
                        break;
                }
            }
        }
    }
    
    void EnableShooting()
    {
        canShoot = true;
    }
}