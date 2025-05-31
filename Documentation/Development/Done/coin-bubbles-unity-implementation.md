# COIN BUBBLES - UNITY IMPLEMENTATION GUIDE

## 🎯 OVERVIEW
Esta guía implementa el nuevo sistema de **Coin Bubbles** - burbujas especiales que dan coins al romperse con cualquier burbuja del jugador.

**CAMBIO CLAVE**: Las monedas ya no son objetos flotantes con imán, ahora son burbujas estáticas que se rompen al impacto.

---

## 📋 PREREQUISITES
- ✅ Bubble shooter básico funcionando
- ✅ Sistema de coins existente
- ✅ Scripts actualizados del Mini-Sprint 1.5
- ✅ Sprites para burbujas normales

---

## 🪙 STEP 1: CREATE COIN BUBBLE PREFAB

### 1.1 Base GameObject
```
1. En Hierarchy: Create Empty GameObject "CoinBubble"
2. Position: (0, 0, 0)
3. Scale: (1, 1, 1)
```

### 1.2 Visual Components
```
1. Add Component: SpriteRenderer
   ├── Sprite: tile_coin.png (o yellow bubble sprite)
   ├── Color: White (255, 255, 255, 255) si usas coin sprite
   ├── Color: Gold (255, 204, 0, 255) si usas yellow bubble
   ├── Sorting Layer: Default
   └── Order in Layer: 5

2. Optional: Add visual indicator
   ├── Create child GameObject "CoinIcon"
   ├── Add SpriteRenderer
   ├── Sprite: tile_coin.png
   ├── Scale: (0.5, 0.5, 1)
   └── Order in Layer: 6
```

### 1.3 Physics Components
```
1. Add Component: CircleCollider2D
   ├── Is Trigger: ❌ unchecked (solid collision)
   ├── Radius: 0.4
   └── Material: None

2. NO Rigidbody2D (static target bubble)
```

### 1.4 CoinBubble Script
```
1. Add Component: CoinBubble.cs
2. Configure en Inspector:
   ├── Base Coin Value: 1
   ├── Bubble Renderer: Drag SpriteRenderer
   ├── Coin Bubble Sprite: tile_coin.png (optional)
   ├── Bob Speed: 2
   ├── Bob Height: 0.1
   └── Coin Pop Effect Prefab: (optional particle effect)
```

### 1.5 Save as Prefab
```
1. Drag CoinBubble to Assets/Prefabs/
2. Name: "CoinBubblePrefab"
3. Delete from scene
```

---

## ⬆️ STEP 2: UPDATE COIN SYSTEM

### 2.1 Configure CoinSystem
```
1. Select CoinSystem GameObject
2. En CoinSystem component:
   ├── Coin Prefab: (keep existing floating coin prefab)
   ├── Coin Bubble Prefab: Drag CoinBubblePrefab ⭐ NEW
   ├── Use Floating Coins: ❌ unchecked (use bubbles instead)
   ├── Coin Spawn Chance: 0.3 (30% chance)
   ├── Min Coins Per Spawn: 1
   ├── Max Coins Per Spawn: 3
   └── Use Patterns: ✓ checked
```

### 2.2 Test Spawning
```
1. Play Mode
2. CoinSystem should now spawn CoinBubbles instead of floating coins
3. Verify bubble patterns appear correctly
```

---

## 🎨 STEP 3: CREATE COIN BUBBLE VISUALS

### 3.1 Coin Bubble Sprite Options

**OPTION A: Use Existing Coin Sprite**
```
1. Use tile_coin.png as main sprite
2. Scale up to match bubble size
3. Add glow/outline effect in shader
```

**OPTION B: Create Golden Bubble**
```
1. Duplicate yellow bubble sprite
2. Rename: "bubble_coin"
3. Edit in image editor:
   ├── Add coin symbol overlay
   ├── Golden tint
   └── Subtle shimmer effect
```

**OPTION C: Composite Design**
```
1. Use yellow bubble as base
2. Add coin icon as child object
3. Rotate coin icon slowly via script
```

### 3.2 Visual Polish
```
RECOMMENDED EFFECTS:
├── Gentle bobbing animation (implemented)
├── Subtle golden glow (shader/particle)
├── Coin icon rotation (if using composite)
└── Sparkle particles (optional)
```

---

## 💰 STEP 4: CONFIGURE GOLDEN TOUCH UPGRADE

### 4.1 Verify Upgrade System
```
1. Play Mode
2. Press 'U' → Open upgrade menu
3. Look for "Golden Touch" upgrade (replaces Coin Magnet)
4. Description: "Coin bubbles give +1 extra coin"
5. Cost progression: 100, 200, 300, 400 coins
```

### 4.2 Test Upgrade Effect
```
TESTING STEPS:
1. Add test coins (press 'C' if helper enabled)
2. Buy Golden Touch Level 1
3. Pop coin bubble → Should give 2 coins (1 base + 1 upgrade)
4. Buy Level 2
5. Pop coin bubble → Should give 3 coins
6. Max level 4 → 5 coins per bubble
```

---

## 🎯 STEP 5: COLLISION BEHAVIOR

