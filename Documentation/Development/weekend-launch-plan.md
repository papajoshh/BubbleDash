# 🚀 PLAN DE LANZAMIENTO EN 72 HORAS - BUBBLE DASH

**Objetivo**: Lanzar en Google Play un juego que genere ingresos desde el día 1
**Fecha actual**: Sábado (Día 2/3)
**Deadline**: Domingo noche - Subir a Google Play
**Estado**: 65% completado

## 📊 RESUMEN EJECUTIVO

Tenemos un juego funcional con mecánicas core implementadas. Necesitamos:
1. Pulir la experiencia de juego
2. Integrar monetización REAL
3. Optimizar para móvil
4. Preparar assets de tienda
5. Subir a Google Play

---

## ✅ CHECKLIST COMPLETA - QUÉ TENEMOS vs QUÉ FALTA

### 🟢 COMPLETADO (Lo que YA funciona)

#### Mecánicas Core
- [x] Movimiento automático del jugador
- [x] Sistema de disparo de burbujas con física
- [x] Detección de combinaciones 3+
- [x] Sistema de momentum (velocidad variable)
- [x] Generación procedural de obstáculos
- [x] Sistema de monedas con persistencia
- [x] Game states (Play, Pause, Game Over)
- [x] Sistema de puntuación

#### UI/UX
- [x] HUD funcional (score, coins, distance)
- [x] Pantalla de Game Over
- [x] Pantalla de Pausa
- [x] Integración con TextMeshPro

#### Efectos y Polish
- [x] Efectos visuales básicos (pop, combos)
- [x] Sistema de sonido preparado
- [x] Screen shake para combos grandes
- [x] Animaciones de monedas

#### Monetización
- [x] AdManager con simulación
- [x] IDs de AdMob configurados
- [x] Flujo de Double Coins
- [x] Sistema de guardado de monedas

### 🔴 CRÍTICO - FALTA PARA MVP (Hacer HOY)

#### 1. Monetización REAL (2-3 horas)
- [ ] Instalar Google Mobile Ads SDK
- [ ] Implementar anuncios reales en AdManager
- [ ] Testear rewarded ads
- [ ] Testear interstitial ads
- [ ] Verificar que los anuncios cargan

#### 2. Balance y Jugabilidad (1-2 horas)
- [ ] Ajustar curva de dificultad
- [ ] Balancear spawn de obstáculos
- [ ] Ajustar física de burbujas
- [ ] Verificar que es divertido y adictivo
- [ ] Asegurar que los primeros 30 segundos enganchan

#### 3. Tutorial Mínimo (1 hora)
- [ ] Pantalla de "Tap to Shoot"
- [ ] Indicador visual primera vez
- [ ] Mostrar objetivo del juego

#### 4. Sonidos Esenciales (1 hora)
- [ ] Conseguir 5 sonidos gratis:
  - Disparo de burbuja
  - Pop de burbuja
  - Recolección moneda
  - Game Over
  - Música de fondo simple

### 🟡 IMPORTANTE - Para Polish (DOMINGO MAÑANA)

#### 5. Optimización Móvil (2 horas)
- [ ] Reducir draw calls
- [ ] Implementar object pooling completo
- [ ] Verificar 60 FPS estables
- [ ] Reducir tamaño de texturas
- [ ] Comprimir audio

#### 6. Mejoras de Retención (2 horas)
- [ ] Sistema de daily rewards básico
- [ ] "Best Score" más prominente
- [ ] Botón "Rate Us" después de X partidas
- [ ] Share score básico

#### 7. Shop Básico (1 hora)
- [ ] Botón "Remove Ads" ($2.99)
- [ ] Compra de monedas (opcional)

### 🟢 LANZAMIENTO - Para Google Play (DOMINGO TARDE)

#### 8. Build Final (1 hora)
- [ ] Configurar keystore
- [ ] Build Release APK
- [ ] Probar en 2-3 dispositivos
- [ ] Verificar que no hay crashes

#### 9. Assets de Tienda (2 horas)
- [ ] Icon 512x512 final
- [ ] Feature Graphic 1024x500
- [ ] 4-8 Screenshots
- [ ] Descripción corta (80 chars)
- [ ] Descripción larga
- [ ] Keywords research

