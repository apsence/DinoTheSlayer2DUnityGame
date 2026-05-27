using UnityEngine;

public class ChangeStateFromAttack : MonoBehaviour
{
    [SerializeField] private PlayerAttack _playerAttack;

    public void LinkWithAttack()
    {
        _playerAttack.Attack();
    }

}
