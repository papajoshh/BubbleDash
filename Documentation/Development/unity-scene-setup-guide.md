# GUÍA COMPLETA DE CONFIGURACIÓN DE ESCENA - BUBBLE DASH

## 📋 PREREQUISITOS
- Unity 2022.3 LTS instalado
- Proyecto configurado para Android
- Scripts compilados sin errores

## 🎮 PASO 1: CREAR LA ESCENA BASE

### 1.1 Nueva Escena
1. **File → New Scene** → Basic (Built-in)
2. **Guardar como**: `Assets/Scenes/GameScene.unity`
3. **Eliminar**: Main Camera y Directional Light por defecto

### 1.2 Configurar Cámara Principal
1. **GameObject → Camera** → Crear nueva cámara
2. **Renombrar**: "Main Camera"
3. **Transform**:
   ```
   Position: (0, 0, -10)
   Rotation: (0, 0, 0)
   Scale: (1, 1, 1)
   ```
4. **Camera Component**:
   ```
   Clear Flags: Solid Color
   Background: Color azul claro (#87CEEB)
   Projection: Orthographic
   Size: 5
   ```
5. **Tag**: MainCamera (importante!)

## 🎯 PASO 2: CONFIGURAR EL JUGADOR

### 2.1 Crear GameObject del Jugador
1. **GameObject → Create Empty** → Renombrar: "Player"
2. **Transform**:
   ```
   Position: (-7, 0, 0)  // Empieza a la izquierda
   Rotation: (0, 0, 0)
   Scale: (1, 1, 1)
   ```

### 2.2 Agregar Componentes Visuales
1. **Agregar SpriteRenderer**:
   - Sprite: Usa `blue_body_squircle.png` o cualquier sprite de personaje
   - Color: Blanco (para ver el sprite original)
   - Order in Layer: 1

2. **Agregar Collider2D**:
   - Add Component → Physics 2D → Box Collider 2D
   - Ajustar tamaño al sprite
   - Is Trigger: NO (false)

3. **Agregar Rigidbody2D**:
   - Add Component → Physics 2D → Rigidbody2D
   - Body Type: Kinematic
   - Freeze Rotation Z: ✓

### 2.3 Agregar Scripts al Jugador
1. **PlayerController.cs**:
   ```
   Base Speed: 3
   Vertical Bounds: (-4, 4)
   Forward Boundary: 20
   ```

2. **MomentumSystem.cs**:
   ```
   Base Speed: 3
   Speed Increment: 0.2
   Max Speed Multiplier: 3
   Momentum Decay Time: 2
   Reset On Miss: ✓
   ```

3. **BubbleShooter.cs**:
   ```
   Shoot Force: 10
   Trajectory Points: 30
   Trajectory Time Step: 0.1
   ```

### 2.4 Configurar el Punto de Disparo
1. **Crear hijo del Player**: GameObject → Create Empty
2. **Renombrar**: "ShootPoint"
3. **Transform**:
   ```
   Position: (0.5, 0, 0)  // Ligeramente adelante del jugador
   ```
4. **Asignar en BubbleShooter**: Arrastrar ShootPoint al campo "Shoot Point"

### 2.5 Configurar LineRenderer para Trayectoria
1. **En el Player GameObject**: Add Component → Line Renderer
2. **Configuración**:
   ```
   Positions: Size 0 (se llenará dinámicamente)
   Width: Start 0.05, End 0.05
   Material: Sprites-Default
   Color: Blanco con 50% alpha
   Use World Space: ✓
   ```
3. **Asignar en BubbleShooter**: El script lo encontrará automáticamente

## 🫧 PASO 3: CREAR EL PREFAB DE BURBUJA

### 3.1 Crear Burbuja Base
1. **GameObject → Create Empty** → Renombrar: "BubblePrefab"
2. **Transform**: Reset (todo en 0)

### 3.2 Configurar Componentes
1. **SpriteRenderer**:
   - Sprite: `blue_body_circle.png` (o cualquier círculo)
   - Order in Layer: 2

2. **CircleCollider2D**:
   - Radius: 0.5 (ajustar al tamaño del sprite)
   - Is Trigger: NO

3. **Rigidbody2D**:
   - Body Type: Dynamic
   - Mass: 1
   - Gravity Scale: 1
   - Collision Detection: Continuous

