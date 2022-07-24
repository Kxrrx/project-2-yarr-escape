using UnityEngine.Audio;
using System;
using UnityEngine;

/// <summary>
/// Modified code from Brackeys "Introduction to AUDIO in Unity", YouTube tutorial:  https://www.youtube.com/watch?v=6OT43pvUyfY
/// </summary>

public class AudioManager : MonoBehaviour {

    public static AudioManager singleton;
    public Sound[] sounds;

    void Awake()
    {
        if(singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);

        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, Sound => Sound.name == name);
        if(s == null)
        {
            Debug.LogWarning("Sounds: " + name + " was not found!");
            return;
        }
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, Sound => Sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sounds: " + name + " was not found!");
            return;
        }
        s.source.Stop();
    }
}
