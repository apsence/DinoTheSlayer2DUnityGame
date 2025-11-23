using System.Collections;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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
    private GameObject _player;
    private bool _gameCompleted;
    private SpawnCoins _spawnCoins;
    private Upgrades _upgrades;
    private TextMeshProUGUI _hpUpgradeCostMesh;
    private TextMeshProUGUI _damageUpgradeCostMesh;
    private Button _upgradeHPButton;
    private Button _upgradeDamageButton;

    void Awake()
    {
        _pauseManager = FindAnyObjectByType<PauseManager>();
        _levelManager = FindAnyObjectByType<LevelManager>();
        _player = GameObject.FindWithTag("Player");
        _spawnCoins = FindAnyObjectByType<SpawnCoins>();

        _upgrades = FindAnyObjectByType<Upgrades>();
        _hpUpgradeCostMesh = GameObject.FindWithTag("HPUpgradeCost").GetComponent<TextMeshProUGUI>();
        _damageUpgradeCostMesh = GameObject.FindWithTag("DamageUpgradeCost").GetComponent<TextMeshProUGUI>();
        _upgradeHPButton = GameObject.FindWithTag("UpgradeHPButton").GetComponent<Button>();
        _upgradeDamageButton = GameObject.FindWithTag("UpgradeDamageButton").GetComponent<Button>();
    }

    public void InitGame()
    {
        if (_player != null)
        {
            UnitStats stats = _player.GetComponent<UnitStats>();
            stats.health = stats.maxHealth;
            stats.RefreshPlayerHUD();
            RefreshPlayerStatsAndUpgrades();
            _player.transform.position = Vector2.zero;
            _player.SetActive(true);
        }

        _levelManager.currentWave = 0;
        isGameEnd = false;

        Color c = fadeImage.color;
        c.a = 1f;
        fadeImage.color = c;

        _pauseManager.Resume();
        StartCoroutine(FadeInCoroutine());
        _levelManager.RestartGame();
        _spawnCoins.RestartGame();
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
            Debug.LogError(winUIList.Count());
        }

        // --- управление интерфейсом игрока ---
        PlayerGUI[] playerGuiList = FindObjectsByType<PlayerGUI>(FindObjectsSortMode.None);
        foreach (PlayerGUI gui in playerGuiList)
        {
            Image img = gui.GetComponent<Image>();
            TextMeshProUGUI tmp = gui.GetComponent<TextMeshProUGUI>();

            if (img != null) img.enabled = !showGameOver;
            if (tmp != null) tmp.enabled = !showGameOver;
        }
    }


    void RefreshPlayerStatsAndUpgrades()
    {
        UnitStats _playerStats = _player.GetComponent<UnitStats>();

        _playerStats.health = _playerStats.defaultMaxHP;
        _playerStats.maxHealth = _playerStats.defaultMaxHP;

        _playerStats.damage = _playerStats.defaultDamage;
        _playerStats.RefreshPlayerHUD();

        Coin _playerCoins = _player.GetComponent<Coin>();
        _playerCoins.coinsTakenByPlayer = 0;
        _playerCoins.RefresfCountOfCoins();



        _upgrades.currentDamageLevel = 0;
        _upgrades.currentHPLevel = 0;
        _upgrades.currentCostOfHPUpgrade = _upgrades.costOfUpgradeHP[0];
        _upgrades.currentCostOfDamageUpgrade = _upgrades.costOfUpgradeDamage[0];

        _hpUpgradeCostMesh.text = _upgrades.currentCostOfHPUpgrade.ToString();
        _damageUpgradeCostMesh.text = _upgrades.currentCostOfDamageUpgrade.ToString();

        _upgradeHPButton.interactable = true;
        _upgradeDamageButton.interactable = true;
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
