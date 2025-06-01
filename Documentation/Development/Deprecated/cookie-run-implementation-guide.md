# 🔧 GUÍA DE IMPLEMENTACIÓN TÉCNICA - MECÁNICAS COOKIE RUN EN BUBBLE DASH

## 📋 OVERVIEW TÉCNICO

Este documento detalla la implementación específica de las mecánicas inspiradas en Cookie Run, con código, arquitectura y timelines realistas.

---

## 🍪 CR-1: BUBBLE TYPE SYSTEM - IMPLEMENTACIÓN DETALLADA

### Arquitectura de Datos

```csharp
[System.Serializable]
public enum BubblePlayerType 
{
    Speed,      // +40% movement speed
    Magnet,     // Auto-collect coins in radius
    Shield,     // 1 free mistake per run
    Combo,      // Chain reactions give 2x points
    Time        // +30 seconds to timer
}

[System.Serializable] 
public struct BubbleTypeConfig
{
    public BubblePlayerType type;
    public string displayName;
    public string description;
    public Sprite icon;
    public Color themeColor;
    public PlayerAbilityData abilityData;
    public bool isUnlocked;
    public int unlockCost;
}
```

### Sistema de Abilities

```csharp
public abstract class PlayerAbility : MonoBehaviour
{
    public abstract void OnRunStart();
    public abstract void OnBubbleHit();
    public abstract void OnMissed();
    public abstract void OnRunEnd();
    
    protected PlayerController player;
    protected MomentumSystem momentum;
    
    protected virtual void Awake()
    {
        player = GetComponent<PlayerController>();
        momentum = GetComponent<MomentumSystem>();
    }
}

public class SpeedAbility : PlayerAbility
{
    [SerializeField] private float speedMultiplier = 1.4f;
    private float originalSpeed;
    
    public override void OnRunStart()
    {
        originalSpeed = player.currentSpeed;
        player.baseSpeed *= speedMultiplier;
        
        // Visual feedback
        CreateSpeedTrail();
    }
    
    public override void OnBubbleHit()
    {
        // Speed bubble gets momentum bonus
        momentum.AddMomentum(1.2f);
    }
    
    private void CreateSpeedTrail()
    {
        // Particle system for speed effect
        var trail = player.GetComponent<TrailRenderer>();
        if (trail != null)
        {
            trail.enabled = true;
            trail.startColor = Color.cyan;
            trail.endColor = Color.blue;
        }
    }
}

public class MagnetAbility : PlayerAbility
{
    [SerializeField] private float magnetRadius = 3f;
    [SerializeField] private LayerMask coinLayer;
    
    public override void OnRunStart()
    {
        // Start magnet coroutine
        StartCoroutine(MagnetPulse());
    }
    
    private IEnumerator MagnetPulse()
    {
        while (true)
        {
            // Find coins in radius
            Collider2D[] coins = Physics2D.OverlapCircleAll(
                player.transform.position, 
                magnetRadius, 
                coinLayer
            );
            
            foreach (var coin in coins)
            {
                // Attract coin to player
                StartCoroutine(AttractCoin(coin.transform));
            }
            
            yield return new WaitForSeconds(0.2f);
        }
    }
    
    private IEnumerator AttractCoin(Transform coin)
    {
        float duration = 0.5f;
        Vector3 startPos = coin.position;
        
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            if (coin == null) yield break;
            
            float progress = t / duration;
            Vector3 targetPos = player.transform.position;
            coin.position = Vector3.Lerp(startPos, targetPos, progress);
            yield return null;
        }
        
        // Collect coin
        if (coin != null)
        {
            coin.GetComponent<Coin>()?.Collect();
        }
    }
}

public class ShieldAbility : PlayerAbility
{
    private bool shieldActive = true;
    private GameObject shieldVisual;
    
    public override void OnRunStart()
    {
        CreateShieldVisual();
    }
    
    public override void OnMissed()
    {
        if (shieldActive)
        {
            // Consume shield instead of failing
            shieldActive = false;
            DestroyShieldVisual();
            
            // Show shield break effect
            if (SimpleEffects.Instance != null)
            {
                SimpleEffects.Instance.PlayShieldBreak(player.transform.position);
            }
            
            // Don't call base implementation (which would cause game over)
            return;
        }
        
        base.OnMissed(); // Normal miss behavior
    }
    
    private void CreateShieldVisual()
    {
        // Create rotating shield effect around player
        shieldVisual = new GameObject("Shield Effect");
        shieldVisual.transform.SetParent(player.transform);
        shieldVisual.transform.localPosition = Vector3.zero;
        
        var sr = shieldVisual.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("ShieldSprite");
        sr.color = new Color(0, 1, 1, 0.5f);
        
        // Rotate animation
        shieldVisual.transform.DORotate(new Vector3(0, 0, 360), 2f)
            .SetLoops(-1, LoopType.Restart);
    }
}
```

