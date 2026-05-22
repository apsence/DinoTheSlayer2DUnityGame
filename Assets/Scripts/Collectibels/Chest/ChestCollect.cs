using System.Collections;
using UnityEngine;

public class ChestCollect : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float collectRange = 3f;

    [Header("Loot")]
    [SerializeField] private CreaterOfRewards createrOfRewards;
    [SerializeField] private float secondsBeforeSpawnLoot = 1f;

    private Animator _animator;
    private bool isCollected;

    void Start()
    {
        _animator = GetComponent<Animator>();

        playerTransform = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<Transform>();
    }

    void Update()
    {
        if (isCollected)
            return;

        float distance = Vector2.Distance(
            playerTransform.position,
            transform.position);

        if (distance < collectRange)
        {
            Collect();
        }
    }

    void Collect()
    {
        if (isCollected)
            return;

        isCollected = true;

        _animator.SetTrigger("Collect");

        StartCoroutine(SpawnLootRoutine());
    }

    IEnumerator SpawnLootRoutine()
    {
        yield return new WaitForSeconds(secondsBeforeSpawnLoot);

        createrOfRewards.CreateReward(transform.position);
    }
}