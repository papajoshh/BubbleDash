# 🔋 SPRINT 1 REVISADO - ENERGY SYSTEM DESIGN

## 📋 CONTEXTO DEL CAMBIO

### Problema Identificado con Timer System:
- Timer system implementado pero **no resuelve el core engagement problem**
- Players se aburren en 10 segundos porque es "esperar que se acabe el tiempo"
- Falta de **stakes inmediatos** en cada acción

### Nueva Dirección - Energy System:
**Reemplazar timer por energy que se drena constantemente**
- Cada bubble hit = supervivencia
- Mini-objetivos = energy boosts críticos
- Game over cuando energy = 0

---

## 🎯 DESIGN CORE DEL ENERGY SYSTEM

### Mecánica Base:
```
STARTING ENERGY: 10 points
ENERGY DRAIN RATE: 1 point/second (ajustable)
ENERGY GAIN:
├── Bubble Hit: +1 energy
├── Combo Bonus: +0.5 per combo level (5 combo = +2.5 total)
├── Mini-Objective Complete: +3 energy
└── Coin Collection: +0.5 energy

GAME OVER: Energy reaches 0
```

### Difficulty Curve (Learning):
```
FIRST 3 RUNS:
├── Starting Energy: 15
├── Drain Rate: 0.5/second
└── Bubble Hit: +1.5 energy

NORMAL DIFFICULTY:
├── Starting Energy: 10  
├── Drain Rate: 1/second
└── Bubble Hit: +1 energy

HARD MODE (unlock later):
├── Starting Energy: 8
├── Drain Rate: 1.5/second  
└── Bubble Hit: +0.8 energy
```

---

## 🎮 MINI-OBJETIVOS SYSTEM

### Objetivo Types (aparecen cada 15-20 segundos):
```
PRECISION OBJECTIVES:
├── "Hit 3 red bubbles" (+3 energy)
├── "Get 5 consecutive hits" (+3 energy)
├── "No miss for 15 seconds" (+4 energy)
└── "Hit bubbles through gap" (+2 energy)

COLLECTION OBJECTIVES:  
├── "Collect 3 coins" (+3 energy)
├── "Hit one of each color" (+4 energy)
├── "Pop 10 bubbles in 20s" (+3 energy)
└── "Collect while at max speed" (+4 energy)

MOMENTUM OBJECTIVES:
├── "Reach max momentum" (+2 energy)
├── "Maintain speed for 10s" (+3 energy)
├── "Get 8+ combo" (+4 energy)
└── "Hit 15 bubbles without missing" (+5 energy)
```

### Objective Selection Logic:
```csharp
// Weighted based on current player state
if (momentum < 5) → More momentum objectives
if (accuracy < 70%) → More precision objectives  
if (coins_collected < 3) → More collection objectives
Random selection from weighted pool every 15-20 seconds
```

---

## 🌊 WAVE/MILESTONE SYSTEM

### Distance-Based Waves:
```
WAVE 1 (0-200m): "LEARNING ZONE"
├── Easy objectives (3 bubble hits, collect 2 coins)
├── Normal bubble patterns
├── Objective frequency: Every 20 seconds
└── Energy drain: -1/second

WAVE 2 (200-400m): "SPEED CHALLENGE"  
├── Momentum-focused objectives
├── Faster bubble movements
├── Objective frequency: Every 18 seconds
└── Energy drain: -1/second

WAVE 3 (400-600m): "PRECISION ZONE"
├── Accuracy-focused objectives  
├── Smaller bubbles, tighter patterns
├── Objective frequency: Every 15 seconds
└── Energy drain: -1.2/second

WAVE 4 (600-800m): "COIN RUSH"
├── Collection-focused objectives
├── More coin bubbles spawn
├── Objective frequency: Every 15 seconds  
└── Energy drain: -1.2/second

WAVE 5 (800m+): "CHAOS MODE"
├── Mixed objectives, higher difficulty
├── All bubble types, complex patterns
├── Objective frequency: Every 12 seconds
└── Energy drain: -1.5/second
```

### Wave Transition Effects:
```
VISUAL: Screen flash + "WAVE X" text display
AUDIO: Achievement sound + music intensity increase
UI: Wave indicator + next milestone distance
ENERGY: +2 energy bonus for reaching new wave
```

---

## 🔧 ENERGY BUFFERS & SAFETY SYSTEMS

### Energy Shield System:
```
SHIELD ACTIVATION:
├── Perfect objective completion (no misses) = 3s shield
├── 10+ combo achievement = 2s shield  
├── Max momentum reached = 1s shield
└── Coin collection streak = 1s shield

SHIELD EFFECT:
├── Energy drain paused during shield
├── Visual indicator (blue glow around energy bar)
├── Audio cue (protective sound)
└── Can stack up to 5 seconds maximum
```

