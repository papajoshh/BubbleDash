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

## 📝 NOTAS PARA FUTURAS CORRECCIONES

### Parallax:
- Considerar usar un sistema de 3 sprites en lugar de 2
- O implementar un shader que haga el wrapping automático
- Asset Store tiene soluciones pre-hechas

### Colisiones:
- Verificar Project Settings → Physics 2D → Layer Collision Matrix
- Probar con OnTriggerEnter2D usando triggers
- Considerar usar Physics2D.OverlapCircle para detección manual

---
Última actualización: 2024-12-31