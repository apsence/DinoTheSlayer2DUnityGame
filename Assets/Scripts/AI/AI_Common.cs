using UnityEngine;
using UnityEngine.AI;

public class AI_Common : MonoBehaviour
{
    [Header("Агрессия")]
    [SerializeField] private float agroRange = 8f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float exitAttackRange = 12f;

    [Header("Перемещение")]
    [SerializeField] private float speed = 3f;

    [Header("Патрулирование")]
    [SerializeField] private float wanderRadius = 4f;
    [SerializeField] private float waitTimeAtPoint = 2f;

    [Header("Анимирование")]
    [SerializeField] private UnitAnimator _unitAnimator;

    [Header("Атака ИИ")]
    [SerializeField] private float attackSpeed;

    [Header("Графика")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("NavMesh")]
    [SerializeField ]private NavMeshAgent _agent;

    private Transform _playerTransform;
    private Health _playerHealth;
    private Attacker _attacker;
    private Health _health;

    private AIStateMachine _sm;

    // Состояния
    private EnemyWanderState _stateWander;
    private EnemyChaseState _stateChase;
    private EnemyAttackState _stateAttack;

    private Vector2 _smoothedVelocity;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _playerTransform = player.transform;
        _playerHealth = player.GetComponent<PlayerReferences>().Health;

        _attacker = GetComponent<Attacker>();
        _health = GetComponent<Health>();

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _agent.stoppingDistance = attackRange * 0.9f;;
        _agent.speed = speed;

        animator.SetFloat("AttackSpeed", attackSpeed);

        // Создаём состояния
        _stateWander = new EnemyWanderState(this, _agent, this,
            _agent.transform.position, wanderRadius, waitTimeAtPoint);
        _stateChase  = new EnemyChaseState(_agent, _playerTransform);
        _stateAttack = new EnemyAttackState(_agent);

        // Машина состояний
        _sm = new AIStateMachine();
        _sm.OnStateChanged += _unitAnimator.OnStateChanged;
        _sm.SetState(_stateWander);
    }

    private void Update()
    {
        if (!_health.IsAlive) return;

        _sm.Update();
        CheckTransitions();
        UpdateSpriteDirection();

        if (_unitAnimator != null)
            _unitAnimator.SetExternalVelocity(_agent.velocity);
    }

    private void CheckTransitions()
    {
        float dist = Vector2.Distance(_playerTransform.position, transform.position);
        var cur = _sm.Current;

        if (cur == _stateWander)
        {
            if (dist < agroRange) _sm.SetState(_stateChase);
        }
        else if (cur == _stateChase)
        {
            if (dist > exitAttackRange)   _sm.SetState(_stateWander);
            else if (dist <= attackRange) _sm.SetState(_stateAttack);
            // иначе Chase.Update() сам двигает агента — ничего не делаем
        }
        else if (cur == _stateAttack)
        {
            // добавляем небольшой буфер чтобы не дёргаться на границе
            float exitBuffer = 0.3f;
            if (dist > attackRange + exitBuffer)
                _sm.SetState(dist > exitAttackRange ? _stateWander : _stateChase);
        }
    }

    // Вызывается из Animation Event
    public void HandleAttack()
    {
        if (_sm.Current != _stateAttack || !_playerHealth.IsAlive) return;
        float sqDist = (_playerTransform.position - transform.position).sqrMagnitude;
        if (sqDist <= attackRange * attackRange)
            _playerHealth.TakeDamage(_attacker.Damage, _attacker.MinDamage, _attacker.MaxDamage);
    }

    private void UpdateSpriteDirection()
    {
        // Сглаживаем velocity чтобы убрать дёрганье
        _smoothedVelocity = Vector2.Lerp(_smoothedVelocity, _agent.velocity, Time.deltaTime * 10f);
        if (_smoothedVelocity.sqrMagnitude < 0.05f) return; // стоим — не флипаем
        _spriteRenderer.flipX = _smoothedVelocity.x < 0;
    }

    void OnDrawGizmosSelected()
    {
        Draw2DCircle(transform.position, agroRange, Color.yellow);
        Draw2DCircle(transform.position, attackRange, Color.red);
        Draw2DCircle(transform.position, exitAttackRange, Color.cyan);
    }

    private void Draw2DCircle(Vector3 center, float radius, Color color)
    {
        Gizmos.color = color;
        int segments = 32;
        Vector3 prev = center + new Vector3(radius, 0, 0);
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector3 next = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
}