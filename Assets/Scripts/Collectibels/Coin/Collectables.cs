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

    [Header("Эффеты")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private float xOffSet;
    [SerializeField] private float yOffSet;

    private EffectSpawner _effectSpawner;

    void Awake()
    {
        _effectSpawner = GetComponent<EffectSpawner>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("PlayerCollider"))
            return;
        PlayerReferences _playerReferences = collision.GetComponentInParent<PlayerReferences>();

        switch (lootType)
        {
            case LootType.Coin:
                //Coin coin = collision.GetComponentInParent<Coin>();
                _playerReferences.Coin.ChangeCoinsCount( + monetsAdd);

                break;

            case LootType.HealthPotion:
                //Health health = collision.GetComponentInParent<Health>();
                _playerReferences.Health.Heal(hpRestore);
                _effectSpawner.Create(prefab, collision.transform, xOffSet, yOffSet);

                break;
        }

        Destroy(gameObject);
    }
}
