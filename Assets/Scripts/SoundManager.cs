using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [SerializeField] private AudioSource music_AudioSource;
    [SerializeField] private AudioClip[] music_AudioClips;
    [SerializeField] private AudioClip music_endgameFirst;
    [SerializeField] private AudioClip music_endgameSecond;

    [SerializeField] private AudioSource sounds_AudioSource;
    [SerializeField] private AudioClip sounds_razbitie;
    [SerializeField] private AudioClip sounds_combo;
    [SerializeField] private AudioClip sounds_falling;
    [SerializeField] private AudioClip sounds_perevertysh;


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
    
    public void ChangeMusicToEnd()
    {
        StopAllCoroutines();
        music_AudioSource.clip = (music_endgameFirst);
        music_AudioSource.Play();
        StartCoroutine(PlayendMusic());
    }
    private IEnumerator PlayendMusic()
    {        
            yield return new WaitUntil(() => music_AudioSource.isPlaying == false);
        music_AudioSource.Stop();
        music_AudioSource.loop = true;
        music_AudioSource.clip = music_endgameSecond;
        music_AudioSource.Play();
        
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
    public void PlayFalling()
    {
        sounds_AudioSource.PlayOneShot(sounds_falling);
    }
    public void PlayPerevertysh()
    {
        sounds_AudioSource.PlayOneShot(sounds_perevertysh);
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
                    while (lastClip == currentClip && music_AudioClips.Length>1)
                    {
                        currentClip = GetRandomMusic();
                    }
                    lastClip = currentClip;
                    music_AudioSource.clip = (currentClip);
                    music_AudioSource.Play();
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
