using UnityEngine;

// Базовый интерфейс для всех анимируемых юнитов
public interface IAnimatable
{
    void UpdateMovementAnimation(Vector2 velocity);
}

// Опциональные интерфейсы
public interface IAttackAnimatable
{
    void StartAttack();
    void EndAttack();
}

public interface IDeathAnimatable
{
    void Die();
    bool IsDead { get; }
}

public interface IDamageAnimatable
{
    void TakeHit();
}

// Комбинированный интерфейс (если нужны все возможности)
public interface IFullAnimatable : IAnimatable, IAttackAnimatable, IDeathAnimatable, IDamageAnimatable
{
}