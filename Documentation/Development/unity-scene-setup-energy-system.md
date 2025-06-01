# 🎮 GUÍA DETALLADA - SETUP UNITY SCENE PARA ENERGY SYSTEM

## 📋 OVERVIEW

Esta guía te explica **exactamente** cómo configurar la escena de Unity para que funcione con el Energy System que voy a implementar. Sigue cada paso **en orden** y usa los valores exactos que te doy.

---

## 🏗️ PARTE 1: MANAGERS SETUP

### Paso 1: Crear Manager GameObjects

En la escena `Main.unity`, crea estos GameObjects **vacíos** en la raíz de la jerarquía:

```
Hierarchy Structure:
├── GameManager (existente)
├── EnergyManager (NUEVO)
├── ObjectiveManager (NUEVO)
├── WaveManager (NUEVO)
├── MomentumSystem (existente, en Player)
├── ScoreManager (existente)
├── CoinSystem (existente)
└── BubbleManager (existente)
```

**Pasos específicos:**
1. Click derecho en Hierarchy → Create Empty
2. Nombra: `EnergyManager`
3. Repetir para: `ObjectiveManager`, `WaveManager`
4. Posición de todos: `(0, 0, 0)`

### Paso 2: Configurar EnergyManager

**En el GameObject `EnergyManager`:**

1. **Add Component** → `EnergyManager` (script que yo implementaré)

2. **Configurar valores en Inspector:**
   ```
   ENERGY SETTINGS:
   ├── Max Energy: 10
   ├── Current Energy: 10 (readonly)
   ├── Energy Drain Rate: 1
   ├── Energy Per Hit: 1
   ├── Energy Per Objective: 3
   └── Energy Per Coin: 0.5

   LEARNING CURVE:
   ├── Beginner Energy Bonus: 5
   ├── Beginner Drain Reduction: 0.5
   └── Beginner Runs Count: 3

   ENERGY SHIELD:
   ├── Max Shield Time: 5
   └── Current Shield Time: 0 (readonly)

   SAFE ZONES:
   ├── Safe Zone Duration: 3
   └── Is In Safe Zone: false (readonly)
   ```

### Paso 3: Configurar ObjectiveManager  

**En el GameObject `ObjectiveManager`:**

1. **Add Component** → `ObjectiveManager` (script que yo implementaré)

2. **Configurar valores en Inspector:**
   ```
   OBJECTIVE SETTINGS:
   ├── Objective Interval: 20
   ├── Objective Time Limit: 30
   └── Max Active Objectives: 1

   OBJECTIVE TYPES:
   └── Available Objectives: (Se configura automáticamente en Start)
   ```

### Paso 4: Configurar WaveManager

**En el GameObject `WaveManager`:**

1. **Add Component** → `WaveManager` (script que yo implementaré)

2. **Configurar valores en Inspector:**
   ```
   WAVE SETTINGS:
   ├── Distance Per Wave: 200
   └── Waves: (Se configura automáticamente en Start)
   ```

---

## 🎨 PARTE 2: UI CANVAS SETUP

### Paso 5: Modificar Canvas Existente

**Encuentra el Canvas existente** (probablemente llamado `Canvas` o `UICanvas`)

**Estructura UI necesaria:**
```
Canvas
├── HudPanel (existente)
│   ├── ScoreText (existente)
│   ├── DistanceText (existente)
│   ├── CoinText (existente)
│   ├── EnergyPanel (NUEVO)
│   ├── ObjectivePanel (NUEVO)
│   └── WavePanel (NUEVO)
├── PausePanel (existente)
└── GameOverPanel (existente)
```

### Paso 6: Crear Energy Panel

**Crear EnergyPanel:**

1. Click derecho en `HudPanel` → UI → Panel
2. Nombre: `EnergyPanel`

3. **Configure RectTransform:**
   ```
   Anchors: Top-Left
   Position: (50, -50)
   Width: 200
   Height: 80
   ```

4. **Panel Background:**
   ```
   Image Component:
   ├── Source Image: None (transparent)
   ├── Color: (0, 0, 0, 100) - Semi-transparent black
   └── Raycast Target: false
   ```

**Dentro de EnergyPanel, crear estos elementos:**

