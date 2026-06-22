using UnityEngine;

public class MinimapVisibility : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private MinimapSettings minimapSettings;

    private float targetAlpha = 1f;

    private void Update()
    {
        canvasGroup.alpha = Mathf.MoveTowards(
            canvasGroup.alpha,
            targetAlpha,
            minimapSettings.fadeSpeed * Time.unscaledDeltaTime
        );

        if (Mathf.Abs(canvasGroup.alpha - targetAlpha) < 0.01f)
            canvasGroup.alpha = targetAlpha;
    }

    public void Show()
    {
        targetAlpha = 1f;
    }

    public void Hide()
    {
        targetAlpha = 0f;
    }
}