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
        int index = Random.Range(0, spawnPoints.Length);
        GameObject enemy = Instantiate(enemyPrefab, spawnPoints[index].position, Quaternion.identity);
        currentEnemies++;
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.spawner = this;
        }
    }
    public void EnemyDied()
    {
        currentEnemies--;
    }
}