#### Energy Background Bar:
1. UI → Image, nombre: `EnergyBarBackground`
2. **RectTransform:**
   ```
   Anchors: Stretch-Middle
   Left: 10, Right: -10
   Top: -15, Bottom: -35
   ```
3. **Image settings:**
   ```
   Source Image: UI/Default
   Image Type: Sliced
   Color: (50, 50, 50, 255) - Dark gray
   ```

#### Energy Fill Bar:
1. UI → Image, nombre: `EnergyBar`
2. **Parent:** `EnergyBarBackground`
3. **RectTransform:**
   ```
   Anchors: Stretch-Stretch
   Left: 2, Right: -2
   Top: -2, Bottom: -2
   ```
4. **Image settings:**
   ```
   Source Image: UI/Default
   Image Type: Filled
   Fill Method: Horizontal
   Fill Origin: Left
   Color: (0, 255, 0, 255) - Green (will change via gradient)
   ```

#### Energy Text:
1. UI → Text - TextMeshPro, nombre: `EnergyText`
2. **RectTransform:**
   ```
   Anchors: Middle-Center
   Position: (0, 8)
   Width: 80
   Height: 20
   ```
3. **TextMeshPro settings:**
   ```
   Text: "10.0"
   Font Size: 14
   Alignment: Center-Middle
   Color: White
   ```

#### Shield Indicator:
1. UI → Image, nombre: `ShieldIndicator`
2. **RectTransform:**
   ```
   Anchors: Top-Right
   Position: (-15, -15)
   Width: 30
   Height: 30
   ```
3. **Image settings:**
   ```
   Source Image: (cualquier icono circular que tengas)
   Color: (0, 255, 255, 255) - Cyan
   Preserve Aspect: true
   ```

#### Shield Time Text:
1. UI → Text - TextMeshPro, nombre: `ShieldTimeText`
2. **Parent:** `ShieldIndicator`
3. **RectTransform:**
   ```
   Anchors: Bottom-Center
   Position: (0, -15)
   Width: 40
   Height: 15
   ```
4. **TextMeshPro settings:**
   ```
   Text: "3.0s"
   Font Size: 10
   Alignment: Center-Middle
   Color: White
   ```

### Paso 7: Crear Objective Panel

**Crear ObjectivePanel:**

1. Click derecho en `HudPanel` → UI → Panel
2. Nombre: `ObjectivePanel`

3. **Configure RectTransform:**
   ```
   Anchors: Top-Center
   Position: (0, -50)
   Width: 300
   Height: 100
   ```

4. **Panel Background:**
   ```
   Image Component:
   ├── Source Image: UI/Default
   ├── Image Type: Sliced
   ├── Color: (20, 20, 40, 200) - Dark blue semi-transparent
   └── Raycast Target: false
   ```

**Dentro de ObjectivePanel:**

#### Objective Title:
1. UI → Text - TextMeshPro, nombre: `ObjectiveTitle`
2. **RectTransform:**
   ```
   Anchors: Top-Stretch
   Left: 10, Right: -10
   Top: -10, Bottom: -30
   ```
3. **TextMeshPro settings:**
   ```
   Text: "OBJECTIVE TITLE"
   Font Size: 16
   Font Style: Bold
   Alignment: Center-Middle
   Color: Yellow
   ```

#### Objective Description:
1. UI → Text - TextMeshPro, nombre: `ObjectiveDescription`
2. **RectTransform:**
   ```
   Anchors: Top-Stretch
   Left: 10, Right: -10
   Top: -35, Bottom: -55
   ```
3. **TextMeshPro settings:**
   ```
   Text: "Objective description here"
   Font Size: 12
   Alignment: Center-Middle
   Color: White
   ```

#### Progress Bar Background:
1. UI → Image, nombre: `ProgressBarBackground`
2. **RectTransform:**
   ```
   Anchors: Bottom-Stretch
   Left: 10, Right: -10
   Top: -70, Bottom: -85
   ```
3. **Image settings:**
   ```
   Source Image: UI/Default
   Color: (100, 100, 100, 255) - Gray
   ```

