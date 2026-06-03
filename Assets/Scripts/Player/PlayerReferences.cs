using UnityEngine;

public class PlayerReferences : MonoBehaviour
{
    [SerializeField] private Coin coin;
    [SerializeField] private Health health;

    public Coin Coin => coin;
    public Health Health => health;
}
