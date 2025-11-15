# TopDownShooter - Patrones de Diseño Implementados

## Descripción General

Este proyecto implementa **5 patrones de diseño** clave en un videojuego tipo Top-Down Shooter en Unity. El objetivo es demostrar cómo estos patrones mejoran la arquitectura, mantenibilidad y escalabilidad del código.

---

## Patrones Implementados

### 1. **Patrón Singleton**

#### Qué es
El patrón Singleton asegura que una clase tenga una única instancia en toda la aplicación, accesible globalmente.

#### Implementación
- **GameManager.cs**: Gestiona el estado global del juego (puntuación, temporizador, referencias a GameStateManager).
- **GameEvents.cs**: Sistema centralizado de eventos para desacoplamiento entre componentes.

#### Código Ejemplo
```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
```

#### Problema que Resuelve
- **Acceso global**: Permite que cualquier objeto acceda al GameManager sin necesidad de referencias manuales.
- **Consistencia**: Garantiza que existe un único punto de control para la lógica global del juego.
- **Simplificación**: Evita pasar referencias entre múltiples niveles de objetos.

---

### 2. **Patrón Object Pool**

#### Qué es
Reutiliza objetos ya creados en lugar de destruirlos y crear nuevos, mejorando el rendimiento.

#### Implementación
- **SpawnerControl.cs**: Mantiene un pool de enemigos (`enemyPool`). Cuando un enemigo muere, se desactiva pero permanece en memoria para reutilización.

#### Código Ejemplo
```csharp
void SpawnEnemy()
{
    GameObject enemy = null;

    foreach (var e in enemyPool)
    {
        if (!e.activeInHierarchy)
        {
            enemy = e;
            break;
        }
    }

    if (enemy == null)
    {
        enemy = Instantiate(enemyPrefab, spawnPoints[index].position, Quaternion.identity);
        enemyPool.Add(enemy);
    }
    else
    {
        enemy.transform.position = spawnPoints[index].position;
        enemy.SetActive(true);
    }
}
```

#### Problema que Resuelve
- **Rendimiento**: Reduce el costo de crear/destruir objetos dinámicamente.
- **GC Pressure**: Minimiza las recolecciones de basura (Garbage Collection).
- **Fluidez**: Permite spawnear muchos enemigos sin picos de lag.

---

### 3. **Patrón Observer**

#### Qué es
Permite que múltiples objetos se suscriban a eventos sin acoplamiento directo.

#### Implementación
- **GameEvents.cs**: Bus de eventos centralizado.
- Los suscriptores se conectan a eventos (OnPlayerDied, OnEnemyDied, OnVictory, OnGameOver, OnGamePaused, OnGameResumed, OnScoreAdded, etc.).

#### Código Ejemplo
```csharp
// En GameEvents.cs
public class GameEvents : MonoBehaviour
{
    public event Action OnVictory;
    public event Action OnGameOver;
    public event Action OnPlayerDied;

    public void RaiseVictory()
    {
        OnVictory?.Invoke();
    }
}

// En UI.cs - Suscriptor
void Start()
{
    GameEvents.Instance.OnVictory += OnVictory;
    GameEvents.Instance.OnPlayerDied += OnPlayerDied;
}

private void OnVictory()
{
    StopTimer();
}
```

#### Ubicaciones de Uso
| Archivo | Suscripciones |
|---------|---------------|
| **UI.cs** | `OnScoreAdded`, `OnGameOver`, `OnVictory`, `OnGamePaused`, `OnGameResumed` |
| **GameManager.cs** | `OnGameOver`, `OnVictory`, `OnPlayerDied` |
| **Enemy.cs** | Emite `OnEnemyDied` al morir |
| **PlayerController.cs** | Emite `OnPlayerDied` al morir, emite `OnPlayerHealthChanged` |
| **GameState.cs** | Estados emiten eventos en `Enter()` (ej: `PlayingState` emite `OnGameResumed`) |

#### Problema que Resuelve
- **Desacoplamiento**: Los objetos no necesitan referencias directas los unos a los otros.
- **Escalabilidad**: Agregar nuevos listeners es trivial.
- **Mantenibilidad**: Cambios en una lógica no afectan a sistemas no relacionados.
- **Reutilización**: El mismo evento puede usarse en múltiples contextos.

---

