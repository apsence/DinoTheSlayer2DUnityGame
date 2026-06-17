using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    [SerializeField] private Image fade;
    [SerializeField] private float fadeTime = 1f;
    private bool isTransitioning;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadScene(string sceneName)
    {
        isTransitioning = true;

        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        yield return Fade(0f, 1f);

        yield return SceneManager.LoadSceneAsync(sceneName);

        yield return Fade(1f, 0f);

        isTransitioning = false;
    }

    private IEnumerator Fade(float from, float to)
    {
        Color color = fade.color;

        float timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;

            color.a = Mathf.Lerp(from, to, timer / fadeTime);
            fade.color = color;

            yield return null;
        }

        color.a = to;
        fade.color = color;
    }
}