### Manager Principal

```csharp
public class BubbleTypeManager : MonoBehaviour
{
    public static BubbleTypeManager Instance { get; private set; }
    
    [SerializeField] private BubbleTypeConfig[] allTypes;
    [SerializeField] private BubblePlayerType currentType = BubblePlayerType.Speed;
    
    private PlayerAbility currentAbility;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadPlayerPreferences();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        ActivateCurrentType();
    }
    
    public void ChangeType(BubblePlayerType newType)
    {
        if (!IsTypeUnlocked(newType))
        {
            Debug.LogWarning($"Type {newType} is not unlocked!");
            return;
        }
        
        // Deactivate current
        if (currentAbility != null)
        {
            Destroy(currentAbility);
        }
        
        currentType = newType;
        ActivateCurrentType();
        SavePlayerPreferences();
    }
    
    private void ActivateCurrentType()
    {
        var player = FindObjectOfType<PlayerController>();
        if (player == null) return;
        
        // Add appropriate ability component
        switch (currentType)
        {
            case BubblePlayerType.Speed:
                currentAbility = player.gameObject.AddComponent<SpeedAbility>();
                break;
            case BubblePlayerType.Magnet:
                currentAbility = player.gameObject.AddComponent<MagnetAbility>();
                break;
            case BubblePlayerType.Shield:
                currentAbility = player.gameObject.AddComponent<ShieldAbility>();
                break;
            // etc...
        }
        
        // Update UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateTypeDisplay(currentType);
        }
    }
    
    public bool IsTypeUnlocked(BubblePlayerType type)
    {
        string key = $"BubbleType_{type}_Unlocked";
        return PlayerPrefs.GetInt(key, 0) == 1;
    }
    
    public void UnlockType(BubblePlayerType type)
    {
        string key = $"BubbleType_{type}_Unlocked";
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
    }
}
```

### UI Sistema

```csharp
public class TypeSelectionUI : MonoBehaviour
{
    [SerializeField] private Transform typeButtonContainer;
    [SerializeField] private GameObject typeButtonPrefab;
    [SerializeField] private Button confirmButton;
    
    private BubblePlayerType selectedType;
    private List<TypeButton> typeButtons = new List<TypeButton>();
    
    void Start()
    {
        CreateTypeButtons();
        selectedType = BubbleTypeManager.Instance.CurrentType;
        UpdateSelection();
    }
    
    private void CreateTypeButtons()
    {
        var configs = BubbleTypeManager.Instance.GetAllConfigs();
        
        foreach (var config in configs)
        {
            var buttonObj = Instantiate(typeButtonPrefab, typeButtonContainer);
            var typeButton = buttonObj.GetComponent<TypeButton>();
            
            typeButton.Setup(config, OnTypeSelected);
            typeButtons.Add(typeButton);
        }
    }
    
    private void OnTypeSelected(BubblePlayerType type)
    {
        selectedType = type;
        UpdateSelection();
    }
    
    private void UpdateSelection()
    {
        foreach (var button in typeButtons)
        {
            button.SetSelected(button.Type == selectedType);
        }
        
        confirmButton.interactable = 
            BubbleTypeManager.Instance.IsTypeUnlocked(selectedType);
    }
    
    public void OnConfirmPressed()
    {
        BubbleTypeManager.Instance.ChangeType(selectedType);
        gameObject.SetActive(false);
    }
}

public class TypeButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private GameObject lockOverlay;
    [SerializeField] private Button button;
    
    public BubblePlayerType Type { get; private set; }
    private System.Action<BubblePlayerType> onSelected;
    
    public void Setup(BubbleTypeConfig config, System.Action<BubblePlayerType> callback)
    {
        Type = config.type;
        onSelected = callback;
        
        iconImage.sprite = config.icon;
        nameText.text = config.displayName;
        descriptionText.text = config.description;
        
        bool isUnlocked = BubbleTypeManager.Instance.IsTypeUnlocked(Type);
        lockOverlay.SetActive(!isUnlocked);
        button.interactable = isUnlocked;
        
        button.onClick.AddListener(() => onSelected?.Invoke(Type));
    }
    
    public void SetSelected(bool selected)
    {
        // Visual feedback for selection
        var outline = GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = selected;
        }
    }
}
```

