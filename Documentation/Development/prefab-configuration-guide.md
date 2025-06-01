# 🎮 GUÍA DE CONFIGURACIÓN - PREFABS PARA ENERGY SYSTEM

## 📋 OVERVIEW

Esta guía detalla cómo configurar y modificar los prefabs existentes para que funcionen correctamente con el Energy System.

---

## 🎯 PARTE 1: PLAYER PREFAB SETUP

### Configuración Player.prefab

**Ubicación:** `/Assets/Prefabs/Player.prefab`

**Verificar que el Player prefab tiene estos components:**

```
Player GameObject:
├── Transform
├── SpriteRenderer ✅
├── Rigidbody2D ✅
├── Collider2D ✅
├── PlayerController ✅ (existente)
├── MomentumSystem ✅ (existente)
└── BubbleShooter ✅ (existente)
```

**NO añadir EnergyManager, ObjectiveManager o WaveManager al Player** - estos van en la escena.

### Verificar PlayerController Settings

**En PlayerController component:**
```
MOVEMENT SETTINGS:
├── Base Speed: 3
└── Current Speed: (readonly)

BOUNDARIES:
├── Vertical Bounds: (-4.5, 2.0)
└── Forward Boundary: 20

STARTING POSITION:
├── Fixed Start Position: (-7, -2.5, 0)
└── Use Fixed Start Position: ✅ true

COLLISION:
└── Obstacle Layer: Default
```

### Verificar MomentumSystem Settings

**En MomentumSystem component:**
```
MOMENTUM SETTINGS:
├── Base Speed: 3
├── Speed Increment: 0.2
├── Speed Increase Per Hit: 0.1
├── Max Speed Multiplier: 3
└── Consecutive Hits: (readonly)

MOMENTUM DECAY:
├── Momentum Decay Time: 2
└── Reset On Miss: ✅ true
```

---

## 🫧 PARTE 2: BUBBLE PREFABS SETUP

### StaticBubble.prefab Configuration

**Ubicación:** `/Assets/Prefabs/Bubbles/StaticBubble.prefab`

**Component Configuration:**
```
StaticBubble GameObject:
├── Transform
├── SpriteRenderer
│   ├── Sprite: [tu bubble sprite]
│   ├── Color: White
│   ├── Sorting Layer: Default
│   └── Order in Layer: 1
├── CircleCollider2D
│   ├── Is Trigger: ❌ false
│   ├── Radius: 0.5
│   └── Material: None
└── StaticBubble (Script)
    ├── Bubble Color: (se asigna dinámicamente)
    └── Health: 1
```

### BubbleShootPrefab.prefab Configuration  

**Ubicación:** `/Assets/Prefabs/Bubbles/BubbleShootPrefab.prefab`

**Component Configuration:**
```
BubbleShootPrefab GameObject:
├── Transform
├── SpriteRenderer
│   ├── Sprite: [tu bubble sprite]
│   ├── Color: White
│   ├── Sorting Layer: Default
│   └── Order in Layer: 2
├── CircleCollider2D
│   ├── Is Trigger: ❌ false
│   ├── Radius: 0.4
│   └── Material: None
├── Rigidbody2D
│   ├── Mass: 1
│   ├── Linear Drag: 0
│   ├── Angular Drag: 0
│   ├── Gravity Scale: 0
│   └── Freeze Rotation Z: ✅ true
├── ShootingBubble (Script)
│   ├── Bubble Color: (se asigna dinámicamente)
│   ├── Shot Speed: 8
│   └── Lifetime: 10
└── SimpleBubbleCollision (Script)
    └── (configuración automática)
```

### CoinBubble.prefab Configuration

**Ubicación:** `/Assets/Prefabs/Bubbles/CoinBubble.prefab`

**Component Configuration:**
```
CoinBubble GameObject:
├── Transform
├── SpriteRenderer
│   ├── Sprite: [tu coin bubble sprite]
│   ├── Color: Gold (255, 215, 0)
│   ├── Sorting Layer: Default
│   └── Order in Layer: 3
├── CircleCollider2D
│   ├── Is Trigger: ✅ true
│   ├── Radius: 0.6
│   └── Material: None
└── CoinBubble (Script)
    ├── Coin Value: 1
    └── Collection Effect: [optional particle system]
```

---

