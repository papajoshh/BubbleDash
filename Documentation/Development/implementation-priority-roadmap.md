# IMPLEMENTATION PRIORITY ROADMAP

## 🎯 CURRENT STATUS
- ✅ Sprint 1: Timer System (COMPLETED)
- ✅ Pre-run Upgrade System (COMPLETED)  
- 🔄 Next: Sprint 2 or Advanced Coin System?

---

## 🤔 IMPLEMENTATION DECISION POINT

### **OPTION A: Continue Roguelite Core (Sprint 2)**
```
🐉 Boss System Implementation
├── Pros: Completes core roguelite loop
├── Pros: Unlocks advanced coin upgrade tiers
├── Pros: Major gameplay milestone
└── Timeline: Sprint 2-3 (Days 3-6)

Then: Advanced Coin System (Sprint 4-5)
```

### **OPTION B: Advanced Coin System First**
```
🪙 Coin Mechanics Overhaul
├── Pros: Immediate gameplay variety
├── Pros: Tests economic balance early
├── Cons: Locks premium features behind non-existent bosses
└── Timeline: Sprint 2-3 (Days 3-6)

Then: Boss System (Sprint 4-5)
```

---

## 💡 RECOMMENDATION: HYBRID APPROACH

### **Mini-Sprint 1.5: Coin Foundation** (4-6 hours)
```
🔧 IMPLEMENT CORE MECHANICS:
├── Bubble-coin collision detection
├── "Any bubble breaks coins" toggle upgrade
├── Coin multiplier upgrade (1-3 levels)
└── Visual feedback for breakable coins

🚫 SKIP FOR NOW:
├── Boss unlock requirements (use coin cost gates instead)
├── Auto collector (premium feature)
├── Coin burst (polish feature)
└── Complex unlock UI
```

### **Then: Sprint 2 - Boss System** (Full implementation)
```
🐉 BOSS SYSTEM:
├── First boss implementation
├── Boss defeat rewards
├── Retroactively unlock premium coin features
└── Connect coin upgrade tiers to boss progression
```

---

## 🔧 MINI-SPRINT 1.5 IMPLEMENTATION

### **Quick Coin Upgrade Additions:**
```csharp
// Add to UpgradeSystem.cs
upgrades.Add(new Upgrade
{
    id = "coin_shooter",
    name = "Coin Hunter", 
    description = "Any bubble can break coin bubbles",
    baseCost = 200, // Temporarily coin-gated instead of boss-gated
    maxLevel = 1
});

upgrades.Add(new Upgrade  
{
    id = "coin_multiplier",
    name = "Golden Touch",
    description = "Coin bubbles give +1 extra coin",
    baseCost = 150,
    costIncrement = 100,
    maxLevel = 3,
    valuePerLevel = 1f
});
```

### **Bubble-Coin Collision (Simple):**
```csharp
// In Bubble.cs
void OnTriggerEnter2D(Collider2D other)
{
    Coin coin = other.GetComponent<Coin>();
    if (coin != null && PlayerPrefs.GetInt("CoinShooterUnlocked") == 1)
    {
        coin.CollectByBubbleImpact();
        // Destroy this bubble
        PopBubble();
    }
}
```

### **Visual Feedback:**
```csharp
// In Coin.cs
void Start()
{
    if (PlayerPrefs.GetInt("CoinShooterUnlocked") == 1)
    {
        // Add subtle target outline or glow
        gameObject.AddComponent<Outline>().effectColor = Color.yellow;
    }
}
```

---

## ⏱️ TIME ESTIMATES

### **Mini-Sprint 1.5: Coin Foundation**
```
⏱️ 4-6 hours total:
├── 2 hours: Bubble-coin collision system
├── 1 hour: Coin multiplier upgrade
├── 1 hour: Visual feedback for shootable coins  
├── 1 hour: UI integration + testing
└── 30 min: Documentation update
```

### **Sprint 2: Boss System** (After coin foundation)
```
⏱️ 8-12 hours total:
├── Boss behavior implementation
├── Boss unlock integration with coin upgrades
├── Premium coin features (auto collector, burst)
├── Complete upgrade tree with boss gates
└── Polish and balance
```

---

## 🎯 RECOMMENDED ACTION

**MY RECOMMENDATION**: 

1. **Do Mini-Sprint 1.5 NOW** (4-6 hours)
   - Implement basic coin shooting mechanics
   - Add coin multiplier upgrade
   - Test economic balance
   - Players get immediate coin variety

2. **Then Sprint 2: Boss System** (8-12 hours)
   - Implement first boss
   - Retroactively add boss unlock gates
   - Add premium coin features
   - Complete roguelite core loop

### **Benefits of this approach:**
- ✅ **Immediate variety** in coin collection  
- ✅ **Test economics early** before boss complexity
- ✅ **Iterative development** - build on working foundation
- ✅ **Player feedback** on coin mechanics before locking behind bosses
- ✅ **Simpler scope** for each implementation phase

---

## ❓ DECISION NEEDED

**¿Quieres que implemente el Mini-Sprint 1.5 ahora?**

```
✅ YES: Implement coin shooting mechanics now (4-6 hours)
⏳ NO: Proceed directly to Sprint 2 Boss System  
🤔 CUSTOM: Different approach/priority
```

Si dices que sí, empiezo inmediatamente con:
1. Bubble-coin collision detection
2. "Coin Hunter" upgrade implementation  
3. Coin multiplier upgrade
4. Visual feedback system
5. Economic balance testing

**¿Procedemos con Mini-Sprint 1.5?**