using TMPro;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinsTakenByPlayer;
    private TextMeshProUGUI _playerCoinsText;

    void Start()
    {
        _playerCoinsText = GameObject.FindWithTag("CoinsCount").GetComponent<TextMeshProUGUI>();
    }

    public void RefresfCountOfCoins()
    {
        _playerCoinsText.text = coinsTakenByPlayer.ToString();
    }
}
