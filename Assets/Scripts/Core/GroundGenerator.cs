using UnityEngine;
using System.Collections.Generic;

public class GroundGenerator : MonoBehaviour
{
    [Header("Ground Settings")]
    public GameObject groundTilePrefab;
    public float tileWidth = 2f;
    public int tilesVisible = 12;
    public float groundY = -4.5f; // Ajustado para nueva posición del jugador
    
    [Header("References")]
    public Transform player;
    
    private Queue<GameObject> groundTiles = new Queue<GameObject>();
    private float nextTileX = 0f;
    
    void Start()
    {
        // Find player if not assigned
        if (player == null)
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
                player = playerController.transform;
        }
        
        // Auto-calculate tile width from sprite bounds
        if (groundTilePrefab != null)
        {
            SpriteRenderer sr = groundTilePrefab.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                tileWidth = sr.bounds.size.x;
                Debug.Log($"Auto-calculated tile width: {tileWidth}");
            }
        }
        
        // Create initial ground tiles
        if (groundTilePrefab != null)
        {
            CreateInitialGround();
        }
        else
        {
            CreateSimpleGround();
        }
    }
    
    void Update()
    {
        if (player != null && groundTilePrefab != null)
        {
            // Check if we need to spawn more ground ahead
            float playerX = player.position.x;
            float spawnX = playerX + (tilesVisible * tileWidth);
            
            if (spawnX >= nextTileX)
            {
                SpawnGroundTile();
            }
            
            // Remove tiles behind player
            CleanupTiles(playerX);
        }
    }
    
    void CreateInitialGround()
    {
        // Start ground from behind player
        float startX = player != null ? player.position.x - tileWidth * 2 : -10f;
        nextTileX = startX;
        
        // Create initial tiles
        for (int i = 0; i < tilesVisible + 5; i++)
        {
            SpawnGroundTile();
        }
    }
    
    void SpawnGroundTile()
    {
        Vector3 spawnPos = new Vector3(nextTileX, groundY, 0);
        GameObject tile = Instantiate(groundTilePrefab, spawnPos, Quaternion.identity);
        
        // Parent to this object for organization
        tile.transform.SetParent(transform);
        
        groundTiles.Enqueue(tile);
        nextTileX += tileWidth;
    }
    
    void CleanupTiles(float playerX)
    {
        // Remove tiles that are far behind the player
        float cleanupDistance = tileWidth * 5;
        
        while (groundTiles.Count > 0)
        {
            GameObject firstTile = groundTiles.Peek();
            if (firstTile == null)
            {
                groundTiles.Dequeue();
                continue;
            }
            
            if (firstTile.transform.position.x < playerX - cleanupDistance)
            {
                Destroy(groundTiles.Dequeue());
            }
            else
            {
                break;
            }
        }
    }
    
    void CreateSimpleGround()
    {
        // Fallback: create a simple long ground sprite
        GameObject simpleGround = new GameObject("SimpleGround");
        simpleGround.transform.SetParent(transform);
        
        SpriteRenderer sr = simpleGround.AddComponent<SpriteRenderer>();
        
        // Try to use existing tile sprite
        if (HasTileSprite())
        {
            sr.sprite = GetTileSprite();
            sr.drawMode = SpriteDrawMode.Tiled;
            sr.size = new Vector2(200, 2); // Very long ground
        }
        else
        {
            // Create a simple colored rectangle
            sr.sprite = CreateSimpleSprite();
            sr.color = new Color(0.3f, 0.7f, 0.3f); // Green ground
        }
        
        simpleGround.transform.position = new Vector3(0, groundY, 1);
        sr.sortingOrder = -5;
        
        // Add collider
        BoxCollider2D col = simpleGround.AddComponent<BoxCollider2D>();
        col.size = new Vector2(200, 2);
    }
    
    bool HasTileSprite()
    {
        // Check if we have tile sprites available
        return Resources.FindObjectsOfTypeAll<Sprite>().Length > 0;
    }
    
    Sprite GetTileSprite()
    {
        // Find a suitable tile sprite
        Sprite[] allSprites = Resources.FindObjectsOfTypeAll<Sprite>();
        foreach (var sprite in allSprites)
        {
            if (sprite.name.ToLower().Contains("tile") || 
                sprite.name.ToLower().Contains("ground"))
            {
                return sprite;
            }
        }
        return allSprites.Length > 0 ? allSprites[0] : null;
    }
    
    Sprite CreateSimpleSprite()
    {
        // Create a simple 1x1 white texture
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.one * 0.5f);
    }
}