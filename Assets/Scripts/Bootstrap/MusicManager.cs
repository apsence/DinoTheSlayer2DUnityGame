using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Аудио")]
    [SerializeField] private AudioSource source;

    [Header("Плейлисты")]
    [SerializeField] private List<AudioClip> menuTracks;
    [SerializeField] private List<AudioClip> gameTracks;

    [Header("Настройки затухания")]

    [Tooltip("Длительность fade out перед сменой трека. Трек плавно затихает за это время перед тем как включится следующий.")]
    [SerializeField] private float fadeDuration = 1f;
    [Tooltip("Длительность fade in при старте каждого нового трека. Музыка нарастает с 0 до полной громкости за это время.")]
    [SerializeField] private float trackFadeInDuration = 2f;

    [Header("Настройки нарастания громкости")]
    [Tooltip("Форма кривой нарастания громкости при fade in. 1 - линейная, 2 - медленный старт, быстрый конец и т.д.")]
    [SerializeField] private float fadeInCurve = 2f;

    private List<AudioClip> currentPlaylist;
    private int currentIndex;

    // Два корутина: один — смена плейлиста (fade out → fade in),
    // другой — основной loop текущего плейлиста.
    private Coroutine _transitionRoutine;
    private Coroutine _playRoutine;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        source.loop = false;
        source.volume = 0f;
        source.spatialBlend = 0f;
        source.playOnAwake = false;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
            SetPlaylist(menuTracks);
        else if (scene.name == "Game")
            SetPlaylist(gameTracks);
    }

    public void SetPlaylist(List<AudioClip> playlist)
    {
        if (playlist == null || playlist.Count == 0) return;

        // Останавливаем предыдущую смену плейлиста, если она ещё идёт
        if (_transitionRoutine != null)
            StopCoroutine(_transitionRoutine);

        _transitionRoutine = StartCoroutine(TransitionToPlaylist(playlist));
    }

    // Плавная смена: fade out текущего → переключить → fade in нового
    private IEnumerator TransitionToPlaylist(List<AudioClip> newPlaylist)
    {
        // Останавливаем playLoop, чтобы он не менял volume во время fade out
        if (_playRoutine != null)
        {
            StopCoroutine(_playRoutine);
            _playRoutine = null;
        }

        // Fade out текущего трека (если что-то играет)
        if (source.isPlaying && source.volume > 0f)
            yield return Fade(source.volume, 0f, fadeDuration, 1f);

        source.Stop();

        currentPlaylist = newPlaylist;
        currentIndex = 0;

        // Запускаем loop нового плейлиста
        _playRoutine = StartCoroutine(PlayLoop());
    }

    private IEnumerator PlayLoop()
    {
        while (true)
        {
            AudioClip clip = currentPlaylist[currentIndex];

            source.Stop();
            source.clip = clip;
            source.volume = 0f;
            source.Play();

            yield return Fade(0f, 1f, trackFadeInDuration, fadeInCurve);

            // Ждём до момента начала fade out
            float timer = 0f;
            float endTime = clip.length - trackFadeInDuration - fadeDuration;

            // Защита: если трек слишком короткий — просто дождёмся конца
            if (endTime < 0f) endTime = 0f;

            while (timer < endTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            // Fade out перед сменой трека
            yield return Fade(1f, 0f, fadeDuration, 1f);

            currentIndex++;
            if (currentIndex >= currentPlaylist.Count)
                currentIndex = 0;
        }
    }

    /// <summary>
    /// Универсальный fade с поддержкой кривой.
    /// curve = 1 → линейный, curve = 2 → квадратичный (медленный старт), и т.д.
    /// </summary>
    private IEnumerator Fade(float from, float to, float duration, float curve = 1f)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float linear = Mathf.Clamp01(t / duration);
            // Pow даёт нелинейность: при curve=2 значение 0.2 → 0.04, 0.5 → 0.25
            float curved = Mathf.Pow(linear, curve);
            source.volume = Mathf.Lerp(from, to, curved);
            yield return null;
        }

        source.volume = to;
    }
}