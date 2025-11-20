using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int currentWave;
    public Wave[] waves;
    public Transform[] spawnPoints;

    [Header("Настройки волн")]
    public float delayBetweenWaves = 5f;
    public float delayBetweenEnemies = 0.1f;

    [System.Serializable]
    public class EnemyGroup
    {
        public GameObject prefab;
        public int count;
    }

    [System.Serializable]
    public class Wave
    {
        public EnemyGroup[] groups;
    }

    void Start()
    {

        currentWave = 0;
        if (waves.Length > 0)
        {
            StartCoroutine(SpawnWave(waves[currentWave]));
        }

    }

    public void OnEnemyKilled()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            currentWave++;
            if (currentWave < waves.Length)
            {
                StartCoroutine(StartNextWave());
            }
            else
            {
                Debug.Log("Все волны пройдены!");
            }
        }
    }

    private IEnumerator StartNextWave()
    {
        Debug.Log($"Все враги уничтожены. Следующая волна через {delayBetweenWaves} секунд...");
        yield return new WaitForSeconds(delayBetweenWaves);
        StartCoroutine(SpawnWave(waves[currentWave]));
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        foreach (EnemyGroup group in wave.groups)
        {
            for (int i = 0; i < group.count; i++)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Instantiate(group.prefab, spawnPoint.position, Quaternion.identity);
                yield return new WaitForSeconds(delayBetweenEnemies);
            }
        }
    }
}