## 💰 PARTE 3: COIN PREFABS SETUP

### CoinPrefab.prefab Configuration

**Ubicación:** `/Assets/Prefabs/CoinPrefab.prefab`

**Component Configuration:**
```
CoinPrefab GameObject:
├── Transform
├── SpriteRenderer
│   ├── Sprite: [tu coin sprite]
│   ├── Color: Gold (255, 215, 0)
│   ├── Sorting Layer: Default
│   └── Order in Layer: 4
├── CircleCollider2D
│   ├── Is Trigger: ✅ true
│   ├── Radius: 0.3
│   └── Material: None
├── Coin (Script)
│   ├── Coin Value: 1
│   ├── Auto Collect Distance: 1.5
│   └── Collection Effect: [optional]
└── RotateSimple (Script) [opcional]
    └── Rotation Speed: (50, 0, 0)
```

---

## 🧱 PARTE 4: OBSTACLE PREFABS SETUP

### Verificar Obstacle Patterns

**Ubicación:** `/Assets/Prefabs/Obstacles/`

**Para cada Obstacle Pattern prefab:**

```
ObstaclePattern GameObject:
├── Transform
├── Child Objects (StaticBubbles)
│   └── [Configurados según StaticBubble.prefab]
└── BubbleObstaclePattern (Script)
    ├── Pattern Type: (Line, Column, Square, Triangle)
    ├── Bubble Count: (se cuenta automáticamente)
    └── Difficulty Rating: 1-5
```

**Verificar que los obstacle patterns usan StaticBubble prefabs como children**, no GameObjects independientes.

---

## 🎨 PARTE 5: UI PREFABS SETUP

### ComboTextPrefab.prefab Configuration

**Ubicación:** `/Assets/Prefabs/UI/ComboTextPrefab.prefab`

**Si existe, verificar configuration:**
```
ComboTextPrefab GameObject:
├── Transform
├── TextMeshPro - Text
│   ├── Text: "COMBO x5!"
│   ├── Font Size: 36
│   ├── Color: Yellow
│   ├── Alignment: Center-Middle
│   └── Sorting Order: 10
└── (Optional) Animator para effects
```

### UpgradeItemPrefab.prefab Configuration

**Ubicación:** `/Assets/Prefabs/UI/UpgradeItemPrefab.prefab`

**Mantener configuración existente** - el Energy System no afecta los upgrades directamente.

---

## 🔧 PARTE 6: MANAGERS PREFAB SETUP

### SimpleEffects GameObject

**Verificar en la escena** que SimpleEffects tiene estos settings:

```
SimpleEffects GameObject:
├── Transform: (0, 0, 0)
└── SimpleEffects (Script)
    ├── BUBBLE POP EFFECT:
    │   ├── Pop Scale Multiplier: 1.5
    │   ├── Pop Duration: 0.3
    │   └── Pop Curve: (AnimationCurve)
    ├── COMBO TEXT EFFECT:
    │   ├── Combo Text Prefab: [ComboTextPrefab]
    │   ├── Combo Text Duration: 1
    │   ├── Combo Text Rise Speed: 2
    │   └── Combo Text Curve: (AnimationCurve)
    ├── SCREEN SHAKE:
    │   ├── Shake Intensity: 0.1
    │   └── Shake Duration: 0.2
    ├── COLOR FLASH:
    │   └── Flash Duration: 0.1
    └── SCREEN FLASH:
        ├── Screen Flash Overlay: (se crea automáticamente)
        └── Default Flash Duration: 0.3
```

---

## 📊 PARTE 7: SCENE OBJECTS SETUP

### Camera Setup

**Main Camera configuration:**
```
Main Camera:
├── Transform: (0, 0, -10)
├── Camera Component:
│   ├── Projection: Orthographic
│   ├── Size: 5
│   ├── Near: 0.3
│   ├── Far: 1000
│   └── Background: (tu color de background)
└── CameraFollow (Script)
    ├── Target: [Player Transform]
    ├── Follow X: ✅ true
    ├── Follow Y: ❌ false
    ├── Smooth Time: 0.3
    └── Offset: (0, 0, -10)
```

### Ground Setup

**Si tienes ground prefabs:**
```
Ground Objects:
├── Transform: (posición según diseño)
├── SpriteRenderer: (tu ground sprite)
├── Collider2D: (para physics si necesario)
└── GroundTiled (Script) [si aplica]
```

