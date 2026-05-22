using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Здоровье")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("GUI")]
    [SerializeField] private PlayerGUI _playerGUI;

    [Header("---НЕ ИГРОК---")]
    [Header("Ссылки")]
    [SerializeField] private CreaterOfRewards createrOfRewards;

    private bool _isDead;
    public int MaxHealth {get;}
    public bool IsAlive => !_isDead && currentHealth > 0;
    public Transform Transform => transform;
    public int CurrentHealth => currentHealth;
    
    void Start()
    {
        currentHealth = maxHealth;
        _isDead = false;
    }
    
    public void TakeDamage(int amount, int _minDamage, int _maxDamage)
    {
        if (_isDead) return;
        
        // Случайный разброс урона
        int finalDamage = amount + Random.Range(_minDamage, _maxDamage + 1);
        finalDamage = Mathf.Max(1, finalDamage); // Минимум 1 урон
        
        currentHealth -= finalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        if(gameObject.CompareTag("Player")) _playerGUI.RefreshPlayerHUDHealthBar(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }

    }
    
    public void Heal(int amount)
    {
        if (_isDead) return;
        if(currentHealth == MaxHealth) return;
        
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        _playerGUI.RefreshPlayerHUDHealthBar(
        currentHealth,
        MaxHealth);
    }
    
    private void Die()
    {
        if (_isDead) return;
        
        _isDead = true;
        
        createrOfRewards.CreateReward(transform.position);
        Destroy(gameObject);        
    }

    public void UpgradeMaxHealth(int amount, int healBonus)
    {
        maxHealth += amount;

        currentHealth = Mathf.Min(currentHealth + healBonus, maxHealth);

        _playerGUI.RefreshPlayerHUDHealthBar(
        currentHealth,
        MaxHealth);
    }
}