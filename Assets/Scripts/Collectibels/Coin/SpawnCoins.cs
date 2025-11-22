using System.Collections;
using UnityEngine;

public class SpawnCoins : MonoBehaviour
{
    [Header("Настройки монеток")]
    public GameObject coinPrefab;
    public int maxCoins = 10;
    public float spawnInterval = 4f;

    [Header("Границы арены")]
    public Transform pointA;
    public Transform pointB;
    public float margin = 75f;

    private int currentCountOfCoins;
    private ClearAndResetGame _clearAndResetGame;

    void Start()
    {
        _clearAndResetGame = FindAnyObjectByType<ClearAndResetGame>();
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
        if (coinPrefab == null)
        {
            Debug.LogError("coinPrefab не назначен в инспекторе!");
            return;
        }
        if (pointA == null || pointB == null)
        {
            Debug.LogError("Границы арены не заданы!");
            return;
        }

        float minX = Mathf.Min(pointA.position.x, pointB.position.x) + margin;
        float maxX = Mathf.Max(pointA.position.x, pointB.position.x) - margin;
        float minY = Mathf.Min(pointA.position.y, pointB.position.y) + margin;
        float maxY = Mathf.Max(pointA.position.y, pointB.position.y) - margin;

        Vector3 randomPos = new Vector3(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY),
            0f
        );

        GameObject coin = Instantiate(coinPrefab, randomPos, Quaternion.identity);
        currentCountOfCoins++;
    }

    public void CoinCollected()
    {
        currentCountOfCoins--;
    }
}
