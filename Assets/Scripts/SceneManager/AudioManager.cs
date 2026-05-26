using System.Collections;
using UnityEngine;

/// <summary>
/// Singleton AudioManager — memutar BGM secara otomatis dan tidak hilang saat ganti scene.
/// Cara pakai: Taruh script ini di GameObject "AudioManager" lalu assign AudioClip BGM di Inspector.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("BGM Settings")]
    public AudioClip bgmClip;       // File musik yang ingin diputar
    [Range(0f, 1f)]
    public float volume = 0.5f;     // Volume BGM (0 = mati, 1 = penuh)
    public bool playOnStart = true; // Otomatis putar saat game mulai

    [Header("Fade Settings")]
    public float fadeDuration = 1f; // Durasi fade in/out dalam detik

    private AudioSource audioSource;

    private void Awake()
    {
        // Singleton: hanya boleh ada 1 AudioManager. Jika sudah ada, hancurkan yang baru.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Tidak hilang saat ganti scene

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = bgmClip;
        audioSource.loop = true;
        audioSource.volume = 0f;
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        if (playOnStart && bgmClip != null)
        {
            PlayBGM();
        }
    }

    /// <summary>
    /// Mulai putar BGM dengan fade in.
    /// </summary>
    public void PlayBGM()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
        StopAllCoroutines();
        StartCoroutine(FadeTo(volume));
    }

    /// <summary>
    /// Ganti BGM ke klip baru dengan fade out → fade in.
    /// </summary>
    public void PlayBGM(AudioClip newClip)
    {
        if (newClip == null) return;
        StopAllCoroutines();
        StartCoroutine(SwitchBGM(newClip));
    }

    /// <summary>
    /// Hentikan BGM dengan fade out.
    /// </summary>
    public void StopBGM()
    {
        StopAllCoroutines();
        StartCoroutine(FadeToAndStop(0f));
    }

    /// <summary>
    /// Pause BGM.
    /// </summary>
    public void PauseBGM()
    {
        audioSource.Pause();
    }

    /// <summary>
    /// Lanjutkan BGM setelah di-pause.
    /// </summary>
    public void ResumeBGM()
    {
        audioSource.UnPause();
    }

    /// <summary>
    /// Set volume secara langsung (tanpa fade).
    /// </summary>
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        audioSource.volume = volume;
    }

    // --- Coroutines Internal ---

    private IEnumerator FadeTo(float targetVolume)
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / fadeDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    private IEnumerator FadeToAndStop(float targetVolume)
    {
        yield return StartCoroutine(FadeTo(targetVolume));
        audioSource.Stop();
    }

    private IEnumerator SwitchBGM(AudioClip newClip)
    {
        // Fade out klip lama
        yield return StartCoroutine(FadeTo(0f));

        audioSource.Stop();
        audioSource.clip = newClip;
        bgmClip = newClip;
        audioSource.Play();

        // Fade in klip baru
        yield return StartCoroutine(FadeTo(volume));
    }
}
