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

    private enum AIState { Wander, Chase, Attack }
    private AIState _state = AIState.Wander;

    private NavMeshAgent _agent;
    private SpriteRenderer _spriteRenderer;
    private Attacker _attacker;
    private Transform _playerTransform;
    private IDamageable _playerDamageable;
    private Vector3 _defaultPosition;
    private Coroutine _wanderCoroutine;
    private bool _isWaiting;

    public bool HasTarget => _playerDamageable != null && _playerDamageable.IsAlive;

    // ─── Инициализация ───────────────────────────────────

    void Awake()
    {
        var player = GameObject.FindWithTag("Player");
        _playerTransform = player.transform;
        _playerDamageable = player.GetComponent<IDamageable>();

        _defaultPosition = transform.position;
        _agent = GetComponent<NavMeshAgent>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _attacker = GetComponent<Attacker>();

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _agent.stoppingDistance = 0;
        _agent.speed = speed;
    }

    void Start()
    {
        _wanderCoroutine = StartCoroutine(WanderRoutine());
    }

    // ─── Главный цикл ────────────────────────────────────

    void Update()
    {
        UpdateState();
        HandleAttack();
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
                if (dist > attackRange * 1.15f)
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

    private void HandleAttack()
    {
        if (_state != AIState.Attack) return;

        if (!HasTarget)
        {
            _unitAnimator?.SetAttacking(false);
            EnterWander();
            return;
        }

        if (Time.time >= _attacker.NextAttackTime)
        {
            _attacker.NextAttackTime = Time.time + _attacker.AttackColdown;
            PerformAttack(_playerDamageable);
        }
    }

    public void PerformAttack(IDamageable target)
    {
        if (target == null || !target.IsAlive) return;

        float distance = Vector2.Distance(transform.position, target.Transform.position);
        if (distance > attackRange) return;

        target.TakeDamage(_attacker.Damage, _attacker.MinDamage, _attacker.MaxDamage);
        _attacker.LastAttackTime = Time.time;
    }

    public void Attack() => PerformAttack(_playerDamageable);

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