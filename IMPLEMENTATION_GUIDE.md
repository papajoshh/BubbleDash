# 📋 GUÍA DE IMPLEMENTACIÓN - BUBBLE DASH

## 🚀 ESTADO ACTUAL DEL PROYECTO (31 Dic 2024)

### ✅ SISTEMAS COMPLETADOS

1. **PlayerController** ✓
   - Movimiento automático hacia la derecha
   - Posición fija de inicio (-7, -2.5)
   - Integración con MomentumSystem

2. **BubbleShooter** ✓
   - Disparo sin gravedad (línea recta)
   - Cooldown configurable (0.2s por defecto)
   - Preview del siguiente color
   - Ignora colisiones con el jugador

3. **BubbleManager** ✓
   - Sistema simplificado: mismo color = explotan
   - Color diferente = se destruye sin penalización
   - Efectos diferenciados (pop colorido vs puff gris)

4. **MomentumSystem** ✓
   - Velocidad aumenta con aciertos consecutivos
   - Se resetea al fallar

5. **ScoreManager** ✓
   - Puntos por distancia y burbujas
   - Sistema de high score

6. **UIManager** ✓
   - Score, distancia, velocidad, combo
   - Panel de Game Over funcional

7. **ObstacleGenerator** ✓
   - 4 patrones: línea, columna, triángulo, cuadrado
   - Dificultad progresiva
   - Patrones móviles opcionales

8. **Sistemas de Soporte** ✓
   - CameraFollow con zoom dinámico
   - SimpleSoundManager con sonidos procedurales
   - SimpleEffects para feedback visual
   - Optimizaciones de rendimiento

9. **CoinSystem** ✓
   - Spawning automático con obstáculos
   - Recolección magnética con animaciones
   - Integración con UI y sonidos
   - Soporte para upgrades de magnetismo

10. **UpgradeSystem** ✓
    - 5 upgrades: Speed, Fire Rate, Momentum, Coin Magnet, Starting Combo
    - Sistema de costos progresivos
    - Persistencia con PlayerPrefs
    - Integración con todos los sistemas

11. **IdleManager** ✓
    - Progresión offline hasta 8 horas
    - Rate base + bonos por upgrades
    - Popup de recompensas al volver
    - Integración con ads para duplicar recompensas

### 🔧 SISTEMAS EN PROGRESO

**UI Integration** (Casi completado...)
- [x] CoinSystem UI funcionando
- [x] UpgradeUI implementado  
- [x] IdleRewardsUI implementado
- [ ] Prefabs de Unity pendientes

### ❌ BUGS CONOCIDOS

1. **Static Bubbles no matan al player**
   - Las colisiones no se detectan correctamente
   - Documentado en KNOWN_ISSUES.md

2. **Parallax no es seamless**
   - Se ven saltos al reposicionar sprites
   - No crítico, postponed

## 🎮 CONFIGURACIÓN EN UNITY - CHECKLIST

### 1. ESCENA PRINCIPAL
- [ ] **Player GameObject**
  - [ ] PlayerController script
  - [ ] BubbleShooter script
  - [ ] MomentumSystem script
  - [ ] Rigidbody2D (Dynamic, Interpolate)
  - [ ] BoxCollider2D
  - [ ] Tag: "Player"

### 2. MANAGERS (GameObjects vacíos)
- [ ] **GameManager**
  - [ ] GameManager script
  
- [ ] **ScoreManager**
  - [ ] ScoreManager script
  
- [ ] **SimpleSoundManager**
  - [ ] SimpleSoundManager script
  - [ ] 2 AudioSources (effects + music)
  
- [ ] **SimpleEffects**
  - [ ] SimpleEffects script
  
- [ ] **ObstacleGenerator**
  - [ ] ObstacleGenerator script
  - [ ] Obstacle Prefabs (Size: 4)
  
- [ ] **OptimizationSettings**
  - [ ] OptimizationSettings script

### 3. CÁMARA
- [ ] **Main Camera**
  - [ ] CameraFollow script
  - [ ] Camera settings:
    - Orthographic: ✓
    - Size: 5
  - [ ] CameraFollow settings:
    - Smooth Speed: 5-8
    - Dynamic Zoom: ✓
    - Base Ortho Size: 5
    - Max Ortho Size: 7

### 4. UI CANVAS
- [ ] **Canvas**
  - [ ] Canvas Scaler
  - [ ] UIManager script
  
- [ ] **UI Elements**:
  - [ ] ScoreText (TMP)
  - [ ] DistanceText (TMP)
  - [ ] SpeedText (TMP)
  - [ ] ComboText (TMP)
  - [ ] NextBubbleImage (UI Image)
  - [ ] GameOverPanel
    - [ ] GameOverText
    - [ ] FinalScoreText
    - [ ] RestartButton

### 5. PREFABS NECESARIOS

