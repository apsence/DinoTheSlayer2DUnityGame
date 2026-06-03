using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class YSort : MonoBehaviour
{
    [Header("---НЕ ИГРОК---")]
    [SerializeField] private int offset = 10;
    private Canvas canvas;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        canvas = GetComponentInChildren<Canvas>();
    }

    private void LateUpdate()
    {
        int order = Mathf.RoundToInt(transform.position.y * -100);

        _spriteRenderer.sortingOrder = order;

        if (canvas != null)
            canvas.sortingOrder = order + offset;
    }
}