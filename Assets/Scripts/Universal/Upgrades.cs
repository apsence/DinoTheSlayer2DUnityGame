using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

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

    private int currentCostOfHPUpgrade;
    private int currentCostOfDamageUpgrade;

    private GameObject _player;
    private TextMeshPro _upgradesSpeaker;

    void Awake()
    {
        currentCostOfHPUpgrade = costOfUpgradeHP[0];
        currentCostOfDamageUpgrade = costOfUpgradeDamage[0];
        _player = GameObject.FindWithTag("Player");
        _upgradesSpeaker = GameObject.FindWithTag("UpgradesSpeaker").GetComponent<TextMeshPro>();
    }

    // ---------------Прокачка---------------
    void UpgradeHP(int coins)
    {
        if (coins >= currentCostOfHPUpgrade)
        {
            UnitStats unitStats = _player.GetComponent<UnitStats>();
            unitStats.maxHealth += hpPerLevel;

            if (unitStats.health < unitStats.maxHealth) unitStats.health += hpPerLevel / 2;
            for (int i = 0; i < costOfUpgradeHP.Count; i++)
            {
                if (costOfUpgradeHP[i] == currentCostOfHPUpgrade && i + 1 < costOfUpgradeHP.Count)
                {
                    currentCostOfHPUpgrade = costOfUpgradeHP[i + 1];
                }
            }
        }
        else
        {
            StartCoroutine(ShowMessageFromUpgradesSpeaker("Not enough gold"));
        }
    }

    void UpgradeDamage(int coins)
    {
        if (coins >= currentCostOfDamageUpgrade)
        {
            UnitStats unitStats = _player.GetComponent<UnitStats>();
            unitStats.damage += damagePerLevel;
            for (int i = 0; i < costOfUpgradeDamage.Count; i++)
            {
                if (costOfUpgradeDamage[i] == currentCostOfHPUpgrade && i + 1 < costOfUpgradeDamage.Count)
                {
                    currentCostOfHPUpgrade = costOfUpgradeDamage[i + 1];
                }
            }
        }
        else
        {
            StartCoroutine(ShowMessageFromUpgradesSpeaker("Not enough gold"));
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
}
