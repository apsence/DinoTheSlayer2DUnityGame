using UnityEngine;
using UnityEngine.Serialization;

public class MenuVisibilty : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [FormerlySerializedAs("actionBarSettings")] [SerializeField] private MenuSettings menuSettings;

    private float targetAlpha = 0f;
    public bool IsVisible => targetAlpha > 0.5f;

    private void Update()
    {
        canvasGroup.alpha = Mathf.MoveTowards(
            canvasGroup.alpha,
            targetAlpha,
            menuSettings.fadeSpeed * Time.unscaledDeltaTime
        );

        if (Mathf.Abs(canvasGroup.alpha - targetAlpha) < 0.01f)
            canvasGroup.alpha = targetAlpha;
    }

    public void Show()
    {
        if (IsVisible) return;
        
        targetAlpha = 1f;
    }

    public void Hide()
    {
        if (!IsVisible) return;
        
        targetAlpha = 0f;
    }
}