4. **Script Bubble.cs**:
   ```
   Max Lifetime: 10
   Stop Velocity Threshold: 0.5
   ```
   - Asignar sprites de colores en los campos correspondientes

### 3.3 Convertir a Prefab
1. **Arrastrar** BubblePrefab a `Assets/Prefabs/`
2. **Eliminar** de la escena
3. **Asignar** el prefab al BubbleShooter en el campo "Bubble Prefab"

## 🎮 PASO 4: CONFIGURAR MANAGERS

### 4.1 Game Manager
1. **GameObject → Create Empty** → Renombrar: "GameManager"
2. **Agregar Scripts**:
   - GameManager.cs
   - ScoreManager.cs
3. **GameManager configuración**:
   ```
   Auto Start Game: ✓
   ```
4. **ScoreManager configuración**:
   ```
   Points Per Meter: 1
   Points Per Bubble: 10
   Combo Multiplier: 5
   ```

### 4.2 Bubble Manager
1. **GameObject → Create Empty** → Renombrar: "BubbleManager"
2. **Agregar Script**: BubbleManager.cs
3. **Configuración**:
   ```
   Detection Radius: 0.6
   Min Match Count: 3
   ```

### 4.3 Ad Manager
1. **GameObject → Create Empty** → Renombrar: "AdManager"
2. **Agregar Script**: AdManager.cs
3. **Configuración**:
   ```
   Test Mode: ✓
   Games Until Interstitial: 3
   Simulate Ads: ✓
   Simulated Ad Duration: 3
   ```

## 📱 PASO 5: CONFIGURAR UI

### 5.1 Crear Canvas Principal
1. **GameObject → UI → Canvas** → Renombrar: "MainCanvas"
2. **Canvas Scaler**:
   ```
   UI Scale Mode: Scale With Screen Size
   Reference Resolution: 1080 x 1920
   Screen Match Mode: 0.5
   ```

### 5.2 HUD Panel (Interfaz durante el juego)
1. **Crear Panel hijo**: Right-click Canvas → UI → Panel → Renombrar: "HUDPanel"
2. **Rect Transform**: Stretch to full screen
3. **Image Component**: Alpha = 0 (transparente)

#### Elementos del HUD:
1. **Score Text** (arriba izquierda):
   ```
   Anchor: Top Left
   Position: (20, -20)
   Text: "Score: 0"
   Font Size: 36
   ```

2. **High Score Text** (arriba centro):
   ```
   Anchor: Top Center
   Position: (0, -20)
   Text: "Best: 0"
   Font Size: 24
   ```

3. **Distance Text** (arriba derecha):
   ```
   Anchor: Top Right
   Position: (-20, -20)
   Text: "0m"
   Font Size: 30
   ```

4. **Combo Text** (centro):
   ```
   Anchor: Center
   Position: (0, 200)
   Text: "Combo x1"
   Font Size: 48
   Color: Amarillo
   ```

5. **Speed Indicator** (barra de velocidad - opcional):
   ```
   Crear: UI → Slider o UI → Image
   Nombre: "SpeedIndicator"
   Anchor: Bottom Center
   Position: (0, 100)
   Width: 300, Height: 30
   ```
   
   **Si usas Image**:
   - Image Type: Filled
   - Fill Method: Horizontal
   - Fill Origin: Left
   - Color: Gradiente de verde a rojo
   
   **Si usas Slider**:
   - Desactivar "Interactable"
   - Eliminar Handle Slide Area
   - Personalizar Fill color

### 5.3 Game Over Panel
1. **Crear Panel hijo**: Right-click Canvas → UI → Panel → Renombrar: "GameOverPanel"
2. **Configurar como popup centrado**:
   ```
   Width: 600
   Height: 800
   Background: Semi-transparente negro
   ```

#### Elementos del Game Over:
1. **Title Text**: "GAME OVER" (Size: 60)
2. **Final Score Text**: "Final Score: 0" (Size: 40)
3. **New High Score Text**: "NEW HIGH SCORE!" (Size: 36, Color: Dorado)
4. **Coins Earned Text**: "Coins Earned: 0" (Size: 30)
5. **Double Coins Button**: "Watch Ad - Double Coins" (Size: 200x80)
6. **Restart Button**: "RESTART" (Size: 200x80)
7. **Quit Button**: "QUIT" (Size: 150x60)

