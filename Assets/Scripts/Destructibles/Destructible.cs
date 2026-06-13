using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] private CreaterOfRewards rewards;

    public void OnDestroyAnimationFinished()
    {
        rewards.CreateReward(transform.position);
        Destroy(gameObject);
    }
}
