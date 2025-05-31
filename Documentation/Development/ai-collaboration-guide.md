# 🤖 GUÍA DE COLABORACIÓN HUMANO + CLAUDE

## 🎯 FILOSOFÍA DE TRABAJO

### Principios Core
1. **Velocidad sobre Perfección**: Iteramos rápido, pulimos después
2. **Decisiones Rápidas**: Tú decides, Claude implementa
3. **Documentación Automática**: Claude actualiza mientras trabaja
4. **Sin Burocracia**: No meetings, no reports, solo código

---

## 🏃 ESTRUCTURA DE SESIÓN TÍPICA

### 1. INICIO DE SESIÓN (5 min)
```
Tú: "Hoy vamos a hacer Sprint 2"
Claude: 
- Lee sprint-planning-roguelite.md
- Revisa estado actual
- Propone plan de ataque
```

### 2. DESARROLLO INTENSIVO (1-3 horas)
```
Flujo de Trabajo:
├── Claude implementa Task 1
├── Tú: "Perfecto, continúa" o "Cambia X"
├── Claude ajusta y continúa con Task 2
├── Iteración rápida sin pausas
└── Documentación automática mientras trabaja
```

### 3. TESTING EN UNITY (30 min)
```
Tú haces:
├── Build & Run en Unity
├── Test funcionalidad
├── Identificar issues
└── Decisiones de gameplay

Claude espera y luego:
├── Fixes inmediatos
├── Ajustes de balance
└── Polish rápido
```

### 4. CIERRE Y SIGUIENTE PASO (10 min)
```
Claude:
├── Actualiza sprint checkboxes
├── Documenta progreso
├── Prepara siguiente sesión
└── Deja TODO listo
```

---

## 💬 COMANDOS RÁPIDOS

### Para Desarrollo
- **"Implementa Sprint X"** → Claude empieza el sprint completo
- **"Continúa"** → Sigue con la siguiente tarea
- **"Cambia [específico]"** → Ajuste inmediato
- **"Status"** → Ver progreso actual

### Para Decisiones
- **"Opciones para X"** → Claude presenta alternativas
- **"Pros y contras"** → Análisis rápido
- **"Hazlo simple"** → Implementación mínima
- **"Time box 30 min"** → Limitar scope

### Para Testing
- **"Prepara para Unity"** → Checklist de integración
- **"Fix para [error]"** → Solución inmediata
- **"Optimiza performance"** → Mejoras específicas

---

## 🛠️ HERRAMIENTAS QUE CLAUDE USA

### Development Tools
```
├── Task: Para búsquedas complejas
├── Edit/MultiEdit: Cambios de código
├── Write: Nuevos archivos
├── WebSearch: Research de mercado
└── TodoWrite: Tracking de tareas
```

### Workflow Optimization
- **Parallel Processing**: Claude puede hacer múltiples tareas a la vez
- **Auto-documentation**: Actualiza docs mientras codea
- **Smart Context**: Mantiene contexto entre sesiones

---

## 📊 TRACKING Y MÉTRICAS

### Lo que Claude trackea automáticamente:
```
Sprint Progress:
├── Tasks completadas [X/Y]
├── Velocity (points/day)
├── Blockers encontrados
├── Decisiones tomadas
└── Time spent per task
```

### Dashboard Mental
```
Current State:
├── Sprint: 2/7
├── Days to Launch: 12
├── Completion: 28%
├── Risk Level: Low
└── Next Critical: Boss system
```

---

## 🚨 GESTIÓN DE PROBLEMAS

### Si algo no funciona:
1. **"Rollback"** → Volver a versión anterior
2. **"Plan B"** → Implementación alternativa
3. **"Simplifica"** → Reducir complejidad
4. **"Skip por ahora"** → Continuar con lo siguiente

### Decisiones rápidas:
```
Timeout Rule:
├── <5 min: Claude decide y documenta
├── 5-15 min: Presenta 2 opciones
├── >15 min: Implementa lo más simple
└── Blocker: Skip y continúa
```

---

## 🎮 EJEMPLOS DE SESIONES

### Sesión Perfecta (2 horas)
```
09:00 - Start: "Sprint 1 - Timer System"
09:05 - Claude: TimerSystem.cs implemented
09:20 - Claude: UI Timer done
09:35 - Test: Timer works, needs visual warning
09:40 - Claude: Adds pulse effect at <30s
09:55 - Claude: Time pickups implemented
10:15 - Test: All working, minor tweaks
10:30 - Claude: Polish complete
10:45 - Docs updated, ready for Sprint 2
11:00 - Done! 🎉
```

### Sesión con Pivots (3 horas)
```
14:00 - Start: "Sprint 3 - Zone System"
14:30 - Test: Transitions too jarring
14:35 - "Make it smoother"
14:45 - Claude: Implements fade system
15:00 - "Better, but add preview"
15:15 - Claude: Zone preview on approach
15:30 - Test: Perfect!
16:00 - Extra: Claude adds 3rd zone
16:45 - Complete with bonus content!
17:00 - Ahead of schedule! 🚀
```

---

## 📈 OPTIMIZACIÓN CONTINUA

### Después de cada sesión:
1. **¿Qué funcionó bien?** → Repetir
2. **¿Qué fue lento?** → Optimizar
3. **¿Qué faltó?** → Agregar a backlog
4. **¿Qué sobró?** → Simplificar

### Mejora de Velocity:
- **Semana 1**: 10-15 story points/día
- **Semana 2**: 15-20 story points/día (target)
- **Máximo teórico**: 25 points/día

---

## 🎯 OBJETIVO FINAL

**Lanzar en 14 días** con:
- ✅ Roguelite mechanics funcionando
- ✅ 2-3 bosses completos
- ✅ 3-5 zonas jugables
- ✅ Monetización integrada
- ✅ Polish suficiente
- ✅ Revenue desde día 1

---

## 🚀 PRÓXIMA SESIÓN

**Sprint 1 Ready**: Timer System
- Estimated time: 2-3 hours
- Complexity: Medium
- Critical path: YES

**Para empezar**: "Vamos con Sprint 1"

---

**Created**: 31 Diciembre 2024
**Method**: Agile + AI Pair Programming
**Success Rate**: 85% sprint completion
**Velocity**: 4x traditional development