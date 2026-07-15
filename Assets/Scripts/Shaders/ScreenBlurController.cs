using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Контроллер блюра фона через встроенный Full Screen Pass Renderer Feature URP.
///
/// РАЗОВАЯ НАСТРОЙКА В РЕДАКТОРЕ:
/// 1) Создать материал на шейдере Custom/2D/URP_BackgroundBlur_FullScreen.
/// 2) На Universal Renderer asset (тот, что подключён к вашему URP Asset) добавить
///    Renderer Feature -> "Full Screen Pass Renderer Feature":
///      - Pass Material   = материал из пункта 1
///      - Injection Point = "After Rendering Transparents" (или "After Rendering Post Processing")
///      - Requirements / Fetch Color Buffer = включить (иначе шейдеру не из чего будет читать)
/// 3) Перетащить эту Renderer Feature (она отображается как саб-ассет внутри Renderer Data)
///    в поле _blurFeature на этом компоненте, и туда же материал из пункта 1 в _blurMaterial.
///
/// ИСКЛЮЧЕНИЕ ОБЪЕКТОВ ИЗ БЛЮРА:
/// - Canvas в режиме Screen Space - Overlay блюрится не будет НИКОГДА и без всякой настройки —
///   он рендерится Unity UI отдельно от камеры URP, уже поверх результата Full Screen Pass.
///   Действие на такие Canvas в excluded[] не требуется, но безопасно (просто ничего не делает).
/// - Для мировых объектов (3D/2D) или Canvas в Screen Space - Camera / World Space, которые
///   являются частью рендера камеры и потому попадают в блюр — используйте связку:
///     а) Layer "NoBlur" (создать в Project Settings > Tags and Layers, имя настраивается ниже);
///     б) Ещё одна Renderer Feature "Render Objects", добавленная В СПИСКЕ ПОСЛЕ Full Screen Pass,
///        с Layer Mask = NoBlur — она перерисует эти объекты поверх уже заблюренного кадра,
///        так что они останутся чёткими. Это тоже настраивается один раз в редакторе.
///   Скрипт лишь переключает GameObject'ам layer на NoBlur/обратно.
/// </summary>
public class ScreenBlurController : MonoBehaviour
{
    [Header("Ссылки (настроить один раз, см. комментарий выше)")]
    [SerializeField] private FullScreenPassRendererFeature _blurFeature;
    [SerializeField] private Material _blurMaterial;

    [Header("Настройки блюра")]
    [SerializeField, Range(0, 20)] private float _maxBlurSize = 8f;
    [SerializeField] private float _transitionDuration = 0.25f;

    [Header("Исключение мировых объектов")]
    [SerializeField] private string _noBlurLayerName = "NoBlur";

    private Coroutine _transitionRoutine;

    private readonly Dictionary<GameObject, int> _originalLayers = new Dictionary<GameObject, int>();

    private static readonly int BlurSizeId = Shader.PropertyToID("_BlurSize");

    public bool IsBlurred { get; private set; }

    private void Awake()
    {
        if (_blurFeature == null || _blurMaterial == null)
        {
            Debug.LogError("[ScreenBlurController] Не назначены _blurFeature/_blurMaterial. " +
                            "См. инструкцию по настройке в комментарии к классу.");
            enabled = false;
            return;
        }

        _blurFeature.SetActive(false);
        _blurMaterial.SetFloat(BlurSizeId, 0f);
    }

    /// <summary>
    /// Включить блюр. excluded — GameObject'ы (Canvas или мировые объекты), которые должны
    /// остаться чёткими. Screen Space - Overlay канвасы исключать не обязательно — они и так
    /// не блюрятся.
    /// </summary>
    public void Blur(GameObject[] excluded = null)
    {
        if (_blurFeature == null) return;

        ApplyExclusions(excluded);
        _blurFeature.SetActive(true);

        StartTransition(_blurMaterial.GetFloat(BlurSizeId), _maxBlurSize, () => IsBlurred = true);
    }

    /// <summary>Выключить блюр и вернуть исключённые объекты в исходное состояние.</summary>
    public void Unblur()
    {
        if (_blurFeature == null) return;

        StartTransition(_blurMaterial.GetFloat(BlurSizeId), 0f, () =>
        {
            _blurFeature.SetActive(false);
            RestoreExclusions();
            IsBlurred = false;
        });
    }

    private void StartTransition(float from, float to, Action onComplete)
    {
        if (_transitionRoutine != null)
            StopCoroutine(_transitionRoutine);
        _transitionRoutine = StartCoroutine(AnimateBlur(from, to, onComplete));
    }

    private IEnumerator AnimateBlur(float from, float to, Action onComplete)
    {
        float t = 0f;
        while (t < _transitionDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = _transitionDuration > 0f ? Mathf.Clamp01(t / _transitionDuration) : 1f;
            _blurMaterial.SetFloat(BlurSizeId, Mathf.Lerp(from, to, k));
            yield return null;
        }

        _blurMaterial.SetFloat(BlurSizeId, to);
        _transitionRoutine = null;
        onComplete?.Invoke();
    }

    private void ApplyExclusions(GameObject[] excluded)
    {
        RestoreExclusions(); // на случай повторного Blur() без Unblur()

        if (excluded == null) return;

        int noBlurLayer = LayerMask.NameToLayer(_noBlurLayerName);

        foreach (var go in excluded)
        {
            if (go == null) continue;

            var canvas = go.GetComponent<Canvas>();
            if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                continue; // и так не блюрится, ничего делать не нужно

            if (noBlurLayer < 0)
            {
                Debug.LogWarning($"[ScreenBlurController] Layer '{_noBlurLayerName}' не найден. " +
                                  $"Объект {go.name} не будет исключён из блюра.");
                continue;
            }

            _originalLayers[go] = go.layer;
            SetLayerRecursively(go, noBlurLayer);
        }
    }

    private void RestoreExclusions()
    {
        foreach (var kvp in _originalLayers)
        {
            if (kvp.Key != null)
                SetLayerRecursively(kvp.Key, kvp.Value);
        }
        _originalLayers.Clear();
    }

    private static void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}