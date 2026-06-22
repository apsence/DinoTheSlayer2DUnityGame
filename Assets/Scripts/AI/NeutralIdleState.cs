using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NeutralIdleState : AIState
{
    public override string Name => "Idle";

    private readonly AIStateMachine _sm;
    private readonly NavMeshAgent _agent;
    private readonly MonoBehaviour _host;
    private readonly float _minIdleTime;
    private readonly float _maxIdleTime;
    private AIState _nextState; // Feeding или Wander
    private Coroutine _coroutine;

    // nextState задаётся снаружи чтобы избежать циклических зависимостей
    public void SetNextState(AIState next) => _nextState = next;

    public NeutralIdleState(AIStateMachine sm, NavMeshAgent agent, MonoBehaviour host,
        float minIdleTime, float maxIdleTime)
    {
        _sm = sm;
        _agent = agent;
        _host = host;
        _minIdleTime = minIdleTime;
        _maxIdleTime = maxIdleTime;
    }

    public override void Enter()
    {
        _agent.isStopped = true;
        _coroutine = _host.StartCoroutine(IdleRoutine());
    }

    public override void Exit()
    {
        if (_coroutine != null)
            _host.StopCoroutine(_coroutine);
    }

    private IEnumerator IdleRoutine()
    {
        yield return new WaitForSeconds(Random.Range(_minIdleTime, _maxIdleTime));
        if (_nextState != null)
            _sm.SetState(_nextState);
    }
}