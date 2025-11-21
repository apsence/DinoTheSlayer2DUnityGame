using System.Collections;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Менеджер волн")]
    public int currentWave;
    public Wave[] waves;
    public Transform[] spawnPoints;

    [Header("Настройки волн")]
    public float delayBetweenWaves = 5f;
    public float delayBetweenEnemies = 0.1f;

    [Header("Настройки спикера")]
    public float speakerDelay = 4f;

    private ClearAndResetGame _clearAndResetGame;
    private TextMeshPro _textSpeaker;

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
        _textSpeaker = GameObject.FindWithTag("WaveSpeaker").GetComponent<TextMeshPro>();
        _clearAndResetGame = FindAnyObjectByType<ClearAndResetGame>();
        currentWave = 0;
        if (waves.Length > 0)
        {
            StartCoroutine(SpawnWave(waves[currentWave]));
        }

    }

    public void OnEnemyKilled()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0 && _clearAndResetGame.isGameEnd == false)
        {
            currentWave++;
            if (currentWave < waves.Length)
            {
                StartCoroutine(StartNextWave());
            }
            else
            {
                ClearAndResetGame _clearAndResetGame = FindAnyObjectByType<ClearAndResetGame>();
                _clearAndResetGame.GameCompleted();
            }
        }
    }

    private IEnumerator StartNextWave()
    {
        Debug.Log($"Все враги уничтожены. Следующая волна через {delayBetweenWaves} секунд...");
        yield return new WaitForSeconds(delayBetweenWaves);
        StartCoroutine(SpeakNextWave(speakerDelay));
        StartCoroutine(SpawnWave(waves[currentWave]));
    }

    private IEnumerator SpeakNextWave(float speakerDelay)
    {
        _textSpeaker.enabled = true;
        yield return new WaitForSeconds(speakerDelay);
        _textSpeaker.enabled = false;
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
