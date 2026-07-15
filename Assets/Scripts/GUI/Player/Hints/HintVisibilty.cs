using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HintVisibility : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI hintHeader;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private HintSettings hintSettings;

    private struct HintData
    {
        public string Header;
        public string Text;
        public float Duration;
    }

    private readonly Queue<HintData> hintQueue = new Queue<HintData>();
    private Coroutine queueRoutine;
    private bool forceHideRequested;

    private float targetAlpha = 0f;
    public bool IsVisible => targetAlpha > 0.5f;

    private void Update()
    {
        canvasGroup.alpha = Mathf.MoveTowards(
            canvasGroup.alpha,
            targetAlpha,
            hintSettings.fadeSpeed * Time.unscaledDeltaTime
        );

        if (Mathf.Abs(canvasGroup.alpha - targetAlpha) < 0.01f)
            canvasGroup.alpha = targetAlpha;
    }

    /// <summary>
    /// Поставить хинт в очередь показа. duration — сколько секунд хинт будет
    /// виден на экране (без учёта времени на fade in/out). Можно вызывать
    /// сколько угодно раз подряд — хинты будут показаны один за другим.
    /// </summary>
    public void CreateHint(string header, string text, float duration)
    {
        hintQueue.Enqueue(new HintData { Header = header, Text = text, Duration = duration });

        if (queueRoutine == null)
            queueRoutine = StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        while (hintQueue.Count > 0)
        {
            HintData hint = hintQueue.Dequeue();

            hintHeader.text = HintTextFormatter.Format(hint.Header, hintSettings);
            hintText.text = HintTextFormatter.Format(hint.Text, hintSettings);

            Show();
            // ждём, пока хинт реально дофейдится до видимого состояния
            yield return new WaitUntil(() => Mathf.Approximately(canvasGroup.alpha, targetAlpha));

            // ждём hint.Duration секунд, но выходим раньше, если позвали ForciblyHide()
            float elapsed = 0f;
            while (elapsed < hint.Duration && !forceHideRequested)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            forceHideRequested = false;

            Hide();
            // ждём завершения fade-out, чтобы следующий хинт не наложился
            // на ещё исчезающий текущий
            yield return new WaitUntil(() => Mathf.Approximately(canvasGroup.alpha, targetAlpha));
        }

        queueRoutine = null;
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

    /// <summary>
    /// Досрочно скрывает ТЕКУЩИЙ показанный хинт (например, по клику на крестик),
    /// не затрагивая остальные хинты в очереди — после fade-out очередь продолжит
    /// показывать следующий хинт как обычно.
    /// </summary>
    public void ForciblyHide()
    {
        if (!IsVisible) return; // сейчас ничего не показано — скрывать нечего

        forceHideRequested = true;
    }
}