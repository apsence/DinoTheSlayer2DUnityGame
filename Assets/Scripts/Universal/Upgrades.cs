using System.Collections.Generic;
using UnityEngine;

public class Upgrades : MonoBehaviour
{
    [Header("Настройки прокачки")]
    [SerializeField] private List<int> costOfUpgradeHP;
    [SerializeField] private List<int> costOfUpgradeDamage;

    [SerializeField] private int hpPerLevel = 10;
    [SerializeField] private int damagePerLevel = 3;
    [SerializeField] private int hpBonus = 5;

    [Header("Текущие уровни")]
    [SerializeField] private int currentHPLevel;
    [SerializeField] private int currentDamageLevel;

    [Header("Текущая стоимость")]
    [SerializeField] private int currentCostOfHPUpgrade;
    [SerializeField] private int currentCostOfDamageUpgrade;

    private Coin _coins;
    private PlayerGUI _playerGUI;
    private Health _health;
    private Attacker _attacker;
    private UpgradeGUI _upgradeGUI;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        _coins = player.GetComponent<Coin>();
        _playerGUI = player.GetComponent<PlayerGUI>();
        _health = player.GetComponent<Health>();
        _attacker = player.GetComponent<Attacker>();
        _upgradeGUI = player.GetComponent<UpgradeGUI>();

    }

    private void Start()
    {
        RefreshUpgradeUI();
    }

    // ---------------- HP ----------------

    public void UpgradeHP()
    {
        if (_upgradeGUI.NeedToShow == false) return;

        if (IsHPUpgradeMaxLevel())
            return;

        int cost = costOfUpgradeHP[currentHPLevel];

        if (_coins.coinsCount < cost)
            return;

        _coins.ChangeCoinsCount(-cost);

        _health.UpgradeMaxHealth(hpPerLevel, hpBonus);

        currentHPLevel++;

        RefreshHPUpgradeUI();
    }

    // ---------------- DAMAGE ----------------

    public void UpgradeDamage()
    {
        if (_upgradeGUI.NeedToShow == false) return;

        if (IsDamageUpgradeMaxLevel())
            return;

        int cost = costOfUpgradeDamage[currentDamageLevel];

        if (_coins.coinsCount < cost)
            return;

        _coins.ChangeCoinsCount(-cost);

        _attacker.UpgradeDamage(damagePerLevel);

        currentDamageLevel++;

        RefreshDamageUpgradeUI();
    }

    // ---------------- UI ----------------

    private void RefreshUpgradeUI()
    {
        RefreshHPUpgradeUI();
        RefreshDamageUpgradeUI();
    }

    private void RefreshHPUpgradeUI()
    {
        if (IsHPUpgradeMaxLevel())
        {
            _playerGUI.HideHPUpgrade();
            return;
        }

        currentCostOfHPUpgrade = costOfUpgradeHP[currentHPLevel];
        _playerGUI.RefreshHPUpgradeCost(currentCostOfHPUpgrade);
    }

    private void RefreshDamageUpgradeUI()
    {
        if (IsDamageUpgradeMaxLevel())
        {
            _playerGUI.HideDamageUpgrade();
            return;
        }

        currentCostOfDamageUpgrade = costOfUpgradeDamage[currentDamageLevel];
        _playerGUI.RefreshDamageUpgradeCost(currentCostOfDamageUpgrade);
    }

    // ---------------- CHECKS ----------------

    private bool IsHPUpgradeMaxLevel()
    {
        return currentHPLevel >= costOfUpgradeHP.Count;
    }

    private bool IsDamageUpgradeMaxLevel()
    {
        return currentDamageLevel >= costOfUpgradeDamage.Count;
    }
}