### 4. **Patrón Command**

#### Qué es
Encapsula una solicitud como un objeto, permitiendo parametrizar clientes con diferentes solicitudes.

#### Implementación
- **Command.cs**: Define la interfaz `ICommand` con método `Execute()`.
- **PlayerController.cs**: Crea instancias de comandos (`FireCommand`, `BurstFireCommand`, `MoveCommand`, `LookCommand`) y los ejecuta en respuesta a entrada del usuario.

#### Código Ejemplo
```csharp
// En Command.cs
public interface ICommand
{
    void Execute();
}

public class FireCommand : ICommand
{
    private PlayerController player;

    public FireCommand(PlayerController p)
    {
        player = p;
    }

    public void Execute()
    {
        player.ExecuteFire();
    }
}

// En PlayerController.cs
private ICommand fireCommand;

void Awake()
{
    fireCommand = new FireCommand(this);
}

public void OnFire(InputAction.CallbackContext context)
{
    if (context.performed && canFire && GameManager.Instance.IsGameActive())
    {
        fireCommand.Execute();
    }
}
```

#### Problema que Resuelve
- **Separación de Responsabilidades**: Entrada del usuario está separada de la lógica de acción.
- **Facilita Testing**: Cada comando puede probarse independientemente.
- **Extensibilidad**: Agregar nuevos comandos no requiere modificar `PlayerController`.
- **Undo/Redo**: Permite implementar un sistema de deshacer si es necesario.
- **Retraso de Ejecución**: Los comandos pueden almacenarse y ejecutarse más tarde.

---

### 5. **Patrón State**

#### Qué es
Permite que un objeto cambie su comportamiento cuando su estado interno cambia.

#### Implementación
- **GameState.cs**: Clase abstracta que define los métodos `Enter()`, `Exit()`, `Update()`, `OnStateTransition()`.
- **Estados Concretos**: `PlayingState`, `PausedState`, `GameOverState`, `VictoryState`.
- **GameStateManager.cs**: Gestiona transiciones entre estados y actualiza el estado actual cada frame.

#### Código Ejemplo
```csharp
// En GameState.cs
public abstract class GameState
{
    protected GameStateManager manager;

    public virtual void Enter() { Debug.Log($"Entrando al estado: {GetType().Name}"); }
    public virtual void Exit() { Debug.Log($"Saliendo del estado: {GetType().Name}"); }
    public virtual void Update() { }
    public abstract void OnStateTransition();
}

// Estados concretos
public class PlayingState : GameState
{
    public override void Enter()
    {
        base.Enter();
        Time.timeScale = 1f;
        GameEvents.Instance?.RaiseGameResumed();
    }
}

public class GameOverState : GameState
{
    public override void Enter()
    {
        base.Enter();
        Time.timeScale = 0f;
    }
}

// En GameStateManager.cs
public void TransitionToState(GameState newState)
{
    currentState?.Exit();
    currentState = newState;
    currentState?.Enter();
}
```

#### Flujo de Estados
```
PlayingState
    ↓ (OnPlayerDied)
GameOverState (pausa el juego)

PlayingState
    ↓ (targetScore alcanzado)
VictoryState (pausa el juego)
```

#### Problema que Resuelve
- **Eliminación de Condicionales**: En lugar de `if (isPaused) { ... } else if (isGameOver) { ... }`, cada estado encapsula su lógica.
- **Comportamiento Claro**: El comportamiento del juego es obvio: hay 4 estados distintos, cada uno con responsabilidad clara.
- **Control de Tiempo**: Centraliza la gestión de `Time.timeScale`.
- **Transiciones Seguras**: `Enter()` y `Exit()` aseguran que la lógica se ejecuta al cambiar de estado.
- **Facilita Debug**: Es fácil saber en qué estado está el juego en cualquier momento.

---

## Integración: Flujo de Ejemplo (Muerte del Jugador)

```
1. PlayerController.LPPlayer(-valor) → lifePoints <= 0
2. GameEvents.RaisePlayerDied()
3. GameManager.OnPlayerDiedTriggered() escucha el evento
4. GameManager.GameOver() ejecuta:
   a. stateManager.TransitionToState(GameOverState)
   b. GameOverState.Enter() → Time.timeScale = 0f (pausa)
   c. GameManager.DisplayGameOver() → muestra canvas, oculta enemigos
5. UI.OnGameOver() escucha el evento y detiene el temporizador
6. La pantalla de Game Over aparece y el juego está pausado
```

