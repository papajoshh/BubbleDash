# 🎮 SPRINT PLANNING - BUBBLE DASH ROGUELITE

## 🤖 METODOLOGÍA DE TRABAJO CON CLAUDE

### Principios de Colaboración Human + AI
1. **Sesiones de Sprint**: Trabajamos en bloques intensivos de desarrollo
2. **Documentación Viva**: Cada sprint actualiza su propio estado
3. **Decisiones Rápidas**: Claude implementa, tú validas
4. **Iteración Continua**: Ajustes sobre la marcha sin burocracia

### Estructura de Trabajo
```
Sesión de Trabajo Típica:
├── 1. Review del sprint actual (5 min)
├── 2. Implementación con Claude (1-3 horas)
├── 3. Testing en Unity (30 min)
├── 4. Decisiones y ajustes (15 min)
└── 5. Documentar progreso (automático)
```

---

## 📅 MASTER TIMELINE - 14 DÍAS AL LANZAMIENTO

**Objetivo**: MVP Roguelite en Google Play Store
**Fecha Inicio**: 31 Diciembre 2024
**Fecha Target**: 14 Enero 2025

### Phases Overview
```
Phase 1: Core Roguelite (Days 1-4)
Phase 2: Content & Polish (Days 5-8)
Phase 3: Monetization (Days 9-11)
Phase 4: Launch Prep (Days 12-14)
```

---

## 🏃 SPRINT 1: TIMER & CORE LOOP (Day 1-2)

### Objetivo
Transformar el juego arcade en roguelite con sistema de tiempo finito.

### User Stories
```
Como jugador quiero:
├── US1.1: Ver un timer que cuenta hacia atrás
├── US1.2: Que el juego termine cuando se acabe el tiempo
├── US1.3: Extender el tiempo con upgrades
├── US1.4: Sentir urgencia pero no estrés excesivo
└── US1.5: Reiniciar rápidamente para otra run
```

### Tareas Técnicas
- [ ] **T1.1**: Crear `TimerSystem.cs`
  - Countdown logic (3 minutos base)
  - Integración con GameManager
  - Eventos de tiempo (warnings, game over)
  
- [ ] **T1.2**: UI del Timer
  - Visual countdown prominente
  - Warnings visuales (<30s, <10s)
  - Efectos de urgencia (pulse, color)
  
- [ ] **T1.3**: Sistema de Extensiones
  - Time pickups (+10s, +20s)
  - Upgrade integration
  - Purchase time extension IAP stub
  
- [ ] **T1.4**: Game Loop Adjustment
  - Quick restart flow
  - Run statistics tracking
  - Death reason: "Time Out"

### Definition of Done
- [ ] Timer visible y funcionando
- [ ] Game over por tiempo
- [ ] Al menos 1 forma de extender tiempo
- [ ] Restart en <3 segundos

### Estimación: 4-6 horas de desarrollo

---

## 🐉 SPRINT 2: FIRST BOSS (Day 3-4)

### Objetivo
Implementar Tree Guardian como primer boss y milestone de progresión.

### User Stories
```
Como jugador quiero:
├── US2.1: Encontrar un boss que bloquea mi progreso
├── US2.2: Entender claramente cómo dañarlo
├── US2.3: Sentir satisfacción al derrotarlo
├── US2.4: Recibir recompensas significativas
└── US2.5: Que el boss sea justo pero desafiante
```

### Tareas Técnicas
- [ ] **T2.1**: `BossSystem.cs` Foundation
  - Boss spawn triggers (distance-based)
  - Health system (5 hits)
  - State machine (idle, attacking, defeated)
  
- [ ] **T2.2**: Tree Guardian Implementation
  - Movement pattern (vertical oscillation)
  - Green bubble barriers spawn
  - Root attack mechanics
  - Death sequence
  
- [ ] **T2.3**: Boss UI
  - Health bar display
  - Boss name introduction
  - Victory celebration UI
  
- [ ] **T2.4**: Rewards System
  - Time extension (+45s)
  - Coins reward (50)
  - Zone unlock trigger
  - Achievement integration

### Definition of Done
- [ ] Boss aparece a 500m
- [ ] Patrón de ataque funcional
- [ ] Feedback visual de daño
- [ ] Recompensas funcionando
- [ ] Boss derrotable en 30-60 segundos

### Estimación: 6-8 horas de desarrollo

---

## 🌍 SPRINT 3: ZONE SYSTEM (Day 5-6)

