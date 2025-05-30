# GUÍA DE DESARROLLO PARALELO - BUBBLE DASH

## 🎯 OBJETIVO
Maximizar eficiencia desarrollando en paralelo: Claude implementa código, Humano maneja assets y configuración.

## 📋 DIVISIÓN DE TAREAS

### 🤖 CLAUDE (Track de Implementación)
**AHORA MISMO - Secuencial**:
1. ✅ PlayerController con movimiento automático
2. ⏳ BubbleShooter con física básica  
3. ⏳ BubbleManager (detección combinaciones)
4. ⏳ MomentumSystem (velocidad variable)
5. ⏳ GameManager (estados del juego)
6. ⏳ ScoreManager y UI básica

### 👨‍💻 HUMANO (Track de Assets y Setup) 
**AHORA MISMO - Paralelo**:
1. 🔄 Configurar Unity para Android
2. 🔄 Descargar y preparar assets visuales
3. 🔄 Configurar Google Play Console account
4. 🔄 Preparar iconos y screenshots base
5. 🔄 Investigar y configurar monetización SDKs

---

## 📱 GUÍA DETALLADA PARA EL HUMANO

### TAREA 1: CONFIGURAR UNITY PARA ANDROID (15 min)
**Objetivo**: Proyecto listo para build Android desde el inicio

#### Pasos:
1. **Abrir Unity Hub** → Verificar Android Build Support instalado
   - Si no: Unity Hub → Installs → Add Modules → Android Build Support

2. **File → Build Settings**:
   ```
   Platform: Android
   Target API Level: 34 (Android 14)
   Minimum API Level: 24 (Android 7.0)
   ```

3. **Edit → Project Settings → Player**:
   ```
   Company Name: TU_NOMBRE
   Product Name: Bubble Dash
   Version: 0.1.0
   Bundle Identifier: com.tunombre.bubbledash
   ```

4. **Configuration**:
   ```
   Scripting Backend: IL2CPP
   Target Architectures: ARM64 ✓ (ARMv7 solo si necesario)
   ```

5. **Verification Build**:
   ```
   File → Build Settings → Build
   Crear carpeta: /Builds/Android/
   Generar APK test (sin firmar está bien)
   ```

**✅ Success Criteria**: APK generado sin errores

---

### TAREA 2: DESCARGAR ASSETS VISUALES (30 min)
**Objetivo**: Assets minimalistas listos para integración

#### 2A: Bubble Sprites/Models
**Fuentes Recomendadas**:
1. **Unity Asset Store** (Gratis):
   - Buscar: "low poly bubbles" o "simple spheres"
   - Descargar: "Simple Geometric Shapes Pack"

2. **Kenney.nl** (Gratis):
   - URL: kenney.nl/assets
   - Descargar: "Shape Pack" o "Platformer Pack"

3. **Unity Primitives** (Fallback):
   - Crear esferas con materiales de colores

#### Assets Necesarios:
```
Bubbles: 4 colores (Rojo, Azul, Verde, Amarillo)
Player: Cubo o esfera simple
Obstacles: Rectángulos/cubos
Background: Gradiente simple o color sólido
```

#### Pasos:
1. Crear carpetas en Unity:
   ```
   Assets/Art/
   ├── Materials/
   ├── Textures/
   ├── Prefabs/
   └── Models/
   ```

2. Importar assets descargados
3. Crear materiales con colores básicos:
   ```
   Red_Bubble: #FF4444
   Blue_Bubble: #4444FF  
   Green_Bubble: #44FF44
   Yellow_Bubble: #FFFF44
   ```

**✅ Success Criteria**: 4 bubble prefabs con colores distintos

---

### TAREA 3: GOOGLE PLAY CONSOLE SETUP (20 min)
**Objetivo**: Cuenta lista para upload el lunes

#### Pasos:
1. **Crear cuenta** (si no tienes):
   - URL: play.google.com/console
   - Fee: $25 USD (one-time)
   - Usar cuenta Google personal o business