---

## Flujo de Ejemplo (Victoria)

```
1. Enemy muere → ui.AddScore(100)
2. UI.AddScore() emite GameEvents.RaiseScoreAdded(100)
3. Si score >= targetScore, UI llama GameManager.Instance.Victory()
4. GameManager.Victory() ejecuta:
   a. stateManager.TransitionToState(VictoryState)
   b. VictoryState.Enter() → Time.timeScale = 0f (pausa)
   c. GameManager.DisplayVictory() → 
      - Pausa el juego
      - Emite GamePaused()
      - Oculta todos los Canvas excepto victoryCanvas
      - Desactiva PlayerController, SpawnerControl, Enemies
      - Muestra puntuación final y tiempo
5. El juego queda en estado de victoria, solo visible la UI de victoria
```

---

## Beneficios Integrados

| Beneficio | Patrones Involucrados |
|-----------|----------------------|
| **Mantenimiento Simple** | State, Observer, Command |
| **Rendimiento Optimizado** | Object Pool, Singleton |
| **Código Desacoplado** | Observer, Command |
| **Fácil de Extender** | Observer, Command, State |
| **Testeable** | Command, Observer, State |
| **Reducción de Bugs** | State (evita estados inconsistentes) |

---

## Archivos Clave

| Archivo | Patrones | Responsabilidad |
|---------|----------|-----------------|
| `GameManager.cs` | Singleton, State, Observer | Gestión global, control de states, transiciones UI |
| `GameEvents.cs` | Singleton, Observer | Bus centralizado de eventos |
| `GameState.cs` | State | Definición de estados abstracto y concretos |
| `GameStateManager.cs` | State | Gestor de transiciones de estados |
| `PlayerController.cs` | Command, Observer | Input → Comandos, emite eventos de salud |
| `UI.cs` | Observer | Suscriptor de eventos (score, pausa, victoria) |
| `Enemy.cs` | Observer, Object Pool | Enemigos reutilizables que emiten eventos |
| `SpawnerControl.cs` | Object Pool, Singleton (referencia) | Generador de enemigos con pool |
| `Command.cs` | Command | Interfaz y comandos concretos |

---

## Cómo Extender

### Agregar un Nuevo Comando
```csharp
public class DashCommand : ICommand
{
    private PlayerController player;

    public DashCommand(PlayerController p)
    {
        player = p;
    }

    public void Execute()
    {
        player.ExecuteDash();
    }
}

// En PlayerController
private ICommand dashCommand;

void Awake()
{
    dashCommand = new DashCommand(this);
}

public void OnDash(InputAction.CallbackContext context)
{
    if (context.performed && GameManager.Instance.IsGameActive())
    {
        dashCommand.Execute();
    }
}
```

### Agregar un Nuevo Listener de Eventos
```csharp
void Start()
{
    GameEvents.Instance.OnEnemyDied += OnEnemyDefeated;
}

private void OnEnemyDefeated()
{
    // Tu lógica aquí
}

void OnDestroy()
{
    GameEvents.Instance.OnEnemyDied -= OnEnemyDefeated;
}
```

### Agregar un Nuevo Estado
```csharp
public class PauseMenuState : GameState
{
    public PauseMenuState(GameStateManager stateManager) : base(stateManager) { }

    public override void Enter()
    {
        base.Enter();
        Time.timeScale = 0f;
        // Mostrar menú de pausa
    }

    public override void Exit()
    {
        base.Exit();
        Time.timeScale = 1f;
    }

    public override void OnStateTransition()
    {
    }
}

// En GameStateManager.cs agregar propiedad
public PauseMenuState PauseMenuState
{
    get { ... return pauseMenuState; }
}
```

---

## Conclusión

Estos 5 patrones trabajan en conjunto para:
- ✅ Reducir acoplamiento entre componentes
- ✅ Hacer el código más legible y mantenible
- ✅ Mejorar el rendimiento (Object Pool)
- ✅ Facilitar testing y debugging
- ✅ Permitir extensión sin modificar código existente

El resultado es un proyecto escalable, profesional y fácil de trabajar para desarrolladores nuevos.
