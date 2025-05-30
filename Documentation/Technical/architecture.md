# BUBBLE DASH - TECHNICAL ARCHITECTURE

## SYSTEM OVERVIEW

### Core Architecture Pattern
**Pattern**: Manager-Component-System (Hybrid Entity-Component-System)
**Rationale**: Unity-friendly, scalable, easy to debug and maintain

### Key Design Principles
1. **Single Responsibility**: Each script has one clear purpose
2. **Loose Coupling**: Managers communicate through events/interfaces
3. **Mobile Optimization**: Object pooling, efficient updates, minimal allocations
4. **Modularity**: Easy to add/remove features without breaking core

## SYSTEM ARCHITECTURE

### Core Systems Hierarchy
```
GameManager (Singleton)
├── ScoreManager (Singleton)
├── UIManager (Singleton)
├── AudioManager (Singleton)
├── UpgradeSystem (Singleton)
└── SaveSystem (Singleton)

Player GameObject
├── PlayerController (MonoBehaviour)
├── MomentumSystem (MonoBehaviour)
└── BubbleShooter (MonoBehaviour)

Game World
├── BubbleManager (MonoBehaviour)
├── ObstacleGenerator (MonoBehaviour)
├── CoinCollector (MonoBehaviour)
└── EffectsManager (MonoBehaviour)
```

## DETAILED SYSTEM SPECIFICATIONS

### 1. GameManager (Singleton)
**Purpose**: Central game state management
**Responsibilities**:
- Game state transitions (Menu, Playing, GameOver, Paused)
- Scene management
- Global game settings
- Integration point for all other managers

**Key Methods**:
```csharp
public void StartGame()
public void PauseGame()
public void GameOver()
public void RestartGame()
public void LoadMainMenu()
```

**Events**:
- `OnGameStart`
- `OnGameOver` 
- `OnGamePause`
- `OnGameResume`

### 2. PlayerController
**Purpose**: Handle player character movement and collision
**Responsibilities**:
- Automatic forward movement
- Vertical boundary constraints
- Collision detection with obstacles
- Integration with MomentumSystem

**Key Properties**:
```csharp
public float baseSpeed = 3f;
public float currentSpeed { get; private set; }
public Vector2 verticalBounds = new Vector2(-4f, 4f);
public bool isAlive { get; private set; }
```

### 3. BubbleShooter
**Purpose**: Handle bubble shooting mechanics
**Responsibilities**:
- Input detection (touch/mouse)
- Trajectory calculation and visualization
- Bubble instantiation and physics application
- Integration with BubbleManager

**Key Methods**:
```csharp
public void Aim(Vector2 screenPosition)
public void Shoot()
public void ShowTrajectory(bool show)
```

### 4. MomentumSystem (CORE FEATURE)
**Purpose**: Manage speed changes based on performance
**Responsibilities**:
- Track consecutive successful shots
- Calculate speed multipliers
- Apply speed changes to PlayerController
- Reset momentum on miss/collision

**Key Properties**:
```csharp
public int consecutiveHits { get; private set; }
public float speedMultiplier { get; private set; }
public float maxSpeedMultiplier = 3f;
public float speedIncrement = 0.1f;
```

### 5. BubbleManager
**Purpose**: Handle bubble logic and combinations
**Responsibilities**:
- Bubble collision detection
- Color matching logic (3+ same color)
- Bubble destruction and effects
- Score calculation for combinations

**Bubble Types**:
```csharp
public enum BubbleType
{
    Red, Blue, Green, Yellow,    // Basic colors
    Rainbow,                     // Matches any color
    Bomb,                       // Destroys area
    Multiplier,                 // Score bonus
    Speed                       // Momentum bonus
}
```

### 6. ObstacleGenerator
**Purpose**: Procedural obstacle generation
**Responsibilities**:
- Generate obstacles based on player progression
- Maintain difficulty curve
- Object pooling for performance
- Obstacle variety and patterns

**Generation Rules**:
- Distance-based difficulty scaling
- Patterns that require specific bubble combinations
- Escape routes always available
- Performance-optimized pooling

## DATA MANAGEMENT

