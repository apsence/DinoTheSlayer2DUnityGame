using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Плавно показывает/скрывает UI-элементы (рамку, подложку и т.д.) через CanvasGroup
/// при наведении курсора или выборе клавиатурой/геймпадом.
/// </summary>
public class ButtonHoverVisual : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Настройки анимации")]
    [SerializeField] private CanvasGroup[] hoverVisuals;
    [SerializeField] private float fadeDuration = 0.15f;
    [SerializeField] private bool reactToKeyboardSelection = true;

    [Header("Звуки (опционально)")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip selectSound;

    private Coroutine fadeCoroutine;
    private bool isHovered = false;
    private bool isSelected = false;

    private void Awake()
    {
        // Мгновенно скрываем все элементы (без анимации)
        SetAlphaImmediate(0f);
    }

    private void OnDisable()
    {
        // Сбрасываем состояния, чтобы при повторном включении не было "залипания"
        isHovered = false;
        isSelected = false;
        SetAlphaImmediate(0f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        EvaluateState(playAudio: true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        EvaluateState(playAudio: false); // при уходе мыши звук не играем
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (reactToKeyboardSelection)
        {
            isSelected = true;
            EvaluateState(playAudio: true);
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (reactToKeyboardSelection)
        {
            isSelected = false;
            EvaluateState(playAudio: false);
        }
    }

    /// <summary>
    /// Определяет, должен ли объект быть видимым, и запускает анимацию.
    /// </summary>
    /// <param name="playAudio">Воспроизводить ли звук при появлении</param>
    private void EvaluateState(bool playAudio)
    {
        bool shouldBeVisible = isHovered || (reactToKeyboardSelection && isSelected);
        StartFade(shouldBeVisible ? 1f : 0f);

        // Воспроизводим звук только если элемент становится видимым и запрошено воспроизведение
        if (shouldBeVisible && playAudio)
        {
            // Выбираем, какой звук играть: приоритет у hoverSound, если наведены, иначе selectSound
            AudioClip clipToPlay = null;
            if (isHovered && hoverSound != null)
                clipToPlay = hoverSound;
            else if (isSelected && selectSound != null)
                clipToPlay = selectSound;

            if (clipToPlay != null && SFXManager.Instance != null)
            {
                SFXManager.Instance.PlaySound(clipToPlay);
            }
        }
    }

    private void StartFade(float targetAlpha)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        if (hoverVisuals == null || hoverVisuals.Length == 0)
            yield break;

        // Определяем начальную альфу по первому не-null элементу
        float startAlpha = 0f;
        foreach (var cg in hoverVisuals)
        {
            if (cg != null)
            {
                startAlpha = cg.alpha;
                break;
            }
        }

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            SetAlphas(currentAlpha);
            yield return null;
        }

        SetAlphas(targetAlpha);
        fadeCoroutine = null;
    }

    private void SetAlphas(float alpha)
    {
        foreach (var cg in hoverVisuals)
        {
            if (cg != null)
            {
                cg.alpha = alpha;
                // Если элемент почти невидим, отключаем raycast, чтобы не мешать кликам
                cg.blocksRaycasts = alpha > 0.01f;
            }
        }
    }

    private void SetAlphaImmediate(float alpha)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        foreach (var cg in hoverVisuals)
        {
            if (cg != null)
            {
                cg.alpha = alpha;
                cg.blocksRaycasts = alpha > 0.01f;
            }
        }
    }
}