using System.Collections.Generic;
using UnityEngine;

public class W94_AudioManager : MonoBehaviour
{
    public static W94_AudioManager instance;
    public List<Sound> sounds = new List<Sound>();

    void Awake()
    {
        instance = this;

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        Sound sound = sounds.Find(sound => sound.name == name);
        sound.source.Play();
    }


    public void PlayOneShot(string name)
    {
        Sound sound = sounds.Find(sound => sound.name == name);
        sound.source.PlayOneShot(sound.clip);
    }

    public void Stop(string name)
    {
        Sound sound = sounds.Find(sound => sound.name == name);
        sound.source.Stop();
    }
}
