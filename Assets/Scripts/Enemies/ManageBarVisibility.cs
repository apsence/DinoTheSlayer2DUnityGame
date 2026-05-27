using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ManageBarVisibility : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image healthBar;
    [SerializeField] private float visibleDuration = 3f;

    private Coroutine hideCoroutine;
    private Health _health;


    private void Awake()
    {
        _health = GetComponent<Health>();
        canvas.enabled = false;
    }

    public void ShowBar()
    {
        canvas.enabled = true;

        RefreshHealthBar();

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        hideCoroutine = StartCoroutine(HideBarAfterTime());
    }

    private IEnumerator HideBarAfterTime()
    {
        yield return new WaitForSeconds(visibleDuration);

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