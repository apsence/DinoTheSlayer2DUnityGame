using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseState : AIState
{
    public override string Name => "Chase";

    private readonly NavMeshAgent _agent;
    private readonly Transform _target;
    private readonly float _updatePathInterval = 0.15f; // пересчёт пути не каждый кадр
    private float _timer;

    public EnemyChaseState(NavMeshAgent agent, Transform target)
    {
        _agent = agent;
        _target = target;
    }

    public override void Enter()
    {
        _agent.isStopped = false;
        _timer = 0f;
        _agent.SetDestination(_target.position);
    }

    public override void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < _updatePathInterval) return;
        _timer = 0f;
        _agent.SetDestination(_target.position);
    }

    public override void Exit()
    {
        _agent.ResetPath();
    }
}