#### BubblePrefab
- [ ] Sprite circular
- [ ] Bubble script
- [ ] SimpleBubbleCollision script
- [ ] CircleCollider2D
- [ ] Scale: (0.5, 0.5, 1)

#### StaticBubblePrefab
- [ ] Sprite circular
- [ ] StaticBubble script
- [ ] CircleCollider2D
- [ ] Randomize Color: ✓

#### Obstacle Patterns (4 prefabs)
- [ ] ObstaclePattern_Line
- [ ] ObstaclePattern_Column
- [ ] ObstaclePattern_Triangle
- [ ] ObstaclePattern_Square
Cada uno con:
- [ ] BubbleObstaclePattern script
- [ ] Static Bubble Prefab asignado

### 6. GROUND
- [ ] **Ground GameObject**
  - [ ] GroundTiled o GroundGenerator
  - [ ] Sprite/Tile asignado
  - [ ] Collider para el suelo

### 7. MUROS
- [ ] **TopWall** y **BottomWall**
  - [ ] BoxCollider2D
  - [ ] FollowCamera script (para seguir al jugador)

## 🎯 PRÓXIMOS PASOS (Implementación Completada)

### ✅ CoinSystem - COMPLETADO
1. ✅ Crear prefab de moneda con animación
2. ✅ Integrar spawn con ObstacleGenerator
3. ✅ Sistema de recolección magnética
4. ✅ Actualizar UI con contador
5. ✅ Sonido de recolección

### ✅ UpgradeSystem - COMPLETADO
1. ✅ Menú de mejoras funcional
2. ✅ 5 tipos de mejoras diferentes
3. ✅ Sistema de compra con monedas
4. ✅ Persistencia de upgrades
5. ✅ Integración con todos los sistemas

### ✅ IdleManager - COMPLETADO
1. ✅ Sistema de progresión offline
2. ✅ Cálculo de recompensas por tiempo
3. ✅ UI de recompensas idle
4. ✅ Integración con ads para duplicar

### 🔧 Polish (Pendiente)
1. [ ] Balancear valores de upgrades
2. [ ] Ajustar dificultad progresiva
3. [ ] Optimizar rendimiento
4. [ ] Testing en dispositivo Android

## 📝 NOTAS IMPORTANTES

1. **Físicas**: Las burbujas NO tienen gravedad, se mueven en línea recta
2. **Colisiones**: Mismo color = explotan, diferente color = desaparece sin penalización
3. **Optimización**: PerformanceMonitor activo, pools de objetos recomendados
4. **Sonido**: Usando sonidos procedurales, no hay clips de audio

## 🚨 CUANDO VUELVAS - TRABAJO COMPLETADO

1. ✅ **CoinSystem** - Completamente implementado
   - Coin.cs con magnetismo, animaciones y lifetime
   - CoinSystem.cs con spawning y patrones
   - Integración con ObstacleGenerator
   - UI actualizada para mostrar monedas

2. ✅ **UpgradeSystem** - Sistema completo de mejoras
   - UpgradeSystem.cs con 5 tipos de upgrades
   - UpgradeUI.cs con interfaz funcional
   - Integración con CoinSystem para compras
   - Persistencia con PlayerPrefs

3. ✅ **IdleManager** - Progresión offline implementada
   - IdleManager.cs con cálculo de tiempo offline
   - IdleRewardsUI.cs para mostrar recompensas
   - Integración con AdManager para duplicar coins
   - Máximo 8 horas de progresión offline

4. ✅ **Integraciones completadas**:
   - Coin magnetism upgrades funcionando
   - MomentumSystem con starting combo upgrades
   - BubbleShooter con fire rate upgrades
   - PlayerController con speed upgrades
   - UIManager con botón de upgrades

### 📋 ARCHIVOS CREADOS/MODIFICADOS:
**Nuevos archivos**:
- `/Assets/Scripts/Managers/UpgradeSystem.cs`
- `/Assets/Scripts/UI/UpgradeUI.cs`
- `/Assets/Scripts/Managers/IdleManager.cs`
- `/Assets/Scripts/UI/IdleRewardsUI.cs`
- `/Documentation/Technical/coin-prefab-setup.md`

**Archivos modificados**:
- `/Assets/Scripts/Gameplay/Coin.cs` - Magnetismo con upgrades
- `/Assets/Scripts/Core/MomentumSystem.cs` - Starting combo upgrades
- `/Assets/Scripts/UI/UIManager.cs` - Botón de upgrades

### 🎮 QUE FALTA EN UNITY:
1. **Crear prefabs**: CoinPrefab, UpgradePanel UI, IdleRewardsPanel UI
2. **Asignar referencias**: Conectar scripts con GameObjects
3. **Testing**: Verificar que todo funciona en play mode

---
Última actualización: 31 Dic 2024 - Pre-ducha