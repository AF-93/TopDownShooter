using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public SpawnerControl spawner;
    [HideInInspector] public Transform player;
    private NavMeshAgent agent;
    [SerializeField] private int health = 100;
    [SerializeField] private int damageP;
    private int maxHealth;
    private HealthBar healthBar;
    private UI ui;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        maxHealth = health;
        healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(health);
        }
        ui = FindFirstObjectByType<UI>();
    }

    void OnEnable()
    {
        health = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(health);
        }
    }

    void Update()
    {
        if (player != null && agent != null && gameObject.activeInHierarchy) //si el jugador existe, el navmesh agente tambien y el enemigo esta activo.
        {
            agent.SetDestination(player.position); //mueve al enemigo hacia la posición del jugador
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Bullet")) //detecta si colisiona con la bala
        {
            Destroy(other.gameObject); //destruye la bala
            health -= 25;
            if (healthBar != null)
                healthBar.SetHealth(health);
            if (health <= 0)
            {
                gameObject.SetActive(false);//desactiva el enemigo
                if (ui != null)
                    ui.AddScore(100); // Suma 100 puntos por enemigo eliminado
            }
            if (spawner != null)//verifica si el spawner no es nulo
            {
                spawner.EnemyDied(); //notifica al spawner que el enemigo ha muerto
            }
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            player.LPPlayer(-damageP);
        }
    }
}
