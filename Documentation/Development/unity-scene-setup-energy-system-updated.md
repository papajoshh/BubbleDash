# 🎮 GUÍA ACTUALIZADA - SETUP UNITY SCENE PARA ENERGY SYSTEM

## 📋 OVERVIEW

Esta guía te explica **exactamente** cómo configurar la escena de Unity para que funcione con el Energy System implementado. **IMPORTANTE:** Esta versión elimina todas las referencias al sistema de Timer obsoleto.

---

## 🗑️ PARTE 0: ELIMINAR ELEMENTOS OBSOLETOS

### Paso 0: Eliminar UI del Timer System

**En el Canvas, busca y ELIMINA estos elementos si existen:**

```
ELIMINAR:
├── TimerPanel ❌
├── TimerText ❌
├── GameOverTimerPanel ❌
├── PreRunTimerPanel ❌
└── Cualquier referencia a "Timer" en UI ❌
```

**En los GameObjects de la escena, ELIMINAR si existen:**
```
ELIMINAR GameObjects:
├── TimerManager ❌
├── GameOverTimerUI ❌
└── TimerUI ❌
```

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

1. **Add Component** → `EnergyManager` (script implementado)

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

1. **Add Component** → `ObjectiveManager` (script implementado)

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

1. **Add Component** → `WaveManager` (script implementado)

2. **Configurar valores en Inspector:**
   ```
   WAVE SETTINGS:
   ├── Distance Per Wave: 200
   └── Waves: (Se configura automáticamente en Start)
   ```

---

## 🎨 PARTE 2: UI CANVAS SETUP

