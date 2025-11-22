using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClearAndResetGame : MonoBehaviour
{
    [Header("GameOverSettings")]
    public float fadeDuration = 2f;
    public Image fadeImage;
    public bool isGameEnd = false;
    private LevelManager _levelManager;

    private PauseManager _pauseManager;
    private GameObject player;
    private bool _revert = false;
    private bool _gameCompleted;

    void Start()
    {
        _pauseManager = FindAnyObjectByType<PauseManager>();
        _levelManager = FindAnyObjectByType<LevelManager>();
        isGameEnd = true;

        player = GameObject.FindWithTag("Player");
        UnitStats stats = player.GetComponent<UnitStats>();
        stats.health = stats.maxHealth;
        stats.RefreshPlayerHUD();

        player.transform.position = Vector2.zero;
    }

    void DestroyAllObjects()
    {
        AI_common[] list = FindObjectsByType<AI_common>(FindObjectsSortMode.None);
        foreach (AI_common ai in list)
        {
            Destroy(ai.gameObject);
        }

        Collectables[] collectables = FindObjectsByType<Collectables>(FindObjectsSortMode.None);
        foreach (Collectables col in collectables)
        {
            Destroy(col.gameObject);
        }

        OnCollisionProjectile[] onCollisionProjectiles = FindObjectsByType<OnCollisionProjectile>(FindObjectsSortMode.None);
        foreach (Collectables collision in collectables)
        {
            Destroy(collision.gameObject);
        }
    }

    public void OnClick()
    {
        player.SetActive(true);

        Color c = fadeImage.color;
        c.a = 1f;
        fadeImage.color = c;

        _pauseManager.Resume();
        StartCoroutine(FadeInCoroutine());
        _levelManager.currentWave = 0;
        isGameEnd = false;

    }

    public void GameOver()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    public void GameCompleted()
    {
        StartCoroutine(FadeOutCoroutine());
        _gameCompleted = true;
    }

    private IEnumerator FadeOutCoroutine()
    {
        float elapsed = 0f;
        Color c = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
        ShowReTryUI();
        _pauseManager.Pause();
    }

    private IEnumerator FadeInCoroutine()
    {
        ShowReTryUI();
        float elapsed = 0f;
        Color c = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
    }

    void ShowReTryUI()
    {
        GameOverUI[] list = FindObjectsByType<GameOverUI>(FindObjectsSortMode.None);

        if (!_gameCompleted)
        {
            foreach (GameOverUI ui in list)
            {
                Image img = ui.GetComponent<Image>();
                TextMeshProUGUI tmp = ui.GetComponent<TextMeshProUGUI>();

                if (img != null) img.enabled = !_revert;
                if (tmp != null) tmp.enabled = !_revert;
            }
        }
        else
        {
            WinUI[] winUIList = FindObjectsByType<WinUI>(FindObjectsSortMode.None);
            foreach (WinUI ui in winUIList)
            {
                Image img = ui.GetComponent<Image>();
                TextMeshProUGUI tmp = ui.GetComponent<TextMeshProUGUI>();

                if (img != null) img.enabled = !_revert;
                if (tmp != null) tmp.enabled = !_revert;
            }
        }
        _revert = !_revert;
    }

}
