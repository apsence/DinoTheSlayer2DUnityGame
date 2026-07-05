using System.Collections;
using UnityEngine;

public class SpriteHitFlash : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color flashColor = Color.white;

    [Header("Pulse Settings")]
    [SerializeField] private float pulseDuration = 0.25f;
    [SerializeField] private float pulseStrength = 1f;

    private Material _mat;
    private Coroutine _routine;

    private static readonly int FlashAmount = Shader.PropertyToID("_FlashAmount");
    private static readonly int FlashColor = Shader.PropertyToID("_FlashColor");

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        _mat = spriteRenderer.material;

        _mat.SetColor(FlashColor, flashColor);
        _mat.SetFloat(FlashAmount, 0f);
    }

    public void PlayFlash()
    {
        if (_routine != null)
            StopCoroutine(_routine);

        _routine = StartCoroutine(PulseRoutine());
    }

    private IEnumerator PulseRoutine()
    {
        float t = 0f;

        // 1) быстрый подъём (вспышка)
        while (t < pulseDuration * 0.3f)
        {   
            t += Time.deltaTime;

            float k = t / (pulseDuration * 0.3f);
            float value = Mathf.Lerp(0f, pulseStrength, k);

            _mat.SetFloat(FlashAmount, value);
            yield return null;
        }

        // 2) спад (затухание)
        while (t < pulseDuration * 0.7f)
        {
            t += Time.deltaTime;

            float k = (t - pulseDuration * 0.3f) / (pulseDuration * 0.4f);
            float value = Mathf.Lerp(pulseStrength, pulseStrength * 0.3f, k);

            _mat.SetFloat(FlashAmount, value);
            yield return null;
        }

        // 3) финальное затухание в ноль
        while (t < pulseDuration)
        {
            t += Time.deltaTime;

            float k = (t - pulseDuration * 0.7f) / (pulseDuration * 0.3f);
            float value = Mathf.Lerp(pulseStrength * 0.3f, 0f, k);

            _mat.SetFloat(FlashAmount, value);
            yield return null;
        }

        _mat.SetFloat(FlashAmount, 0f);
        _routine = null;
    }
}