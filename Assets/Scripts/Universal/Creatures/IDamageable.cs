using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int amount, int _minDamage, int _maxDamage);
    bool IsAlive { get; }
    Transform Transform { get; }
}