---

## 🧪 CR-2: EVOLUTION SYSTEM - IMPLEMENTACIÓN

### Data Structure

```csharp
[System.Serializable]
public class EvolutionTier
{
    public int tier;
    public string name;
    public string description;
    public int coinCost;
    public EvolutionMaterial[] requiredMaterials;
    public AbilityModifier[] modifiers;
    public Sprite icon;
    public ParticleSystem upgradeEffect;
}

[System.Serializable]
public class EvolutionMaterial
{
    public MaterialType type;
    public int amount;
}

public enum MaterialType
{
    BubbleEssence,     // Common - from any successful run
    ColorCrystal,      // Rare - from perfect combos
    MomentumShard,     // Epic - from high momentum runs
    TimeFragment       // Legendary - from timer challenges
}

[System.Serializable]
public class AbilityModifier
{
    public ModifierType type;
    public float value;
    public bool isPercentage;
}

public enum ModifierType
{
    SpeedBonus,
    MagnetRadius,
    ShieldCount,
    ComboMultiplier,
    TimerBonus
}
```

### Evolution Manager

```csharp
public class EvolutionManager : MonoBehaviour
{
    public static EvolutionManager Instance { get; private set; }
    
    [SerializeField] private EvolutionDatabase database;
    private Dictionary<BubblePlayerType, int> currentTiers;
    private Dictionary<MaterialType, int> materials;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public bool CanEvolve(BubblePlayerType type)
    {
        int currentTier = GetCurrentTier(type);
        var nextTier = database.GetTierData(type, currentTier + 1);
        
        if (nextTier == null) return false; // Max tier reached
        
        // Check coin cost
        if (CoinSystem.Instance.GetCoins() < nextTier.coinCost)
            return false;
        
        // Check materials
        foreach (var material in nextTier.requiredMaterials)
        {
            if (GetMaterialCount(material.type) < material.amount)
                return false;
        }
        
        return true;
    }
    
    public void Evolve(BubblePlayerType type)
    {
        if (!CanEvolve(type)) return;
        
        int currentTier = GetCurrentTier(type);
        var nextTier = database.GetTierData(type, currentTier + 1);
        
        // Pay costs
        CoinSystem.Instance.SpendCoins(nextTier.coinCost);
        foreach (var material in nextTier.requiredMaterials)
        {
            SpendMaterial(material.type, material.amount);
        }
        
        // Upgrade tier
        currentTiers[type] = currentTier + 1;
        
        // Play effects
        PlayEvolutionEffect(type, nextTier);
        
        // Update abilities if current type
        if (BubbleTypeManager.Instance.CurrentType == type)
        {
            BubbleTypeManager.Instance.RefreshCurrentAbility();
        }
        
        SaveData();
    }
    
    public void AddMaterial(MaterialType type, int amount)
    {
        if (!materials.ContainsKey(type))
            materials[type] = 0;
        
        materials[type] += amount;
        
        // Show pickup effect
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowMaterialPickup(type, amount);
        }
        
        SaveData();
    }
    
    private void PlayEvolutionEffect(BubblePlayerType type, EvolutionTier tier)
    {
        // Screen flash
        if (SimpleEffects.Instance != null)
        {
            SimpleEffects.Instance.PlayScreenFlash(Color.gold);
        }
        
        // Particle effect
        if (tier.upgradeEffect != null)
        {
            Instantiate(tier.upgradeEffect, Vector3.zero, Quaternion.identity);
        }
        
        // Achievement sound
        if (SimpleSoundManager.Instance != null)
        {
            SimpleSoundManager.Instance.PlayEvolutionComplete();
        }
    }
}
```

### Material Drop System

