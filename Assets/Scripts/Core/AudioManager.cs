using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource sfxSource;
    public AudioSource musicSource;

    public AudioClip backgroundMusic;
    public AudioClip damageSound;
    public AudioClip spellCastSound;
    public AudioClip enemyDeathSound;

    public float SfxVolume;
    public float MusicVolume;

    private void Awake()
    {
        if (Instance != null) throw new InvalidOperationException("Cannot initialize AudioManager multiple times");
        Instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        sfxSource.volume = SfxVolume * SfxVolume;
        musicSource.volume = MusicVolume * MusicVolume;
        PlayMusic(backgroundMusic);
    }

    public void PlaySound(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (!musicSource.isPlaying)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
}
