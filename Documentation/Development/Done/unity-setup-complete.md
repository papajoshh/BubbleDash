# 🎮 UNITY SETUP GUIDE - SISTEMAS COMPLETADOS

## 📊 RESUMEN DE IMPLEMENTACIÓN

**Estado**: Todos los sistemas core están implementados ✅  
**Progreso**: CoinSystem + UpgradeSystem + IdleManager completados  
**Archivos**: 11 sistemas principales funcionando  

## 🔧 CONFIGURACIÓN EN UNITY

### 1. MANAGERS SETUP

#### GameManager Setup
- GameObject: "GameManager"
- Scripts: GameManager, CoinSystem, UpgradeSystem, IdleManager
- **Configuración CoinSystem**:
  - Coin Prefab: (crear CoinPrefab)
  - Coin Spawn Chance: 0.3
  - Min Coins Per Spawn: 1
  - Max Coins Per Spawn: 3
  - Use Patterns: ✓

#### UpgradeSystem Settings
- Enable Idle Progression: ✓
- Idle Coins Per Second: 0.1
- Max Idle Hours: 8
- Max Idle Coins: 500

### 2. UI CANVAS SETUP

#### Main Canvas Hierarchy
```
Canvas (UI Manager)
├── HUD Panel
│   ├── ScoreText (TMP)
│   ├── DistanceText (TMP)
│   ├── ComboText (TMP)
│   ├── CoinText (TMP)      ← NUEVO
│   ├── NextBubbleImage
│   ├── SpeedIndicator
│   └── UpgradeButton       ← NUEVO
├── GameOverPanel
│   ├── [elementos existentes]
│   └── DoubleCoinsButton
├── UpgradePanel           ← NUEVO
│   ├── CloseButton
│   ├── TotalCoinsText
│   ├── ScrollView
│   │   └── UpgradeContainer (Vertical Layout Group)
│   └── SelectedUpgradeInfo
└── IdleRewardsPanel       ← NUEVO
    ├── OfflineTimeText
    ├── CoinsEarnedText
    ├── IdleRateText
    ├── ClaimButton
    └── DoubleRewardsButton
```

### 3. COIN PREFAB CREATION

#### GameObject: "CoinPrefab"
1. **Components**:
   - Sprite Renderer (color: yellow)
   - Circle Collider 2D (trigger, radius: 0.3)
   - Coin script

2. **Coin Script Settings**:
   - Coin Value: 1
   - Rotation Speed: 180
   - Bob Speed: 2
   - Bob Height: 0.2
   - Magnet Range: 3
   - Magnet Speed: 8
   - Lifetime: 20

3. **Save as**: Assets/Prefabs/Gameplay/CoinPrefab

### 4. UI PREFABS CREATION - GUÍA DETALLADA

#### 🔧 UpgradePanel - Paso a Paso

##### A. Crear la Estructura Base
1. **En el Canvas existente**, click derecho → UI → Panel
2. **Renombrar** a "UpgradePanel"
3. **Rect Transform**:
   - Anchors: Middle Center (preset)
   - Width: 800
   - Height: 600
   - Position: (0, 0, 0)

##### B. Configurar el Background
1. **Image Component** del UpgradePanel:
   - Color: (0, 0, 0, 0.9) - Negro semi-transparente
   - Raycast Target: ✓ (para bloquear clicks detrás)

##### C. Crear el Header
1. **Click derecho en UpgradePanel** → UI → Text - TextMeshPro
2. **Renombrar** a "HeaderText"
3. **Configuración**:
   - Text: "UPGRADES"
   - Font Size: 36
   - Alignment: Center
   - Rect Transform:
     - Anchors: Top Stretch
     - Height: 60
     - Position Y: -30

