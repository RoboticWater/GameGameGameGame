﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    #region VARIABLES

    public static MusicManager INSTANCE;
    public string sceneMusic;
    public float transitionTime = 4.5f;
    public AudioFile[] musicFiles;
    private bool fadeOut = false;
    private bool fadeIn = false;
    private string fadeInUsedString;
    private string fadeOutUsedString;

    public string current;

    #endregion

    #region METHODS
    void Awake() {
        if (INSTANCE == null) {
            INSTANCE = this;
            foreach( var m in musicFiles ) {
                m.source = gameObject.AddComponent<AudioSource>();
                m.source.clip = m.audioClip;
                m.source.volume = m.volume;
                m.source.loop = true;
                m.source.playOnAwake = false;
            }
        } else if (INSTANCE != this) {
            INSTANCE.sceneMusic = this.sceneMusic;
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        
    }

    public static void PlayMusic(string name) {
        AudioFile m = Find(name);
        if (m == null) return;
        m.source.Play();
    }
    public static void StopMusic(string name) {
        AudioFile m = Find(name);
        if (m == null) return;
        m.source.Stop();
    }

    public static void PauseMusic(string name) {
        AudioFile m = Find(name);
        if (m == null) return;
        m.source.Pause();
    } 
    public static void UnPauseMusic(string name) {
        AudioFile m = Find(name);
        if (m == null) return;
        m.source.UnPause();
    }

    public void PauseCurrentMusic() {
        PauseMusic(current);
    }

    public void UnPauseCurrentMusic() {
        UnPauseMusic(current);
    } 

    public static void FadeOut(string name, float duration) {
        INSTANCE.StartCoroutine(INSTANCE.IFadeOut(name, duration));
    }
    public static void FadeIn(string name, float targetVolume, float duration) {
        INSTANCE.StartCoroutine(INSTANCE.IFadeIn(name, targetVolume, duration));
    }

    public void UpdateVolume() {
        _UpdateVolume();
    }

    public static void _UpdateVolume() {
        if (INSTANCE == null) {
            Debug.LogWarning("Instance of MusicManager does not exist");
            return;
        }
        float master = PersistentDataManager.playerSettings.MasterVolume;
        float music = master * PersistentDataManager.playerSettings.MusicVolume;

        foreach (AudioFile m in INSTANCE.musicFiles) {
            m.source.volume = m.volume * music;
        }
    }

    public void CheckTransition() {
        float master = PersistentDataManager.playerSettings.MasterVolume;
        float music = master * PersistentDataManager.playerSettings.MusicVolume;
        AudioFile c = Find(sceneMusic);
        if (current == null) {
            if (c!=null)
                FadeIn(sceneMusic, music * c.volume, transitionTime);
        } else if (current != sceneMusic) {
            if (current!="")
                FadeOut(current, transitionTime);
            if (c!=null)
                FadeIn(sceneMusic, music * c.volume, transitionTime);
        }
        current = sceneMusic;
    }

    #endregion

    #region PRIVATE METHODS

    private static AudioFile Find(string name) {
        AudioFile m = Array.Find(INSTANCE.musicFiles, a => a.audioName == name);

        if (m == null) {
            Debug.LogError("Sound name " + name + " not found!" );
        }

        return m;
    }

    private IEnumerator IFadeOut(String name, float duration) {
        AudioFile m = Find(name);
        if (m != null) {
            if (fadeOut == true) {
                Debug.Log("Couldn't handle two fade outs at once: " + name + ", " + fadeOutUsedString + ". Stopped the music " + name);
                StopMusic(name);
            } else {
                fadeOut = true;
                float startVol = m.source.volume;
                fadeOutUsedString = name;
                while (m.source.volume > 0) {
                    m.source.volume -= startVol * Time.deltaTime / duration;
                    yield return null;
                }
                m.source.Stop();
                yield return new WaitForSeconds(duration);
                fadeOut = false;
            }
        }
    }

    private IEnumerator IFadeIn(string name, float targetVolume, float duration) {
        AudioFile m = Find(name);
        if (m != null) {
            if (fadeIn == true) {
                Debug.Log("Couldn't handle two fade ins at once: " + name + ", " + fadeInUsedString + ". Stopped the music " + name);
                StopMusic(fadeInUsedString);
                PlayMusic(name);
            } else {
                fadeIn = true;
                fadeInUsedString = name;
                m.source.volume = 0f;
                m.source.Play();
                while ( m.source.volume < targetVolume ) {
                    m.source.volume += Time.deltaTime / duration;
                    yield return null;
                }
                yield return new WaitForSeconds(duration);
                fadeIn = false;
            }
        }
    }

    #endregion
}
