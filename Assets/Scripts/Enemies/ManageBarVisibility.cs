using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ManageBarVisibility : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image healthBar;
    [SerializeField] private float visibleDuration = 3f;
    [SerializeField] private float distanceToShowBar;
    private float currentVisibleTime = 0f;

    private Coroutine hideCoroutine;
    private Health _health;
    private Transform _playerTransform;


    private void Awake()
    {
        _health = GetComponent<Health>();
        canvas.enabled = false;
    }

    void Start()
    {
        _playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    void Update()
    {
        Vector2 diff = (_playerTransform.position - transform.position);
        float sqrDistance = diff.sqrMagnitude;

        if(sqrDistance < distanceToShowBar * distanceToShowBar)
        {
            ShowBar();
        }
    }

    public void ShowBar()
    {
        canvas.enabled = true;
        RefreshHealthBar();

        // Если юнит уже мертв — сразу прячем и выходим
        if (_health != null && !_health.IsAlive)
        {
            canvas.enabled = false;
            return;
        }

        // Перезапускаем таймер
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        currentVisibleTime = 0f;
        hideCoroutine = StartCoroutine(HideBarAfterTime());
    }

    private IEnumerator HideBarAfterTime()
    {
        while (currentVisibleTime < visibleDuration)
        {
            // Проверяем жив ли игрок/юнит на каждом кадре
            if (_health != null && !_health.IsAlive)
            {
                canvas.enabled = false;
                hideCoroutine = null;
                yield break;
            }

            currentVisibleTime += Time.deltaTime;
            yield return null;  // ждём следующий кадр
        }

        // Таймер истёк — скрываем
        canvas.enabled = false;
        hideCoroutine = null;
    }

    public void RefreshHealthBar()
    {
        if(_health){
            if (healthBar != null)
            {
                float ratio = (float)_health.CurrentHealth / _health.MaxHealth;
                healthBar.fillAmount = ratio;
            }
        }
    }
}