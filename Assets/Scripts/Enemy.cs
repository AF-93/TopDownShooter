using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public SpawnerControl spawner;
    [HideInInspector] public Transform player;
    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
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
            gameObject.SetActive(false);
            if (spawner != null)
            {
                spawner.EnemyDied();
            }
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Eh Gato dame todo!");
        }
    }
}
