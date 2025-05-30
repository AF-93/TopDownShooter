using UnityEngine;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public SpawnerControl spawner;
    [HideInInspector] public Transform player;
    public float speed = 3f;

    void Update()
    {
        if (player != null && gameObject.activeInHierarchy)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
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
            // Aquí puedes agregar lógica para manejar la colisión con el jugador
            Debug.Log("Eh Gato dame todo!");
        }
    }
}
