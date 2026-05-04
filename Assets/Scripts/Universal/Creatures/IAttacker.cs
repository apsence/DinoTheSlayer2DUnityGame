public interface IAttacker
{
    int Damage { get; }
    void PerformAttack(IDamageable target);
}
