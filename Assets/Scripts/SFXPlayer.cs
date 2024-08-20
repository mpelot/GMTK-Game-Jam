using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public static SFXPlayer instance;

    public List<SFXClip> sfxClips;
    public AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySFX(string clipName)
    {
        foreach (SFXClip clip in sfxClips)
        {
            if (clip.clipName == clipName)
            {
                audioSource.PlayOneShot(clip.clip, clip.volume);
                return;
            }
        }
    }
}

[System.Serializable]
public class SFXClip
{
    public string clipName;
    public AudioClip clip;
    public float volume = 1.0f;
}