### Safe Zones (Wave Transitions):
```
BETWEEN WAVES (every 200m):
├── 3 seconds of no energy drain
├── "SAFE ZONE" text indicator
├── Moment to see progress/plan ahead
├── Visual breathing room effect
└── Automatic +1 energy bonus
```

---

## 🎯 INTEGRATION CON SISTEMAS EXISTENTES

### Momentum System:
```
UNCHANGED: Speed/combo mechanics work the same
NEW: High momentum gives energy efficiency bonus
├── 5+ momentum: Energy hits give +1.2 instead of +1
├── 10+ momentum: Energy hits give +1.5 instead of +1  
├── Max momentum: Energy drain reduced to 0.8/second
└── Momentum loss on miss: No energy penalty (just loss of bonus)
```

### Upgrade System:
```
NEW UPGRADE CATEGORIES:

ENERGY MASTERY:
├── "Extra Battery": +2 starting energy
├── "Efficient Shooting": +0.2 energy per hit  
├── "Slow Drain": -0.2 energy drain rate
└── "Shield Master": +1 second shield duration

OBJECTIVE MASTERY:
├── "Easy Targets": Objectives 20% easier
├── "Bonus Hunter": +1 energy from objectives
├── "Quick Goals": Objectives appear 10% more often
└── "Perfect Timing": Shield on every objective complete
```

### Roguelite Integration:
```
PRE-RUN CHOICES (future sprint):
├── "High Risk": Start with 6 energy, +2 per hit
├── "Safe Start": Start with 15 energy, normal rates
├── "Speed Run": Double drain, double rewards
└── "Practice Mode": Half drain rate (learning)
```

---

## 📊 BALANCING PARAMETERS

### Key Variables to Monitor:
```csharp
public class EnergyBalanceConfig
{
    [Header("Base Settings")]
    public float startingEnergy = 10f;
    public float energyDrainRate = 1f; // per second
    public float energyPerHit = 1f;
    public float energyPerObjective = 3f;
    
    [Header("Difficulty Scaling")]
    public float drainRateIncreasePerWave = 0.2f;
    public float maxDrainRate = 2f;
    
    [Header("Safety Systems")]
    public float shieldDurationPerObjective = 3f;
    public float safeZoneDuration = 3f;
    public int maxShieldStackSeconds = 5;
    
    [Header("Learning Curve")]
    public float beginnerEnergyBonus = 5f; // First 3 runs
    public float beginnerDrainReduction = 0.5f;
    public int beginnerRunsCount = 3;
}
```

### Success Metrics:
```
TARGET METRICS:
├── Average run duration: 2-4 minutes (vs current 30s-1min)
├── Player engagement: No "dead time" feeling
├── Learning curve: 80% complete first objective
├── Retention: Players want "one more try" immediately
└── Difficulty: 70% of players reach Wave 3 within 5 attempts
```

---

## 🚨 POTENTIAL RISKS & MITIGATIONS

### Risk 1: Too Stressful
```
INDICATORS: Players quit after 1-2 tries, negative feedback
MITIGATION: Reduce drain rate, increase starting energy
MONITORING: Track attempt count vs completion rate
```

### Risk 2: Too Easy  
```
INDICATORS: Players easily reach 5+ minutes, no tension
MITIGATION: Increase drain rate progressively
MONITORING: Track average run duration
```

### Risk 3: Objective Frustration
```
INDICATORS: Players ignore objectives, only focus on hits
MITIGATION: Make objectives more achievable, increase energy bonus
MONITORING: Track objective completion rate
```

### Risk 4: Learning Curve Too Steep
```
INDICATORS: New players die in <30 seconds consistently
MITIGATION: Extended tutorial mode, guided first run
MONITORING: Track new player first-run completion rate
```

---

## 🎯 SPRINT 1 SUCCESS CRITERIA

### Must Have:
- [x] Timer system removed completely
- [ ] Energy system core mechanics working
- [ ] Basic mini-objectives (3-4 types minimum)
- [ ] Wave progression (at least 3 waves)
- [ ] Energy UI display with clear feedback

### Should Have:
- [ ] Energy shield system working
- [ ] Safe zones between waves
- [ ] Integration with existing momentum system
- [ ] Basic difficulty curve (beginner mode)

### Could Have:
- [ ] Advanced objective types
- [ ] Energy upgrade integration
- [ ] Visual polish for energy effects
- [ ] Audio feedback for energy events

### Won't Have (Future Sprints):
- [ ] Roguelite pre-run choices
- [ ] Advanced upgrade categories
- [ ] Zone unlocks/progression
- [ ] Daily objectives system

---

**NEXT STEP**: Detailed Unity implementation guide
**TIMELINE**: 5-7 days implementation + 2 days testing
**PRIORITY**: HIGH - Core engagement fix for immediate testing