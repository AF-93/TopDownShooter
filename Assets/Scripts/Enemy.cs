using UnityEngine;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public SpawnerControl spawner;
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            {
                Destroy(gameObject);
            }
        }

    }
        void OnDestroy()
        {
            if (spawner != null)
                spawner.EnemyDied();
        }
}
