using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HoverUnderline : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI text;

    private string baseText;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        baseText = text.text;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.text = $"<u>{baseText}</u>";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.text = baseText;
    }
}