using UnityEngine;
using System.Collections.Generic;

public class CreaterOfRewards : MonoBehaviour
{
    [SerializeField] private List<LootEntry> loot;
    [SerializeField] private ItemSpawner itemSpawner;

    [Header("Монеты")]
    [SerializeField] private float spreadRadius;
    [SerializeField] private float yOffSet;

    public void CreateReward(Vector3 pos)
    {
        foreach (LootEntry entry in loot)
        {
            itemSpawner.SpawnMultiple(
                entry.lootType,
                pos,
                entry.amount,
                yOffSet,
                spreadRadius);
        }
    }
}
