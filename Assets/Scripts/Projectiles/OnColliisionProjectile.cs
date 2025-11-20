using UnityEngine;

public class OnCollisionProjectile : MonoBehaviour
{
    public int damage;
    public GameObject owner;
    public GameObject player;
    public float maxDistance = 3000f;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");

        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance > maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // игнорируем владельца
        if (other.gameObject == owner) return;

        // игнорируем союзников
        if (owner.CompareTag("Enemy") && other.CompareTag("Enemy")) return;

        UnitStats stats = other.GetComponent<UnitStats>();
        if (stats != null)
        {
            stats.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
