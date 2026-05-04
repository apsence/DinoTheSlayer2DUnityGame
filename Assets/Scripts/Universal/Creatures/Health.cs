using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Здоровье")]
    [SerializeField] private int _maxHealth = 100;
    
    [Header("События")]
    public UnityEvent<int> OnDamageTaken;      // (finalDamage)
    public UnityEvent<int> OnHealthChanged;     // (currentHealth)
    public UnityEvent OnDeath;
    public UnityEvent OnHeal;

    [Header("GUI")]
    [SerializeField] private PlayerGUI _playerGUI;
    private int _currentHealth;
    private bool _isDead;
    
    public int CurrentHealth => _currentHealth;
    public int MaxHealth {get {return MaxHealth; } set{_maxHealth = value; }}
    public bool IsAlive => !_isDead && _currentHealth > 0;
    public Transform Transform => transform;
    
    void Start()
    {
        _currentHealth = _maxHealth;
        _isDead = false;
    }
    
    public void TakeDamage(int amount, int _minDamage, int _maxDamage)
    {
        if (_isDead) return;
        
        // Случайный разброс урона
        int finalDamage = amount + Random.Range(_minDamage, _maxDamage + 1);
        finalDamage = Mathf.Max(1, finalDamage); // Минимум 1 урон
        
        _currentHealth -= finalDamage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        
        // События
        OnDamageTaken?.Invoke(finalDamage);
        OnHealthChanged?.Invoke(_currentHealth);
        
        Debug.Log($"{name} получил {finalDamage} урона. Осталось здоровья: {_currentHealth}");
        
        if(gameObject.CompareTag("Player")) _playerGUI.RefreshPlayerHUDHealthBar(_currentHealth, _maxHealth);

        if (_currentHealth <= 0)
        {
            Die();
        }

    }
    
    public void Heal(int amount)
    {
        if (_isDead) return;
        
        _currentHealth += amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        
        OnHealthChanged?.Invoke(_currentHealth);
        OnHeal?.Invoke();
        
        Debug.Log($"{name} вылечен на {amount}. Здоровья: {_currentHealth}");
    }
    
    private void Die()
    {
        if (_isDead) return;
        
        _isDead = true;
        OnDeath?.Invoke();
        
        Debug.Log($"{name} умер!");
        
        // Отключаем компоненты при смерти
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
        
        var collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;
        
        // Активируем анимацию смерти
        var animator = GetComponent<UnitAnimator>();
        if (animator != null) animator.Die();
    }
}