#### Progress Bar Fill:
1. UI → Image, nombre: `ObjectiveProgressBar`
2. **Parent:** `ProgressBarBackground`
3. **RectTransform:**
   ```
   Anchors: Stretch-Stretch
   Left: 1, Right: -1
   Top: -1, Bottom: -1
   ```
4. **Image settings:**
   ```
   Source Image: UI/Default
   Image Type: Filled
   Fill Method: Horizontal
   Fill Origin: Left
   Color: (0, 255, 0, 255) - Green
   ```

#### Progress Text:
1. UI → Text - TextMeshPro, nombre: `ObjectiveProgressText`
2. **RectTransform:**
   ```
   Anchors: Bottom-Left
   Position: (15, -75)
   Width: 60
   Height: 15
   ```
3. **TextMeshPro settings:**
   ```
   Text: "0/5"
   Font Size: 11
   Color: White
   ```

#### Timer Text:
1. UI → Text - TextMeshPro, nombre: `ObjectiveTimer`
2. **RectTransform:**
   ```
   Anchors: Bottom-Right
   Position: (-15, -75)
   Width: 60
   Height: 15
   ```
3. **TextMeshPro settings:**
   ```
   Text: "30s"
   Font Size: 11
   Alignment: Right-Middle
   Color: White
   ```

### Paso 8: Crear Wave Panel

**Crear WavePanel:**

1. Click derecho en `HudPanel` → UI → Panel
2. Nombre: `WavePanel`

3. **Configure RectTransform:**
   ```
   Anchors: Top-Right
   Position: (-50, -50)
   Width: 180
   Height: 60
   ```

**Dentro de WavePanel:**

#### Wave Text:
1. UI → Text - TextMeshPro, nombre: `WaveText`
2. **RectTransform:**
   ```
   Anchors: Top-Stretch
   Left: 5, Right: -5
   Top: -5, Bottom: -25
   ```
3. **TextMeshPro settings:**
   ```
   Text: "WAVE 1: LEARNING"
   Font Size: 12
   Font Style: Bold
   Alignment: Center-Middle
   Color: White
   ```

#### Wave Progress Bar:
1. UI → Image, nombre: `WaveProgressBackground`
2. **RectTransform:**
   ```
   Anchors: Bottom-Stretch
   Left: 5, Right: -5
   Top: -30, Bottom: -40
   ```
3. **Image settings:**
   ```
   Source Image: UI/Default
   Color: (80, 80, 80, 255) - Dark gray
   ```

#### Wave Progress Fill:
1. UI → Image, nombre: `WaveProgressBar`
2. **Parent:** `WaveProgressBackground`
3. **RectTransform:**
   ```
   Anchors: Stretch-Stretch
   Left: 1, Right: -1
   Top: -1, Bottom: -1
   ```
4. **Image settings:**
   ```
   Source Image: UI/Default
   Image Type: Filled
   Fill Method: Horizontal
   Fill Origin: Left
   Color: (255, 200, 0, 255) - Orange
   ```

#### Distance Text:
1. UI → Text - TextMeshPro, nombre: `DistanceToNextWaveText`
2. **RectTransform:**
   ```
   Anchors: Bottom-Stretch
   Left: 5, Right: -5
   Top: -45, Bottom: -55
   ```
3. **TextMeshPro settings:**
   ```
   Text: "150m to next wave"
   Font Size: 10
   Alignment: Center-Middle
   Color: (200, 200, 200, 255) - Light gray
   ```

### Paso 9: Crear Safe Zone Panel

**Crear SafeZonePanel:**

1. Click derecho en `Canvas` → UI → Panel
2. Nombre: `SafeZonePanel`

3. **Configure RectTransform:**
   ```
   Anchors: Stretch-Stretch
   Left: 0, Right: 0
   Top: 0, Bottom: 0
   ```

4. **Panel Background:**
   ```
   Image Component:
   ├── Source Image: None
   ├── Color: (0, 255, 255, 50) - Cyan very transparent
   └── Raycast Target: false
   ```

**Dentro de SafeZonePanel:**

#### Safe Zone Text:
1. UI → Text - TextMeshPro, nombre: `SafeZoneText`
2. **RectTransform:**
   ```
   Anchors: Middle-Center
   Position: (0, 0)
   Width: 300
   Height: 80
   ```
