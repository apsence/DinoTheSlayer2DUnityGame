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
    private TextMeshProUGUI _textSpeaker;
    private int _aliveEnemies = 0;
    private bool _nextWaveScheduled = false;
    private LevelManager _levelManager;


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
        _textSpeaker = GameObject.FindWithTag("WaveSpeaker").GetComponent<TextMeshProUGUI>();
        _clearAndResetGame = FindAnyObjectByType<ClearAndResetGame>();
        _levelManager = FindAnyObjectByType<LevelManager>();
        currentWave = 0;
        if (waves.Length > 0)
        {
            StartCoroutine(SpawnWave(waves[currentWave]));
        }

    }

    public void OnEnemyKilled()
    {
        _aliveEnemies--;

        if (_aliveEnemies <= 0 && !_clearAndResetGame.isGameEnd && !_nextWaveScheduled)
        {
            Debug.LogError("Все враги убиты");
            _nextWaveScheduled = true;

            currentWave++;
            if (currentWave < waves.Length)
            {
                StartCoroutine(StartNextWave());
            }
            else
            {
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
        _aliveEnemies = 0;
        _nextWaveScheduled = false;

        foreach (EnemyGroup group in wave.groups)
        {
            for (int i = 0; i < group.count; i++)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                GameObject enemy = Instantiate(group.prefab, spawnPoint.position, Quaternion.identity);

                // Проставляем ссылку на LevelManager
                UnitStats stats = enemy.GetComponent<UnitStats>();
                if (stats != null)
                {
                    stats._levelManager = this;
                    _aliveEnemies++;
                }
                else
                {
                    Debug.LogWarning($"На префабе {group.prefab.name} нет UnitStats!");
                }

                yield return new WaitForSeconds(delayBetweenEnemies);
            }
        }
    }


}
