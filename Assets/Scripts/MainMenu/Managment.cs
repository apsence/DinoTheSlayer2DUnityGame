using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Managment : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private Image fade;
    public void StartNewGame()
    {
        StartCoroutine(FadeIn(fadeDuration));
    }

    public void LoadGame()
    {
        
    }

    public void Settings()
    {
        
    }

    public void Exit()
    {
        Application.Quit();
    }

    private IEnumerator FadeIn(float duration)
    {
        Color color = fade.color;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            color.a = t / duration;
            fade.color = color;

            yield return null;
        }

        color.a = 1f;
        fade.color = color;

        Debug.Log(TransitionManager.Instance);
        TransitionManager.Instance.LoadScene("Game");
    }
}
