using UnityEngine;

public class BubbleObstaclePattern : MonoBehaviour
{
    [Header("Pattern Settings")]
    public PatternType patternType = PatternType.Line;
    public int bubbleCount = 3;
    public float spacing = 1f;
    
    [Header("Bubble Settings")]
    public GameObject staticBubblePrefab;
    public bool randomizeColors = true;
    public BubbleColor fixedColor = BubbleColor.Red;
    
    [Header("Movement (Optional)")]
    public bool moveVertically = false;
    public float moveSpeed = 1f;
    public float moveRange = 2f;
    
    private Vector3 startPosition;
    private int moveDirection = 1;
    
    public enum PatternType
    {
        Line,       // Horizontal line
        Column,     // Vertical line
        Triangle,   // Triangle shape
        Square,     // Square shape
        Random      // Random positions
    }
    
    void Start()
    {
        startPosition = transform.position;
        GeneratePattern();
    }
    
    void Update()
    {
        if (moveVertically)
        {
            // Calculate movement
            float movement = moveSpeed * moveDirection * Time.deltaTime;
            Vector3 newPos = transform.position;
            newPos.y += movement;
            
            // Check upper bound
            if (newPos.y > startPosition.y + moveRange)
            {
                newPos.y = startPosition.y + moveRange;
                moveDirection = -1;
            }
            // Check lower bound
            else if (newPos.y < startPosition.y - moveRange)
            {
                newPos.y = startPosition.y - moveRange;
                moveDirection = 1;
            }
            
            transform.position = newPos;
        }
    }
    
    void GeneratePattern()
    {
        if (staticBubblePrefab == null)
        {
            Debug.LogError("No static bubble prefab assigned!");
            return;
        }
        
        switch (patternType)
        {
            case PatternType.Line:
                GenerateLine();
                break;
            case PatternType.Column:
                GenerateColumn();
                break;
            case PatternType.Triangle:
                GenerateTriangle();
                break;
            case PatternType.Square:
                GenerateSquare();
                break;
            case PatternType.Random:
                GenerateRandom();
                break;
        }
    }
    
    void GenerateLine()
    {
        float startX = -(bubbleCount - 1) * spacing * 0.5f;
        
        for (int i = 0; i < bubbleCount; i++)
        {
            Vector3 localPos = new Vector3(startX + i * spacing, 0, 0);
            CreateBubble(localPos);
        }
    }
    
    void GenerateColumn()
    {
        float startY = -(bubbleCount - 1) * spacing * 0.5f;
        
        for (int i = 0; i < bubbleCount; i++)
        {
            Vector3 localPos = new Vector3(0, startY + i * spacing, 0);
            CreateBubble(localPos);
        }
    }
    
    void GenerateTriangle()
    {
        int row = 0;
        int bubbleIndex = 0;
        
        while (bubbleIndex < bubbleCount)
        {
            int bubblesInRow = row + 1;
            float rowStartX = -row * spacing * 0.5f;
            
            for (int i = 0; i < bubblesInRow && bubbleIndex < bubbleCount; i++)
            {
                Vector3 localPos = new Vector3(rowStartX + i * spacing, -row * spacing * 0.8f, 0);
                CreateBubble(localPos);
                bubbleIndex++;
            }
            row++;
        }
    }
    
    void GenerateSquare()
    {
        int sideLength = Mathf.CeilToInt(Mathf.Sqrt(bubbleCount));
        float startX = -(sideLength - 1) * spacing * 0.5f;
        float startY = -(sideLength - 1) * spacing * 0.5f;
        
        int bubbleIndex = 0;
        for (int y = 0; y < sideLength && bubbleIndex < bubbleCount; y++)
        {
            for (int x = 0; x < sideLength && bubbleIndex < bubbleCount; x++)
            {
                Vector3 localPos = new Vector3(startX + x * spacing, startY + y * spacing, 0);
                CreateBubble(localPos);
                bubbleIndex++;
            }
        }
    }
    
    void GenerateRandom()
    {
        float range = spacing * 2f;
        
        for (int i = 0; i < bubbleCount; i++)
        {
            Vector3 localPos = new Vector3(
                Random.Range(-range, range),
                Random.Range(-range, range),
                0
            );
            CreateBubble(localPos);
        }
    }
    
    void CreateBubble(Vector3 localPosition)
    {
        GameObject bubble = Instantiate(staticBubblePrefab, transform);
        bubble.transform.localPosition = localPosition;
        
        // Set color if not randomizing
        if (!randomizeColors)
        {
            StaticBubble staticBubble = bubble.GetComponent<StaticBubble>();
            if (staticBubble != null)
            {
                staticBubble.randomizeColor = false;
                staticBubble.SetColor(fixedColor);
            }
        }
    }
    
    // Destroy pattern from a specific bubble hit
    public void OnBubbleDestroyed()
    {
        // Could implement chain destruction or other effects
        // For now, let individual bubbles handle their own destruction
    }
    
    void OnDrawGizmosSelected()
    {
        // Preview the pattern in editor
        Gizmos.color = Color.yellow;
        
        switch (patternType)
        {
            case PatternType.Line:
                Gizmos.DrawWireCube(transform.position, new Vector3(bubbleCount * spacing, 1, 0));
                break;
            case PatternType.Column:
                Gizmos.DrawWireCube(transform.position, new Vector3(1, bubbleCount * spacing, 0));
                break;
            case PatternType.Triangle:
            case PatternType.Square:
            case PatternType.Random:
                float size = bubbleCount * spacing * 0.5f;
                Gizmos.DrawWireCube(transform.position, new Vector3(size, size, 0));
                break;
        }
    }
}