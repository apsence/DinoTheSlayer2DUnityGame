using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private PlayerBinds playerBinds;
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
            if(Input.GetKey(playerBinds.attack)){
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

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;
            
            hit.GetComponentInChildren<Health>()?.TakeDamage(_attacker.Damage, _attacker.MinDamage, _attacker.MaxDamage);
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
