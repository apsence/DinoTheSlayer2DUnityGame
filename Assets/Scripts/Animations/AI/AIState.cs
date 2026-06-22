public abstract class AIState
{
    public abstract string Name { get; }
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
}