### Objetivo
Implementar sistema de zonas con progresión visual y mecánica.

### User Stories
```
Como jugador quiero:
├── US3.1: Ver cambios visuales al avanzar
├── US3.2: Sentir progresión entre zonas
├── US3.3: Desbloquear nuevas zonas permanentemente
├── US3.4: Que cada zona se sienta única
└── US3.5: Objetivos claros de progresión
```

### Tareas Técnicas
- [ ] **T3.1**: `ZoneManager.cs`
  - Zone progression logic
  - Background transition system
  - Zone unlock persistence
  - Distance tracking per zone
  
- [ ] **T3.2**: Zone Content (Forest + Desert)
  - Background assets integration
  - Zone-specific obstacles
  - Color palette changes
  - Ambient differences
  
- [ ] **T3.3**: Zone Transitions
  - Smooth visual transitions
  - "Entering Desert" notifications
  - Progress indicators
  - Zone completion tracking
  
- [ ] **T3.4**: Zone Persistence
  - Save zone unlocks
  - Show locked zones (grayed out)
  - Zone selection UI (future)

### Definition of Done
- [ ] 2 zonas funcionando (Forest, Desert)
- [ ] Transiciones suaves entre zonas
- [ ] Persistencia de desbloqueos
- [ ] Visual feedback de progresión

### Estimación: 5-6 horas de desarrollo

---

## 📈 SPRINT 4: META-PROGRESSION (Day 7-8)

### Objetivo
Expandir el sistema de upgrades actual para meta-progresión roguelite.

### User Stories
```
Como jugador quiero:
├── US4.1: Mejorar mi personaje entre runs
├── US4.2: Sentir que cada run importa
├── US4.3: Tener objetivos a largo plazo
├── US4.4: Decisiones significativas de upgrade
└── US4.5: Ver mi progreso acumulado
```

### Tareas Técnicas
- [ ] **T4.1**: Upgrade Tree Expansion
  - Time-specific upgrades (+30s, +60s, +90s)
  - Boss damage upgrades
  - Zone-specific bonuses
  - Prestige system foundation
  
- [ ] **T4.2**: Currency Rebalancing
  - Coins from time survived
  - Boss kill bonuses
  - Zone completion rewards
  - Daily run bonuses
  
- [ ] **T4.3**: Progression UI Upgrade
  - Visual upgrade tree
  - Progress bars everywhere
  - Statistics dashboard
  - Run history
  
- [ ] **T4.4**: Achievement System
  - First boss kill
  - Zone unlocks
  - Speed achievements
  - Cumulative progress

### Definition of Done
- [ ] 10+ upgrades funcionando
- [ ] Economía balanceada
- [ ] UI clara de progresión
- [ ] Achievements básicos

### Estimación: 5-6 horas de desarrollo

---

## 💰 SPRINT 5: MONETIZATION INTEGRATION (Day 9-10)

### Objetivo
Integrar monetización sin interrumpir el gameplay flow.

### User Stories
```
Como jugador quiero:
├── US5.1: Opciones justas para gastar dinero
├── US5.2: Progreso posible sin pagar
├── US5.3: Beneficios claros al pagar
├── US5.4: No sentirme presionado
└── US5.5: Ofertas que valgan la pena
```

### Tareas Técnicas
- [ ] **T5.1**: IAP Integration
  - Emergency time (+60s) - $0.99
  - Zone unlock bundles - $2.99
  - Starter packs - $1.99
  - Remove ads - $4.99
  
- [ ] **T5.2**: Ad Placement
  - Rewarded: Double coins, extra time
  - Interstitial: Every 3-4 runs
  - Banner: Main menu only
  - Opt-in focus
  
- [ ] **T5.3**: Special Offers
  - First-time player discount
  - Boss defeat celebration offers
  - Time pressure purchases
  - Daily deals system
  
- [ ] **T5.4**: Analytics Events
  - Purchase funnels
  - Ad engagement
  - Conversion tracking
  - Revenue optimization

### Definition of Done
- [ ] IAP funcionando en test mode
- [ ] Ads integrados y no intrusivos  
- [ ] Ofertas contextuales
- [ ] Analytics tracking

### Estimación: 6-8 horas de desarrollo

---

## 🎨 SPRINT 6: POLISH & CONTENT (Day 11-12)

### Objetivo
Pulir la experiencia y añadir contenido para retención.

