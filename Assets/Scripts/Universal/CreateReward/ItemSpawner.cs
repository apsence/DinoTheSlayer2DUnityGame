using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject healthPotionPrefab;

    // Создание одного предмета
    public void Spawn(LootType lootType, Vector3 position, float yOffset = 0f)
    {
        GameObject prefab = GetPrefabByLootType(lootType);

        if (prefab == null)
        {
            Debug.LogWarning($"Нет prefab для loot type: {lootType}");
            return;
        }

        Vector3 spawnPosition = new Vector3(
            position.x,
            position.y - yOffset,
            position.z);

        Instantiate(prefab, spawnPosition, Quaternion.identity);
    }

    // Создание нескольких предметов
    public void SpawnMultiple(LootType lootType, Vector3 basePosition, int count, float yOffset = 0f, float spreadRadius = 0f)
    {
        GameObject prefab = GetPrefabByLootType(lootType);

        if (prefab == null)
        {
            Debug.LogWarning($"Нет prefab для loot type: {lootType}");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos;

            // Разброс
            if (spreadRadius > 0f)
            {
                Vector2 randomOffset =
                    Random.insideUnitCircle * spreadRadius;

                spawnPos = new Vector3(
                    basePosition.x + randomOffset.x,
                    basePosition.y + randomOffset.y - yOffset,
                    basePosition.z);
            }
            else
            {
                spawnPos = new Vector3(
                    basePosition.x,
                    basePosition.y - yOffset,
                    basePosition.z);
            }

            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }

    private GameObject GetPrefabByLootType(LootType lootType)
    {
        switch (lootType)
        {
            case LootType.Coin:
                return coinPrefab;

            case LootType.HealthPotion:
                return healthPotionPrefab;

            default:
                return null;
        }
    }
}