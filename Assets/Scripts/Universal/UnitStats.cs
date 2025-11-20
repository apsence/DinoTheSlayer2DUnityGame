using UnityEngine;

public class UnitStats : MonoBehaviour
{
    public int damage;
    public int health;

    public void TakeDamage(int amount)
    {
        int finalDamage = amount + Random.Range(1, 3);
        health -= finalDamage;

        Debug.Log($"{gameObject.name} получил {finalDamage} урона. Осталось здоровья: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} погиб");

        LevelManager manager = FindFirstObjectByType<LevelManager>();
        if (manager != null)
        {
            manager.OnEnemyKilled();
        }

        Destroy(gameObject);
    }
}
