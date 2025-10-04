using UnityEngine;
using System.Collections.Generic;
using DefaultNamespace;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicController : MonoBehaviour
{
    private static BackgroundMusicController instance;

    [Header("Background Music Settings")]
    [SerializeField] private List<AudioClip> audioClips = new List<AudioClip>();

    private AudioSource audioSource;

    public static BackgroundMusicController Instance => instance;

    private void Awake()
    {
        // Singleton pattern - ensure only one instance exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSource();
            DI.globalScope.register(this);
        }
        else
        {
            // Destroy duplicate instances
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Set up audio source for background music
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Play a specific audio clip from the list
    /// </summary>
    /// <param name="index">Index of the audio clip to play</param>
    public void PlayMusic(int index, bool stopIfPlayingSame = true)
    {
        if (audioClips != null && index >= 0 && index < audioClips.Count && audioClips[index] != null)
        {
            if (audioSource.isPlaying && !stopIfPlayingSame && audioSource.clip == audioClips[index])
            {
                return;
            }
            audioSource.clip = audioClips[index];
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Invalid audio clip index or null audio clip at index " + index);
        }
    }

    /// <summary>
    /// Play a specific audio clip
    /// </summary>
    /// <param name="clip">Audio clip to play</param>
    public void PlayMusic(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Cannot play null audio clip");
        }
    }

    /// <summary>
    /// Stop the currently playing music
    /// </summary>
    public void StopMusic()
    {
        audioSource.Stop();
    }

    /// <summary>
    /// Pause the currently playing music
    /// </summary>
    public void PauseMusic()
    {
        audioSource.Pause();
    }

    /// <summary>
    /// Resume the paused music
    /// </summary>
    public void ResumeMusic()
    {
        audioSource.UnPause();
    }

    /// <summary>
    /// Set the volume of the background music
    /// </summary>
    /// <param name="volume">Volume level (0.0 to 1.0)</param>
    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// Get the current volume
    /// </summary>
    public float GetVolume()
    {
        return audioSource.volume;
    }

    /// <summary>
    /// Check if music is currently playing
    /// </summary>
    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }

    /// <summary>
    /// Add an audio clip to the list
    /// </summary>
    /// <param name="clip">Audio clip to add</param>
    public void AddAudioClip(AudioClip clip)
    {
        if (clip != null && !audioClips.Contains(clip))
        {
            audioClips.Add(clip);
        }
    }

    /// <summary>
    /// Remove an audio clip from the list
    /// </summary>
    /// <param name="clip">Audio clip to remove</param>
    public void RemoveAudioClip(AudioClip clip)
    {
        audioClips.Remove(clip);
    }

    /// <summary>
    /// Get the list of audio clips
    /// </summary>
    public List<AudioClip> GetAudioClips()
    {
        return new List<AudioClip>(audioClips);
    }
}
