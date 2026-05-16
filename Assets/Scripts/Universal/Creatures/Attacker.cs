using UnityEngine;
using UnityEngine.Events;

public class Attacker : MonoBehaviour, IAttacker
{
    [Header("Атака")]
    [SerializeField] private int _damage = 10;
    [SerializeField] private float _attackCooldown = 0.2f;
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private int _minDamage = 1;
    [SerializeField] private int _maxDamage = 3;
    [SerializeField] private float _nextAttackTime;
    
    [Header("Layer Settings")]
    [SerializeField] private LayerMask _targetLayers = -1;
    
    [Header("Events")]
    public UnityEvent OnAttackPerformed;
    public UnityEvent<IDamageable> OnHitTarget;
    
    private float _lastAttackTime;
    private IDamageable _currentTarget;
    
    public int Damage {get {return _damage;} set {_damage = value; }}
    public float NextAttackTime {get {return _nextAttackTime;} set {_nextAttackTime = value;} }
    public float AttackColdown => _attackCooldown;
    
    void Update()
    {
        // Автоматический поиск цели в радиусе (опционально)
        FindNearestTarget();
    }
    
    private void FindNearestTarget()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, _attackRange, _targetLayers);
        
        float closestDistance = _attackRange;
        IDamageable closestTarget = null;
        
        foreach (var target in targets)
        {
            var damageable = target.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive && damageable.Transform != transform)
            {
                float distance = Vector2.Distance(transform.position, damageable.Transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = damageable;
                }
            }
        }
        
        _currentTarget = closestTarget;
    }
    
    public void PerformAttack(IDamageable target)
    {
        // Debug.Log($"PerformAttack() called! target = {target}");
        
        // if (target == null) 
        // {
        //     Debug.Log("target is NULL!");
        //     return;
        // }
        // if (!target.IsAlive) 
        // {
        //     Debug.Log("target is dead!");
        //     return;
        // }
        // if (Time.time < _lastAttackTime + _attackCooldown) 
        // {
        //     Debug.Log("on cooldown!");
        //     return;
        // }
        
        float distance = Vector2.Distance(transform.position, target.Transform.position);
        if (distance > _attackRange) 
        {
            Debug.Log($"out of range! distance = {distance}, attackRange = {_attackRange}");
            return;
        }
        
        Debug.LogWarning($"💥 ATTACKING from {name} (ID: {GetInstanceID()})");
        target.TakeDamage(Damage, _minDamage, _maxDamage);
        
        _lastAttackTime = Time.time;
        OnAttackPerformed?.Invoke();
        OnHitTarget?.Invoke(target);
    }

    public void Attack()
    {
        AttackCurrentTarget();
    }
    
    // Атака по текущему найденному
    public void AttackCurrentTarget()
    {
        Debug.Log($"AttackCurrentTarget() called! _currentTarget = {_currentTarget}");
        PerformAttack(_currentTarget);
    }
    
    // Атака по конкретному объекту
    public void AttackTarget(GameObject target)
    {
        var damageable = target.GetComponent<IDamageable>();
        PerformAttack(damageable);
    }
    
    // Ручная установка цели
    public void SetTarget(IDamageable target)
    {
        _currentTarget = target;
    }
    
    public bool HasTarget => _currentTarget != null && _currentTarget.IsAlive;
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}