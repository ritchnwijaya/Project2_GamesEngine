using UnityEngine;

public class RainAudioController : MonoBehaviour
{
    [SerializeField] private ParticleSystem rainParticles;
    [SerializeField] private AudioSource rainAudio;

    private void Awake()
    {
        if (!rainParticles) rainParticles = GetComponent<ParticleSystem>();
        if (!rainAudio) rainAudio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!rainParticles || !rainAudio) return;

        bool isRaining = rainParticles.isPlaying;

        if (isRaining && !rainAudio.isPlaying)
            rainAudio.Play();

        if (!isRaining && rainAudio.isPlaying)
            rainAudio.Stop();
    }
}
