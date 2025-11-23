using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class UnitStats : MonoBehaviour
{
    [Header("Stats")]
    public int damage;
    public int health;
    public int maxHealth = 100;

    [Header("Default Player Stats")]
    public int defaultMaxHP = 100;
    public int defaultDamage = 6;

    [Header("UI (Player Only)")]
    public Image healthBar;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI damageText;

    [HideInInspector] public LevelManager _levelManager;

    void Start()
    {
        RefreshPlayerHUD();
    }

    public void TakeDamage(int amount)
    {
        int finalDamage = amount + Random.Range(1, 3);

        health -= finalDamage;
        health = Mathf.Clamp(health, 0, maxHealth);

        Debug.Log($"{gameObject.name} получил {finalDamage} урона. Осталось здоровья: {health}");

        if (CompareTag("Player"))
        {
            RefreshPlayerHUD();
        }

        if (health <= 0)
        {
            Die();
        }
    }

    public void RefreshPlayerHUD()
    {
        if (healthBar != null)
        {
            float ratio = (float)health / maxHealth;
            healthBar.fillAmount = ratio;
        }

        if (healthText != null)
        {
            healthText.text = $"{health}";
        }

        if (damageText != null)
        {
            damageText.text = $"{damage}";
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} погиб");

        if (_levelManager != null && CompareTag("Enemy"))
        {
            _levelManager.OnEnemyKilled();
        }

        if (gameObject.tag == "Player" && health <= 0)
        {
            ClearAndResetGame _gameEnd = FindAnyObjectByType<ClearAndResetGame>();
            _gameEnd.GameOver();
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }


}