3. **TextMeshPro settings:**
   ```
   Text: "SAFE ZONE"
   Font Size: 36
   Font Style: Bold
   Alignment: Center-Middle
   Color: (0, 255, 255, 255) - Cyan
   ```

### Paso 10: Crear Completion Effect Panel

**Crear CompletionEffectPanel:**

1. Click derecho en `Canvas` → UI → Panel
2. Nombre: `CompletionEffect`

3. **Configure RectTransform:**
   ```
   Anchors: Middle-Center
   Position: (0, 50)
   Width: 400
   Height: 100
   ```

4. **Panel Background:**
   ```
   Image Component:
   ├── Source Image: None
   ├── Color: Transparent
   └── Raycast Target: false
   ```

**Dentro de CompletionEffect:**

#### Completion Text:
1. UI → Text - TextMeshPro, nombre: `CompletionText`
2. **RectTransform:**
   ```
   Anchors: Stretch-Stretch
   Left: 0, Right: 0
   Top: 0, Bottom: 0
   ```
3. **TextMeshPro settings:**
   ```
   Text: "OBJECTIVE COMPLETE!"
   Font Size: 24
   Font Style: Bold
   Alignment: Center-Middle
   Color: (0, 255, 0, 255) - Green
   ```

---

## 🔧 PARTE 3: ASIGNAR COMPONENTS A UI SCRIPTS

### Paso 11: Configurar EnergyUI Component

**En el GameObject `EnergyPanel`:**

1. **Add Component** → `EnergyUI` (script que yo implementaré)

2. **Drag and drop referencias en Inspector:**
   ```
   ENERGY DISPLAY:
   ├── Energy Bar: [EnergyBar GameObject]
   ├── Energy Bar Background: [EnergyBarBackground GameObject]
   ├── Energy Text: [EnergyText GameObject]
   └── Energy Gradient: (Create New Gradient)

   SHIELD DISPLAY:
   ├── Shield Indicator: [ShieldIndicator GameObject]  
   └── Shield Time Text: [ShieldTimeText GameObject]

   SAFE ZONE DISPLAY:
   ├── Safe Zone Panel: [SafeZonePanel GameObject]
   └── Safe Zone Text: [SafeZoneText GameObject]

   WAVE DISPLAY:
   ├── Wave Text: [WaveText GameObject]
   ├── Wave Progress Bar: [WaveProgressBar GameObject]
   └── Distance To Next Wave Text: [DistanceToNextWaveText GameObject]

   EFFECTS:
   ├── Pulse Intensity: 1.2
   └── Low Energy Threshold: 0.3
   ```

3. **Configurar Energy Gradient:**
   - Click en `Energy Gradient` → Create new Gradient
   - Color keys:
     ```
     Position 0%: Red (255, 0, 0)
     Position 50%: Yellow (255, 255, 0)  
     Position 100%: Green (0, 255, 0)
     ```

### Paso 12: Configurar ObjectiveUI Component

**En el GameObject `ObjectivePanel`:**

1. **Add Component** → `ObjectiveUI` (script que yo implementaré)

2. **Drag and drop referencias en Inspector:**
   ```
   OBJECTIVE DISPLAY:
   ├── Objective Panel: [ObjectivePanel GameObject]
   ├── Objective Title: [ObjectiveTitle GameObject]
   ├── Objective Description: [ObjectiveDescription GameObject]
   ├── Objective Progress Bar: [ObjectiveProgressBar GameObject]
   ├── Objective Progress Text: [ObjectiveProgressText GameObject]
   ├── Objective Icon: [null por ahora]
   └── Objective Timer: [ObjectiveTimer GameObject]

   COMPLETION EFFECTS:
   ├── Completion Effect: [CompletionEffect GameObject]
   ├── Completion Text: [CompletionText GameObject]
   ├── Completion Color: Green (0, 255, 0)
   └── Failure Color: Red (255, 0, 0)
   ```

---

## 🎮 PARTE 4: CONFIGURAR INITIAL STATE

### Paso 13: Set Initial UI State

**Manually set estos GameObjects como INACTIVE** (unchecked) en Inspector:

