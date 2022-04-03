using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public AudioObj[] audioObjs;

    public static AudioManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);


        foreach(AudioObj audioObj in audioObjs)
        {
            audioObj.source = gameObject.AddComponent<AudioSource>();
            audioObj.source.clip = audioObj.clip;

            audioObj.source.volume = audioObj.volume;
            audioObj.source.pitch = audioObj.pitch;
            audioObj.source.loop = audioObj.loop;

        }
    }

    public void Play(string name)
    {
        AudioObj audioObj = Array.Find(audioObjs, sound => sound.clipName == name);
        if(audioObj == null)
        {
            Debug.LogWarning($"No sound {name} found in AudioManager...");
            return;
        }
        audioObj.source.Play();
    }

    private void Start()
    {
        Play("Theme");
    }
}
