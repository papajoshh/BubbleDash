using UnityEngine;

public class BubbleShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bubblePrefab;
    public Transform shootPoint;
    public float shootForce = 10f;
    public LineRenderer trajectoryLine;
    
    [Header("Aiming")]
    public int trajectoryPoints = 30;
    public float trajectoryTimeStep = 0.1f;
    public LayerMask obstacleLayer = 1;
    
    [Header("Bubble Queue")]
    public BubbleColor nextBubbleColor;
    public Transform nextBubbleDisplay;
    
    private Camera mainCamera;
    private bool isAiming = false;
    private Vector2 aimDirection;
    private bool canShoot = true;
    
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
        
        // Apply force
        bubbleRb.AddForce(aimDirection * shootForce, ForceMode2D.Impulse);
        
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
        
        // Update visual preview if exists
        if (nextBubbleDisplay != null)
        {
            SpriteRenderer previewSprite = nextBubbleDisplay.GetComponent<SpriteRenderer>();
            if (previewSprite != null)
            {
                // Set preview color
                switch (nextBubbleColor)
                {
                    case BubbleColor.Red:
                        previewSprite.color = Color.red;
                        break;
                    case BubbleColor.Blue:
                        previewSprite.color = Color.blue;
                        break;
                    case BubbleColor.Green:
                        previewSprite.color = Color.green;
                        break;
                    case BubbleColor.Yellow:
                        previewSprite.color = Color.yellow;
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