### Paso 5: Estructura UI Principal

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
├── GameOverPanel (existente)
├── SafeZonePanel (NUEVO)
└── CompletionEffect (NUEVO)
```

### Paso 6: Crear Energy Panel

**Crear EnergyPanel:**

1. Click derecho en `HudPanel` → UI → Panel
2. Nombre: `EnergyPanel`

3. **Configure RectTransform:**
   ```
   Anchors: Top-Left
   Position: (150, -50)
   Width: 250
   Height: 40
   ```

4. **Crear Slider para Energy:**
   - Click derecho en `EnergyPanel` → UI → Slider
   - Nombre: `EnergySlider`
   - **RectTransform:**
     ```
     Anchors: Stretch-Stretch
     Left: 10, Right: -10
     Top: -5, Bottom: -5
     ```
   - **Slider Component:**
     ```
     Min Value: 0
     Max Value: 1
     Value: 1
     Whole Numbers: false
     ```

5. **Configurar Fill Area:**
   - En `EnergySlider/Fill Area/Fill`
   - **Image Component:**
     ```
     Color: (0, 255, 0, 255) - Green
     ```

6. **Crear Energy Text:**
   - UI → Text - TextMeshPro, nombre: `EnergyText`
   - Parent: `EnergyPanel`
   - **RectTransform:**
     ```
     Anchors: Center
     Position: (0, 0)
     Width: 50
     Height: 30
     ```
   - **TextMeshPro:**
     ```
     Text: "10"
     Font Size: 18
     Alignment: Center
     Color: White
     ```

### Paso 7: Crear Shield Display

**Dentro de EnergyPanel:**

1. **Crear ShieldPanel:**
   - UI → Panel, nombre: `ShieldPanel`
   - Parent: `EnergyPanel`
   - **RectTransform:**
     ```
     Anchors: Right
     Position: (-130, 0)
     Width: 100
     Height: 30
     ```

2. **Crear Shield Slider:**
   - UI → Slider, nombre: `ShieldSlider`
   - Parent: `ShieldPanel`
   - Configurar similar a EnergySlider pero más pequeño

3. **Crear Shield Text:**
   - UI → Text - TextMeshPro, nombre: `ShieldText`
   - Parent: `ShieldPanel`
   - Text: "5s"

### Paso 8: Crear Objective Panel

**Crear ObjectivePanel:**

1. Click derecho en `HudPanel` → UI → Panel
2. Nombre: `ObjectivePanel`

3. **Configure RectTransform:**
   ```
   Anchors: Top-Center
   Position: (0, -100)
   Width: 400
   Height: 80
   ```

4. **Estructura interna:**
   ```
   ObjectivePanel
   ├── ObjectiveTitle (TextMeshPro)
   ├── ObjectiveDescription (TextMeshPro)
   ├── ObjectiveProgressBar (Image con Fill Type)
   ├── ObjectiveProgressText (TextMeshPro)
   └── ObjectiveTimer (TextMeshPro)
   ```

### Paso 9: Crear Wave Panel

**Crear WavePanel:**

1. Click derecho en `HudPanel` → UI → Panel
2. Nombre: `WavePanel`

3. **Configure RectTransform:**
   ```
   Anchors: Top-Right
   Position: (-150, -50)
   Width: 200
   Height: 40
   ```

4. **Crear Wave Text:**
   - UI → Text - TextMeshPro, nombre: `WaveText`
   - Text: "WAVE 1: LEARNING"

5. **Crear Wave Progress Text:**
   - UI → Text - TextMeshPro, nombre: `WaveProgressText`
   - Text: "0%"

### Paso 10: Crear Safe Zone Panel

**Crear SafeZonePanel:**

1. Click derecho en `Canvas` (NO en HudPanel) → UI → Panel
2. Nombre: `SafeZonePanel`

3. **Configure RectTransform:**
   ```
   Anchors: Middle-Center
   Position: (0, 100)
   Width: 400
   Height: 100
   ```

4. **Crear Safe Zone Text:**
   - UI → Text - TextMeshPro, nombre: `SafeZoneText`
   - Text: "SAFE ZONE"
   - Font Size: 36
   - Color: Cyan

5. **Crear Safe Zone Background:**
   - En SafeZonePanel → Image Component
   - Color: (0, 255, 255, 30) - Cyan muy transparente

---

## 🔧 PARTE 3: ASIGNAR COMPONENTS

### Paso 11: Configurar EnergyUI Component

**Crear GameObject para EnergyUI:**

1. Click derecho en Canvas → Create Empty
2. Nombre: `EnergyUIController`
3. **Add Component** → `EnergyUI`

4. **Asignar referencias en Inspector:**
   ```
   ENERGY BAR:
   ├── Energy Slider: [EnergySlider]
   ├── Energy Fill Image: [EnergySlider/Fill Area/Fill]
   ├── Energy Text: [EnergyText]
   
   ENERGY COLORS:
   ├── High Energy Color: Green (0, 255, 0)
   ├── Medium Energy Color: Yellow (255, 255, 0)
   ├── Low Energy Color: Red (255, 0, 0)
   ├── Critical Energy Color: Dark Red (200, 50, 50)
   
   SHIELD DISPLAY:
   ├── Shield Panel: [ShieldPanel]
   ├── Shield Slider: [ShieldSlider]
   ├── Shield Text: [ShieldText]
   ├── Shield Icon: [null]
   
   SAFE ZONE DISPLAY:
   ├── Safe Zone Panel: [SafeZonePanel]
   ├── Safe Zone Text: [SafeZoneText]
   ├── Safe Zone Background: [SafeZonePanel Image]
   ```

### Paso 12: Configurar ObjectiveUI Component

**Crear GameObject para ObjectiveUI:**

1. Click derecho en Canvas → Create Empty
2. Nombre: `ObjectiveUIController`
3. **Add Component** → `ObjectiveUI`

4. **Asignar referencias en Inspector:**
   ```
   OBJECTIVE DISPLAY:
   ├── Objective Panel: [ObjectivePanel]
   ├── Objective Title: [ObjectiveTitle]
   ├── Objective Description: [ObjectiveDescription]
   ├── Objective Progress Slider: [null] // Solo si usas slider
   ├── Objective Progress Image: [ObjectiveProgressBar] // Si usas image con fill
   ├── Objective Progress Text: [ObjectiveProgressText]
   ├── Objective Icon: [null]
   ├── Objective Timer Text: [ObjectiveTimer]
   
   WAVE DISPLAY:
   ├── Wave Panel: [WavePanel]
   ├── Wave Text: [WaveText]
   ├── Wave Progress Text: [WaveProgressText]
   
   NOTA: Para la barra de progreso del objetivo puedes usar:
   - Slider: asigna en Objective Progress Slider
   - Image con Fill: asigna en Objective Progress Image
   ```

---

## 🎮 PARTE 4: CONFIGURAR INITIAL STATE

### Paso 13: Set Initial UI State

**Desactivar estos GameObjects al inicio:**

```
Panels que empiezan INACTIVE:
├── ObjectivePanel ❌
├── SafeZonePanel ❌
├── ShieldPanel ❌
```

**Panels que deben empezar ACTIVE:**
```
Panels que empiezan ACTIVE:
├── EnergyPanel ✅
├── WavePanel ✅
├── HudPanel ✅
```

---

## 📋 PARTE 5: TESTING CHECKLIST

### Testing del Energy System:

1. **Press Play**
2. **Verificar:**
   - ✅ Energy bar visible mostrando 10 (o 15 en beginner mode)
   - ✅ Wave text mostrando "WAVE 1: LEARNING ZONE"
   - ✅ Energy bajando gradualmente
   
3. **Al disparar bubbles:**
   - ✅ Energy sube con cada hit
   - ✅ Objectives aparecen después de 5 segundos
   
4. **Si no disparas:**
   - ✅ Energy baja hasta 0
   - ✅ Game Over cuando energy = 0

---

## 🚨 SOLUCIÓN DE PROBLEMAS

### NullReferenceException en EnergyUI/ObjectiveUI
- Verificar que TODAS las referencias están asignadas en Inspector

### UI no aparece
- Verificar que los GameObjects están activos
- Verificar que Canvas tiene Canvas Scaler configurado

### Energy no cambia
- Verificar que EnergyManager GameObject está en la escena
- Verificar que GameManager tiene los eventos OnGameStart/OnGameOver

### Los paneles están en posiciones incorrectas
- Revisar los Anchors de cada panel
- Usar Canvas Scaler con "Scale With Screen Size"

---

## ⚠️ NOTAS IMPORTANTES

1. **NO crear ningún TimerManager o TimerUI** - El sistema de timer ha sido reemplazado por el Energy System

2. **El Game Over ahora ocurre cuando Energy = 0**, no por tiempo

3. **Los objetivos tienen su propio timer interno** mostrado en el ObjectivePanel

4. **El ShieldPanel solo aparece cuando hay shield activo**

5. **SafeZonePanel aparece entre waves automáticamente**

¡Con esta configuración el Energy System estará listo para funcionar!