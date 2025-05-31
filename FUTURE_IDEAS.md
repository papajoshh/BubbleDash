# FUTURE IDEAS - BUBBLE DASH

## 🚀 FEATURE IDEAS

### 1. CHASING OBSTACLES SYSTEM
**Concepto**: Obstáculos que aparecen desde la derecha y persiguen al jugador
**Comportamiento**:
```
1. Spawn: Aparecen fuera de pantalla a la derecha
2. Approach: Se mueven más rápido que el jugador para alcanzarlo
3. Accompany: Al llegar a la posición X del jugador, igualan su velocidad
4. Timer: Acompañan durante X segundos (ej: 5-10 segundos)
5. Exit: Aceleran hacia la izquierda y desaparecen
```

**Implementación sugerida**:
```csharp
public class ChasingObstacle : MonoBehaviour {
    public float approachSpeed = 8f;
    public float accompanyCuration = 5f;
    public float exitSpeed = 12f;
    public float heightVariation = 3f; // Random Y offset
    
    enum State { Approaching, Accompanying, Exiting }
}
```

**Tipos de perseguidores**:
- Burbujas enemigas que disparan hacia ti
- Misiles teledirigidos que debes destruir
- Nubes de obstáculos que bloquean disparos
- Power-ups temporales que debes atrapar

### 2. SISTEMA DE CARRILES (LANES)
- 3-5 carriles predefinidos en Y
- Algunos obstáculos solo aparecen en ciertos carriles
- El jugador podría cambiar de carril (opcional)

### 3. BOSS OBSTACLES
- Obstáculos grandes que requieren múltiples hits
- Aparecen cada X distancia como mini-jefes
- Patrones de ataque específicos

### 4. ENVIRONMENTAL HAZARDS
- Zonas de viento que afectan la trayectoria de burbujas
- Lluvia que hace las burbujas más pesadas
- Portales que teletransportan burbujas

## 📝 NOTAS DE IMPLEMENTACIÓN

**Prioridad**: BAJA - Primero completar el juego base
**Complejidad**: MEDIA - Requiere nuevo sistema de movimiento
**Valor añadido**: ALTO - Añade mucha variedad al gameplay

---
Fecha de documentación: 2024-12-31