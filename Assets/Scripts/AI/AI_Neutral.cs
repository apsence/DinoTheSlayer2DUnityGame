using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AI_Neutral : MonoBehaviour
{
    [Header("Перемещение")]
    [SerializeField] private float speed = 2f;

    [Header("Блуждание")]
    [SerializeField] private float wanderRadius = 15f;
    [SerializeField] private float minWanderTime = 3f;
    [SerializeField] private float maxWanderTime = 8f;
    [SerializeField] private float waitTimeAtPoint = 2f;

    [Header("Idle → Feeding")]
    [SerializeField] private float minIdleTimeBeforeFeeding = 4f;
    [SerializeField] private float maxIdleTimeBeforeFeeding = 10f;
    [SerializeField] private float feedingDuration = 5f;

    [Header("Анимирование")]
    [SerializeField] private UnitAnimator _unitAnimator;

    [Header("Графика")]
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [Header("NavMesh")]
    [SerializeField] private NavMeshAgent _agent;

    private enum AIState { Idle, Wander, Feeding }
    private AIState _state = AIState.Idle;

    private Coroutine _stateCoroutine;

    // ─── Инициализация ───────────────────────────────────

    private void Awake()
    {
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _agent.stoppingDistance = 0;
        _agent.speed = speed;
    }

    private void Start()
    {
        EnterIdle();
    }

    // ─── Главный цикл ────────────────────────────────────

    private void Update()
    {
        UpdateSpriteDirection();

        if (_unitAnimator != null)
            _unitAnimator.SetExternalVelocity(_agent.velocity);
    }

    // ─── Переходы состояний ──────────────────────────────

    private void EnterIdle()
    {
        _state = AIState.Idle;
        _agent.isStopped = true;
        _agent.ResetPath();

        _stateCoroutine = StartCoroutine(IdleRoutine());
    }

    private void EnterWander()
    {
        _state = AIState.Wander;
        _agent.isStopped = false;

        _stateCoroutine = StartCoroutine(WanderRoutine());
    }

    private void EnterFeeding()
    {
        _state = AIState.Feeding;
        _agent.isStopped = true;
        _agent.ResetPath();
        _unitAnimator?.Feeding();

        _stateCoroutine = StartCoroutine(FeedingRoutine());
    }

    // ─── Корутины состояний ──────────────────────────────

    // Idle: ждём случайное время, потом уходим в Feeding.
    // После Feeding вернёмся сюда — и уже отсюда пойдём в Wander.
    private bool _feedingDoneFlag = false;

    private IEnumerator IdleRoutine()
    {
        if (_feedingDoneFlag)
        {
            // После кормёжки — сразу в блуждание
            _feedingDoneFlag = false;
            SwitchCoroutine(EnterWander);
            yield break;
        }

        float idleWait = Random.Range(minIdleTimeBeforeFeeding, maxIdleTimeBeforeFeeding);
        yield return new WaitForSeconds(idleWait);

        SwitchCoroutine(EnterFeeding);
    }

    private IEnumerator WanderRoutine()
    {
        float wanderTime = Random.Range(minWanderTime, maxWanderTime);
        Vector3 destination = GetRandomNavMeshPoint(transform.position, wanderRadius);
        _agent.SetDestination(destination);

        // Ждём либо прибытия, либо истечения времени
        float timer = 0f;
        yield return new WaitUntil(() =>
        {
            timer += Time.deltaTime;
            return timer >= wanderTime ||
                   (!_agent.pathPending && _agent.remainingDistance < 0.3f);
        });

        // Постояли на месте
        _agent.isStopped = true;
        yield return new WaitForSeconds(waitTimeAtPoint);

        SwitchCoroutine(EnterIdle);
    }

    private IEnumerator FeedingRoutine()
    {
        yield return new WaitForSeconds(feedingDuration);

        _unitAnimator?.StopFeeding();

        _feedingDoneFlag = true;
        SwitchCoroutine(EnterIdle);
    }

    // ─── Вспомогательные ─────────────────────────────────

    // Останавливает текущий корутин и запускает новый через метод перехода
    private void SwitchCoroutine(System.Action enterMethod)
    {
        if (_stateCoroutine != null)
            StopCoroutine(_stateCoroutine);

        enterMethod();
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

    private void UpdateSpriteDirection()
    {
        if (_agent.velocity.sqrMagnitude < 0.01f) return;
        _spriteRenderer.flipX = _agent.velocity.x < 0;
    }

    // ─── Gizmos ──────────────────────────────────────────

    private void OnDrawGizmosSelected()
    {
        Draw2DCircle(transform.position, wanderRadius, Color.green);
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