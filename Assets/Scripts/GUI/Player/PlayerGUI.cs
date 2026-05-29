using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerGUI : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI hpUpgradeCostText;
    [SerializeField] private TextMeshProUGUI damageUpgradeCostText;
    [SerializeField] private TextMeshProUGUI hpHotKey;
    [SerializeField] private TextMeshProUGUI damageHotKey;
    [SerializeField] private Image upgradeHPImage;
    [SerializeField] private Image upgradeDamageImage;
    [SerializeField] private Image heart;
    [SerializeField] private Image sword;
    [SerializeField] private Image hpGreenTick;
    [SerializeField] private Image damageGreenTick;
    [SerializeField] private Button hpUpgradeButton;
    [SerializeField] private Button damageUpgradeButton;
    [SerializeField] private Attacker _attacker;
    [SerializeField] private Health _health;

    void Start()
    {
        RefreshPlayerHUDDamage(_attacker.Damage);
        RefreshPlayerHUDHealthBar(_health.CurrentHealth, _health.MaxHealth);
    }

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



    public void RefreshHPUpgradeCost(int value)
    {
        hpUpgradeCostText.text = value.ToString();
    }

    public void RefreshDamageUpgradeCost(int value)
    {
        damageUpgradeCostText.text = value.ToString();
    }

    public void HideHPUpgrade()
    {
        hpUpgradeCostText.alpha = 0;
        hpHotKey.alpha = 0;
        hpUpgradeButton.interactable = false;
        upgradeHPImage.color = new Color32(150, 150, 150, 255);
        heart.color = new Color32(150, 150, 150, 255);
        hpGreenTick.color = new Color32(255, 255, 255, 255);
    }

    public void HideDamageUpgrade()
    {
        damageUpgradeCostText.alpha = 0;
        damageHotKey.alpha = 0;
        damageUpgradeButton.interactable = false;
        upgradeDamageImage.color = new Color32(150, 150, 150, 255);
        sword.color = new Color32(150, 150, 150, 255);
        damageGreenTick.color = new Color32(255, 255, 255, 255);
    }
}
