# BUBBLE DASH - PROJECT CONTEXT

## PROJECT OVERVIEW
**Game Name**: Bubble Dash  
**Genre**: Idle Bubble Shooter + Endless Runner Hybrid  
**Platform**: Android ONLY (Google Play) - No iOS/Mac development  
**Development Timeline**: Weekend Project (48-72 hours)  
**Team**: Solo Developer (Programmer only)  
**Target**: Generate revenue from day 1

## CURRENT STATUS (31 DIC 2024)
- ✅ Market research completed
- ✅ Strategic plan defined  
- ✅ Core arcade mechanics implemented
- ✅ All 11 core systems functional
- ✅ Roguelite analysis completed
- 🔄 Roguelite transformation starting
- ⏳ Monetization integration pending
- ⏳ Art assets integration pending

## CORE GAME CONCEPT (ROGUELITE PIVOT)
**NEW Unique Selling Point**: First-ever Roguelite Bubble Shooter
- Time-limited runs (3-5 minutes) creating urgency
- Boss encounters that stop progress until defeated
- Zone-based progression (Forest → Desert → Ocean → Sky → Space)
- Meta-progression between runs (permanent upgrades)
- "Momentum System": More consecutive bubble pops = faster character movement
- Idle progression for offline coin generation

## TECHNICAL SPECIFICATIONS
- **Engine**: Unity 2022.3 LTS
- **Target Platform**: Android API 21+
- **Art Style**: Minimalist/Low Poly (using free Unity assets)
- **Monetization**: Hybrid (Ads 70% + IAP 25% + Battle Pass 5%)

## KEY DIFFERENTIATORS
1. **Momentum System**: Unique mechanic linking bubble popping to movement speed
2. **Idle Integration**: Offline progression keeps players engaged
3. **Minimalist Aesthetic**: Feature, not limitation
4. **Multi-monetization**: Multiple revenue streams from launch

## IMMEDIATE PRIORITIES
1. **Core Mechanics Implementation**: Bubble shooting + character movement
2. **Gameplay Loop**: Score system + combo mechanics
3. **Basic UI**: Functional interface for testing
4. **Monetization Framework**: Ads + IAP integration setup

## DEVELOPMENT GUIDELINES FOR CLAUDE
When working on this project:

### 1. ALWAYS READ FIRST
- Check `/Documentation/Development/current-sprint.md` for active tasks
- Review `/Documentation/Technical/architecture.md` for code structure
- Consult `/Documentation/Strategy/game-design.md` for design decisions

### 2. CODE STANDARDS
- Follow Unity C# conventions
- Use meaningful component names (PlayerController, BubbleManager, etc.)
- Comment only when business logic is complex
- Prioritize performance for mobile (object pooling, efficient updates)

### 3. ART CONSTRAINTS
- **NO custom art creation** - use only free Unity Asset Store resources
- Maintain consistent minimalist style
- 3-4 color palette maximum
- Low poly aesthetic for performance

### 4. DECISION PRIORITY
1. **Functionality first**: Get it working
2. **Mobile performance**: Keep it smooth
3. **Monetization ready**: Prepare for ads/IAP
4. **Polish last**: Only after core works

### 5. UNITY LIFECYCLE BEST PRACTICES
**CRITICAL: Always follow proper Unity lifecycle patterns**

#### Awake() - Component Initialization
- UI panel state setup (SetActive, initial visibility)
- Singleton instance assignment
- Component initialization that doesn't depend on other objects
- Reference assignments within the same GameObject

#### Start() - System Integration  
- Button event listeners
- Cross-component references (FindObjectOfType, etc.)
- Singleton Instance usage
- Dependencies on other systems being ready

#### Why This Matters:
- **Awake()** runs before **Start()**, ensuring proper initialization order
- **SetActive(false)** calls in **Awake()** prevent panels from flickering
- **Singleton Instances** should be set in **Awake()** before other scripts try to use them in **Start()**

#### Example Pattern:
```csharp
void Awake()
{
    // UI state initialization
    if (myPanel != null)
        myPanel.SetActive(false);
    
    // Singleton setup
    if (Instance == null)
        Instance = this;
}

void Start()
{
    // Button listeners
    if (myButton != null)
        myButton.onClick.AddListener(OnClick);
    
    // Use other singletons
    if (GameManager.Instance != null)
        GameManager.Instance.OnGameStart += HandleGameStart;
}
```

### 6. TESTING APPROACH
- Test on mobile resolution (1080x1920)
- Verify touch controls work properly
- Check performance on lower-end devices
- Validate monetization integration points

## PROJECT STRUCTURE
```
FirstArcade/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/           # Game mechanics
│   │   ├── UI/             # Interface controllers
│   │   ├── Monetization/   # Ads & IAP
│   │   └── Managers/       # Game state management
│   ├── Prefabs/           # Reusable objects
│   ├── Materials/         # Minimalist materials
│   └── Scenes/           # Game scenes
├── Documentation/        # Project documentation
└── CLAUDE.md            # This context file
```

## CRITICAL SUCCESS METRICS (UPDATED)
- **Day 1-2**: Timer system + First boss ✅
- **Day 3-4**: Zone system implementation
- **Day 5-8**: Meta-progression + Content
- **Day 9-11**: Monetization integration  
- **Day 12-14**: Polish + Launch prep
- **Week 2**: Android build in Google Play
- **Month 1**: 5,000 downloads, $15K revenue

## EMERGENCY PROTOCOLS
If stuck for >2 hours on any feature:
1. Simplify the approach
2. Check Unity Asset Store for solutions
3. Implement minimal viable version
4. Document for post-launch improvement

## REVENUE PROJECTIONS (ROGUELITE MODEL)
- **Conservative**: $500/day at 2K DAU (Month 3)
- **Realistic**: $3,800/day at 8K DAU (Month 6)
- **Optimistic**: $19,500/day at 25K DAU (Month 12)
- **Break-even**: Week 3 post-launch

## 📋 SPRINT METHODOLOGY WITH CLAUDE

### How We Work Together:
1. **Start Session**: "Hoy vamos a hacer Sprint X"
2. **Claude Implements**: Rapid development using all tools
3. **You Test**: Unity validation and decisions
4. **Iterate Fast**: No bureaucracy, just results

### Key Documents:
- **Sprint Planning**: `/Documentation/Development/sprint-planning-roguelite.md`
- **Current Sprint Status**: Check sprint checkboxes
- **Technical Status**: `IMPLEMENTATION_GUIDE.md`

### Implementation Guides (CRITICAL):
- **Upgrade System**: `/Documentation/Development/upgrade-system-implementation.md`
- **Idle System**: `/Documentation/Development/idle-system-implementation.md` 
- **Coin System**: `/Documentation/Development/coin-system-implementation.md`

### IMPORTANT: Documentation Structure Rules:
- `/Documentation/Technical/` = Architecture, high-level design
- `/Documentation/Development/` = Implementation guides, step-by-step tutorials
- `/Documentation/Strategy/` = Game design, business strategy
- `/Documentation/Marketing/` = Monetization, marketing plans

**NEVER create implementation guides in `/Technical/` folder - they belong in `/Development/`**

---
**Last Updated**: 2024-12-31
**Current Sprint**: Ready to start Sprint 1 (Timer System)
**Next Session Goal**: Transform arcade to roguelite
**Priority**: CRITICAL - 14 days to launch