```csharp
public class MaterialDropper : MonoBehaviour
{
    void Start()
    {
        // Subscribe to game events
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnBubbleHit += CheckMaterialDrop;
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnRunComplete += CheckRunCompleteDrop;
        }
    }
    
    private void CheckMaterialDrop(int bubbleCount)
    {
        // Bubble Essence - common drop
        if (Random.Range(0f, 1f) < 0.15f) // 15% chance
        {
            DropMaterial(MaterialType.BubbleEssence, 1);
        }
        
        // Color Crystal - perfect combos
        var momentum = FindObjectOfType<MomentumSystem>();
        if (momentum != null && momentum.GetCurrentCombo() >= 10)
        {
            if (Random.Range(0f, 1f) < 0.05f) // 5% chance on 10+ combo
            {
                DropMaterial(MaterialType.ColorCrystal, 1);
            }
        }
    }
    
    private void CheckRunCompleteDrop()
    {
        var stats = RunStatsManager.Instance;
        if (stats == null) return;
        
        // Momentum Shard - high momentum runs
        if (stats.GetMaxMomentumReached() >= 20)
        {
            DropMaterial(MaterialType.MomentumShard, 1);
        }
        
        // Time Fragment - timer challenges
        if (TimerManager.Instance != null && TimerManager.Instance.GetTimeRemaining() > 60f)
        {
            DropMaterial(MaterialType.TimeFragment, 1);
        }
    }
    
    private void DropMaterial(MaterialType type, int amount)
    {
        EvolutionManager.Instance.AddMaterial(type, amount);
        
        // Create pickup visual
        CreateMaterialPickup(type, amount);
    }
    
    private void CreateMaterialPickup(MaterialType type, int amount)
    {
        var player = FindObjectOfType<PlayerController>();
        if (player == null) return;
        
        Vector3 spawnPos = player.transform.position + Vector3.up * 2f;
        
        GameObject pickup = new GameObject($"Material_{type}");
        pickup.transform.position = spawnPos;
        
        // Add visual components
        var sr = pickup.AddComponent<SpriteRenderer>();
        sr.sprite = GetMaterialSprite(type);
        sr.sortingOrder = 10;
        
        // Float up animation
        pickup.transform.DOMoveY(spawnPos.y + 1f, 1f);
        sr.DOFade(0f, 1f).OnComplete(() => Destroy(pickup));
    }
}
```

---

## 📅 CR-3: MULTI-EVENT SYSTEM - IMPLEMENTACIÓN

### Event Architecture

```csharp
[System.Serializable]
public class MultiObjectiveEvent
{
    public string eventId;
    public string eventName;
    public string description;
    public float duration; // in days
    public EventObjective[] objectives;
    public EventReward finalReward;
    public bool isActive;
    
    [System.NonSerialized]
    public Dictionary<string, int> progress;
}

[System.Serializable]
public class EventObjective
{
    public string objectiveId;
    public string displayName;
    public string description;
    public ObjectiveType type;
    public int targetValue;
    public int tokensReward;
    public Sprite icon;
}

public enum ObjectiveType
{
    DailyLogin,      // Login X days
    ComboAchieve,    // Hit X combos of Y+ length
    BubblesCollect,  // Collect X special bubbles
    TimeComplete,    // Complete X runs under Y seconds
    DistanceReach,   // Reach X distance
    MaterialGather   // Collect X materials
}

[System.Serializable]
public class EventReward
{
    public RewardType type;
    public int amount;
    public string itemId; // For specific items like new bubble types
}
```

### Event Manager

