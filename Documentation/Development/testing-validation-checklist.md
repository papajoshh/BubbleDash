# ✅ TESTING & VALIDATION CHECKLIST - ENERGY SYSTEM

## 📋 OVERVIEW

Esta guía te ayuda a validar que el Energy System esté funcionando correctamente después de la implementación. Incluye tests específicos, expected behaviors, y troubleshooting.

---

## 🔧 PARTE 1: PRE-IMPLEMENTATION VALIDATION

### Scene Setup Validation

**Antes de press Play, verificar:**

✅ **Managers en Scene:**
- [ ] EnergyManager GameObject existe y está activo
- [ ] ObjectiveManager GameObject existe y está activo  
- [ ] WaveManager GameObject existe y está activo
- [ ] GameManager existe (previo)
- [ ] ScoreManager existe (previo)

✅ **UI Setup:**
- [ ] EnergyPanel visible en HudPanel
- [ ] ObjectivePanel existe pero está INACTIVE
- [ ] WavePanel visible en HudPanel
- [ ] SafeZonePanel existe pero está INACTIVE
- [ ] CompletionEffect existe pero está INACTIVE

✅ **Component References:**
- [ ] EnergyUI tiene todas las referencias assigned (energy bar, text, etc.)
- [ ] ObjectiveUI tiene todas las referencias assigned
- [ ] No hay missing references (None) en Inspector

✅ **Initial Values:**
- [ ] EnergyManager: Max Energy = 10, Drain Rate = 1
- [ ] ObjectiveManager: Interval = 20, Time Limit = 30
- [ ] WaveManager: Distance Per Wave = 200

---

## 🎮 PARTE 2: FUNCTIONAL TESTING

### Test 1: Basic Energy System

**Objetivo:** Verificar que el energy system core funciona

**Steps:**
1. Press Play
2. Observar energy bar
3. NO disparar por 15 segundos
4. Verificar game over

**Expected Behavior:**
```
✅ Energy bar starts at 10.0 (o 15.0 en beginner mode)
✅ Energy text muestra "10.0" o "15.0"
✅ Energy bar gradually decreases (rojo cuando bajo)
✅ Después de 10-15 segundos (depending on beginner mode):
   → Energy reaches 0
   → Game Over triggered
   → "Energy Depleted" message en console
```

**Troubleshooting:**
- **Energy no baja:** Verificar EnergyManager está activo y has pressed Play correctly
- **Game Over no triggers:** Verificar GameManager.GameOver() method existe
- **UI no updates:** Verificar EnergyUI references están assigned

### Test 2: Energy Gain from Bubble Hits

**Objetivo:** Verificar que bubble hits dan energía

**Steps:**
1. Press Play
2. Disparar y hit 5 bubbles seguidas
3. Observar energy bar

**Expected Behavior:**
```
✅ Cada bubble hit aumenta energy en +1
✅ Energy bar moves up (green color)
✅ Energy text updates en real time
✅ Console shows: "Energy gained from bubble hit: +1.0 (Total: X)"
✅ Player puede maintain energy level disparando regularly
```

**Troubleshooting:**
- **Energy no increases:** Verificar MomentumSystem calls EnergyManager.OnBubbleHit()
- **Gain amount incorrect:** Check energyPerHit value en EnergyManager
- **No visual feedback:** Verificar energy bar fillAmount updates

### Test 3: Beginner Mode

**Objetivo:** Verificar que beginner mode está working

**Steps:**
1. Delete PlayerPrefs (Edit → Clear All PlayerPrefs)
2. Press Play (should be first run)
3. Observar energy settings

**Expected Behavior:**
```
✅ Energy starts at 15.0 (not 10.0)
✅ Energy drains slower (0.5/second instead of 1/second)
✅ Console shows: "Energy System Started - Starting Energy: 15, Drain Rate: 0.5"
✅ Easier to survive initial runs
```

**Troubleshooting:**
- **Beginner mode no activa:** Verificar PlayerPrefs cleared, check beginnerRunsCount setting
- **Wrong starting energy:** Check beginnerEnergyBonus value
- **Wrong drain rate:** Check beginnerDrainReduction value

### Test 4: Mini-Objectives System

**Objetivo:** Verificar que objectives aparecen y se pueden completar

**Steps:**
1. Press Play
2. Wait 5-10 seconds
3. Completar el objective que aparece
4. Observar rewards

**Expected Behavior:**
```
✅ Después de ~5 segundos: First objective appears
✅ ObjectivePanel becomes active
✅ Objective shows: Title, description, progress bar, timer
✅ Progress updates as you work toward objective
✅ Al completar:
   → "OBJECTIVE COMPLETE!" aparece
   → Energy increases by +3
   → Shield appears por 3 seconds
   → Panel disappears after 2 seconds
```

