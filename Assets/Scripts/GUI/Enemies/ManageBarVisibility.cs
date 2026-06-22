using System.Collections;
using UnityEngine;

public class ManageBarVisibility : MonoBehaviour
{
    [SerializeField] private float visibleDuration = 3f;
    [SerializeField] private float distanceToShowBar;

    private Coroutine _hideCoroutine;
    private Health _health;
    private Transform _playerTransform;
    private HPBarFollower _hpBarFollower;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _hpBarFollower = GetComponent<HPBarFollower>();
        _hpBarFollower.HideBar();
    }

    private void Start()
    {
        _playerTransform = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        float sqrDistance = (_playerTransform.position - transform.position).sqrMagnitude;
        if (sqrDistance < distanceToShowBar * distanceToShowBar)
            ShowBar();
    }

    public void ShowBar()
    {
        if (_health != null && !_health.IsAlive) return;

        _hpBarFollower.ShowBar();

        if (_hideCoroutine != null)
            StopCoroutine(_hideCoroutine);

        _hideCoroutine = StartCoroutine(HideBarAfterTime());
    }

    private IEnumerator HideBarAfterTime()
    {
        float timer = 0f;

        while (timer < visibleDuration)
        {
            if (_health != null && !_health.IsAlive)
            {
                _hpBarFollower.HideBar();
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        _hpBarFollower.HideBar();
        _hideCoroutine = null;
    }
}