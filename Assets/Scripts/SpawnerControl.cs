using UnityEngine;

public class SpawnerControl : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [Range(0.1f, 5f)]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int maxEnemies;
    private float timer = 0f;
    private Transform[] spawnPoints;
    private int currentEnemies = 0;
    private System.Collections.Generic.List<GameObject> enemyPool = new System.Collections.Generic.List<GameObject>();

    void Start()
    {
        spawnPoints = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            spawnPoints[i] = transform.GetChild(i);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval && currentEnemies < maxEnemies)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }
    void SpawnEnemy()
    {
        GameObject enemy = null;

        // Busca un enemigo inactivo en el pool
        foreach (var e in enemyPool)
        {
            if (!e.activeInHierarchy)
            {
                enemy = e;
                break;
            }
        }

        // Si no hay, instancia uno nuevo y agrégalo al pool
        if (enemy == null)
        {
            int index = Random.Range(0, spawnPoints.Length);
            enemy = Instantiate(enemyPrefab, spawnPoints[index].position, Quaternion.identity);
            enemyPool.Add(enemy);
        }
        else
        {
            int index = Random.Range(0, spawnPoints.Length);
            enemy.transform.position = spawnPoints[index].position;
            enemy.transform.rotation = Quaternion.identity;
            enemy.SetActive(true);
        }

        currentEnemies++;
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.spawner = this;
            
            enemyScript.player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
    public void EnemyDied()
    {
        currentEnemies--;
    }
}
