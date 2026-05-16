using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Collectables : MonoBehaviour
{
    private Coin _coin;
    private PlayerGUI _playerGUI;
    void Awake()
    {
        _coin = FindAnyObjectByType<Coin>();
        _playerGUI = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerGUI>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Destroy(gameObject);
            _coin.coinsCount += 1;
            _playerGUI.RefreshPlayerHUDCoins(_coin.coinsCount);
        }
    }
}
