using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Здоровье")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private float delayBeforeDestroyPlayer;
    [SerializeField] private float hpRegenRate = 0.5f;
    [SerializeField] private int hpRegen = 1;  

    [Header("GUI")]
    [SerializeField] private PlayerGUI _playerGUI;

    [Header("---НЕ ИГРОК---")]
    [SerializeField] private float delayBeforeDestroyAI;
    [Header("Ссылки")]
    
    
    private CreaterOfRewards _createrOfRewards;

    private bool _isDead;
    private ManageBarVisibility manageBarVisibility;
    private UnitAnimator _unitAnimator;

    public int MaxHealth => maxHealth;
    public bool IsAlive => !_isDead && currentHealth > 0;
    public Transform Transform => transform;
    public int CurrentHealth => currentHealth;
    
    void Start()
    {
        currentHealth = maxHealth;
        _isDead = false;
        manageBarVisibility = GetComponent<ManageBarVisibility>();
        _unitAnimator = GetComponent<UnitAnimator>();
        _createrOfRewards = GetComponent<CreaterOfRewards>();

        if(hpRegen > 0 && hpRegenRate > 0)
        {
            StartCoroutine(Regen());
        }
    }
    
    public void TakeDamage(int amount, int _minDamage, int _maxDamage)
    {
        if (_isDead) return;
        // Случайный разброс урона
        int finalDamage = amount + Random.Range(_minDamage, _maxDamage + 1);
        finalDamage = Mathf.Max(1, finalDamage); // Минимум 1 урон
        
        currentHealth -= finalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        if(gameObject.CompareTag("PlayerScripts")) {
            _playerGUI.RefreshPlayerHUDHealthBar(currentHealth, maxHealth);
        }
        else if(gameObject.CompareTag("Enemy"))
        {
            manageBarVisibility.ShowBar();
        }

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

        if (gameObject.CompareTag("Player"))
        {
            Destroy(gameObject, delayBeforeDestroyPlayer);
        }
        else if(gameObject.CompareTag("Enemy"))
        {
            _createrOfRewards.CreateReward(transform.position);
            _unitAnimator.Die();

            gameObject.GetComponent<Rigidbody2D>().simulated = false;
            gameObject.GetComponent<Collider2D>().enabled = false;

            Destroy(gameObject, delayBeforeDestroyAI);
        } else if (gameObject.CompareTag("Destructible"))
        {
            gameObject.GetComponent<Animator>().SetTrigger("Destroy");
            gameObject.GetComponent<Rigidbody2D>().simulated = false;
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
                
    }

    IEnumerator Regen()
    {
        while (IsAlive)
        {
            yield return new WaitForSeconds(hpRegenRate);
            currentHealth = Mathf.Min(currentHealth + hpRegen, maxHealth);
            if(gameObject.CompareTag("PlayerScripts")) {
                _playerGUI.RefreshPlayerHUDHealthBar(currentHealth, maxHealth);
            }
            else
            {
                manageBarVisibility.RefreshHealthBar();
            }
        }

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