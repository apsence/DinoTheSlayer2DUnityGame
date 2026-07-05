using UnityEngine;
using System.Collections;

public class InventoryVisibility : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    private bool _isVisible = false;
    private bool _isAnimating = false;

    public bool IsVisible => _isVisible;
    public bool IsAnimating => _isAnimating;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        // Начальное состояние – скрыто
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        _isVisible = false;
    }

    public void Show()
    {
        if (_isVisible || _isAnimating) return;
        StartCoroutine(ShowRoutine());
    }

    public void Hide()
    {
        if (!_isVisible || _isAnimating) return;
        StartCoroutine(HideRoutine());
    }

    public void ShowInstant()
    {
        StopAllCoroutines();
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        _isVisible = true;
        _isAnimating = false;
    }

    public void HideInstant()
    {
        StopAllCoroutines();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        _isVisible = false;
        _isAnimating = false;
    }

    private IEnumerator ShowRoutine()
    {
        _isAnimating = true;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        _isVisible = true;
        _isAnimating = false;
    }

    private IEnumerator HideRoutine()
    {
        _isAnimating = true;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        _isVisible = false;
        _isAnimating = false;
    }
}