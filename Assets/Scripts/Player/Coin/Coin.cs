using TMPro;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinsCount;
    [SerializeField] private PlayerGUI playerGUI;

    // Обновляет количество монет (и добавляет, если false)
    public void ChangeCoinsCount(int amount)
    {
        coinsCount += amount;

        if(coinsCount < 0)
        {
            coinsCount = 0;
        }

        RefreshCoinsGUI();
    }

    void RefreshCoinsGUI()
    {
        playerGUI.RefreshPlayerHUDCoins(coinsCount);
    }
}
