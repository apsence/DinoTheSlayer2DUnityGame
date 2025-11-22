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
    private bool _gameCompleted;

    void Awake()
    {
        _pauseManager = FindAnyObjectByType<PauseManager>();
        _levelManager = FindAnyObjectByType<LevelManager>();
        player = GameObject.FindWithTag("Player");
    }

    public void InitGame()
    {
        if (player != null)
        {
            UnitStats stats = player.GetComponent<UnitStats>();
            stats.health = stats.maxHealth;
            stats.RefreshPlayerHUD();
            player.transform.position = Vector2.zero;
            player.SetActive(true);
        }

        _levelManager.currentWave = 0;
        isGameEnd = false;

        Color c = fadeImage.color;
        c.a = 1f;
        fadeImage.color = c;

        _pauseManager.Resume();
        StartCoroutine(FadeInCoroutine());
    }

    public void OnClick()
    {
        InitGame();
    }

    public void GameOver()
    {
        isGameEnd = true;
        StartCoroutine(FadeOutCoroutine());
    }

    public void GameCompleted()
    {
        isGameEnd = true;
        _gameCompleted = true;
        StartCoroutine(FadeOutCoroutine());
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
        DestroyAllObjects();
        ShowReTryUI(true);
        _pauseManager.Pause();
    }

    private IEnumerator FadeInCoroutine()
    {
        ShowReTryUI(false);
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

    void ShowReTryUI(bool showGameOver)
    {
        if (!_gameCompleted)
        {
            GameOverUI[] list = FindObjectsByType<GameOverUI>(FindObjectsSortMode.None);
            foreach (GameOverUI ui in list)
            {
                Image img = ui.GetComponent<Image>();
                TextMeshProUGUI tmp = ui.GetComponent<TextMeshProUGUI>();

                if (img != null) img.enabled = showGameOver;
                if (tmp != null) tmp.enabled = showGameOver;
            }
        }
        else
        {
            WinUI[] winUIList = FindObjectsByType<WinUI>(FindObjectsSortMode.None);
            foreach (WinUI ui in winUIList)
            {
                Image img = ui.GetComponent<Image>();
                TextMeshProUGUI tmp = ui.GetComponent<TextMeshProUGUI>();

                if (img != null) img.enabled = showGameOver;
                if (tmp != null) tmp.enabled = showGameOver;
            }
        }
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
        foreach (OnCollisionProjectile collision in onCollisionProjectiles)
        {
            Destroy(collision.gameObject);
        }
    }

}
