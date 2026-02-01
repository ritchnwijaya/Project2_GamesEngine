using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] private AudioSource audioSource;

    [Header("Scene Music")]
    [SerializeField] private AudioClip startMenuMusic;
    [SerializeField] private AudioClip scene1Music;
    [SerializeField] private AudioClip scene2Music;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!audioSource) audioSource = GetComponent<AudioSource>();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioClip newClip = null;

        if (scene.name == "Title")          newClip = startMenuMusic;  // <-- Startszene Name!
        else if (scene.name == "Home")    newClip = scene1Music;
        else if (scene.name == "farmCity")    newClip = scene2Music;

        if (newClip != null && audioSource.clip != newClip)
        {
            audioSource.clip = newClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
