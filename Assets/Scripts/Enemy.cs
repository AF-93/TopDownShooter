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
        if (player != null && agent != null && gameObject.activeInHierarchy)
        {
            agent.SetDestination(player.position);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            health -= 25;
            if (healthBar != null)
                healthBar.SetHealth(health);
            if (health <= 0)
            {
                gameObject.SetActive(false);
                
                GameEvents.Instance?.RaiseEnemyDied();
                
                if (ui != null)
                    ui.AddScore(100);
            }
            if (spawner != null)
            {
                spawner.EnemyDied();
            }
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            player.LPPlayer(-damageP);
        }
    }
}
