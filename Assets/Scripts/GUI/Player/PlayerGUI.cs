using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerGUI : MonoBehaviour
{
    public Image healthBar;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI damageText;


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
}