**Common Objectives to Test:**
- **"Hit X bubbles"** - Dispara X bubbles
- **"Get X combo"** - Hit X bubbles consecutively
- **"Collect X coins"** - Collect coin bubbles/coins

**Troubleshooting:**
- **No objectives appear:** Verificar ObjectiveManager está activo
- **Progress no updates:** Verificar MomentumSystem/CoinSystem calling objective methods
- **Completion no works:** Check ObjectiveUI component references

### Test 5: Shield System

**Objetivo:** Verificar que shields protect contra energy drain

**Steps:**
1. Complete un objective para get shield
2. Observar shield indicator
3. Wait mientras shield is active
4. Verificar energy no drains

**Expected Behavior:**
```
✅ Shield indicator appears (cyan rotating icon)
✅ Shield time text shows countdown "3.0s", "2.9s", etc.
✅ Energy drain pauses mientras shield is active
✅ Energy bar stops decreasing
✅ Shield disappears cuando timer reaches 0
✅ Energy drain resumes normally
```

### Test 6: Wave System

**Objetivo:** Verificar wave transitions

**Steps:**
1. Play normally
2. Reach 200m distance
3. Observar wave transition
4. Continue to 400m

**Expected Behavior:**
```
✅ Initially: "WAVE 1: LEARNING ZONE"
✅ Wave progress bar fills as distance increases
✅ At 200m:
   → Screen flash effect
   → "WAVE 2: SPEED CHALLENGE" appears
   → Safe zone activates por 3 seconds
   → +1 energy bonus
   → Cyan overlay durante safe zone
✅ At 400m: Wave 3 transition occurs
```

**Troubleshooting:**
- **Wave no changes:** Verificar ScoreManager fires OnDistanceChanged event
- **Safe zone no appears:** Check SafeZonePanel reference en EnergyUI
- **No visual effects:** Verificar SimpleEffects.ScreenFlash() method exists

### Test 7: Safe Zones

**Objetivo:** Verificar que safe zones pause energy drain

**Steps:**
1. Reach wave transition (200m)
2. Stop shooting durante safe zone
3. Observar energy level

**Expected Behavior:**
```
✅ "SAFE ZONE" text appears en screen center
✅ Cyan tint overlay covers screen
✅ Energy drain pauses por 3 seconds
✅ Energy bar stops decreasing
✅ Safe zone text pulses/animates
✅ After 3 seconds: everything returns to normal
```

---

## 📊 PARTE 3: PERFORMANCE TESTING

### Test 8: Mobile Performance

**Objetivo:** Verificar performance en mobile resolution

**Steps:**
1. Set Game view to mobile resolution (1080x1920)
2. Press Play y observe frame rate
3. Generate multiple objectives y effects
4. Check for lag o stuttering

**Expected Performance:**
```
✅ Steady 60fps (o 30fps minimum)
✅ UI elements scale correctly
✅ Text is readable at mobile resolution
✅ No significant frame drops durante effects
✅ Memory usage stable
```

### Test 9: Extended Play Session

**Objetivo:** Verificar stability over time

**Steps:**
1. Play por 5+ minutes continuously
2. Complete multiple objectives
3. Go through several wave transitions
4. Observar for memory leaks o issues

**Expected Behavior:**
```
✅ Performance remains consistent
✅ No gradual slowdown
✅ UI continues updating correctly
✅ No error messages en console
✅ Audio doesn't degrade
```

---

## 🐛 PARTE 4: EDGE CASE TESTING

### Test 10: Energy Edge Cases

**Test scenarios:**

**Scenario A: Energy Overflow**
1. Get energy very high (15+)
2. Complete objective
3. Verificar energy doesn't break

**Scenario B: Rapid Energy Changes**
1. Hit bubbles very quickly
2. Complete objective simultaneously
3. Verificar smooth UI updates

**Scenario C: Game Over During Objective**
1. Let energy get very low
2. Try to complete objective at last second
3. Verificar graceful handling

### Test 11: UI Edge Cases

**Test scenarios:**

**Scenario A: Rapid Objective Changes**
1. Complete objective immediately when it appears
2. Verificar UI transitions smoothly

**Scenario B: Multiple Effects Simultaneously**
1. Complete objective during wave transition
2. Verificar all effects play correctly

**Scenario C: Pause Durante Effects**
1. Pause game during shield or safe zone
2. Resume y verificar states are preserved

### Test 12: Integration Edge Cases

**Test scenarios:**

**Scenario A: GameManager State Changes**
1. Pause/resume multiple times
2. Restart game multiple times
3. Verificar energy system resets correctly

