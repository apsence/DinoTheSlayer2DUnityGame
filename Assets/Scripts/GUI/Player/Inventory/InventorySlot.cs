using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] private Image highlight;
    [SerializeField] private float fadeSpeed = 8f;
    [SerializeField] private InventorySettings settings;

    private float targetAlpha;

    private void Awake()
    {
        if (highlight == null)
        {
            return;
        }


        Color c = highlight.color;
        c.a = 0f;
        highlight.color = c;
        
    }

    private void Update()
    {
        if (highlight == null) return;

        Color c = highlight.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, settings.fadeSpeed * Time.unscaledDeltaTime);
        highlight.color = c;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetAlpha = 1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetAlpha = 0f;
    }
}