### 5.4 Pause Panel
1. **Crear Panel hijo**: Similar a Game Over Panel
2. **Elementos**:
   - Title: "PAUSED"
   - Resume Button
   - Quit Button

### 5.5 Configurar UI Manager
1. **Agregar al Canvas**: UIManager.cs
2. **Asignar TODOS los elementos de UI**:
   - Arrastrar cada Text y Button a sus campos correspondientes
   - Verificar que no quede ningún campo vacío

## 🌍 PASO 6: CONFIGURAR EL MUNDO

### 6.1 Crear Carpeta de Organización
1. **En la Hierarchy**: GameObject → Create Empty → Renombrar: "World"
2. **Transform**: Reset (Position 0,0,0)
3. **Propósito**: Contendrá todos los elementos del mundo

### 6.2 Crear Límites del Mundo (Paredes)

#### Pared Superior (Top Wall):
1. **GameObject → Create Empty** → Renombrar: "TopWall"
2. **Hacer hijo de "World"** (arrastrar dentro)
3. **Transform**:
   ```
   Position: (0, 5.5, 0)
   Rotation: (0, 0, 0)
   Scale: (30, 1, 1)
   ```
4. **Add Component → Physics 2D → Box Collider 2D**:
   - Size: (1, 1) - Se escalará con el transform
   - Is Trigger: NO

#### Pared Inferior (Bottom Wall):
1. **GameObject → Create Empty** → Renombrar: "BottomWall"
2. **Hacer hijo de "World"**
3. **Transform**:
   ```
   Position: (0, -5.5, 0)
   Rotation: (0, 0, 0)
   Scale: (30, 1, 1)
   ```
4. **Add Component → Physics 2D → Box Collider 2D**:
   - Size: (1, 1)
   - Is Trigger: NO

#### Pared Trasera (Back Wall) - OPCIONAL:
1. **GameObject → Create Empty** → Renombrar: "BackWall"
2. **Hacer hijo de "World"**
3. **Transform**:
   ```
   Position: (-10, 0, 0)
   Rotation: (0, 0, 0)
   Scale: (1, 12, 1)
   ```
4. **Add Component → Physics 2D → Box Collider 2D**

### 6.3 Crear Zona de Burbujas (Bubble Area)

1. **GameObject → Create Empty** → Renombrar: "BubbleArea"
2. **Hacer hijo de "World"**
3. **Transform**:
   ```
   Position: (5, 0, 0)  // Área donde aparecerán obstáculos de burbujas
   ```
4. **Propósito**: Aquí se generarán las burbujas estáticas/obstáculos

### 6.4 Background del Mundo

#### Opción A - Color Sólido (Más Simple):
1. La cámara ya tiene Background color configurado
2. No necesitas hacer nada más

#### Opción B - Sprite de Fondo:
1. **GameObject → 2D Object → Sprite** → Renombrar: "Background"
2. **Hacer hijo de "World"**
3. **Transform**:
   ```
   Position: (0, 0, 5)  // Atrás de todo
   Scale: (20, 12, 1)   // Ajustar según necesites
   ```
4. **SpriteRenderer**:
   - Sprite: `tile_background_grass.png` o cualquier fondo
   - Order in Layer: -10
   - Color: Puedes oscurecerlo un poco

#### Opción C - Parallax Background (Avanzado):
1. **Crear múltiples capas de fondo**:
   - Background Layer 1: Nubes (Order -5)
   - Background Layer 2: Árboles lejanos (Order -8)
   - Background Layer 3: Cielo (Order -10)
2. **Cada capa se mueve a diferente velocidad**

### 6.5 Crear Obstáculo de Prueba

Para probar el Game Over:
1. **GameObject → Create Empty** → Renombrar: "TestObstacle"
2. **Transform**:
   ```
   Position: (5, 0, 0)  // En el camino del jugador
   Scale: (1, 2, 1)
   ```
3. **Add Component → SpriteRenderer**:
   - Sprite: Cualquier sprite o usa el tile_grey.png
   - Color: Rojo para que sea visible
   - Order in Layer: 1
4. **Add Component → Physics 2D → Box Collider 2D**:
   - Is Trigger: YES
5. **Tag**: "Obstacle" (crear este tag si no existe)

### 6.6 Estructura Final de la Jerarquía