#### 10. Upload a Google Play (1 hora)
- [ ] Subir APK a Internal Testing
- [ ] Completar formularios
- [ ] Configurar precios IAP
- [ ] Submit para revisión

---

## 📅 CRONOGRAMA DETALLADO

### SÁBADO (HOY) - Día 2
**Meta**: Juego 100% jugable con monetización real

#### Mañana (YA HECHO ✅)
- Implementación de sistemas core
- Obstáculos y monedas funcionando

#### Tarde (HACER AHORA):
```
14:00-16:00: Integrar Google Mobile Ads SDK
16:00-17:00: Balance de gameplay
17:00-18:00: Tutorial mínimo
18:00-19:00: Conseguir y agregar sonidos
19:00-20:00: Testing en móvil real
```

### DOMINGO - Día 3
**Meta**: Polish y subir a tienda

#### Mañana:
```
09:00-11:00: Optimización de performance
11:00-12:00: Daily rewards simple
12:00-13:00: Shop básico (Remove Ads)
```

#### Tarde:
```
14:00-15:00: Build final + testing
15:00-17:00: Crear assets de tienda
17:00-18:00: Subir a Google Play
18:00-19:00: Buffer para problemas
```

---

## 🎯 PRIORIDADES ABSOLUTAS

### 1. MONETIZACIÓN (Sin esto no hay ingresos)
```
CRÍTICO: Los anuncios DEBEN funcionar
- Rewarded video después de cada muerte
- Interstitial cada 3 partidas
- El usuario DEBE ver valor en ver anuncios
```

### 2. RETENCIÓN (Que vuelvan a jugar)
```
VITAL: Primeros 30 segundos perfectos
- Tutorial intuitivo
- Acción inmediata
- Recompensa rápida
- "One more game" feeling
```

### 3. VIRALITY (Crecimiento orgánico)
```
IMPORTANTE: Facilitar compartir
- Share score con link a tienda
- "Beat my score!"
- Screenshots atractivos
```

---

## 💰 PROYECCIÓN DE INGRESOS

### Semana 1 (Lanzamiento Soft)
- 100-500 descargas orgánicas
- $5-20/día en ads
- 0-2 compras Remove Ads

### Mes 1 (Con ASO básico)
- 1,000-5,000 descargas
- $30-100/día
- 10-50 compras Remove Ads

### Objetivo 3 meses
- 10,000+ descargas
- $100-300/día
- Evaluar actualizaciones

---

## 🚨 BLOQUEADORES POTENCIALES Y SOLUCIONES

### "Los anuncios no cargan"
- SOLUCIÓN: Tener IDs de prueba como backup
- SOLUCIÓN: Verificar configuración AdMob

### "El juego va lento en mi teléfono"
- SOLUCIÓN: Reducir partículas
- SOLUCIÓN: Bajar resolución de sprites
- SOLUCIÓN: Menos obstáculos simultáneos

### "No es divertido"
- SOLUCIÓN: Más feedback visual/sonoro
- SOLUCIÓN: Rewards más frecuentes
- SOLUCIÓN: Dificultad más gradual

### "Google Play rechaza la app"
- SOLUCIÓN: Revisar políticas
- SOLUCIÓN: Quitar permisos innecesarios
- SOLUCIÓN: Mejorar descripción

---

## 📝 TAREAS INMEDIATAS (PRÓXIMAS 2 HORAS)

### Para Claude:
1. Crear guía de integración de Google Mobile Ads paso a paso
2. Implementar sistema de tutorial simple
3. Agregar daily reward básico

### Para Humano:
1. Descargar Google Mobile Ads Unity SDK
2. Buscar 5 sonidos gratis en freesound.org
3. Probar el juego en móvil real y reportar FPS

---

## ✅ CRITERIOS DE ÉXITO

El juego estará listo cuando:
1. [ ] Los anuncios cargan y dan rewards
2. [ ] Funciona a 30+ FPS en dispositivo medio
3. [ ] Es adictivo (quieres jugar "una más")
4. [ ] No hay crashes en 10 partidas seguidas
5. [ ] Está subido a Google Play Internal Testing

---

**RECORDATORIO**: El objetivo NO es hacer el juego perfecto, es lanzar algo que genere $10/día y mejorarlo con los datos reales de usuarios.

**DEADLINE FINAL**: Domingo 20:00 - El APK debe estar en Google Play Console.