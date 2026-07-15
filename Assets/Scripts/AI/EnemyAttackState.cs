using UnityEngine.AI;

public class EnemyAttackState : AIState
{
    public override string Name => "Attack";

    private readonly NavMeshAgent _agent;

    public EnemyAttackState(NavMeshAgent agent)
    {
        _agent = agent;
    }

    public override void Enter()
    {
        _agent.isStopped = true;
        _agent.ResetPath();
    }
    
    public override void Exit()
    {
        _agent.isStopped = false; 
    }
}