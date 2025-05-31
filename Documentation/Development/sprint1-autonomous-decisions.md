# SPRINT 1 - DECISIONES AUTÓNOMAS DE CLAUDE

## 📋 RESUMEN
Durante la implementación del Sprint 1 (Timer System), tomé las siguientes decisiones de diseño de manera autónoma. Estas decisiones requieren tu revisión y aprobación.

---

## 🎯 DECISIONES DE DESIGN GAMEPLAY

### 1. **Duración Base del Timer: 3 Minutos**
**Decisión**: Establecí 180 segundos (3 minutos) como duración base
**Razón**: 
- Coincide con el objetivo del sprint (3-5 minutos)
- Permite runs rápidas pero no apresuradas
- 3 minutos dan tiempo para 1-2 upgrades mid-run
**Variables**: `baseDurationSeconds = 180f` en TimerManager
**¿Cambiar?**: ¿Prefieres 4 o 5 minutos base?

### 2. **Warnings del Timer**
**Decisión**: Warning a 60s, Urgent a 30s
**Razón**: 
- Warning al minuto final permite estrategia
- Urgent a 30s genera tensión real sin pánico
- Tiempo suficiente para reaccionar
**Variables**: `warningTime = 60f`, `urgentWarningTime = 30f`
**¿Cambiar?**: ¿Diferentes umbrales de tensión?

### 3. **Head Start Upgrade: +30s por nivel**
**Decisión**: Cada nivel de "Head Start" añade 30 segundos al timer
**Razón**:
- Nivel 1: +30s (3.5 min total) - mejora notable
- Nivel 2: +60s (4 min total) - valor significativo  
- Nivel 3: +90s (4.5 min total) - inversión premium
**Variables**: `upgrade.GetCurrentValue() * 30f`
**¿Cambiar?**: ¿Diferente progresión temporal?

---

## 🎨 DECISIONES DE UI/UX

### 4. **Estados Visuales del Timer**
**Decisión**: 3 estados con colores y animaciones
- **Normal**: Blanco, sin animación
- **Warning**: Amarillo + pulso suave
- **Urgent**: Rojo + pulso rápido + shake
**Razón**: Feedback claro sin ser molesto
**¿Cambiar?**: ¿Diferentes efectos visuales?

### 5. **GameOver Screen Específico**
**Decisión**: Pantalla separada para Game Over por timer vs colisión
**Razón**: 
- "TIME'S UP!" vs "GAME OVER!" comunica causa
- Stats diferentes (tiempo de run vs distancia)
- Permite diferentes incentivos/recompensas
**¿Cambiar?**: ¿Unificar pantallas de Game Over?

### 6. **Coins por Run = 10% del Score**
**Decisión**: Reward coins = finalScore * 0.1, mínimo 5 coins
**Razón**:
- Escala con performance del player
- Mínimo garantiza progresión siempre
- 10% se siente balanceado vs upgrades
**Variables**: `coinsEarnedThisRun = Mathf.FloorToInt(finalScore * 0.1f)`
**¿Cambiar?**: ¿Diferente fórmula de recompensa?

---

## ⚙️ DECISIONES TÉCNICAS

### 7. **Timer Independiente de TimeScale**
**Decisión**: Timer continúa corriendo en menús de pausa
**Razón**: 
- Mantiene presión roguelite constante
- Evita cheese pausando infinitamente
- Menús upgrade/idle deben ser decisiones rápidas
**Excepción**: Pause button sí detiene timer
**¿Cambiar?**: ¿Timer completamente independiente?

### 8. **Eventos del Timer System**
**Decisión**: Sistema de eventos para desacoplar componentes
```csharp
OnTimerChanged, OnTimerWarning, OnTimerUrgent, OnTimerExpired
```
**Razón**: Permite múltiples UI/sistemas reaccionar independientemente
**¿Cambiar?**: ¿Diferente arquitectura de eventos?

