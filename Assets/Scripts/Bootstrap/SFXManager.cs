using UnityEngine;

/// <summary>
/// Отвечает за мгновенное воспроизведение звуков (эффекты, UI).
/// Находится на том же объекте, что и MusicManager.
/// </summary>
public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [Header("Аудиоисточник для эффектов")]
    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {
        // Т.к. объект [Audio System] уже защищен от удаления через MusicManager,
        // здесь DontDestroyOnLoad можно не писать, если они висят на одном GameObject.
        // Но базовую проверку на синглтон сделать стоит:
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Проигрывает короткий.
    /// </summary>
    public void PlaySound(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}