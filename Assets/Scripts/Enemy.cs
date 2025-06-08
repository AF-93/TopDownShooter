using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public SpawnerControl spawner;
    [HideInInspector] public Transform player;
    private NavMeshAgent agent;
    [SerializeField] private int health = 100;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
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
            gameObject.SetActive(false);//desactiva el enemigo
            if (spawner != null)//verifica si el spawner no es nulo
            {
                spawner.EnemyDied(); //notifica al spawner que el enemigo ha muerto
            }
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Eh Gato dame todo!");
        }
    }
}