### 9. **Integration con GameManager**
**Decisión**: Nuevo método `TriggerGameOverTimer()` separado de `GameOver()`
**Razón**: 
- Distingue causa de game over
- Permite logic específica para cada tipo
- Facilita analytics futuras
**¿Cambiar?**: ¿Unificar métodos de Game Over?

---

## 🔧 DECISIONES DE IMPLEMENTACIÓN

### 10. **TimerUI con DOTween**
**Decisión**: Animaciones con DOTween + SetUpdate(true)
**Razón**: 
- Consistente con resto del proyecto
- SetUpdate(true) funciona durante pause
- Smooth animations mejoran juice
**¿Cambiar?**: ¿Diferentes librerías de animación?

### 11. **Placeholder Stats en GameOver**
**Decisión**: Algunos stats como "Bubbles Popped", "Distance" quedan como "--"
**Razón**: 
- No hay tracking systems aún implementados
- Preparado para futuras métricas
- No bloquea MVP del timer
**TODO**: Implementar RunStatsManager en siguiente sprint
**¿Cambiar?**: ¿Implementar tracking ahora?

### 12. **Timer Format: MM:SS**
**Decisión**: Formato minutos:segundos, con opción de milisegundos
**Razón**: 
- MM:SS es familiar y legible
- Milisegundos disponibles para precision si necesario
- Fácil de parsear visualmente durante gameplay
**¿Cambiar?**: ¿Solo segundos? ¿Formato diferente?

---

## 🎮 DECISIONES DE GAME FEEL

### 13. **Timer No Pausa en Upgrade Menu** ❌ CHANGED
**Decisión Original**: Timer sigue corriendo durante upgrade compras
**NUEVA DECISIÓN**: Upgrades solo pre-run, no durante gameplay
**Razón del Cambio**: 
- Usuario feedback: Más lógico tener upgrades pre-run
- Coins no están disponibles hasta final de run
- Gameplay más puro sin interrupciones
- Decisiones de upgrade sin presión temporal
**Status**: ✅ IMPLEMENTADO - Pre-run upgrade system

### 14. **Sin Time Extensions como Power-ups**
**Decisión**: Solo upgrades permanentes extienden tiempo, no power-ups temporales
**Razón**: 
- Mantiene runs predecibles en duración
- Focus en skill y strategy vs luck
- Evita complicar balance de power-ups
**¿Cambiar?**: ¿Añadir time extensions como rewards?

---

## ✅ ACCIONES REQUERIDAS

**REVISAR**:
1. ¿Duración base de 3 minutos correcta?
2. ¿Head Start +30s por nivel balanceado?
3. ¿Reward coins = 10% score apropiado?
4. ✅ Timer upgrade menu: RESUELTO - Pre-run system implementado

**IMPLEMENTAR DESPUÉS** (no en este sprint):
1. RunStatsManager para métricas detalladas
2. Sound effects para timer warnings
3. Screen effects para urgency states
4. Analytics de timer performance

---

## 🔄 CHANGELOG DE DECISIONES

**31 DIC 2024**:
- Todas las decisiones iniciales documentadas
- Listo para review y ajustes

**31 DIC 2024 - UPDATE**:
- ✅ Pre-run upgrade system implementado
- ✅ MainMenuUI creado para navigation
- ✅ GameOverTimerUI simplificado (solo Play Again + Main Menu)
- ✅ UpgradeUI ahora solo funciona desde main menu
- ✅ Timer system limpio (no pausas para upgrades)

**PRÓXIMAS REVISIONES**:
- Post-testing feedback del nuevo flow
- Balance adjustments basados en gameplay
- Performance optimizations si necesario

---

**STATUS**: ✅ Implementación completa + Pre-run system upgrade  
**IMPACT**: 🟢 Improved - Mejor UX y game flow  
**TESTING NEEDED**: ⚠️ Complete flow testing requerido (Main Menu → Upgrades → Game → Game Over)