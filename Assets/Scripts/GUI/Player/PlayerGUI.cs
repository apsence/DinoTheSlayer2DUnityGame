using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerGUI : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI coinsText;


    public void RefreshPlayerHUDHealthBar(int _current, int _max)
    {
        if (healthBar != null)
        {
            float ratio = (float)_current / _max;
            healthBar.fillAmount = ratio;
        }

        if (healthText != null)
        {
            healthText.text = $"{_current}";
        }

    }

    public void RefreshPlayerHUDDamage(int damage)
    {
        if (damageText != null)
        {
            damageText.text = $"{damage}";
        }
    }

    public void RefreshPlayerHUDCoins(int count)
    {
        if(coinsText != null)
        {
            coinsText.text = $"{count}";
        }
    }
}
