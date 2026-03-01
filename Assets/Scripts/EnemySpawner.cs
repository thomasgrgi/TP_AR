using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public Transform[] waypoints;
    public float spawnInterval = 2f;

    private int numberOfEnemies = 0;

    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnInterval);
    }

    void SpawnEnemy()
    {   
        if (numberOfEnemies < 5)
        {
            var enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            var enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                if (waypoints != null && waypoints.Length > 0)
                {
                    enemyController.SetWaypoints(waypoints);
                }
                else
                {
                    Debug.LogError("[EnemySpawner] Waypoints array is null or empty!");
                }
            }
            else
            {
                Debug.LogError("[EnemySpawner] Failed to get EnemyController component from spawned enemy!");
            }
            numberOfEnemies += 1;
        }
    }
}
