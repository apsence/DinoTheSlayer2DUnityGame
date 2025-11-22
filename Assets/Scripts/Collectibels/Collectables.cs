using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Collectables : MonoBehaviour
{

    private SpawnCoins _spawnCoins;
    private TextMeshProUGUI _playerCoinsText;
    private Coin _coin;
    void Awake()
    {
        _coin = FindAnyObjectByType<Coin>();
        _spawnCoins = FindAnyObjectByType<SpawnCoins>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Destroy(gameObject);
            _coin.coinsTakenByPlayer += 1;
            _coin.RefresfCountOfCoins();
            _spawnCoins.CoinCollected();

        }
    }
}
