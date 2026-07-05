using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradesGUIVisibility : MonoBehaviour
{
    [Header("Merchant detection")]
    [SerializeField] private List<Transform> merchants;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float distanceToShowHotkey;

    [Header("Hotkey hint")]
    [SerializeField] private GameObject hotkeyHint;
    [SerializeField] private float hotkeyHintFadeDuration = 0.3f;

    [Header("Upgrade menu")]
    [SerializeField] private GameObject menuGUI;
    [SerializeField] private float menuFadeDuration = 0.5f;

    private CanvasGroup _hotkeyCanvasGroup;
    private CanvasGroup _menuCanvasGroup;

    private bool _isOpen;
    private bool _isMenuAnimating;
    private Coroutine _menuCoroutine;
    private Coroutine _hotkeyCoroutine;

    private bool _wasNearMerchant;
    public bool IsNearMerchant { get; private set; }
    public bool IsOpen => _isOpen;

    private void Awake()
    {
        _hotkeyCanvasGroup = hotkeyHint.GetComponent<CanvasGroup>();
        if (_hotkeyCanvasGroup == null)
            _hotkeyCanvasGroup = hotkeyHint.AddComponent<CanvasGroup>();
        
        hotkeyHint.SetActive(true);
        _hotkeyCanvasGroup.alpha = 0f;
        
        _menuCanvasGroup = menuGUI.GetComponent<CanvasGroup>();
        if (_menuCanvasGroup == null)
            _menuCanvasGroup = menuGUI.AddComponent<CanvasGroup>();

        _menuCanvasGroup.alpha = 0f;
        _menuCanvasGroup.interactable = false;
        _menuCanvasGroup.blocksRaycasts = false;
        _isOpen = false;
        _isMenuAnimating = false;

        _wasNearMerchant = false;
    }

    private void Update()
    {
        bool currentNear = CheckDistance();
        IsNearMerchant = currentNear;
        
        if (currentNear != _wasNearMerchant)
        {
            _wasNearMerchant = currentNear;
            if (currentNear)
                FadeHotkeyHintIn();
            else
                FadeHotkeyHintOut();
        }
    }

    private bool CheckDistance()
    {
        float threshold = distanceToShowHotkey * distanceToShowHotkey;
        foreach (Transform m in merchants)
        {
            if ((m.position - playerTransform.position).sqrMagnitude < threshold)
                return true;
        }
        return false;
    }
    

    public void Show()
    {
        if (_isOpen || _isMenuAnimating) return;
        if (_menuCoroutine != null) StopCoroutine(_menuCoroutine);
        _menuCoroutine = StartCoroutine(FadeMenu(0f, 1f, menuFadeDuration, true));
    }

    public void Hide()
    {
        if (!_isOpen || _isMenuAnimating) return;
        if (_menuCoroutine != null) StopCoroutine(_menuCoroutine);
        _menuCoroutine = StartCoroutine(FadeMenu(1f, 0f, menuFadeDuration, false));
    }

    private IEnumerator FadeMenu(float from, float to, float duration, bool open)
    {
        _isMenuAnimating = true;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            _menuCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        _menuCanvasGroup.alpha = to;
        if (open)
        {
            _isOpen = true;
        }
        else
        {
            _isOpen = false;
        }
        _isMenuAnimating = false;
        _menuCoroutine = null;
    }
    

    private void FadeHotkeyHintIn()
    {
        if (_hotkeyCoroutine != null) StopCoroutine(_hotkeyCoroutine);
        _hotkeyCoroutine = StartCoroutine(FadeHotkey(0f, 1f, hotkeyHintFadeDuration));
    }

    private void FadeHotkeyHintOut()
    {
        if (_hotkeyCoroutine != null) StopCoroutine(_hotkeyCoroutine);
        _hotkeyCoroutine = StartCoroutine(FadeHotkey(1f, 0f, hotkeyHintFadeDuration));
    }

    private IEnumerator FadeHotkey(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            _hotkeyCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        _hotkeyCanvasGroup.alpha = to;
        _hotkeyCoroutine = null;
    }
}