### User Stories
```
Como jugador quiero:
├── US6.1: Efectos satisfactorios
├── US6.2: Más variedad de contenido
├── US6.3: Razones para volver diariamente
├── US6.4: Sentir calidad en el juego
└── US6.5: Compartir mis logros
```

### Tareas Técnicas
- [ ] **T6.1**: Additional Bosses
  - Sand Wyrm (Desert boss)
  - Basic patterns only
  - Reuse systems from Tree Guardian
  
- [ ] **T6.2**: Polish Pass
  - Particle effects upgrade
  - Sound effect improvements
  - UI animations
  - Transition smoothing
  
- [ ] **T6.3**: Daily Systems
  - Daily rewards
  - Daily challenges
  - Login bonuses
  - Special events framework
  
- [ ] **T6.4**: Social Features
  - Share button
  - Leaderboards
  - Achievement sharing
  - Rate game prompt

### Definition of Done
- [ ] 2 bosses totalmente funcionales
- [ ] Efectos visuales mejorados
- [ ] Daily rewards activo
- [ ] Share functionality

### Estimación: 8-10 horas de desarrollo

---

## 🚀 SPRINT 7: LAUNCH PREPARATION (Day 13-14)

### Objetivo
Preparar el juego para lanzamiento en Google Play.

### User Stories
```
Como desarrollador quiero:
├── US7.1: Build estable y optimizado
├── US7.2: Store listing atractivo
├── US7.3: Monetización probada
├── US7.4: Analytics funcionando
└── US7.5: Plan de lanzamiento claro
```

### Tareas Técnicas
- [ ] **T7.1**: Performance Optimization
  - Profiling y fixes
  - Memory optimization
  - Loading time reduction
  - Device compatibility
  
- [ ] **T7.2**: Store Assets
  - Screenshots (5-8)
  - Feature graphic
  - Promo video (30s)
  - Store description
  
- [ ] **T7.3**: Final Testing
  - Full monetization flow
  - Achievement testing
  - Cross-device testing
  - Crash reporting
  
- [ ] **T7.4**: Launch Config
  - Remote config setup
  - A/B test preparation
  - Event tracking
  - Soft launch regions

### Definition of Done
- [ ] APK <100MB
- [ ] Store listing completo
- [ ] Sin crashes críticos
- [ ] Monetización validada

### Estimación: 6-8 horas

---

## 📊 VELOCITY TRACKING

### Sprint Complexity Points
```
Sprint 1 (Timer): 8 points
Sprint 2 (Boss): 13 points  
Sprint 3 (Zones): 10 points
Sprint 4 (Meta): 10 points
Sprint 5 (Money): 12 points
Sprint 6 (Polish): 15 points
Sprint 7 (Launch): 10 points
TOTAL: 78 points
```

### Estimated Velocity
- **Con Claude**: 15-20 points/día
- **Testing/Unity**: 50% del tiempo
- **Decision Making**: 20% del tiempo
- **Development**: 30% del tiempo

---

## 🎯 CONTINGENCY PLANS

### Si nos retrasamos:
1. **Cortar Sprint 6** parcialmente (solo 1 boss, menos polish)
2. **Simplificar zonas** (solo visual, no mecánicas)
3. **Monetización básica** (solo time extensions)
4. **Soft launch** en región pequeña primero

### Si vamos adelantados:
1. **Más bosses** (3-4 total)
2. **Más zonas** (Ocean, Sky)
3. **Battle Pass** básico
4. **Multiplayer** asíncrono

---

## 📝 DAILY STANDUP TEMPLATE

```markdown
## Standup - [FECHA]

### ✅ Completado ayer:
- Task X
- Task Y

### 🎯 Plan para hoy:
- Sprint N, Task A
- Sprint N, Task B

### 🚧 Blockers:
- Ninguno / Descripción

### 📊 Sprint Progress:
- Sprint N: X/Y tareas (Z%)
- Velocity: X points/day
```

---

## 🔄 ACTUALIZACIÓN CONTINUA

Este documento debe actualizarse:
1. Al completar cada tarea (✅)
2. Al encontrar blockers
3. Al tomar decisiones de scope
4. Al finalizar cada sprint

**Siguiente Review**: Al completar Sprint 1

---

**Creado**: 31 Diciembre 2024
**Metodología**: Agile + AI Collaboration
**Timeline**: 14 días al lanzamiento
**Estado**: READY TO START 🚀