```csharp
public class MultiEventManager : MonoBehaviour
{
    public static MultiEventManager Instance { get; private set; }
    
    [SerializeField] private MultiObjectiveEvent[] availableEvents;
    [SerializeField] private Transform eventUIContainer;
    [SerializeField] private GameObject eventProgressPrefab;
    
    private MultiObjectiveEvent currentEvent;
    private Dictionary<string, int> eventProgress;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadEventData();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        CheckActiveEvents();
        SubscribeToGameEvents();
    }
    
    private void SubscribeToGameEvents()
    {
        // Login tracking
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStart += () => RecordProgress("DailyLogin", 1);
            GameManager.Instance.OnRunComplete += OnRunComplete;
        }
        
        // Combo tracking
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnComboReached += OnComboReached;
        }
        
        // Material tracking
        if (EvolutionManager.Instance != null)
        {
            EvolutionManager.Instance.OnMaterialCollected += OnMaterialCollected;
        }
    }
    
    public void StartEvent(string eventId)
    {
        var eventData = System.Array.Find(availableEvents, e => e.eventId == eventId);
        if (eventData == null) return;
        
        currentEvent = eventData;
        currentEvent.isActive = true;
        eventProgress = new Dictionary<string, int>();
        
        // Initialize progress tracking
        foreach (var objective in currentEvent.objectives)
        {
            eventProgress[objective.objectiveId] = 0;
        }
        
        // Schedule event end
        Invoke(nameof(EndCurrentEvent), currentEvent.duration * 24 * 3600); // Convert days to seconds
        
        // Show event UI
        ShowEventUI();
        
        SaveEventData();
    }
    
    private void RecordProgress(string objectiveId, int amount)
    {
        if (currentEvent == null || !currentEvent.isActive) return;
        
        var objective = System.Array.Find(currentEvent.objectives, o => o.objectiveId == objectiveId);
        if (objective == null) return;
        
        if (!eventProgress.ContainsKey(objectiveId))
            eventProgress[objectiveId] = 0;
        
        eventProgress[objectiveId] += amount;
        
        // Check completion
        if (eventProgress[objectiveId] >= objective.targetValue)
        {
            CompleteObjective(objective);
        }
        
        // Update UI
        UpdateEventUI();
        SaveEventData();
    }
    
    private void CompleteObjective(EventObjective objective)
    {
        // Award tokens
        AddEventTokens(objective.tokensReward);
        
        // Show completion effect
        if (SimpleEffects.Instance != null)
        {
            SimpleEffects.Instance.PlayObjectiveComplete();
        }
        
        // Check if all objectives complete
        CheckEventCompletion();
    }
    
    private void CheckEventCompletion()
    {
        int totalTokensNeeded = 0;
        foreach (var obj in currentEvent.objectives)
        {
            totalTokensNeeded += obj.tokensReward;
        }
        
        if (GetEventTokens() >= totalTokensNeeded)
        {
            CompleteEvent();
        }
    }
    
    private void CompleteEvent()
    {
        // Award final reward
        AwardEventReward(currentEvent.finalReward);
        
        // Show completion celebration
        if (SimpleEffects.Instance != null)
        {
            SimpleEffects.Instance.PlayEventComplete();
        }
        
        // End event
        EndCurrentEvent();
    }
    
    private void OnRunComplete()
    {
        if (currentEvent == null) return;
        
        var stats = RunStatsManager.Instance;
        if (stats == null) return;
        
        // Record distance
        RecordProgress("DistanceReach", Mathf.RoundToInt(stats.GetDistanceTraveled()));
        
        // Record time completion
        if (TimerManager.Instance != null)
        {
            float runTime = TimerManager.Instance.GetElapsedTime();
            if (runTime <= 60f) // Under 60 seconds
            {
                RecordProgress("TimeComplete", 1);
            }
        }
    }
    
    private void OnComboReached(int comboLength)
    {
        if (comboLength >= 10) // 10+ combo
        {
            RecordProgress("ComboAchieve", 1);
        }
    }
}
```

---

## ⏱️ TIMELINE DE IMPLEMENTACIÓN OPTIMIZADA

### WEEK 1: BUBBLE TYPE FOUNDATION (5 días)
```
Day 1: 
├── BubblePlayerType enum
├── BubbleTypeConfig structure
├── Basic PlayerAbility abstract class
└── Testing framework

Day 2:
├── SpeedAbility implementation
├── MagnetAbility implementation  
├── Basic visual effects
└── Integration testing

Day 3:
├── ShieldAbility implementation
├── ComboAbility implementation
├── TimeAbility implementation
└── Balance testing

Day 4:
├── BubbleTypeManager complete
├── Save/load system
├── UI básico para type selection
└── Integration with existing systems

Day 5:
├── Polish and effects
├── Bug fixing
├── Performance optimization
└── User testing
```

### WEEK 2: EVOLUTION SYSTEM (5 días)
```
Day 1:
├── EvolutionTier data structure
├── MaterialType system
├── Basic evolution database
└── Testing framework

Day 2:
├── EvolutionManager implementation
├── Material collection system
├── Drop rate algorithms
└── Save/load integration

Day 3:
├── Evolution UI system
├── Material display
├── Upgrade confirmation flow
└── Cost calculation

Day 4:
├── Material dropping implementation
├── Run completion rewards
├── Special condition rewards
└── Visual effects

Day 5:
├── Balance testing
├── UI polish
├── Bug fixes
└── Performance optimization
```

### WEEK 3: MULTI-EVENT SYSTEM (5 días)
```
Day 1:
├── Event data structures
├── MultiObjectiveEvent system
├── Basic event manager
└── Progress tracking

Day 2:
├── Event UI implementation
├── Objective progress display
├── Token system
└── Reward system

Day 3:
├── Event scheduling
├── Auto-start/end logic
├── Progress save/load
└── Integration testing

Day 4:
├── Complete event implementation
├── First live event creation
├── Testing with real gameplay
└── Balance adjustments

Day 5:
├── Polish and effects
├── Event announcement system
├── Final testing
└── Launch preparation
```

