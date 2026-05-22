using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class AI_Common : MonoBehaviour
{
    [Header("Атака")]
    [SerializeField] private float agroRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float maxDistanceToPlayer;
    [SerializeField] private float _exitAttackRange;

    [Header("Перемещение")]
    [SerializeField] private float speed;

    [Header("Патрулирование")]
    private Vector2 _targetWanderPoint;
    [SerializeField] private float wanderRadius = 2f;      // радиус случайного перемещения
    [SerializeField] private float waitTimeAtPoint = 2f;   // время ожидания в точке
    [SerializeField] private float minWanderTime = 3f;     // минимум до след. точки
    [SerializeField] private float maxWanderTime = 6f;     // максимум до след. точки
    [Header("Анимирование")]
    [SerializeField] private UnitAnimator _unitAnimator;

    [SerializeField] private LayerMask _targetLayers = -1;
    private Transform _playerTransform;
    private bool _isAttacking;
    private bool _isMovingToWanderPoint;
    private bool _isWaiting; 
    private Vector2 _defaultPosition;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rb;
    private Attacker _attacker;
    private Coroutine _wanderCoroutine;
    private IDamageable _currentTarget;

    public bool HasTarget => _currentTarget != null && _currentTarget.IsAlive;
    void Awake()
    {
        _playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        _defaultPosition = transform.position;
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _attacker = GetComponent<Attacker>();
    }
    void Start()
    {
        _wanderCoroutine = StartCoroutine(WanderRoutine());
    }

    void Update()
    {
        FindNearestTarget();
    }

    void FixedUpdate()
    {
        float distance = DistanceToPlayer();
        // Если игрок рядом - включаем режим атаки
        if (distance < agroRange && !_isAttacking)
        {
            _isAttacking = true;
            StopWanderMovement(); // останавливаем блуждание
        }
        else if (distance > _exitAttackRange && _isAttacking)
        {
            _isAttacking = false;
            StartCoroutine(WanderRoutine()); // перезапускаем блуждание
            _unitAnimator.SetAttacking(false);
        }

        if (_isAttacking)
        {
            bool shouldAttack = distance < attackRange;
            _unitAnimator.SetAttacking(shouldAttack);

            if (shouldAttack && Time.time >= _attacker.NextAttackTime)
            {
                _attacker.NextAttackTime = Time.time + _attacker.AttackColdown;
                Attack();
            }
        }
        
        // Применяем движение
        if (_isAttacking)
        {
            MoveToPlayer();
        }
        else if (_isMovingToWanderPoint)
        {
            MoveToWanderPoint();
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
        }
    }

    private void MoveToWanderPoint()
    {
        Vector2 direction = (_targetWanderPoint - (Vector2)transform.position).normalized;
        
        // Обновляем направление спрайта
        UpdateSpriteDirection(direction.x);
        
        _rb.linearVelocity = direction * speed;
        
        // Если достигли точки
        if (Vector2.Distance(transform.position, _targetWanderPoint) < 0.1f)
        {
            _rb.linearVelocity = Vector2.zero;
            _isMovingToWanderPoint = false;
            StartCoroutine(WaitAtPoint());
        }
    }
    
    // ⏰ Корутина блуждания (выбирает случайные точки)
    private IEnumerator WanderRoutine()
    {
        while (!_isAttacking) // пока не атакуем
        {
            // Ждём случайное время перед выбором новой точки
            float waitTime = UnityEngine.Random.Range(minWanderTime, maxWanderTime);
            yield return new WaitForSeconds(waitTime);
            
            if (!_isAttacking && !_isWaiting)
            {
                // Выбираем случайную точку вокруг дефолтной позиции
                _targetWanderPoint = _defaultPosition + UnityEngine.Random.insideUnitCircle * wanderRadius;
                _isMovingToWanderPoint = true;
            }
        }
    }
    
    // ⏸ Ожидание в точке (чтобы не дрожал)
    private IEnumerator WaitAtPoint()
    {
        _isWaiting = true;
        yield return new WaitForSeconds(waitTimeAtPoint);
        _isWaiting = false;
    }

    private void StopWanderMovement()
    {
        _isMovingToWanderPoint = false;
        _isWaiting = false;
        StopCoroutine(_wanderCoroutine); // останавливаем все корутины
        
        // Мгновенно останавливаем физику
        _rb.linearVelocity = Vector2.zero;
    }

    float DistanceToPlayer()
    {
        float distance = Vector2.Distance( _playerTransform.position, transform.position);
        return distance;
    }

    void MoveToPlayer()
    {
        Vector2 direction = ((Vector2)_playerTransform.position - (Vector2)transform.position).normalized;
        UpdateSpriteDirection(direction.x);
        if(DistanceToPlayer() < maxDistanceToPlayer) {
            _rb.linearVelocity = Vector2.zero;
            return;
        }
        _rb.linearVelocity = direction * speed;
    }

    private void UpdateSpriteDirection(float directionX)
    {
        if (directionX < 0)
            _spriteRenderer.flipX = true;
        else if (directionX > 0)
            _spriteRenderer.flipX = false;
    }

    //* ---Таргеты---
        private void FindNearestTarget()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, _attacker.AttackRange, _targetLayers);
        
        float closestDistance = _attacker.AttackRange;
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
        
        float distance = Vector2.Distance(transform.position, target.Transform.position);
        if (distance > _attacker.AttackRange) return;
        
        Debug.LogWarning($"💥 ATTACKING from {name} (ID: {GetInstanceID()})");
        target.TakeDamage(_attacker.Damage, _attacker.MinDamage, _attacker.MaxDamage);
        
        _attacker.LastAttackTime = Time.time;
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

}
