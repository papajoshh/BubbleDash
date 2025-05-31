# SPRINT DE IMPLEMENTACIÓN ACTUAL - BUBBLE DASH

**Fecha**: 2024-12-30
**Objetivo**: Completar el MVP jugable con obstáculos, progresión y polish básico

## 🎯 OBJETIVOS DEL SPRINT

1. **Sistema de Obstáculos** - Generación procedural básica
2. **Progresión de Dificultad** - Incremento gradual
3. **Polish Básico** - Efectos visuales y feedback
4. **Sonidos Mínimos** - Efectos esenciales
5. **Sistema de Monedas** - Recolección y guardado

---

## 🤖 TAREAS PARA CLAUDE (Implementación de Scripts)

### 1. ObstacleGenerator.cs ⏳
**Descripción**: Sistema de generación procedural de obstáculos
**Funcionalidades**:
- Spawn de obstáculos adelante del jugador
- Diferentes tipos de obstáculos
- Incremento de dificultad con el tiempo
- Object pooling para performance

### 2. ObstaclePattern.cs ⏳
**Descripción**: Patrones predefinidos de obstáculos
**Funcionalidades**:
- Patrones fáciles, medios, difíciles
- Combinaciones de burbujas estáticas
- Espacios para que el jugador pase

### 3. CoinSystem.cs ⏳
**Descripción**: Sistema de monedas coleccionables
**Funcionalidades**:
- Spawn de monedas en el mundo
- Detección de colección
- Guardado persistente
- Integración con UI

### 4. SimpleEffects.cs ⏳
**Descripción**: Efectos visuales básicos sin partículas
**Funcionalidades**:
- Pop effect para burbujas (scale animation)
- Coin collection effect
- Combo text popup

### 5. SimpleSoundManager.cs ⏳
**Descripción**: Gestor de sonidos básico
**Funcionalidades**:
- Reproducir efectos de sonido
- Control de volumen
- Singleton pattern

---

## 👨‍💻 TAREAS PARA HUMANO (Unity Scene Setup)

### 1. CREAR PREFABS DE OBSTÁCULOS ⏳

#### Obstacle Type 1: "Wall Pattern"
1. **Crear GameObject vacío** → Renombrar: "ObstacleWall"
2. **Estructura**:
   ```
   ObstacleWall
   ├── BubbleStatic1 (Position: 0, 2, 0)
   ├── BubbleStatic2 (Position: 0, 1, 0)
   ├── BubbleStatic3 (Position: 0, 0, 0)
   ├── BubbleStatic4 (Position: 0, -1, 0)
   └── BubbleStatic5 (Position: 0, -2, 0)
   ```
3. **Para cada BubbleStatic**:
   - Duplicar el BubblePrefab
   - Eliminar Rigidbody2D (queremos que sean estáticas)
   - CircleCollider2D → Is Trigger: NO
   - Tag: "Obstacle"
   - Asignar color aleatorio del sprite
4. **Guardar como Prefab** en Assets/Prefabs/Obstacles/

#### Obstacle Type 2: "Gap Pattern"
1. **Crear GameObject vacío** → Renombrar: "ObstacleGap"
2. **Estructura** (deja hueco en el medio):
   ```
   ObstacleGap
   ├── TopGroup
   │   ├── BubbleStatic1 (Position: 0, 3, 0)
   │   └── BubbleStatic2 (Position: 0, 2, 0)
   └── BottomGroup
       ├── BubbleStatic3 (Position: 0, -2, 0)
       └── BubbleStatic4 (Position: 0, -3, 0)
   ```
3. **Mismo setup que Type 1**
4. **Guardar como Prefab**

#### Obstacle Type 3: "Diamond Pattern"
1. **Crear GameObject vacío** → Renombrar: "ObstacleDiamond"
2. **Estructura** (forma de diamante):
   ```
   ObstacleDiamond
   ├── BubbleStatic1 (Position: 0, 0, 0)     [Centro]
   ├── BubbleStatic2 (Position: -0.5, 0.5, 0) [Arriba-Izq]
   ├── BubbleStatic3 (Position: 0.5, 0.5, 0)  [Arriba-Der]
   ├── BubbleStatic4 (Position: -0.5, -0.5, 0)[Abajo-Izq]
   └── BubbleStatic5 (Position: 0.5, -0.5, 0) [Abajo-Der]
   ```
3. **Guardar como Prefab**

### 2. CREAR PREFAB DE MONEDA ⏳

1. **GameObject → 2D Object → Sprite** → Renombrar: "CoinPrefab"
2. **Configurar**:
   ```
   Sprite: tile_coin.png
   Order in Layer: 3
   Scale: (0.5, 0.5, 1)
   ```