```
Panels to START INACTIVE:
├── ObjectivePanel ❌
├── SafeZonePanel ❌
├── CompletionEffect ❌
└── ShieldIndicator ❌
```

**Panels que deben empezar ACTIVE:**
```
Panels to START ACTIVE:
├── EnergyPanel ✅
├── WavePanel ✅
└── HudPanel ✅ (existing)
```

### Paso 14: Player Prefab Setup

**Si tienes Player como prefab:**

1. **Abrir Player prefab** para editar
2. Verificar que estos components existen:
   ```
   Player GameObject:
   ├── PlayerController ✅ (existente)
   ├── MomentumSystem ✅ (existente)
   ├── SpriteRenderer ✅ (existente)
   ├── Collider2D ✅ (existente)
   └── [NO añadir EnergyManager aquí - va en escena]
   ```

### Paso 15: GameManager Integration

**En el GameObject `GameManager` existente:**

Verificar que el GameManager.cs tiene estos eventos que mis scripts necesitan:
```csharp
// Estos eventos deben existir en GameManager:
public System.Action OnGameStart;
public System.Action OnGameOver;  
public System.Action OnGameRestart;
```

Si no existen, agregámelos manualmente o yo los añadiré en mi implementación.

---

## 📋 PARTE 5: TESTING CHECKLIST

### Paso 16: Verification Checklist

**Antes de testing, verificar:**

✅ **Managers Setup:**
- [ ] EnergyManager GameObject existe con component
- [ ] ObjectiveManager GameObject existe con component  
- [ ] WaveManager GameObject existe con component

✅ **UI Structure:**
- [ ] EnergyPanel con todos los elementos child
- [ ] ObjectivePanel con todos los elementos child
- [ ] WavePanel con todos los elementos child
- [ ] SafeZonePanel configurado
- [ ] CompletionEffect configurado

✅ **Component Referencias:**
- [ ] EnergyUI tiene todas las referencias assigned
- [ ] ObjectiveUI tiene todas las referencias assigned
- [ ] Energy Gradient configurado correctamente

✅ **Initial States:**
- [ ] Panels inactivos están unchecked
- [ ] Panels activos están checked
- [ ] Todos los TextMeshPro tienen placeholder text

### Paso 17: First Run Test

**Cuando yo termine la implementación, para testing:**

1. **Press Play**
2. **Verificar que aparece:**
   - Energy bar mostrando 10.0 (o 15.0 en beginner mode)
   - Wave text mostrando "WAVE 1: LEARNING ZONE"  
   - Energy bar empezando a bajar lentamente
3. **Disparar bubbles:**
   - Energy debería subir con cada hit
   - Progress hacia objetivos debería aparecer
4. **No disparar por varios segundos:**
   - Energy debería bajar hasta game over

---

## 🚨 TROUBLESHOOTING COMÚN

### Problema: NullReferenceException
**Solución:** Verificar que todas las UI referencias están assigned en EnergyUI y ObjectiveUI

### Problema: UI elements no aparecen
**Solución:** Verificar que Canvas tiene GraphicRaycaster y Canvas Scaler

### Problema: Energy no cambia
**Solución:** Verificar que EnergyManager está en la escena y activo

### Problema: Objectives no aparecen
**Solución:** Verificar que ObjectivePanel está correctamente referenciado en ObjectiveUI

### Problema: Text es demasiado pequeño
**Solución:** Ajustar Canvas Scaler a "Scale With Screen Size" y Reference Resolution 1920x1080

---

## ⚙️ CONFIGURACIONES FINALES

### Canvas Scaler Settings:
```
Canvas Scaler Component:
├── UI Scale Mode: Scale With Screen Size
├── Reference Resolution: 1920 x 1080
├── Screen Match Mode: Match Width Or Height  
├── Match: 0.5
└── Reference Pixels Per Unit: 100
```

### Canvas Settings:
```
Canvas Component:
├── Render Mode: Screen Space - Overlay
├── Pixel Perfect: false
├── Sort Order: 0
└── Target Display: Display 1
```

**¡LISTO!** Una vez que termines este setup, estaré listo para que pruebes el Energy System completo.

**¿Algún paso no está claro o necesitas más detalles sobre alguna configuración específica?**