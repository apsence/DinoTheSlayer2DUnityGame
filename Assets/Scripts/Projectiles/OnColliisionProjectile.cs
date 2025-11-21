using UnityEngine;

public class OnCollisionProjectile : MonoBehaviour
{
    public int damage;
    public string ownerTag;
    public GameObject player;
    public float maxDistance = 3000f;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        DestroyOutOfBorderProjectile();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(ownerTag)) return;

        if (ownerTag == "Enemy" && other.CompareTag("Enemy")) return;
        if (ownerTag == "Player" && other.CompareTag("Player")) return;

        UnitStats stats = other.GetComponent<UnitStats>();
        if (stats != null)
        {
            stats.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    void DestroyOutOfBorderProjectile()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance > maxDistance)
        {
            Destroy(gameObject);
        }
    }
}
