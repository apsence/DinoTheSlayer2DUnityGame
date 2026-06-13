using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
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
    [SerializeField] private float minWanderTime = 3f;
    [SerializeField] private float maxWanderTime = 6f;

    [Header("Анимирование")]
    [SerializeField] private UnitAnimator _unitAnimator;

    [Header("Атака ИИ")]
    [SerializeField] private float attackSpeed;
    
    private Transform _playerTransform;
    private Health _playerHealth;

    private enum AIState { Wander, Chase, Attack }
    private AIState _state = AIState.Wander;

    private NavMeshAgent _agent;
    private SpriteRenderer _spriteRenderer;
    private Attacker _attacker;
    private Vector3 _defaultPosition;
    private Coroutine _wanderCoroutine;
    private bool _isWaiting;

    private Health _health;

    public bool HasTarget => _playerTransform != null && _playerHealth.IsAlive;

    // ─── Инициализация ───────────────────────────────────

    void Awake()
    {
        GameObject _player = GameObject.FindGameObjectWithTag("Player");
        _playerTransform = _player.GetComponent<Transform>();
        _playerHealth = _player.GetComponent<PlayerReferences>().Health;

        _defaultPosition = transform.position;
        _agent = GetComponent<NavMeshAgent>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _attacker = GetComponent<Attacker>();
        _health = GetComponent<Health>();

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _agent.stoppingDistance = 0;
        _agent.speed = speed;

        GetComponent<Animator>().SetFloat("AttackSpeed", attackSpeed);
    }

    void Start()
    {
        _wanderCoroutine = StartCoroutine(WanderRoutine());
    }

    // ─── Главный цикл ────────────────────────────────────

    void Update()
    {
        if(!_health.IsAlive) return;
        UpdateState();
        //HandleAttack();
        UpdateSpriteDirection();

        if (_unitAnimator != null)
            _unitAnimator.SetExternalVelocity(_agent.velocity);
    }

    // ─── Машина состояний ────────────────────────────────

    private void UpdateState()
    {
        float dist = DistanceToPlayer();

        switch (_state)
        {
            case AIState.Wander:
                if (dist < agroRange)
                    EnterChase();
                break;

            case AIState.Chase:
                if (dist > exitAttackRange)
                    EnterWander();
                else if (dist <= attackRange)
                    EnterAttack();
                else
                {
                    _agent.isStopped = false;
                    _agent.SetDestination(_playerTransform.position);
                }
                break;

            case AIState.Attack:
                if (dist > attackRange)
                {
                    if (dist > exitAttackRange)
                        EnterWander();
                    else
                        EnterChase();
                }
                break;
        }
    }

    private void EnterChase()
    {
        _state = AIState.Chase;
        StopWander();
        _unitAnimator?.SetAttacking(false);
        _agent.isStopped = false;
        _agent.SetDestination(_playerTransform.position);
    }

    private void EnterAttack()
    {
        _state = AIState.Attack;
        _agent.isStopped = true;
        _agent.ResetPath();
        _unitAnimator?.SetAttacking(true);
    }

    private void EnterWander()
    {
        if (_state == AIState.Wander) return;
        _state = AIState.Wander;
        _unitAnimator?.SetAttacking(false);
        _agent.isStopped = false;
        _wanderCoroutine = StartCoroutine(WanderRoutine());
    }

    // ─── Атака ───────────────────────────────────────────

    public void HandleAttack()
    {
        if (_state != AIState.Attack) return;

        if (!HasTarget)
        {
            _unitAnimator?.SetAttacking(false);
            EnterWander();
            return;
        }
            PerformAttack();
    }

    public void PerformAttack()
    {
        if(_playerHealth.IsAlive != true) return;

        float distance = (_playerTransform.position - transform.position).sqrMagnitude;
        if (distance > attackRange * attackRange) 
        {
            //Debug.Log($"Дистанция больше атак ренжа: дистанция: {distance}, атак ренж: {attackRange}");
            return;
        }

        _playerHealth.TakeDamage(_attacker.Damage, _attacker.MinDamage, _attacker.MaxDamage);
        //_attacker.LastAttackTime = Time.time;
    }

    // ─── Патрулирование ──────────────────────────────────

    private IEnumerator WanderRoutine()
    {
        while (_state == AIState.Wander)
        {
            float waitTime = Random.Range(minWanderTime, maxWanderTime);
            yield return new WaitForSeconds(waitTime);

            if (_state != AIState.Wander || _isWaiting) continue;

            Vector3 wanderPoint = GetRandomNavMeshPoint(_defaultPosition, wanderRadius);
            _agent.isStopped = false;
            _agent.SetDestination(wanderPoint);

            yield return new WaitUntil(() =>
                _state != AIState.Wander ||
                (!_agent.pathPending && _agent.remainingDistance < 0.3f));

            if (_state != AIState.Wander) yield break;

            _agent.isStopped = true;
            _isWaiting = true;
            yield return new WaitForSeconds(waitTimeAtPoint);
            _isWaiting = false;
            _agent.isStopped = false;
        }
    }

    private void StopWander()
    {
        _isWaiting = false;
        if (_wanderCoroutine != null)
        {
            StopCoroutine(_wanderCoroutine);
            _wanderCoroutine = null;
        }
    }

    private Vector3 GetRandomNavMeshPoint(Vector3 origin, float radius)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 rand = Random.insideUnitCircle * radius;
            Vector3 candidate = origin + new Vector3(rand.x, rand.y, 0f);
            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, radius, NavMesh.AllAreas))
                return hit.position;
        }
        return origin;
    }

    // ─── Вспомогательные ─────────────────────────────────

    private float DistanceToPlayer() =>
        Vector2.Distance(_playerTransform.position, transform.position);

    private void UpdateSpriteDirection()
    {
        if (_agent.velocity.sqrMagnitude < 0.01f) return;
        _spriteRenderer.flipX = _agent.velocity.x < 0;
    }

    // ─── Gizmos ──────────────────────────────────────────

    void OnDrawGizmosSelected()
    {
        Draw2DCircle(transform.position, agroRange, Color.yellow);
        Draw2DCircle(transform.position, attackRange, Color.red);
        Draw2DCircle(transform.position, exitAttackRange, Color.cyan);
        Vector3 def = Application.isPlaying ? _defaultPosition : transform.position;
        Draw2DCircle(def, wanderRadius, Color.green);
    }

    private void Draw2DCircle(Vector3 center, float radius, Color color)
    {
        Gizmos.color = color;
        int segments = 32;
        Vector3 prev = center + new Vector3(radius, 0, 0);
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector3 next = center + new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius, 0);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
}