using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitStats : MonoBehaviour
{
    [Header("Stats")]
    public int damage;
    public int health;
    public int maxHealth = 100;

    [Header("UI (Player Only)")]
    public Image healthBar;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI damageText;

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
            healthText.text = $"HP: {health}";
        }

        if (damageText != null)
        {
            damageText.text = $"DMG: {damage}";
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} погиб");

        LevelManager manager = FindFirstObjectByType<LevelManager>();
        if (manager != null && CompareTag("Enemy"))
        {
            manager.OnEnemyKilled();
        }

        Destroy(gameObject);
    }
}
