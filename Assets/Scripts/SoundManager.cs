using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [SerializeField] private AudioSource music_AudioSource;
    [SerializeField] private AudioClip[] music_AudioClips;

    [SerializeField] private AudioSource sounds_AudioSource;
    [SerializeField] private AudioClip sounds_razbitie;
    [SerializeField] private AudioClip sounds_combo;

    [SerializeField] private bool music_enabled;
    [SerializeField] private bool sound_enabled;


    private void Start()
    {
        if (Instance == null)
            Instance = this;
        StartCoroutine(PlayRandomMusic());
    }

    public void ChangeSoundState()
    {
        sound_enabled = !sound_enabled;
        sounds_AudioSource.mute = !sound_enabled;
    }
    
    public void ChangeMusicState()
    {
        music_enabled = !music_enabled;
        music_AudioSource.mute = !music_enabled;
    }

    public bool GetSoundState()
    {
        return sound_enabled;
    }

    public bool GetMusicState()
    {
        return music_enabled;
    }

    public void PlayRazbitie()
    {
        sounds_AudioSource.PlayOneShot(sounds_razbitie);
    }

    public void PlayCombo()
    {
        sounds_AudioSource.PlayOneShot(sounds_combo);
    }

    private IEnumerator PlayRandomMusic()
    {
        if (music_AudioClips.Length > 0)
        {
            AudioClip lastClip = null;
            AudioClip currentClip = GetRandomMusic();
            while (true)
            {
                if (!music_AudioSource.isPlaying)
                {
                    while (lastClip == currentClip)
                    {
                        currentClip = GetRandomMusic();
                    }
                    lastClip = currentClip;
                    music_AudioSource.PlayOneShot(currentClip);
                }
                yield return new WaitForEndOfFrame();
            }
        }
        yield break;
    }

    private AudioClip GetRandomMusic()
    {        
           return   music_AudioClips[Random.Range(0, music_AudioClips.Length)];       
    }
}
