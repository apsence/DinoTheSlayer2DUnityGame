using UnityEngine;

public class ChestCollect : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float collectRange = 3f;
    [SerializeField] private CreateCoin _createCoin;
    [SerializeField] private int coinsInside;
    [SerializeField] private float spreadRadius;
    [SerializeField] private float yOffSet;
    private Animator _animator;
    private bool isCollected;

    void Start()
    {
        _animator = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }
    void Update()
    {
        if(isCollected) return;
        float distance = Vector2.Distance(playerTransform.position, transform.position);
        if(distance < collectRange)
        {
            Collect();
        }
    }

    void Collect()
    {
        if(!isCollected){
            _animator.SetTrigger("Collect");
            isCollected = true;
            _createCoin.CreateMultipleCoins(transform.position, coinsInside, yOffSet, spreadRadius);
        }
    }
}
