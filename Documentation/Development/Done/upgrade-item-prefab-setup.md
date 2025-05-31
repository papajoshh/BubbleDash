# 🎯 UPGRADE ITEM PREFAB - GUÍA DE CREACIÓN

## 📋 OVERVIEW
El UpgradeItemPrefab es un elemento UI que se instancia dinámicamente dentro del ScrollView para mostrar cada upgrade disponible.

---

## 🔧 CREACIÓN PASO A PASO

### 1. Crear el GameObject Base
1. **En la Hierarchy** (NO dentro del Canvas todavía):
   - GameObject → Create Empty
   - Renombrar a "UpgradeItemPrefab"

2. **Agregar RectTransform**:
   - Add Component → Rect Transform (reemplaza el Transform normal)
   - Width: 760 (para caber en el ScrollView con padding)
   - Height: 80
   - Scale: (1, 1, 1)

### 2. Agregar Componentes Base
1. **Add Component** → UI → Image
   - Color: (0.3, 0.3, 0.3, 1) - Gris oscuro
   - Raycast Target: ✓

2. **Add Component** → Layout → Layout Element
   - Preferred Height: 80
   - Flexible Width: 1

### 3. Crear la Estructura Interna

#### A. Nombre del Upgrade
1. **Click derecho en UpgradeItemPrefab** → UI → Text - TextMeshPro
2. **Renombrar** a "NameText"
3. **Rect Transform**:
   - Anchors: Left, Middle
   - Anchor Min: (0, 0.5)
   - Anchor Max: (0, 0.5)
   - Pivot: (0, 0.5)
   - Position: (20, 10, 0)
   - Size: (300, 30)
4. **TextMeshPro Component**:
   - Text: "Upgrade Name"
   - Font Size: 20
   - Font Style: Bold
   - Color: Blanco
   - Alignment: Left, Middle

#### B. Descripción del Upgrade
1. **Click derecho en UpgradeItemPrefab** → UI → Text - TextMeshPro
2. **Renombrar** a "DescriptionText"
3. **Rect Transform**:
   - Anchors: Left, Middle
   - Position: (20, -15, 0)
   - Size: (400, 25)
4. **TextMeshPro Component**:
   - Text: "Upgrade description here"
   - Font Size: 14
   - Color: Gris claro (#CCCCCC)
   - Alignment: Left, Middle

#### C. Nivel Actual
1. **Click derecho en UpgradeItemPrefab** → UI → Text - TextMeshPro
2. **Renombrar** a "LevelText"
3. **Rect Transform**:
   - Anchors: Right, Middle
   - Anchor Min: (1, 0.5)
   - Anchor Max: (1, 0.5)
   - Pivot: (1, 0.5)
   - Position: (-180, 0, 0)
   - Size: (100, 40)
4. **TextMeshPro Component**:
   - Text: "Lv. 0/10"
   - Font Size: 16
   - Color: Cyan (#00FFFF)
   - Alignment: Center, Middle

#### D. Botón de Compra
1. **Click derecho en UpgradeItemPrefab** → UI → Button - TextMeshPro
2. **Renombrar** a "BuyButton"
3. **Rect Transform**:
   - Anchors: Right, Middle
   - Anchor Min: (1, 0.5)
   - Anchor Max: (1, 0.5)
   - Pivot: (1, 0.5)
   - Position: (-20, 0, 0)
   - Size: (120, 60)
4. **Image Component**:
   - Color: Verde (#44FF44)
5. **Buscar el Text (TMP) hijo del botón** y configurar:
   - Text: "$50"
   - Font Size: 18
   - Style: Bold
   - Alignment: Center

#### E. Texto de Costo (referencia para el script)
1. **Seleccionar** el Text (TMP) que está dentro del BuyButton
2. **Renombrar** a "CostText"
3. Este será referenciado por el script

### 4. Agregar el Script UpgradeItemUI

El script UpgradeUI.cs ya contiene la definición de UpgradeItemUI, pero necesitas agregarlo al prefab:

1. **Seleccionar UpgradeItemPrefab**
2. **Add Component** → UpgradeItemUI
3. **Asignar las referencias**:
   - Name Text: NameText
   - Description Text: DescriptionText
   - Level Text: LevelText
   - Cost Text: CostText (el texto dentro del BuyButton)
   - Buy Button: BuyButton

### 5. Visual Polish (Opcional pero Recomendado)

#### A. Agregar Borde/Outline
1. **Add Component** → UI → Outline (al UpgradeItemPrefab)
2. **Effect Color**: Negro (0,0,0,0.5)
3. **Effect Distance**: (1, -1)

#### B. Agregar Hover Effect
1. **En el BuyButton**, configurar **Button Component**:
   - Normal Color: Verde (#44FF44)
   - Highlighted Color: Verde Claro (#66FF66)
   - Pressed Color: Verde Oscuro (#22AA22)
   - Disabled Color: Gris (#666666)

#### C. Icono del Upgrade (Opcional)
1. **Click derecho en UpgradeItemPrefab** → UI → Image
2. **Renombrar** a "UpgradeIcon"
3. **Rect Transform**:
   - Position: (-20, 0, 0) desde la izquierda
   - Size: (60, 60)
4. **Image Component**:
   - Sprite: (Asignar icono según el upgrade)

### 6. Guardar como Prefab

1. **Arrastrar** UpgradeItemPrefab desde Hierarchy a:
   - `Assets/Prefabs/UI/UpgradeItemPrefab`
2. **Eliminar** el GameObject de la Hierarchy (ya está guardado como prefab)

### 7. Asignar al UpgradeUI

1. **Seleccionar** el UpgradePanel en tu Canvas
2. **En el script UpgradeUI**:
   - Upgrade Item Prefab: Asignar el prefab que acabas de crear

---

## 🎨 LAYOUT VISUAL

```
┌─────────────────────────────────────────────────────────┐
│ [Icon]  Speed Boost                           Lv.3/10  [$125] │
│         Increase base movement speed                     │
└─────────────────────────────────────────────────────────┘
```

---

## 🔍 VERIFICACIÓN EN RUNTIME

Cuando el juego esté corriendo:
1. El UpgradeUI creará automáticamente estos items
2. Cada upgrade del UpgradeSystem tendrá su propio item
3. Los botones cambiarán de color según el estado:
   - Verde: Puede comprar
   - Rojo: Sin suficientes monedas
   - Gris: Nivel máximo alcanzado

---

## ⚠️ ERRORES COMUNES

1. **Items no aparecen**: 
   - Verificar que el prefab está asignado en UpgradeUI
   - Verificar que UpgradeSystem tiene upgrades definidos

2. **Layout roto**:
   - El UpgradeContainer debe tener Vertical Layout Group
   - El prefab debe tener Layout Element con Preferred Height

3. **Botones no funcionan**:
   - Verificar que el Canvas tiene un EventSystem
   - El prefab debe tener Image component con Raycast Target = true

4. **Texto no se actualiza**:
   - Verificar que todas las referencias están asignadas en el prefab
   - Los campos no deben ser null en el inspector

---

## 🎮 TESTING MANUAL

Para probar sin jugar:
1. Instancia manualmente el prefab dentro del UpgradeContainer
2. Duplica varias veces para ver el scroll
3. Verifica que el layout se ajusta correctamente
4. Los textos deben ser visibles y legibles

---

**Creado**: 31 Diciembre 2024
**Componente**: UI System - Upgrades
**Dependencias**: UpgradeUI.cs, UpgradeSystem.cs