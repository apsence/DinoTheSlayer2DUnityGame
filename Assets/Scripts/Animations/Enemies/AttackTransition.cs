using UnityEngine;

public class AttackTransition : MonoBehaviour
{
    [SerializeField] private AI_Common ai_common;

    public void HandleAttack()
    {
        ai_common.HandleAttack();
    }
}