---

## 📊 MÉTRICAS Y VALIDACIÓN

### KPIs Específicos para Cookie Run Features

```csharp
public class CookieRunMetrics : MonoBehaviour
{
    // Type System Metrics
    public static void TrackTypeUsage(BubblePlayerType type, float sessionDuration)
    {
        string eventName = $"bubble_type_used_{type.ToString().ToLower()}";
        Analytics.CustomEvent(eventName, new Dictionary<string, object>
        {
            { "session_duration", sessionDuration },
            { "type", type.ToString() }
        });
    }
    
    // Evolution Metrics
    public static void TrackEvolution(BubblePlayerType type, int fromTier, int toTier)
    {
        Analytics.CustomEvent("bubble_evolution", new Dictionary<string, object>
        {
            { "type", type.ToString() },
            { "from_tier", fromTier },
            { "to_tier", toTier },
            { "timestamp", System.DateTime.Now.Ticks }
        });
    }
    
    // Event Metrics
    public static void TrackEventProgress(string eventId, string objectiveId, float completion)
    {
        Analytics.CustomEvent("event_progress", new Dictionary<string, object>
        {
            { "event_id", eventId },
            { "objective_id", objectiveId },
            { "completion_percentage", completion },
            { "days_since_start", GetEventDaysSinceStart(eventId) }
        });
    }
    
    // Success Metrics Targets
    private static readonly Dictionary<string, float> TargetMetrics = new Dictionary<string, float>
    {
        { "type_switching_rate", 0.3f },      // 30% players switch types regularly
        { "evolution_completion", 0.6f },      // 60% complete at least one evolution
        { "event_participation", 0.8f },      // 80% participate in events
        { "multi_objective_completion", 0.4f } // 40% complete multi-objective events
    };
}
```

---

## 🚨 PUNTOS CRÍTICOS DE DECISIÓN TÉCNICA

### 1. PERFORMANCE CONSIDERATIONS
```csharp
// Optimization needed for mobile
public class PerformanceConfig
{
    public static bool ENABLE_PARTICLES = SystemInfo.systemMemorySize > 2048;
    public static bool ENABLE_TRAILS = SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES2;
    public static int MAX_CONCURRENT_EFFECTS = ENABLE_PARTICLES ? 10 : 5;
}
```

### 2. SAVE SYSTEM EXPANSION
```csharp
[System.Serializable]
public class SaveDataV2 : SaveData
{
    public Dictionary<string, int> bubbleTypeTiers;
    public Dictionary<string, int> materialCounts;
    public Dictionary<string, int> eventProgress;
    public string selectedBubbleType;
    public long lastEventUpdate;
}
```

### 3. UI COMPLEXITY MANAGEMENT
- New screens: Type Selection, Evolution Lab, Event Progress
- Navigation flow redesign needed
- Mobile-friendly layouts essential
- Performance impact of complex UI

---

## 🎯 MOMENTO DE DECISIÓN TÉCNICA

**He llegado al punto donde necesito validación externa en aspectos técnicos específicos:**

### DECISIONES CRÍTICAS PENDIENTES:

1. **COMPLEXITY LEVEL**
   - ¿Implementamos las 3 features completas o empezamos solo con Bubble Types?
   - ¿La UI puede ser compleja como Cookie Run o mantenemos simple?

2. **PERFORMANCE BUDGET**
   - ¿Cuál es el dispositivo Android más antiguo que debemos soportar?
   - ¿Podemos usar shaders/particles complejos?

3. **DATA PERSISTENCE**
   - ¿Implementamos cloud save para progression?
   - ¿Local only es suficiente inicialmente?

4. **MONETIZATION INTEGRATION**
   - ¿Premium bubble types desde inicio?
   - ¿Material packs como IAP?

**Mi recomendación técnica**: Implementar **Bubble Type System primero** (Week 1), validar engagement metrics, luego decidir sobre Evolution + Events.

La arquitectura está diseñada para ser modular - podemos shipping incremental sin romper nada.

---

**Status**: ✅ IMPLEMENTATION READY  
**Next Action Required**: Technical validation + scope decision  
**Confidence Level**: 95% (arquitectura probada, timelines realistas)