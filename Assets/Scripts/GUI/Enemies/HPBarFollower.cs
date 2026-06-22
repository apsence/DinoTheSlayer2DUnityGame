using UnityEngine;
using UnityEngine.UI;

public class HPBarFollower : MonoBehaviour
{
    [SerializeField] private GameObject _hpBarPrefab; // префаб бара
    [SerializeField] private Vector3 _offset = new Vector3(0, 0.5f, 0);

    private Health _health;
    private Image _fillImage;
    private RectTransform _bar;

    private static Canvas _sharedCanvas; // один на всех

    public void ShowBar() => _bar.gameObject.SetActive(true);
    public void HideBar() => _bar.gameObject.SetActive(false);

    private void Awake()
    {
        _health = GetComponent<Health>();

        // Находим общий канвас один раз
        if (_sharedCanvas == null)
            _sharedCanvas = GameObject.FindWithTag("HPBarsCanvas").GetComponent<Canvas>();

        // Создаём свой бар в общем канвасе
        GameObject barInstance = Instantiate(_hpBarPrefab, _sharedCanvas.transform);
        barInstance.SetActive(false);
        _bar = barInstance.GetComponent<RectTransform>();
        _fillImage = barInstance.GetComponentInChildren<Image>();

        _health.OnHealthChanged += UpdateBar;
    }

    private void LateUpdate()
    {
        if (_bar != null)
            _bar.position = transform.position + _offset;
    }

    private void UpdateBar(int current, int max)
    {
        _fillImage.fillAmount = (float)current / max;
    }

    private void OnDestroy()
    {
        _health.OnHealthChanged -= UpdateBar;
        if (_bar != null)
            Destroy(_bar.gameObject);
    }
}