---

## 🎮 PARTE 8: AUDIO SETUP

### SimpleSoundManager Setup

**Verificar configuration:**
```
SimpleSoundManager GameObject:
├── Transform: (0, 0, 0)
├── AudioSource Components:
│   ├── Music Source
│   ├── SFX Source
│   └── UI Source
└── SimpleSoundManager (Script)
    ├── AUDIO CLIPS:
    │   ├── Bubble Pop Sound
    │   ├── Coin Collect Sound
    │   ├── Game Over Sound
    │   ├── Achievement Sound
    │   └── Background Music
    ├── VOLUME SETTINGS:
    │   ├── Master Volume: 1.0
    │   ├── Music Volume: 0.7
    │   └── SFX Volume: 0.8
    └── AUTO PLAY MUSIC: ✅ true
```

---

## 🔗 PARTE 9: INTEGRATION CHECKLIST

### Verificar References Entre Prefabs

**Player debe poder encontrar:**
- [x] StaticBubble components en obstacles
- [x] CoinBubble components para collection
- [x] Coin components para collection

**BubbleShooter debe poder instanciar:**
- [x] BubbleShootPrefab correctamente configured
- [x] Con correct physics y collision settings

**ObstacleGenerator debe poder instanciar:**
- [x] Obstacle pattern prefabs
- [x] Coin prefabs en posiciones válidas

### Script Integration Points

**Verificar que estos scripts van a poder comunicarse:**

```
Energy System Integration:
├── MomentumSystem.OnBubbleHit() → EnergyManager.OnBubbleHit()
├── CoinBubble.Collect() → EnergyManager.OnCoinCollected()
├── Coin.Collect() → EnergyManager.OnCoinCollected()
└── SimpleBubbleCollision → ObjectiveManager.OnBubbleHit()
```

---

## 🚨 TESTING CHECKLIST POR PREFAB

### Player Prefab Testing:
- [ ] Player se mueve correctamente
- [ ] MomentumSystem responds a bubble hits
- [ ] Collisions funcionan con obstacles
- [ ] Starting position es consistente

### Bubble Prefabs Testing:
- [ ] StaticBubbles se destruyen al ser hit
- [ ] ShootingBubbles vuelan en dirección correcta
- [ ] CoinBubbles trigger collection correctly
- [ ] Collision detection funciona entre bubble types

### Coin Prefabs Testing:
- [ ] Coins se pueden colectar
- [ ] Auto-collection funciona si implementado
- [ ] Visual feedback works on collection

### Obstacle Prefabs Testing:
- [ ] Patterns spawn correctly
- [ ] StaticBubbles en patterns respond correctly
- [ ] No overlapping issues

### UI Prefabs Testing:
- [ ] ComboText appears y animate correctly
- [ ] Text es readable en todas las resoluciones

---

## 📱 MOBILE CONSIDERATIONS

### Performance Settings por Prefab:

**Para optimización móvil:**

```
Rigidbody2D Settings:
├── Sleeping Mode: Start Awake
├── Interpolate: None (en bubbles)
└── Collision Detection: Discrete

SpriteRenderer Settings:
├── Additional Shader Channels: None
└── Material: Default UI Material

TextMeshPro Settings:
├── Extra Padding: ❌ false
├── Rich Text: ❌ false (si no necesario)
└── Raycast Target: ❌ false (en textos no clickeables)
```

---

## 🔧 FINAL VERIFICATION

### Pre-Implementation Checklist:

**Antes de que yo implemente el código:**
- [ ] Todos los prefabs están en las carpetas correctas
- [ ] Player prefab tiene all required components
- [ ] UI prefabs están properly configured
- [ ] Scene objects están correctly positioned
- [ ] Audio setup es functional
- [ ] Camera follow está working

**Post-Implementation Testing:**
- [ ] Energy bar responds to gameplay
- [ ] Objectives appear y se pueden completar
- [ ] Waves transition correctly
- [ ] All UI elements update properly
- [ ] Audio feedback works for energy events

---

**¡PERFECTO!** Con estos prefabs correctly configured, el Energy System funcionará seamlessly con tu setup existente.

**¿Hay algún prefab específico que necesites más detalles sobre su configuración?**