using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask enemyLayer;
    private Attacker _attacker;
    private PlayerAnimator _playerAnimator;
    private bool _onCooldawn;

    void Awake()
    {
        _attacker = GetComponent<Attacker>();
        _playerAnimator = GetComponent<PlayerAnimator>();
    }

    void Update()
    {
        if (!_onCooldawn){
            if(Input.GetKey(KeyCode.Mouse0)){
                PrepareAttack();
                _onCooldawn = true;
                StartCoroutine(WaitForCooldawn());
            }
        }
    }

    public void PrepareAttack()
    {
        _playerAnimator.ChangeState(PlayerState.Attack);
    }

    public void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        foreach (Collider2D enemy in hits)
        {
            enemy.GetComponent<Health>()?.TakeDamage(_attacker.Damage, _attacker.MinDamage, _attacker.MaxDamage);
            Debug.Log(enemy.name + ": " + enemy.GetComponent<Health>().CurrentHealth);
        }
        _playerAnimator.ChangeState(PlayerState.Idle);
    }

    private IEnumerator WaitForCooldawn()
    {
        yield return new WaitForSeconds(_attacker.AttackColdown);
        _onCooldawn = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRange
        );
    }
}
