using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Common Audio Clips")]
    public AudioClip win;
    public AudioClip loos;
    public AudioClip congratulation;
    public AudioClip button;
    public AudioClip popup;
    public AudioClip bgMusic;

    [Header("Settings")]
    public bool isMusicEnabled = true;
    public bool isSfxEnabled = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (transform.parent == null) DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Start BG music on the very first frame
        if (bgMusic != null)
        {
            PlayBGMusic(isMusicEnabled);
        }
    }

    public void PlayBGMusic(bool shouldPlay)
    {
        if (bgMusic == null || musicSource == null) return;

        isMusicEnabled = shouldPlay;

        if (shouldPlay)
        {
            musicSource.clip = bgMusic;
            musicSource.loop = true;
            if (!musicSource.isPlaying) musicSource.Play();
            musicSource.mute = false;
        }
        else
        {
            // We mute or stop so the logic stays enabled
            musicSource.mute = true;
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (!isSfxEnabled || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlaySFX(AudioClip clip, float delay)
    {
        if (!isSfxEnabled || clip == null) return;
        StartCoroutine(PlayDelayedSFX(clip, delay));
    }

    private IEnumerator PlayDelayedSFX(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        sfxSource.PlayOneShot(clip);
    }

    public void SetSfxEnabled(bool isEnabled)
    {
        sfxSource.enabled = isEnabled;
    }

    public void  musicSourceController(bool isEnabled)
    {
        musicSource.enabled = isEnabled;
    }
}