using System;

public class AIStateMachine
{
    public AIState Current { get; private set; }
    public event Action<AIState, AIState> OnStateChanged; // (from, to)

    public void SetState(AIState newState)
    {
        if (Current == newState) return;

        AIState prev = Current;
        Current?.Exit();
        Current = newState;
        Current.Enter();

        OnStateChanged?.Invoke(prev, Current);
    }

    public void Update()
    {
        Current?.Update();
    }
}