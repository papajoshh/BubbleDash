# COIN BUBBLES - FINAL IMPLEMENTATION GUIDE

## 🎯 OVERVIEW
Las **Coin Bubbles** son burbujas especiales que otorgan monedas al romperse. Funcionan **exactamente como burbujas normales** pero con recompensa adicional.

**CONCEPTO CLAVE**: Solo hay 4 colores de burbujas en el juego (Rojo, Azul, Verde, Amarillo). Las coin bubbles son versiones doradas de estos mismos colores y requieren **color matching** para romperse.

---

## 🎨 VISUAL DESIGN

### Sprites Necesarios (4 coin bubble sprites):
```
1. red_coin_bubble.png    - Burbuja roja con símbolo de moneda
2. blue_coin_bubble.png   - Burbuja azul con símbolo de moneda  
3. green_coin_bubble.png  - Burbuja verde con símbolo de moneda
4. yellow_coin_bubble.png - Burbuja amarilla con símbolo de moneda
```

### Diseño Visual Sugerido:
```
OPCIÓN A: Overlay de moneda
├── Base: Sprite de burbuja normal del color
├── Overlay: Símbolo de moneda dorado en el centro
├── Borde: Brillo dorado sutil
└── Transparencia: Misma que burbujas normales

OPCIÓN B: Versión dorada
├── Base: Sprite de burbuja con tinte dorado
├── Centro: Símbolo "$" o moneda
├── Efecto: Partículas doradas sutiles
└── Brillo: Efecto de shimmer animado
```

---

## 🔧 COMPORTAMIENTO TÉCNICO

### Color Matching:
```
✅ Burbuja Roja → Coin Bubble Roja = ROMPE (da coins)
✅ Burbuja Azul → Coin Bubble Azul = ROMPE (da coins)
❌ Burbuja Roja → Coin Bubble Azul = NO ROMPE (miss)
❌ Burbuja Verde → Coin Bubble Amarilla = NO ROMPE (miss)
```

### Spawn System:
```
- CoinSystem maneja el spawning
- 30% chance de spawn por patrón de obstáculos
- Se spawneán en patrones (línea, triángulo, etc.)
- Color aleatorio entre los 4 disponibles
```

### Reward System:
```
Base: 1 coin por bubble
Golden Touch L1: 2 coins por bubble
Golden Touch L2: 3 coins por bubble
Golden Touch L3: 4 coins por bubble
Golden Touch L4: 5 coins por bubble (max)
```

---

## 🎮 UNITY SETUP

### 1. Create Coin Bubble Prefab
```
GameObject Setup:
├── CoinBubble (GameObject)
│   ├── SpriteRenderer
│   ├── CircleCollider2D (No trigger)
│   └── CoinBubble.cs
└── CoinIcon (Child - Optional)
    └── SpriteRenderer (coin symbol)
```

### 2. Configure CoinBubble Component
```
CoinBubble.cs Settings:
├── Bubble Color: Yellow (default, randomiza en Start)
├── Randomize Color: ✓ checked
├── Base Coin Value: 1
├── Bubble Renderer: (auto-assigned)
├── Red Coin Bubble Sprite: red_coin_bubble.png
├── Blue Coin Bubble Sprite: blue_coin_bubble.png
├── Green Coin Bubble Sprite: green_coin_bubble.png
├── Yellow Coin Bubble Sprite: yellow_coin_bubble.png
├── Bob Speed: 2
└── Bob Height: 0.1
```

### 3. Update CoinSystem
```
CoinSystem GameObject:
├── Coin Prefab: (existing floating coin)
├── Coin Bubble Prefab: CoinBubblePrefab ⭐
├── Use Floating Coins: ❌ unchecked
├── Coin Spawn Chance: 0.3
└── Use Patterns: ✓ checked
```

---

## 🎯 GAMEPLAY FLOW

### Player Experience:
```
1. Ve coin bubble dorada del color X
2. Selecciona burbuja del mismo color X  
3. Apunta y dispara
4. Si coincide → Coin bubble explota → Gana coins
5. Si no coincide → Solo se destruye proyectil → Miss
```

### Strategic Decisions:
```
💭 "Tengo burbuja roja, veo coin bubble roja lejos"
💭 "¿Uso mi burbuja azul en enemigo o espero coin bubble azul?"
💭 "Cluster de 3 coin bubbles verdes, necesito verde!"
💭 "Con Golden Touch L4 vale la pena apuntar a coins"
```

---

## 🔮 FUTURE UPGRADES

### Bubble Breaker (Boss 1 Unlock):
```
// Uncomment in CoinBubble.cs when implemented
if (PlayerPrefs.GetInt("BubbleBreakerUnlocked", 0) == 1)
{
    canBreak = true; // ANY color breaks coin bubbles
}
```

### Coin Shower (Boss 2 Unlock):
```
- Coin bubble spawns 2-3 extra coin bubbles al explotar
- Chain reaction potential
```

---

## 🧪 TESTING CHECKLIST

### Basic Functionality:
- [ ] Coin bubbles spawn con colores aleatorios
- [ ] Color matching funciona correctamente
- [ ] Miss cuando colores no coinciden
- [ ] Coins se otorgan al romper
- [ ] Golden Touch multiplica coins

### Visual Testing:
- [ ] Cada color tiene sprite único de coin bubble
- [ ] Bobbing animation funciona
- [ ] Efectos de partículas al explotar
- [ ] Feedback visual claro de color

### Balance Testing:
- [ ] 30% spawn rate se siente bien
- [ ] Distribución de colores es equitativa
- [ ] Recompensa de coins es balanceada
- [ ] Dificultad de apuntar es apropiada

---

## 🎨 SPRITE CREATION GUIDE

### Para crear los 4 coin bubble sprites:

**Método 1: Photoshop/GIMP**
```
1. Abrir sprite de burbuja base (ej: blue_body_circle.png)
2. Duplicar capa
3. Aplicar overlay dorado (Hue/Saturation)
4. Añadir símbolo de moneda en el centro
5. Aplicar outer glow dorado
6. Exportar como [color]_coin_bubble.png
7. Repetir para los 4 colores
```

**Método 2: Unity Shader (Avanzado)**
```
1. Crear material "CoinBubbleMaterial"
2. Shader con:
   - Base texture: bubble sprite
   - Overlay texture: coin symbol
   - Emission: golden glow
   - Time-based shimmer
```

---

## ❗ NOTAS IMPORTANTES

1. **NO hay 5to tipo de burbuja** - Solo 4 colores existen
2. **Coin bubbles siguen reglas normales** - Color matching requerido
3. **Visual distintivo es clave** - Jugador debe identificar rápidamente
4. **Balance es crucial** - No demasiadas coins, no muy pocas

---

**STATUS**: ✅ Sistema completo y funcional  
**COMPLEJIDAD**: Media - Requiere 4 sprites custom  
**TIEMPO ESTIMADO**: 45-60 minutos con creación de sprites