##### D. Crear Total Coins Display
1. **Click derecho en UpgradePanel** → UI → Text - TextMeshPro
2. **Renombrar** a "TotalCoinsText"
3. **Configuración**:
   - Text: "Coins: 0"
   - Font Size: 24
   - Alignment: Right
   - Color: Amarillo (#FFD700)
   - Rect Transform:
     - Anchors: Top Right
     - Width: 200
     - Height: 40
     - Position: (-20, -70)

##### E. Crear el ScrollView
1. **Click derecho en UpgradePanel** → UI → Scroll View
2. **Rect Transform**:
   - Anchors: Stretch Stretch
   - Left: 20, Right: 20
   - Top: 120, Bottom: 80

##### F. Configurar el Content del ScrollView
1. **Seleccionar** ScrollView/Viewport/Content
2. **Renombrar** a "UpgradeContainer"
3. **Agregar Component** → Vertical Layout Group:
   - Spacing: 10
   - Child Force Expand: Width ✓
   - Child Control Size: Width ✓, Height ✓
   - Padding: (10, 10, 10, 10)
4. **Agregar Component** → Content Size Fitter:
   - Vertical Fit: Preferred Size

##### G. Crear Close Button
1. **Click derecho en UpgradePanel** → UI → Button - TextMeshPro
2. **Renombrar** a "CloseButton"
3. **Rect Transform**:
   - Anchors: Top Right
   - Width: 50, Height: 50
   - Position: (-10, -10)
4. **Button Image**:
   - Color: Rojo (#FF4444)
5. **Text (TMP)** hijo del botón:
   - Text: "X"
   - Font Size: 24
   - Style: Bold

##### H. Crear Selected Upgrade Info
1. **Click derecho en UpgradePanel** → UI → Text - TextMeshPro
2. **Renombrar** a "SelectedUpgradeInfo"
3. **Configuración**:
   - Text: "" (vacío por defecto)
   - Font Size: 18
   - Alignment: Center
   - Color: Verde claro para éxito, Rojo para error
   - Rect Transform:
     - Anchors: Bottom Stretch
     - Height: 60
     - Position Y: 10

##### I. Asignar el Script UpgradeUI
1. **Seleccionar UpgradePanel**
2. **Add Component** → UpgradeUI
3. **Asignar referencias**:
   - Upgrade Panel: UpgradePanel (self)
   - Upgrade Container: UpgradeContainer
   - Total Coins Text: TotalCoinsText
   - Selected Upgrade Info: SelectedUpgradeInfo
   - Close Upgrade Button: CloseButton

---

#### 🎁 IdleRewardsPanel - Paso a Paso

##### A. Crear la Estructura Base
1. **En el Canvas**, click derecho → UI → Panel
2. **Renombrar** a "IdleRewardsPanel"
3. **Rect Transform**:
   - Anchors: Middle Center
   - Width: 600
   - Height: 400
   - Position: (0, 0, 0)

##### B. Configurar el Background
1. **Image Component**:
   - Color: (0.1, 0.2, 0.3, 0.95) - Azul oscuro
   - Agregar borde si tienes sprite de UI

##### C. Crear Welcome Text
1. **Click derecho en IdleRewardsPanel** → UI → Text - TextMeshPro
2. **Renombrar** a "OfflineTimeText"
3. **Configuración**:
   - Text: "Welcome back!\nYou were away for 0h 0m"
   - Font Size: 22
   - Alignment: Center
   - Rect Transform:
     - Anchors: Top Stretch
     - Height: 80
     - Position Y: -40

##### D. Crear Coins Earned Display
1. **Click derecho en IdleRewardsPanel** → UI → Text - TextMeshPro
2. **Renombrar** a "CoinsEarnedText"
3. **Configuración**:
   - Text: "You earned 0 coins!"
   - Font Size: 32
   - Alignment: Center
   - Style: Bold
   - Color: Amarillo (#FFD700)
   - Rect Transform:
     - Anchors: Middle Center
     - Width: 400
     - Height: 60
     - Position Y: 20

##### E. Crear Idle Rate Info
1. **Click derecho en IdleRewardsPanel** → UI → Text - TextMeshPro
2. **Renombrar** a "IdleRateText"
3. **Configuración**:
   - Text: "Idle rate: 0.1 coins/sec"
   - Font Size: 16
   - Alignment: Center
   - Color: Gris claro
   - Rect Transform:
     - Anchors: Middle Center
     - Width: 300
     - Height: 30
     - Position Y: -20

##### F. Crear Claim Button
1. **Click derecho en IdleRewardsPanel** → UI → Button - TextMeshPro
2. **Renombrar** a "ClaimButton"
3. **Rect Transform**:
   - Anchors: Bottom Center
   - Width: 200, Height: 60
   - Position: (0, 80, 0)
4. **Button Image**:
   - Color: Verde (#44FF44)
5. **Text (TMP)**:
   - Text: "CLAIM"
   - Font Size: 24
   - Style: Bold

##### G. Crear Double Rewards Button
1. **Click derecho en IdleRewardsPanel** → UI → Button - TextMeshPro
2. **Renombrar** a "DoubleRewardsButton"
3. **Rect Transform**:
   - Anchors: Bottom Center
   - Width: 250, Height: 50
   - Position: (0, 20, 0)
4. **Button Image**:
   - Color: Dorado (#FFA500)
5. **Text (TMP)**:
   - Text: "Watch Ad - Double Rewards!"
   - Font Size: 18

##### H. Agregar Visual Polish (Opcional)
1. **Coin Icon**: Agregar Image con sprite de moneda
2. **Animation**: Agregar Animation component para coin bounce
3. **Particles**: Sistema de partículas doradas al claim

##### I. Asignar el Script IdleRewardsUI
1. **Seleccionar IdleRewardsPanel**
2. **Add Component** → IdleRewardsUI
3. **Asignar referencias**:
   - Idle Rewards Panel: IdleRewardsPanel (self)
   - Offline Time Text: OfflineTimeText
   - Coins Earned Text: CoinsEarnedText
   - Idle Rate Text: IdleRateText
   - Claim Button: ClaimButton
   - Double Rewards Button: DoubleRewardsButton

---

#### 🎮 Upgrade Button en HUD

##### Agregar al HUD existente:
1. **En tu HUD Panel**, click derecho → UI → Button - TextMeshPro
2. **Renombrar** a "UpgradeButton"
3. **Rect Transform**:
   - Anchors: Top Right (o donde prefieras)
   - Width: 120, Height: 50
   - Position: (-10, -10) desde top right
4. **Button Image**:
   - Color: Morado (#9B59B6) o tu elección
5. **Text (TMP)**:
   - Text: "Upgrades"
   - Font Size: 18
6. **En UIManager**:
   - Asignar UpgradeButton en el campo correspondiente

---

#### 💡 TIPS IMPORTANTES

1. **Desactivar los panels al inicio**:
   - UpgradePanel.SetActive(false)
   - IdleRewardsPanel.SetActive(false)
   
2. **Canvas Scaler**:
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1080 x 1920
   
3. **Sorting Order**:
   - IdleRewardsPanel debe estar encima de todo
   - UpgradePanel en segundo lugar
   - HUD normal debajo

4. **Testing**:
   - Activa manualmente los panels para verificar layout
   - Prueba en diferentes aspect ratios
   - Verifica que los botones funcionan

### 5. SCRIPT ASSIGNMENTS

#### UIManager Updates
- **CoinText**: Assign coin display text
- **UpgradeButton**: Assign upgrade menu button

#### Player GameObject
- **MomentumSystem**: Should have speedIncreasePerHit = 0.1

#### ObstacleGenerator  
- **Already integrated**: Spawns coins automatically

## 🧪 TESTING CHECKLIST

### Core Systems
- [ ] **Coins spawn** with obstacles
- [ ] **Coin collection** adds to counter
- [ ] **Coin UI** updates correctly
- [ ] **Coin magnetism** works within range
- [ ] **Coin sounds** play on collection

### Upgrade System
- [ ] **Upgrade menu** opens with button
- [ ] **Purchase upgrades** with coins
- [ ] **Upgrades persist** after restart
- [ ] **Speed upgrade** affects player speed
- [ ] **Fire rate upgrade** reduces cooldown
- [ ] **Coin magnet upgrade** increases range

### Idle System
- [ ] **Close/reopen app** shows idle rewards
- [ ] **Idle calculations** are correct
- [ ] **Ad integration** doubles rewards
- [ ] **Idle UI** displays properly

## 🎯 VALORES RECOMENDADOS

### Upgrade Costs (Balanceados)
- **Speed Boost**: 50 + 25*level
- **Rapid Fire**: 75 + 35*level  
- **Momentum Master**: 100 + 50*level
- **Coin Magnet**: 60 + 30*level
- **Head Start**: 150 + 75*level

### Idle Rates
- **Base Rate**: 0.1 coins/second (360 coins/hour)
- **With Upgrades**: Up to ~0.5 coins/second
- **Max Offline**: 8 hours = ~1440 coins

### Coin Spawning
- **Spawn Chance**: 30% per obstacle
- **Coins Per Spawn**: 1-3 coins
- **Patterns**: Line, Triangle, Square, Cross

## 🚀 PERFORMANCE NOTES

1. **Object Pooling**: Coins auto-destroy, no pooling needed yet
2. **UI Updates**: Only when values change
3. **Idle Calculations**: Only on app focus/pause
4. **Upgrade Persistence**: PlayerPrefs (lightweight)

## 🔧 TROUBLESHOOTING

### Coins Not Spawning
- Check CoinSystem.coinPrefab is assigned
- Verify coinSpawnChance > 0
- Check ObstacleGenerator is calling SpawnCoinsAtPosition

### Upgrades Not Working
- Verify UpgradeSystem instance exists in scene
- Check PlayerController.baseSpeed references
- Ensure BubbleShooter.shootCooldown updates

### Idle Not Working
- Check IdleManager is on DontDestroyOnLoad object
- Verify DateTime parsing works on target platform
- Check Application.focusChanged events

---
**Created**: Dec 31, 2024 - Post Implementation  
**Status**: Ready for Unity integration  
**Next**: Create prefabs and test gameplay