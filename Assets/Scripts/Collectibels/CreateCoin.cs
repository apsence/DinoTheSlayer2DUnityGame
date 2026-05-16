using UnityEngine;

public class CreateCoin : MonoBehaviour
{
    [SerializeField] private GameObject coin;


    // Базовый метод
    public void Create(Vector3 position, float yOffset = 0f)
    {
        GameObject newCoin = Instantiate(coin, new Vector3(position.x, position.y - yOffset, position.z), Quaternion.identity);
        //Debug.LogWarning(newCoin.transform.position);
    }

    // Перегрузка для создания нескольких монет
    public void CreateMultipleCoins(Vector3 basePosition, int count, float yOffset = 0f, float spreadRadius = 0f)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = basePosition;
            
            // Если нужен разброс монет
            if (spreadRadius > 0)
            {
                Vector2 randomOffset = Random.insideUnitCircle * spreadRadius;
                spawnPos = new Vector3(
                    basePosition.x + randomOffset.x,
                    basePosition.y + randomOffset.y - yOffset,
                    basePosition.z
                );
            }
            else
            {
                spawnPos = new Vector3(basePosition.x, basePosition.y - yOffset, basePosition.z);
            }
            
            Create(spawnPos); // yOffset уже учтён, ничего не передаем
        }
    }
}
