using UnityEngine;

public class SpawnerControl : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [Range(0.1f, 10f)]
    [SerializeField] private float spawnInterval = 2f; // Intervalo de tiempo entre cada generación de enemigos
    [SerializeField] private int maxEnemies; // Máximo de enemigos que se pueden generar al mismo tiempo
    [SerializeField] private int absoluteMaxEnemies; // Límite absoluto de enemigos que se pueden generar
    [Range(1, 60)]
    [SerializeField] private float increaseInterval; // Cada cuántos segundos aumenta el máximo de enemigos
    [SerializeField] private int increaseAmount = 1; // Cuánto aumenta cada vez el máximo de enemigos
    private float timer = 0f; // Temporizador para controlar el intervalo de generación
    private float increaseTimer = 0f; // Temporizador para controlar el aumento del máximo de enemigos
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
        increaseTimer += Time.deltaTime;

        if (timer >= spawnInterval && currentEnemies < maxEnemies)
        {
            SpawnEnemy();
            timer = 0f;
        }

        // Aumenta el máximo cada cierto tiempo, sin superar el límite absoluto
        if (increaseTimer >= increaseInterval)
        {
            if (maxEnemies < absoluteMaxEnemies) // Compara si el máximo actual es menor que el absoluto
            {
                maxEnemies = Mathf.Min(maxEnemies + increaseAmount, absoluteMaxEnemies); // Aumenta el máximo, pero no supera el absoluto
            }
            increaseTimer = 0f;// Reinicia el temporizador de aumento
        }
    }
    void SpawnEnemy()
    {
        GameObject enemy = null;

        // Busca un enemigo inactivo en el pool
        foreach (var e in enemyPool)
        {
            if (!e.activeInHierarchy)// Verifica si el enemigo está inactivo
            {
                enemy = e; // Asigna el enemigo inactivo encontrado
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
        else // Si hay un enemigo inactivo, lo reutiliza
        {
            int index = Random.Range(0, spawnPoints.Length);
            enemy.transform.position = spawnPoints[index].position;
            enemy.transform.rotation = Quaternion.identity;
            enemy.SetActive(true);
        }

        currentEnemies++;
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null) // Asegúrate de que el enemigo tenga un script Enemy
        {
            enemyScript.spawner = this;
            
            enemyScript.player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
    public void EnemyDied() // Método para llamar cuando un enemigo muere
    {
        currentEnemies--;
    }
}
