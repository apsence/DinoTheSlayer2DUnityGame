using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Upgrades : MonoBehaviour
{
    [Header("Настройки прокачки")]
    public List<int> costOfUpgradeHP;
    public List<int> costOfUpgradeDamage;
    public int maxLevelOfUpgrades = 5;
    public int hpPerLevel = 10;
    public int damagePerLevel = 3;
    [Header("Настройки спикера прокачки")]
    public float upgradesSpeakerDuration = 3f;

    [Header("Отладка")]
    public int currentCostOfHPUpgrade;
    public int currentCostOfDamageUpgrade;
    public int currentHPLevel = 0;
    public int currentDamageLevel = 0;

    private Coin _coins;
    private TextMeshProUGUI _upgradesSpeaker;
    private UnitStats _unitStats;
    private TextMeshProUGUI _HPUpgradeCost;
    private TextMeshProUGUI _DamageUpgradeCost;
    private Button _upgradeHPButton;
    private Button _upgradeDamageButton;
    private Speaker _speaker;

    void Awake()
    {
        currentCostOfHPUpgrade = costOfUpgradeHP[0];
        currentCostOfDamageUpgrade = costOfUpgradeDamage[0];
        _coins = FindAnyObjectByType<Coin>();
        _unitStats = GameObject.FindWithTag("Player").GetComponent<UnitStats>();
        _upgradesSpeaker = GameObject.FindWithTag("UpgradesSpeaker").GetComponent<TextMeshProUGUI>();
        _HPUpgradeCost = GameObject.FindWithTag("HPUpgradeCost").GetComponent<TextMeshProUGUI>();
        _DamageUpgradeCost = GameObject.FindWithTag("DamageUpgradeCost").GetComponent<TextMeshProUGUI>();
        _upgradeHPButton = GameObject.FindWithTag("UpgradeHPButton").GetComponent<Button>();
        _upgradeDamageButton = GameObject.FindWithTag("UpgradeDamageButton").GetComponent<Button>();
        _speaker = FindAnyObjectByType<Speaker>();
    }

    void Start()
    {
        RefreshDamageCost();
        RefreshHPCost();
    }

    // ---------------Прокачка---------------
    public void UpgradeHP()
    {
        BlockHPUpgrade();

        int cost = costOfUpgradeHP[currentHPLevel];
        if (_coins.coinsTakenByPlayer >= cost)
        {
            _coins.coinsTakenByPlayer -= cost;
            _coins.RefresfCountOfCoins();

            _unitStats.maxHealth += hpPerLevel;
            _unitStats.health = Mathf.Min(_unitStats.health + hpPerLevel / 2, _unitStats.maxHealth);
            _unitStats.RefreshPlayerHUD();

            currentHPLevel++;

            if (currentHPLevel < costOfUpgradeHP.Count)
            {
                currentCostOfHPUpgrade = costOfUpgradeHP[currentHPLevel];
            }
            else
            {
                _HPUpgradeCost.text = "";
            }

            RefreshHPCost();
            BlockHPUpgrade();
        }
        else
        {
            _speaker.AddCoroutine(ShowMessageFromUpgradesSpeaker("Not enough gold"));
        }
    }

    public void UpgradeDamage()
    {
        BlockDamageUpgrade();

        int cost = costOfUpgradeDamage[currentDamageLevel];
        if (_coins.coinsTakenByPlayer >= cost)
        {
            _coins.coinsTakenByPlayer -= cost;
            _coins.RefresfCountOfCoins();

            _unitStats.damage += damagePerLevel;
            _unitStats.RefreshPlayerHUD();

            currentDamageLevel++;

            if (currentDamageLevel < costOfUpgradeDamage.Count)
            {
                currentCostOfDamageUpgrade = costOfUpgradeDamage[currentDamageLevel];
            }
            else
            {
                _DamageUpgradeCost.text = "";
            }
            RefreshDamageCost();
            BlockDamageUpgrade();
        }
        else
        {
            _speaker.AddCoroutine(ShowMessageFromUpgradesSpeaker("Not enough gold"));
        }
    }

    // ---------------Прокачка---------------

    IEnumerator ShowMessageFromUpgradesSpeaker(string message)
    {
        _upgradesSpeaker.enabled = true;
        _upgradesSpeaker.text = message;

        yield return new WaitForSeconds(upgradesSpeakerDuration);

        _upgradesSpeaker.enabled = false;
    }

    void RefreshDamageCost()
    {
        _DamageUpgradeCost.text = currentCostOfDamageUpgrade.ToString();
    }

    void RefreshHPCost()
    {
        _HPUpgradeCost.text = currentCostOfHPUpgrade.ToString();
    }

    void BlockDamageUpgrade()
    {
        if (currentDamageLevel >= costOfUpgradeDamage.Count)
        {
            _speaker.AddCoroutine(ShowMessageFromUpgradesSpeaker("Damage upgrade maxed out"));
            _DamageUpgradeCost.text = "";
            _upgradeDamageButton.interactable = false;
            return;
        }
    }

    void BlockHPUpgrade()
    {
        if (currentHPLevel >= costOfUpgradeHP.Count)
        {
            _speaker.AddCoroutine(ShowMessageFromUpgradesSpeaker("HP upgrade maxed out"));
            _HPUpgradeCost.text = "";
            _upgradeHPButton.interactable = false;
            return;
        }
    }
}
