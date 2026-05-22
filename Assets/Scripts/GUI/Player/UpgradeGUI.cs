using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeGUI : MonoBehaviour
{
    [SerializeField] private List<Transform> merchants;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float distanceToShowGUI;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.3f;
    private bool needToShow;
    public bool NeedToShow => needToShow;
    
    private Coroutine currentFadeCoroutine;
    private bool isGUIVisible;

    void Update()
    {
        needToShow = IsThereNeedToShowGUI();
        
        if (needToShow && !isGUIVisible)
        {
            StartFade(1f);
            isGUIVisible = true;
        }
        else if (!needToShow && isGUIVisible)
        {
            StartFade(0f);
            isGUIVisible = false;
        }
    }

    private void StartFade(float targetAlpha)
    {
        // останавливаем текущую анимацию, если она идёт
        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);
        
        currentFadeCoroutine = StartCoroutine(FadeCanvas(targetAlpha));
    }

    private IEnumerator FadeCanvas(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            
            // используем обычный Lerp без SmoothStep для простоты
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }
        
        // фиксируем конечное значение
        canvasGroup.alpha = targetAlpha;
        
        // блокируем взаимодействие только когда полностью скрыто
        if (targetAlpha == 0f)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else if (targetAlpha == 1f)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        
        currentFadeCoroutine = null;
    }

    bool IsThereNeedToShowGUI()
    {
        foreach (Transform gm in merchants)
        {
            float sqrDistance = (gm.position - playerTransform.position).sqrMagnitude;
            
            if (sqrDistance < distanceToShowGUI * distanceToShowGUI)
                return true;
        }
        
        return false;
    }

    void OnDrawGizmosSelected()
    {
        foreach (var m in merchants)
            Gizmos.DrawWireSphere(m.position, distanceToShowGUI);
    }
}