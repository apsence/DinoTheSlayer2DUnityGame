using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyWanderState : AIState
{
    public override string Name => "Wander";

    private readonly AI_Common _ai;
    private readonly NavMeshAgent _agent;
    private readonly MonoBehaviour _host; // для StartCoroutine
    private readonly Vector3 _defaultPosition;
    private readonly float _wanderRadius;
    private readonly float _minWanderTime;
    private readonly float _maxWanderTime;
    private readonly float _waitTimeAtPoint;

    private Coroutine _coroutine;

    public EnemyWanderState(AI_Common ai, NavMeshAgent agent, MonoBehaviour host,
        Vector3 defaultPos, float wanderRadius,
        float waitTimeAtPoint)
    {
        _ai = ai;
        _agent = agent;
        _host = host;
        _defaultPosition = defaultPos;
        _wanderRadius = wanderRadius;
        _waitTimeAtPoint = waitTimeAtPoint;
    }

    public override void Enter()
    {
        _agent.isStopped = false;
        _coroutine = _host.StartCoroutine(WanderRoutine());
    }

    public override void Exit()
    {
        if (_coroutine != null)
            _host.StopCoroutine(_coroutine);
    }

    private IEnumerator WanderRoutine()
    {
        while (true)
        {
            Vector3 point = GetRandomNavMeshPoint(_defaultPosition, _wanderRadius);
            _agent.isStopped = false;
            _agent.SetDestination(point);

            yield return null; // ← ждём один кадр чтобы агент начал считать путь

            yield return new WaitUntil(() =>
                !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance + 0.1f);

            _agent.isStopped = true;
            yield return new WaitForSeconds(_waitTimeAtPoint);
            _agent.isStopped = false;
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
}