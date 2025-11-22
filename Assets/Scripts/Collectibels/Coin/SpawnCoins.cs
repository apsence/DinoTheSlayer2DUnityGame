using System.Collections;
using UnityEngine;

public class SpawnCoins : MonoBehaviour
{

    [Header("Настройки монеток")]
    public GameObject coinPrefab;
    public int maxCoins = 10;
    public float spawnInterval = 4f;
    private Transform _gameArea;
    private int currentCountOfCoins;
    private ClearAndResetGame _clearAndResetGame;

    void Start()
    {
        _gameArea = GameObject.FindWithTag("GameArea").GetComponent<Transform>();
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (_clearAndResetGame.isGameEnd) break;
            if (currentCountOfCoins < maxCoins)
            {
                CreateCoin();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void CreateCoin()
    {
        Vector3 randomPos = new Vector3(
            Random.Range(_gameArea.position.x - _gameArea.localScale.x / 2, _gameArea.position.x + _gameArea.localScale.x / 2),
            Random.Range(_gameArea.position.y - _gameArea.localScale.y / 2, _gameArea.position.y + _gameArea.localScale.y / 2),
            0
        );

        Instantiate(coinPrefab, randomPos, Quaternion.identity, _gameArea);
        currentCountOfCoins++;
    }

    public void CoinCollected()
    {
        currentCountOfCoins--;
    }
}
