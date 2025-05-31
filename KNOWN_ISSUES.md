# KNOWN ISSUES - BUBBLE DASH

## 🐛 BUGS PENDIENTES

### 1. PARALLAX NO ES SEAMLESS
**Descripción**: El sistema de parallax background muestra saltos visibles cuando reposiciona los sprites.
**Archivos afectados**: 
- `/Assets/Scripts/Core/ParallaxBackground.cs`
- `/Assets/Scripts/Core/ParallaxSeamlessDouble.cs`
**Impacto**: Visual, no afecta gameplay
**Prioridad**: Baja
**Notas**: Intentamos múltiples soluciones con sprites dobles pero causaban posiciones extremas.

### 2. STATIC BUBBLES NO MATAN AL PLAYER
**Descripción**: Las burbujas estáticas (obstáculos) no causan Game Over cuando el jugador las toca.
**Archivos afectados**:
- `/Assets/Scripts/Core/StaticBubble.cs`
- `/Assets/Scripts/Core/PlayerController.cs`
**Impacto**: Gameplay crítico - obstáculos no son peligrosos
**Prioridad**: Alta
**Notas**: 
- Detección de colisión implementada en ambos lados
- Los Debug.Log no se ejecutan, sugiere problema con layers o Rigidbody
- Verificar que Player tiene Rigidbody2D y las Static Bubbles tienen Collider2D

### 3. CLICK EN POPUP DISPARA BURBUJA
**Descripción**: Al hacer click en botones de popups pausados (upgrade, idle rewards), se dispara una burbuja cuando se cierra el popup.
**Archivos afectados**:
- `/Assets/Scripts/Core/BubbleShooter.cs`
- `/Assets/Scripts/UI/UpgradeUI.cs`
- `/Assets/Scripts/UI/IdleRewardsUI.cs`
**Impacto**: UX molesto - acciones no deseadas al cerrar menús
**Prioridad**: Media
**Causa probable**: BubbleShooter sigue detectando input mientras el juego está pausado con `timeScale = 0`
**Notas**: 
- Solo afecta clicks en botones de UI, no en el fondo
- Sucede al resumir el juego después de usar popup
- Posible solución: Verificar que GameManager.IsPlaying() antes de disparar

## 📝 NOTAS PARA FUTURAS CORRECCIONES

### Parallax:
- Considerar usar un sistema de 3 sprites en lugar de 2
- O implementar un shader que haga el wrapping automático
- Asset Store tiene soluciones pre-hechas

### Colisiones:
- Verificar Project Settings → Physics 2D → Layer Collision Matrix
- Probar con OnTriggerEnter2D usando triggers
- Considerar usar Physics2D.OverlapCircle para detección manual

### Click en Popup:
- Agregar check `if (!GameManager.Instance.IsPlaying()) return;` en BubbleShooter.HandleInput()
- O usar EventSystem.current.IsPointerOverGameObject() para detectar clicks en UI
- Considerar agregar delay de 0.1s después de resumir juego antes de permitir shooting

---
Última actualización: 2024-12-31