**Scenario B: Component Disable/Enable**
1. Disable energy UI temporarily
2. Re-enable y verificar reconnection

---

## 📋 PARTE 5: VALIDATION CHECKLIST

### Core Functionality ✅

- [ ] **Energy System:**
  - [ ] Energy drains at correct rate
  - [ ] Energy gains from bubble hits
  - [ ] Game over triggers at 0 energy
  - [ ] Beginner mode works correctly

- [ ] **Mini-Objectives:**
  - [ ] Objectives appear every ~20 seconds
  - [ ] Progress tracking works
  - [ ] Completion gives energy + shield
  - [ ] Failure handling works

- [ ] **Wave System:**
  - [ ] Waves transition at correct distances
  - [ ] Safe zones activate between waves
  - [ ] Wave themes are clear
  - [ ] Progress tracking accurate

- [ ] **Safety Systems:**
  - [ ] Shields pause energy drain
  - [ ] Safe zones provide breathing room
  - [ ] Beginner mode is easier
  - [ ] Balance feels fair

### UI/UX ✅

- [ ] **Visual Feedback:**
  - [ ] Energy bar updates smoothly
  - [ ] Colors indicate energy level
  - [ ] Objective progress is clear
  - [ ] Wave information visible

- [ ] **Audio Feedback:**
  - [ ] Energy events have sound
  - [ ] Objective completion has sound
  - [ ] Wave transitions have sound
  - [ ] Shield effects have sound

- [ ] **Animations:**
  - [ ] UI elements animate smoothly
  - [ ] Screen effects work correctly
  - [ ] Text animations are clear
  - [ ] Transitions feel polished

### Performance ✅

- [ ] **Mobile Performance:**
  - [ ] 60fps on target devices
  - [ ] Memory usage reasonable
  - [ ] Battery impact minimal
  - [ ] No performance degradation

- [ ] **Stability:**
  - [ ] No crashes during extended play
  - [ ] No memory leaks
  - [ ] Clean game state transitions
  - [ ] Proper cleanup on quit

---

## 🎯 PARTE 6: SUCCESS CRITERIA

### Must Pass (Para considerarlo funcional):

1. **Energy System Core:** ✅
   - Energy drains y gains correctly
   - Game over works at 0 energy
   - UI feedback is clear

2. **Basic Objectives:** ✅
   - At least 3 objective types working
   - Completion gives energy reward
   - Progress tracking accurate

3. **Wave Transitions:** ✅
   - Waves change at correct distances
   - Visual feedback works
   - Safe zones provide relief

4. **Beginner Mode:** ✅
   - First 3 runs are easier
   - Learning curve is reasonable
   - New players can complete objectives

### Should Pass (Para considerarlo polished):

1. **Advanced Features:** ✅
   - Shield system working
   - Smart objective selection
   - Multiple wave themes

2. **Polish:** ✅
   - Smooth animations
   - Clear audio feedback
   - Mobile-optimized UI

### Could Pass (Nice to have):

1. **Advanced Balance:** ✅
   - Momentum affects energy efficiency
   - Dynamic difficulty adjustment
   - Perfect objective timing

---

## 🚨 CRITICAL ISSUES TO WATCH FOR

### Game-Breaking Issues:
- **Energy gets stuck** (no drain o gain)
- **Infinite objectives** spawn
- **UI becomes unresponsive**
- **Game over doesn't trigger**

### User Experience Issues:
- **Energy drains too fast** (frustrating)
- **Objectives too hard** (discouraging)
- **UI elements too small** (mobile)
- **Audio too loud/quiet** (annoying)

### Performance Issues:
- **Frame rate drops** below 30fps
- **Memory leaks** over time
- **Audio glitches** o delays
- **Battery drain** excessive

---

## 📞 FINAL VALIDATION CALL

**After completing all tests:**

### Green Light Criteria (✅ Ready for Sprint 2):
- All "Must Pass" criteria met
- No game-breaking issues
- Smooth 5+ minute play sessions
- Clear improvement over timer system

### Yellow Light Criteria (⚠️ Needs Tweaking):
- Some "Should Pass" criteria not met
- Minor balance issues
- Performance concerns on lower-end devices
- UI needs polish

### Red Light Criteria (❌ Back to Implementation):
- Any "Must Pass" criteria failed
- Game-breaking bugs present
- Worse experience than timer system
- Major performance issues

---

**¡PERFECTO!** Con estos tests puedes validar completamente que el Energy System esté working como expected y providing la improved experience que buscamos.

**¿Hay algún test específico que quieras que detalle más o algún edge case particular que te preocupe?**