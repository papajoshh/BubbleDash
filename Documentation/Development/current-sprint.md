# BUBBLE DASH - SPRINT ACTUAL

## SPRINT GOAL
**Objetivo**: Implementar mecánicas core funcionales y gameplayloop básico
**Duración**: 48-72 horas (Weekend Sprint)
**Estado**: 🔄 EN PROGRESO

## BACKLOG PRIORIZADO

### 🔴 CRÍTICO (Hacer HOY)
- [ ] **PlayerController**: Personaje con movimiento automático
- [ ] **BubbleShooter**: Sistema de disparo con física básica
- [ ] **BubbleManager**: Detección de combinaciones y eliminación
- [ ] **MomentumSystem**: Velocidad variable basada en aciertos
- [ ] **ScoreManager**: Puntuación básica por distancia y burbujas

### 🟡 IMPORTANTE (Hacer MAÑANA)
- [ ] **ObstacleGenerator**: Generación procedural de obstáculos simples
- [ ] **GameManager**: Estados del juego (Playing, GameOver, Restart)
- [ ] **UIManager**: Interfaz básica (Score, Game Over, Restart)
- [ ] **CoinSystem**: Recolección de monedas básica
- [ ] **UpgradeSystem**: Mejoras simples (velocidad, disparo)

### 🟢 DESEADO (Si hay tiempo)
- [ ] **IdleManager**: Progresión offline básica
- [ ] **AudioManager**: Efectos de sonido simples
- [ ] **EffectsManager**: Partículas para feedback visual
- [ ] **SaveSystem**: Persistencia de progreso básica

## ARQUITECTURA DE CÓDIGO

### Estructura de Scripts Requerida
```
Assets/Scripts/
├── Core/
│   ├── PlayerController.cs      # Movimiento del personaje
│   ├── BubbleShooter.cs        # Sistema de disparo
│   ├── BubbleManager.cs        # Lógica de burbujas
│   └── MomentumSystem.cs       # Sistema de velocidad
├── Managers/
│   ├── GameManager.cs          # Estados del juego
│   ├── ScoreManager.cs         # Puntuación
│   ├── UIManager.cs            # Interfaz
│   └── UpgradeSystem.cs        # Mejoras
├── Gameplay/
│   ├── ObstacleGenerator.cs    # Obstáculos procedurales
│   ├── CoinCollector.cs        # Recolección de monedas
│   └── EffectsManager.cs       # Efectos visuales
└── Utilities/
    ├── ObjectPool.cs           # Pool de objetos
    └── SaveSystem.cs           # Persistencia
```

## DEFINICIONES TÉCNICAS

### PlayerController Requirements
- **Movimiento automático** en eje X (velocidad variable)
- **Restricción en Y** (bounds del juego)
- **Momentum system integration** (velocidad basada en aciertos)
- **Colisión con obstáculos** (trigger game over)

### BubbleShooter Requirements
- **Aim system** con línea de trayectoria
- **Physics-based shooting** (Rigidbody2D + forces)
- **Touch/mouse input** para apuntar y disparar
- **Bubble pooling** para optimización

### BubbleManager Requirements
- **Color matching detection** (3+ burbujas del mismo color)
- **Physics collision** entre burbujas
- **Bubble destruction** con efectos
- **Score calculation** por combinaciones

## ASSETS REQUERIDOS

### Immediate Assets Needed
1. **Bubble sprites**: 4 colores básicos (azul, rojo, verde, amarillo)
2. **Player sprite**: Cubo/esfera simple low-poly
3. **Obstacle prefabs**: Formas geométricas básicas
4. **UI elements**: Botones, panels, texto

### Asset Sources Priority
1. **Unity Asset Store** (Free Low Poly packs)
2. **Kenney.nl** (Free game assets)
3. **Poly Pizza** (Free 3D models)
4. **Unity Primitive objects** como fallback

## TESTING CHECKLIST

### Core Mechanics Testing
- [ ] El personaje se mueve automáticamente
- [ ] Se pueden disparar burbujas con toque/click
- [ ] Las burbujas de mismo color se eliminan al juntarse
- [ ] La velocidad del personaje aumenta con aciertos consecutivos
- [ ] El juego termina al chocar con obstáculo
- [ ] La puntuación se actualiza correctamente

### Mobile Testing Requirements
- [ ] **Touch controls** funcionan en dispositivo real
- [ ] **Performance** mantiene 30+ FPS
- [ ] **Screen resolution** se adapta correctamente
- [ ] **Battery usage** es razonable para sesiones de 5 minutos

## DAILY GOALS

### HOY (Día 1)
**Target**: Core mechanics funcionando
- Personaje que se mueve
- Burbujas que se disparan
- Detección básica de combinaciones
- Game over funcional

### MAÑANA (Día 2)
**Target**: Gameplay loop completo
- Obstáculos procedurales
- Sistema de puntuación completo
- UI básica funcional
- Sistema de mejoras simple

### DOMINGO (Día 3)
**Target**: Polish y preparación para release
- Efectos visuales básicos
- Audio feedback
- Build de Android optimizado y testeado
- Preparación de assets para Google Play Store

### LUNES (Día 4)
**Target**: Internal Testing Release
- Google Play Console setup completo
- Upload a Internal Testing track
- Screenshots y store listing básico
- Testing con usuarios reales

## BLOCKERS Y DECISIONES

### Potential Blockers
1. **Physics complexity** → Simplificar si es necesario
2. **Performance issues** → Usar object pooling desde el inicio
3. **Touch input problems** → Probar en dispositivo real temprano
4. **Asset quality** → Priorizar funcionalidad sobre estética

### Quick Decision Framework
- **Si algo toma >2 horas** → Simplificar approach
- **Si necesitas art custom** → Usar primitivos de Unity
- **Si performance es malo** → Reducir calidad visual
- **Si mechanic es compleja** → Implementar versión MVP

---

**Próxima actualización**: Final del día 1
**Responsable**: Claude + Desarrollador
**Review**: Cada 4 horas durante desarrollo activo