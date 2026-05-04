using UnityEngine;
using System;

public class UnitAnimator : MonoBehaviour
{
    [Header("Компоненты")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D _rb;
    
    [Header("Настройки передвижения")]
    [SerializeField] private float movementThreshold = 0.1f;
    
    [Header("Опционально")]
    [SerializeField] private bool _hasAttack;
    [SerializeField] private bool _hasDeath;
    [SerializeField] private bool _hasDamage;
    
    // Состояния
    private bool _isAttacking;
    private bool _isDead;
    
    // События для внешней логики
    public event Action OnAttackStart;
    public event Action OnAttackEnd;
    public event Action OnDeath;
    public event Action OnHit;
    
    void Start()
    {
        AutoDetectCapabilities();
        SetupAnimatorParameters();
    }
    
    private void AutoDetectCapabilities()
    {
        // Проверяем наличие параметров в Animator
        _hasAttack = HasParameter("isAttacking") && HasParameter("attackTrigger");
        _hasDeath = HasParameter("isDead") || HasParameter("deathTrigger");
        _hasDamage = HasParameter("hitTrigger");
        
        Debug.Log($"[{name}] Animator capabilities - Attack: {_hasAttack}, Death: {_hasDeath}, Damage: {_hasDamage}");
    }
    
    private bool HasParameter(string paramName)
    {
        if (_animator == null) return false;
        
        foreach (var param in _animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }
    
    private void SetupAnimatorParameters()
    {
        // Устанавливаем значения по умолчанию (если параметры существуют)
        if (_hasAttack)
        {
            _animator.SetBool("isAttacking", false);
        }
        if (_hasDeath && HasParameter("isDead"))
        {
            _animator.SetBool("isDead", false);
        }
    }
    
    void Update()
    {
        if (_isDead) return;
        
        if (_rb != null && !_isAttacking)
        {
            UpdateMovementAnimation(_rb.linearVelocity);
        }
    }
    
    public void UpdateMovementAnimation(Vector2 velocity)
    {
        if (_isDead) return;
        
        bool isMoving = velocity.sqrMagnitude > movementThreshold * movementThreshold;
        
        if (HasParameter("isMoving"))
            _animator.SetBool("isMoving", isMoving);
        
    }

    // В классе UnitAnimator:
    public void SetAttacking(bool attacking)
    {
        //Debug.Log($"SetAttacking called: attacking={attacking}, _isAttacking={_isAttacking}, _isDead={_isDead}, _hasAttack={_hasAttack}");
        
        if (_isDead || !_hasAttack) return;
        if (_isAttacking == attacking) 
        {
            Debug.Log("Already in this state, skipping");
            return;
        }
        
        //Debug.Log($"Changing state to: {attacking}");
        _isAttacking = attacking;
        _animator.SetBool("isAttacking", attacking);
        
        if (attacking)
        {
            _animator.SetTrigger("attackTrigger");
            OnAttackStart?.Invoke();
        }
        else
        {
            OnAttackEnd?.Invoke();
        }
    }
    
    // Death methods
    public void Die()
    {
        if (_isDead) return;
        
        _isDead = true;
        
        if (_hasDeath)
        {
            if (HasParameter("deathTrigger"))
                _animator.SetTrigger("deathTrigger");
            if (HasParameter("isDead"))
                _animator.SetBool("isDead", true);
        }
        
        OnDeath?.Invoke();
    }
    
    // Damage methods
    public void TakeHit()
    {
        if (_isDead || !_hasDamage) return;
        
        _animator.SetTrigger("hitTrigger");
        OnHit?.Invoke();
    }
    
    // Public properties
    public bool IsAttacking => _isAttacking;
    public bool IsDead => _isDead;
    public bool HasAttack => _hasAttack;
    public bool HasDeath => _hasDeath;
}