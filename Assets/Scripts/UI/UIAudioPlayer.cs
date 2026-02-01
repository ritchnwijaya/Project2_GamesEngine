using UnityEngine;

public class UIAudioPlayer : MonoBehaviour
{
    public static UIAudioPlayer Instance;

    public AudioSource source;
    public AudioClip clickSound;
    public AudioClip hoverSound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayClick()
    {
        source.PlayOneShot(clickSound);
    }

    public void PlayHover()
    {
        source.PlayOneShot(hoverSound);
    }
}
