using UnityEngine;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public SpawnerControl spawner;
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            {
                gameObject.SetActive(false);
            }
        }

    }
        void OnDisable()
        {
            if (spawner != null)
                spawner.EnemyDied();
        }
}