2. **Crear nueva aplicación**:
   ```
   App name: Bubble Dash
   Default language: Spanish (o English)
   App or game: Game
   Free or paid: Free
   ```

3. **Configuración básica**:
   ```
   Category: Casual
   Content rating: Everyone
   Target audience: 13+
   ```

4. **Preparar para Internal Testing**:
   - App signing por Google (recomendado)
   - Create Internal Testing track

**✅ Success Criteria**: App creada en Google Play Console, lista para upload

---

### TAREA 4: ICONOS Y SCREENSHOTS BASE (25 min)
**Objetivo**: Assets visuales básicos para store listing

#### 4A: App Icon (1024x1024)
**Herramientas**:
- Canva (gratis): canva.com
- GIMP (gratis): gimp.org
- O cualquier editor

**Especificaciones**:
```
Tamaño: 1024x1024 PNG
Estilo: Minimalista, colores vibrantes
Elementos: Burbuja + elemento de velocidad/movimiento
Colores: Azul primario + naranja acento
```

**Template Sugerido**:
- Fondo: Gradiente azul
- Centro: Burbuja grande con brillo
- Detalle: Líneas de velocidad o trail

#### 4B: Screenshots Template
**Tamaños Android**:
```
Phone: 1080x1920 (mínimo 2 screenshots)
Tablet: 1200x1600 (opcional pero recomendado)
```

**Por ahora**: Crear templates vacíos, screenshots reales el domingo

**✅ Success Criteria**: Icon 1024x1024 y templates de screenshots

---

### TAREA 5: MONETIZACIÓN SDKs RESEARCH (20 min)
**Objetivo**: Información y setup preparado para integración

#### 5A: Google AdMob Setup
1. **Crear cuenta AdMob**: 
   - URL: admob.google.com
   - Usar misma cuenta que Google Play Console

2. **Crear nueva app**:
   ```
   Platform: Android
   App name: Bubble Dash
   Store listing: Pendiente (agregar después)
   ```

3. **Crear Ad Units**:
   ```
   Rewarded Video: "Double Coins"
   Interstitial: "Game Over"
   Banner: "Menu Banner" (opcional)
   ```

4. **Obtener IDs importantes**:
   ```
   App ID: ca-app-pub-XXXXXXXX~XXXXXXXX
   Rewarded ID: ca-app-pub-XXXXXXXX/XXXXXXXX
   Interstitial ID: ca-app-pub-XXXXXXXX/XXXXXXXX
   ```

#### 5B: Unity Ads (Backup)
- Crear proyecto en Unity Dashboard
- Obtener Game ID para Android

**✅ Success Criteria**: AdMob account creado, Ad Units configurados, IDs guardados

---

## ⚡ SINCRONIZACIÓN CON CLAUDE

### Comunicación Durante Desarrollo
**Cada 30-45 minutos**: Update de progreso mutuo
- "Task X completada, empezando Y"
- "Problema en Z, necesito input"
- "Listo para siguiente fase"

### Checkpoints Críticos
1. **Checkpoint 1** (45 min): Unity Android setup + Basic assets
2. **Checkpoint 2** (90 min): Google Play + Icons listos
3. **Checkpoint 3** (120 min): Claude tiene PlayerController, tu tienes monetización setup

### Integration Points
- **Claude termina PlayerController** → Tú pruebas en Android build
- **Tú terminas assets** → Claude los integra en código
- **Claude termina core mechanics** → Testing conjunto

---

## 🎯 SUCCESS METRICS HOY

### Para las próximas 2 horas:
- ✅ Unity configurado para Android builds
- ✅ Assets básicos importados y organizados  
- ✅ Google Play Console setup completado
- ✅ Claude: PlayerController + BubbleShooter funcionando

### End of Day Goal:
- 🎮 Juego jugable (dispara burbujas, personaje se mueve)
- 📱 APK funcional en dispositivo Android real
- 🏪 Store setup listo para upload lunes

---

**¿LISTO PARA EMPEZAR? Confirma y ambos empezamos nuestros tracks simultáneamente.**