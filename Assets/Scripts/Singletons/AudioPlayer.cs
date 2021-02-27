using System.Collections;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Global music player
/// </summary>
public class AudioPlayer : MonoBehaviour
{
    /// <summary>
    /// Implents the singleton pattern
    /// </summary>
    public static AudioPlayer Instance { get; private set; }

    private AudioSource audioSource;

    // Init
    void Awake()
    {
        // Preserve itself between scenes
        DontDestroyOnLoad(gameObject);

        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Play a new song or smoothly replace a playing song with a new one
    /// </summary>
    /// <param name="clip">The new song to play</param>
    public void Play(AudioClip clip)
    {
        if (clip != null)
        {
            if (audioSource.isPlaying)
            {
                if (clip != audioSource.clip)
                {
                    StartCoroutine(nameof(ReplaceClip), clip);
                }
            }
            else
            {
                audioSource.clip = clip;
                FadeIn();
            }
        }
    }

    /// <summary>
    /// Fade out the song and stop playing
    /// </summary>
    public void Stop(float time = 0.5f)
    {
        StartCoroutine(nameof(StopCoroutine), time);
    }

    /// <summary>
    /// Continue whatever song was playing
    /// </summary>
    public void Continue()
    {
        FadeIn();
    }

    /// <summary>
    /// Set the audio clip for the AudioPlayer
    /// </summary>
    /// <param name="newClip">The new audio clip</param>
    public void SetMusic(AudioClip newClip)
    {
        audioSource.clip = newClip;
    }


    // Fade in the music
    private void FadeIn()
    {
        audioSource.volume = 0;
        audioSource.Play();
        audioSource.DOFade(1f, 6f);
    }


    // Replace a playing clip with another one
    private IEnumerator ReplaceClip(AudioClip newClip)
    {
        StartCoroutine(nameof(Stop), 0.5f);
        yield return new WaitForSecondsRealtime(0.7f);
        audioSource.clip = newClip;
        FadeIn();
    }


    // The implementation for Stop
    private IEnumerator StopCoroutine(float time)
    {
        audioSource.DOFade(0, time);
        yield return new WaitForSecondsRealtime(time);
        audioSource.Stop();
    }
}