3. **Add Components**:
   - CircleCollider2D → Is Trigger: YES
   - Tag: "Coin" (crear este tag)
4. **Agregar Script** (cuando Claude lo cree): Coin.cs
5. **Opcional**: Agregar rotación simple
   - Crear script RotateSimple.cs:
   ```csharp
   transform.Rotate(0, 0, 100 * Time.deltaTime);
   ```
6. **Guardar como Prefab** en Assets/Prefabs/

### 3. CREAR SPAWNER POINTS ⏳

1. **En la escena**, crear GameObject vacío: "SpawnManager"
2. **Crear hijos** (puntos de spawn):
   ```
   SpawnManager
   ├── ObstacleSpawnPoint (Position: 15, 0, 0)
   └── CoinSpawnPoint (Position: 12, 0, 0)
   ```
3. **Agregar ObstacleGenerator.cs** al SpawnManager (cuando esté listo)

### 4. CONFIGURAR EFECTOS VISUALES ⏳

1. **Crear GameObject**: "EffectsManager"
   - Add Component → SimpleEffects.cs
   - Configurar valores:
     ```
     Pop Scale Multiplier: 1.5
     Pop Duration: 0.3
     Shake Intensity: 0.1
     Flash Duration: 0.1
     ```

2. **OPCIONAL - Crear ComboTextPrefab**:
   - GameObject → 3D Object → Text - TextMeshPro
   - Renombrar: "ComboTextPrefab"
   - TextMeshPro settings:
     ```
     Text: "COMBO x5!"
     Font Size: 36
     Color: Amarillo (#FFFF00)
     Alignment: Center
     ```
   - RectTransform: Width 200, Height 50
   - Guardar como Prefab en Assets/Prefabs/UI/
   - Asignar en SimpleEffects → Combo Text Prefab

3. **Configurar Animation Curves** (en SimpleEffects Inspector):
   - Pop Curve: Hacer curva 0→1→0
   - Combo Text Curve: Hacer curva 0→1.2→1

### 5. CONFIGURAR AUDIO SOURCES ⏳

1. **En el jugador**: Add Component → Audio Source
   - Play On Awake: NO
   - Spatial Blend: 0 (2D sound)
   
2. **Crear GameObject**: "SoundManager"
   - Add Component → Audio Source (para música)
   - Add Component → SimpleSoundManager.cs (cuando esté listo)

### 5. CREAR UI PARA MONEDAS ⏳

1. **En HUDPanel**, crear nuevo texto: "CoinText"
   ```
   Anchor: Top Right
   Position: (-100, -20)
   Text: "Coins: 0"
   Font Size: 30
   Color: Dorado (#FFD700)
   ```
2. **Asignar al UIManager** cuando se actualice

---

## 📊 PROGRESO DEL SPRINT

### Claude está trabajando en:
- [ ] ObstacleGenerator.cs
- [ ] ObstaclePattern.cs
- [ ] CoinSystem.cs
- [ ] SimpleEffects.cs
- [ ] SimpleSoundManager.cs

### Humano debe hacer:
- [ ] Crear 3 prefabs de obstáculos
- [ ] Crear prefab de moneda
- [ ] Configurar spawn points
- [ ] Preparar audio sources
- [ ] Actualizar UI para monedas

---

## 🔄 FLUJO DE TRABAJO PARALELO

1. **Claude implementa** → ObstacleGenerator.cs
2. **Mientras tanto, Humano** → Crea los 3 prefabs de obstáculos
3. **Claude termina** → Avisa qué prefabs necesita asignados
4. **Humano asigna** → Y prueba
5. **Repetir** con cada sistema

---

## ⚡ COMUNICACIÓN DURANTE EL SPRINT

**Claude informará**:
- "ObstacleGenerator.cs completado - necesita prefabs asignados en [campo específico]"
- "CoinSystem.cs listo - asignar CoinPrefab en [inspector field]"

**Humano informará**:
- "Prefabs de obstáculos listos"
- "Problema encontrado: [descripción]"
- "UI de monedas configurada"

---

## 🎯 DEFINICIÓN DE "COMPLETADO"

El sprint estará completo cuando:
1. ✅ Los obstáculos se generen adelante del jugador
2. ✅ La dificultad aumente gradualmente
3. ✅ Las monedas se puedan recolectar
4. ✅ Los efectos visuales den feedback
5. ✅ Al menos 3 sonidos funcionen (pop, coin, game over)
6. ✅ El juego se sienta "pulido" para MVP

---

**SIGUIENTE PASO INMEDIATO**: Claude empezará con ObstacleGenerator.cs mientras el Humano crea los prefabs de obstáculos.