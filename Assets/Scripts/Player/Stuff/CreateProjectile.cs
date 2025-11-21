using UnityEngine;

public class CreatepProjectile : MonoBehaviour
{
    public GameObject prefab;
    public float projectileSpeedBoost;
    public float projectileSpeedMultiplier;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - transform.position).normalized;

            float spawnOffset = 200f; // расстояние от центра игрока
            Vector2 spawnPos = (Vector2)transform.position + direction * spawnOffset;

            GameObject projectile = Instantiate(prefab, spawnPos, Quaternion.identity);
            OnCollisionProjectile proj = projectile.GetComponent<OnCollisionProjectile>();
            if (proj != null)
            {
                // берём урон у игрока (родителя, где висит UnitStats)
                UnitStats stats = GetComponentInParent<UnitStats>();
                if (stats != null)
                {
                    proj.damage = stats.damage;
                    proj.ownerTag = gameObject.tag;
                }
            }


            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            rb.linearVelocity = direction * projectileSpeedBoost * projectileSpeedMultiplier;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.AngleAxis(angle + 90f, Vector3.forward);

        }

    }
}
