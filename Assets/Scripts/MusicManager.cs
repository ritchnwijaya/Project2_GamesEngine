using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Music")]
    public AudioSource audioSource;
    public AudioClip scene1Music;
    public AudioClip scene2Music;

    private string currentSceneName;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == currentSceneName)
            return;

        currentSceneName = scene.name;

        AudioClip newClip = null;

        if (scene.name == "Home")
            newClip = scene1Music;
        else if (scene.name == "farmCity")
            newClip = scene2Music;

        if (newClip != null && audioSource.clip != newClip)
        {
            audioSource.clip = newClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
