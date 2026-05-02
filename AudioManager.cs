using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip crimeAlertSound;
    public AudioClip ambientMusic;

    private AudioSource alertAudioSource;
    private AudioSource musicAudioSource;

    public CrimeSystem crimeSystem;

    void Start()
    {
        // Setup alert audio source
        alertAudioSource = GetComponent<AudioSource>();
        if (alertAudioSource == null)
        {
            alertAudioSource = gameObject.AddComponent<AudioSource>();
        }

        // Setup music audio source
        musicAudioSource = gameObject.AddComponent<AudioSource>();
        musicAudioSource.clip = ambientMusic;
        musicAudioSource.loop = true;
        musicAudioSource.volume = 0.3f; // Adjust volume as needed

        // Start ambient music
        if (ambientMusic != null)
        {
            musicAudioSource.Play();
        }
        else
        {
            Debug.LogWarning("Ambient music clip is missing!");
        }
    }

    public void PlayCrimeAlert()
    {
        if (crimeAlertSound != null && alertAudioSource != null)
        {
            alertAudioSource.PlayOneShot(crimeAlertSound);
        }
        else
        {
            Debug.LogWarning("Crime alert sound or AudioSource is missing!");
        }
    }
}