```
Scene Hierarchy:
├── Main Camera
├── Player
│   └── ShootPoint
├── GameManager
├── BubbleManager  
├── AdManager
├── MainCanvas
│   ├── HUDPanel
│   │   ├── ScoreText
│   │   ├── HighScoreText
│   │   ├── DistanceText
│   │   ├── ComboText
│   │   └── SpeedIndicator
│   ├── GameOverPanel
│   └── PausePanel
└── World
    ├── TopWall
    ├── BottomWall
    ├── BackWall (opcional)
    ├── BubbleArea
    ├── Background (opcional)
    └── TestObstacle
```

### 6.7 Verificación Visual

En la Scene View deberías ver:
- **Jugador** a la izquierda (-7, 0)
- **Paredes** arriba y abajo formando un "túnel"
- **Obstáculo de prueba** en el medio
- **Área despejada** para disparar burbujas

### 6.8 Tags y Layers

1. **Edit → Project Settings → Tags and Layers**
2. **Crear Tag**: "Obstacle"
3. **Aplicar Tags**:
   - Player: "Player"
   - Obstáculos: "Obstacle"
   - Paredes: Sin tag especial

## 🎯 PASO 7: CONFIGURAR PHYSICS 2D

1. **Edit → Project Settings → Physics 2D**:
   ```
   Gravity: (0, -9.81)
   Default Material: None
   Velocity Iterations: 8
   Position Iterations: 3
   ```

2. **Layer Configuration**:
   - Default: Todo por defecto
   - No necesitas layers especiales por ahora

## 🚀 PASO 8: PRUEBA INICIAL

### 8.1 Lista de Verificación Pre-Prueba
- [ ] Player tiene todos los scripts (PlayerController, BubbleShooter, MomentumSystem)
- [ ] BubbleShooter tiene asignado: Bubble Prefab y Shoot Point
- [ ] Bubble Prefab tiene el script Bubble.cs
- [ ] Todos los Managers están en la escena
- [ ] UI Manager tiene todos los elementos asignados
- [ ] Main Camera está taggeada como "MainCamera"

### 8.2 Primera Prueba
1. **Play** → Deberías ver:
   - Jugador moviéndose automáticamente hacia la derecha
   - Click/Touch muestra línea de trayectoria
   - Soltar dispara una burbuja
   - UI muestra puntuación

### 8.3 Problemas Comunes y Soluciones

**"NullReferenceException en UIManager"**
- Solución: Asignar todos los elementos de UI en el Inspector

**"Las burbujas no se disparan"**
- Verificar que Bubble Prefab está asignado
- Verificar que ShootPoint está asignado

**"No se ve la línea de trayectoria"**
- Verificar que LineRenderer tiene material Sprites-Default
- Verificar que el color no es completamente transparente

**"El jugador no se mueve"**
- Verificar que Time.timeScale = 1
- Verificar que GameManager.autoStartGame = true

## 📝 PASO 9: AJUSTES FINALES

### 9.1 Optimización Móvil
1. **Quality Settings** (Edit → Project Settings → Quality):
   - Crear perfil "Mobile"
   - V Sync Count: Don't Sync
   - Anti Aliasing: Disabled

### 9.2 Guardar la Escena
1. **Ctrl+S** para guardar
2. **File → Build Settings** → Add Open Scenes

### 9.3 Prefab del Jugador (Opcional)
1. Arrastrar Player a carpeta Prefabs
2. Útil para múltiples niveles

## 🎮 CONTROLES DE PRUEBA

### En Editor:
- **Click izquierdo**: Apuntar y disparar
- **ESC**: Pausar/Reanudar
- **R**: Restart (solo en Game Over)

### En Móvil:
- **Touch**: Apuntar y disparar
- **Back button**: Pausar

## ✅ CHECKLIST FINAL

- [ ] Player se mueve automáticamente
- [ ] Disparos funcionan con física
- [ ] Burbujas detectan colisiones
- [ ] Score aumenta con distancia
- [ ] Combo system funciona
- [ ] Game Over se activa (crear obstáculo de prueba)
- [ ] UI se actualiza correctamente
- [ ] Botón "Double Coins" aparece
- [ ] Ads simulados muestran logs

---

**¡LISTO!** Tu escena está configurada. Ahora puedes empezar a agregar obstáculos, mejorar visuales y pulir el gameplay.