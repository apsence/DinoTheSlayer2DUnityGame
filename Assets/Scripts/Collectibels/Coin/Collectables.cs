using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Collectables : MonoBehaviour
{
    [Header("Характеристики монеты")]
    [SerializeField] private int monetsAdd;

    [Header("Характеристики зелья")]
    [SerializeField] private int hpRestore;

    [Header("Тип лута")]
    [SerializeField] private LootType lootType;


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        switch (lootType)
        {
            case LootType.Coin:
                Coin coin = collision.GetComponentInChildren<Coin>();
                coin.ChangeCoinsCount( + monetsAdd);

                break;

            case LootType.HealthPotion:
                Health health = collision.GetComponentInChildren<Health>();
                health.Heal(hpRestore);

                break;
        }

        Destroy(gameObject);
    }
}
