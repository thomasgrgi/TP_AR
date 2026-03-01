using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int enemiesKilled = 0;
    public int enemiesReachedEnd = 0;
    public int maxEnemiesAllowed = 5;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        Debug.Log("GameManager Start – game has started");
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
        Debug.Log("Enemy killed! Total: " + enemiesKilled);
    }

    public void EnemyReachedEnd()
    {
        enemiesReachedEnd++;
        Debug.Log("Enemy reached end! Total: " + enemiesReachedEnd);

        if (enemiesReachedEnd >= maxEnemiesAllowed)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        Debug.Log("Game Over!");
        // Ajoute ici la logique de fin de partie (UI, arrêt du jeu, etc.)
    }
}