### 5.1 Verify Collision Logic
```
EXPECTED BEHAVIOR:
✅ ANY player bubble can pop coin bubbles
✅ No color matching required
✅ Coin bubble pops on impact
✅ Player bubble also destroyed
✅ Awards coins + score + momentum
```

### 5.2 Test All Bubble Types
```
Test with each color:
├── Red bubble → Coin bubble = ✅ Works
├── Blue bubble → Coin bubble = ✅ Works
├── Green bubble → Coin bubble = ✅ Works
└── Yellow bubble → Coin bubble = ✅ Works
```

---

## 🔧 STEP 6: INTEGRATION TESTING

### 6.1 Complete Flow Test
```
1. Start new game
2. Play until coin bubbles spawn
3. Shoot at coin bubble
4. Verify:
   ├── Coin bubble pops
   ├── Coins added to total
   ├── Visual effect plays
   ├── Sound effect plays
   ├── Score increases
   └── Momentum increases
```

### 6.2 Edge Cases
```
TEST SCENARIOS:
├── Multiple coin bubbles in cluster
├── Coin bubble behind regular bubbles
├── Missing coin bubble (should destroy projectile)
├── Coin bubble at screen edge
└── Rapid coin bubble collection
```

---

## 🎨 STEP 7: OPTIONAL ENHANCEMENTS

### 7.1 Advanced Visual Effects
```
PARTICLE SYSTEM:
1. Create particle system "CoinBurstEffect"
2. Configure:
   ├── Shape: Circle, radius 0.5
   ├── Emission: Burst 10 particles
   ├── Color: Gold gradient
   ├── Size: 0.1 - 0.3
   ├── Lifetime: 0.5s
   └── Gravity: -2 (coins fall)

3. Assign to CoinBubble.coinPopEffectPrefab
```

### 7.2 Coin Shower Animation
```
When coin bubble pops:
1. Spawn 3-5 coin sprites
2. Animate outward arc
3. Then move toward coin UI
4. Destroy on arrival
```

---

## 🐛 TROUBLESHOOTING

### Common Issues:

**❌ Coin bubbles not spawning:**
```
Solution: Check CoinSystem.useFloatingCoins = false
Verify: coinBubblePrefab is assigned
Check: coinSpawnChance > 0
```

**❌ Can't pop coin bubbles:**
```
Solution: Verify CoinBubble has CircleCollider2D
Check: SimpleBubbleCollision detects coin bubble type
Ensure: No Rigidbody2D on coin bubble
```

**❌ No coins awarded:**
```
Solution: Check CoinSystem.Instance exists
Verify: Golden Touch upgrade applying correctly
Check: Console for error messages
```

**❌ Visual doesn't look golden:**
```
Solution: Check sprite assignment in prefab
Try: Different sprite or color tint
Consider: Adding child coin icon
```

**❌ Coin bubbles move/fall:**
```
Solution: Remove any Rigidbody2D component
Ensure: Static bubble behavior
Check: No movement scripts attached
```

---

## ✅ SUCCESS CRITERIA

### Must Work:
- [ ] Coin bubbles spawn instead of floating coins
- [ ] ANY bubble color can pop coin bubbles
- [ ] Coins awarded on pop (1-5 based on upgrade)
- [ ] Visual feedback on collection
- [ ] Sound plays on collection
- [ ] Golden Touch upgrade increases coin value

### Should Work:
- [ ] Coin bubble patterns spawn correctly
- [ ] Gentle bobbing animation
- [ ] Score and momentum also increase
- [ ] Consistent spawn rate (30%)

### Nice to Have:
- [ ] Particle effects on pop
- [ ] Coin shower animation
- [ ] Special golden bubble sprite
- [ ] UI coin counter animation

---

## 📊 ECONOMY BALANCE

### Coin Income Projection:
```
NO UPGRADES:
├── 1 coin per bubble
├── ~10 coin bubbles per 3-min run
└── Total: ~10 coins per run

WITH GOLDEN TOUCH L4:
├── 5 coins per bubble
├── ~10 coin bubbles per 3-min run
└── Total: ~50 coins per run

SKILLED PLAY:
├── Target more coin bubbles
├── 15-20 bubbles per run possible
└── Total: 75-100 coins per run
```

### Upgrade ROI:
```
Golden Touch Total Cost: 100+200+300+400 = 1000 coins
Break-even: ~20-25 runs
Long-term value: 5x coin income
```

---

## 🎮 NEXT STEPS

### Immediate:
1. **Test the implementation** thoroughly
2. **Balance spawn rates** based on feel
3. **Adjust coin values** if needed

### Future Enhancements (Sprint 4+):
1. **Coin Shower** upgrade (spawn extra bubbles)
2. **Chain Reaction** upgrade (pop nearby coins)
3. **Diamond Bubbles** (rare 5x value)
4. **Boss-locked** premium upgrades

---

**STATUS**: ✅ Ready for Unity Implementation  
**ESTIMATED TIME**: 30-45 minutes  
**COMPLEXITY**: Low-Medium  
**DEPENDENCIES**: Bubble system, CoinSystem functional

## 🎯 SUMMARY

The coin bubble system transforms coins from passive collectibles to **active shooting targets**, adding strategic depth while maintaining simple, intuitive gameplay. Players now actively hunt for golden opportunities!