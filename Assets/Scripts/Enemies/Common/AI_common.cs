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
    private Transform _playerTransform;
    private bool _isAttacking;
    private bool _isMovingToWanderPoint;
    private bool _isWaiting; 
    private Vector2 _defaultPosition;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rb;
    private Attacker _attacker;
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
        StartCoroutine(WanderRoutine());
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
                _attacker.Attack();
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
        StopAllCoroutines(); // останавливаем все корутины
        
        // Мгновенно останавливаем физику
        _rb.linearVelocity = Vector2.zero;
    }

    float DistanceToPlayer()
    {
        float distance = Vector2.Distance( _playerTransform.position, transform.position);
        //float positiveDistance = Math.Abs(distance);
        return distance;
    }

    Vector2 DirectionToDefaultPosition()
    {
        return ((Vector2)transform.position - _defaultPosition).normalized;
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

}
