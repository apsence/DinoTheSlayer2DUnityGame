using TMPro;
using UnityEngine;

public class UpgradesGUIRefreshCoins : MonoBehaviour
{
    [SerializeField] private Coin coin;
    [SerializeField] private TextMeshProUGUI text;

    void Update()
    {
        text.text = coin.coinsCount.ToString();
    }
}