### Save System Architecture
**Method**: JSON serialization to PlayerPrefs (mobile-friendly)
**Data Structure**:
```csharp
[System.Serializable]
public class GameData
{
    public float highScore;
    public int totalCoins;
    public int totalDistance;
    public UpgradeData upgrades;
    public SettingsData settings;
    public IdleData idleProgress;
}
```

### Upgrade System
**Pattern**: ScriptableObject-based upgrade definitions
**Benefits**: Easy balancing, designer-friendly, runtime efficient

```csharp
[CreateAssetMenu]
public class UpgradeDefinition : ScriptableObject
{
    public string upgradeName;
    public int[] costs;           // Cost per level
    public float[] values;        // Benefit per level
    public int maxLevel;
}
```

## PERFORMANCE OPTIMIZATION

### Object Pooling Strategy
**Critical Objects for Pooling**:
- Bubbles (high spawn/destroy frequency)
- Obstacles (continuous generation)
- Coins (frequent collection)
- Particle effects (visual feedback)

**Implementation**:
```csharp
public class ObjectPool<T> where T : MonoBehaviour
{
    private Queue<T> pool = new Queue<T>();
    private T prefab;
    
    public T Get() { /* Implementation */ }
    public void Return(T obj) { /* Implementation */ }
}
```

### Update Optimization
**Techniques**:
- Use `FixedUpdate` only for physics
- Cache component references
- Minimize `GetComponent` calls
- Use object pooling to avoid instantiate/destroy
- Coroutines for non-critical updates

### Memory Management
- Avoid allocations in Update loops
- Use StringBuilder for dynamic text
- Pool temporary objects
- Unload unused assets between scenes

## MOBILE-SPECIFIC CONSIDERATIONS

### Touch Input Handling
```csharp
// Optimized touch input
public class TouchInput : MonoBehaviour
{
    private Camera mainCamera;
    private Vector2 lastTouchPosition;
    
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            HandleTouch(touch);
        }
    }
}
```

### Resolution Scaling
- Use Canvas Scaler with "Scale With Screen Size"
- Test on multiple aspect ratios (16:9, 19.5:9, 4:3)
- Safe area handling for notched devices
- Dynamic UI scaling based on screen DPI

### Battery Optimization
- Target 30 FPS for battery life
- Reduce update frequency for non-critical systems
- Use efficient rendering (minimal overdraw)
- Pause systems when app goes to background

## MONETIZATION INTEGRATION POINTS

### Ad Integration Architecture
```csharp
public class AdManager : MonoBehaviour
{
    public void ShowRewardedAd(System.Action<bool> callback)
    public void ShowInterstitial()
    public bool IsRewardedAdReady()
    
    // Integration points in game flow
    private void OnGameOver() // Interstitial opportunity
    private void OnRewardRequest() // Rewarded video
}
```

### IAP Integration Points
- Upgrade system (permanent improvements)
- Cosmetic unlocks (character skins)
- Currency packages (coins/gems)
- Ad removal purchase
- Premium battle pass

## DEBUGGING AND TESTING

### Debug Tools
- In-game debug panel (development builds only)
- Performance profiler integration
- Save/load game state for testing
- Skip to specific game states
- Cheat codes for upgrade testing

### Testing Framework
```csharp
// Unit testing structure
[TestFixture]
public class BubbleManagerTests
{
    [Test]
    public void TestColorMatching() { /* Test implementation */ }
    
    [Test] 
    public void TestScoreCalculation() { /* Test implementation */ }
}
```

## DEPLOYMENT ARCHITECTURE

### Build Pipeline
1. **Development Build**: Debug symbols, profiler, cheats enabled
2. **Staging Build**: Production settings, test monetization
3. **Production Build**: Optimized, real monetization, analytics

### Platform Specific Settings
- **Android ONLY**: API Level 21+, ARM64 + ARMv7, IL2CPP
- **Target Devices**: Android 7.0+ (API 24) for optimal compatibility
- **Optimization**: Managed Stripping Level Medium, Script Call Optimization
- **Rendering**: Vulkan + OpenGL ES 3.0 fallback

---

**Architecture Review**: Weekly during development
**Performance Target**: 60 FPS on mid-range devices (2019+)
**Memory Target**: <500MB RAM usage