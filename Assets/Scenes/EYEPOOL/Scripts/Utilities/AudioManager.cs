using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using System;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0.1f, 3f)]
    public float pitch = 1f;
    public bool loop = false;
    public bool playAsOneShot = false;
    public AudioMixerGroup mixer;
    
    [Header("Fade Settings")]
    public bool useFadeIn = false;
    [Range(0.1f, 10f)]
    public float fadeInTime = 1f;
    public bool useFadeOut = false;
    [Range(0.1f, 10f)]
    public float fadeOutTime = 1f;
    
    [HideInInspector]
    public AudioSource source;
}

public class AudioManager : MonoBehaviour
{
    [SerializeField] private bool debugMode = false;
    public Sound[] sounds;
    
    void Awake()
    {
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.mixer;
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        BackgroundSounds();
        PeriodicSoundHandler(); 
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) 
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        
        if (s.useFadeIn)
        {
            StartCoroutine(FadeIn(s, s.fadeInTime));
        }
        else
        {
            if (s.playAsOneShot)
            {
                s.source.PlayOneShot(s.clip, s.volume);
            }
            else
            {
                s.source.volume = s.volume;
                s.source.Play();                
            }

        }

        if(debugMode)
        {
            Debug.Log("Playing sound: " + s.name);
        }
    }
    
    public void PlayPoint(string name, Vector3 position)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        
        if (s == null) return;

        if(debugMode)
        {
            Debug.Log("Playing 3D sound: " + s.name + " at position " + position.ToString());   
        }
        s.source.spatialBlend = 1.0f; 
        AudioSource.PlayClipAtPoint(s.clip, position, s.volume);
    }

    public void StopLoop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;

        if (s.useFadeOut)
        {
            StartCoroutine(FadeOut(s, s.fadeOutTime));
        }
        else
        {
            s.source.Stop();
        }
    }

    public void Pause(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        s.source.Pause();
    }
    
    public void UnPause(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        s.source.UnPause();
    }

    public void IncreaseVolume(string name, float amount)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        s.source.volume = Mathf.Clamp(s.source.volume + amount, 0.1f, 1f);
    }

    public void DecreaseVolume(string name, float amount)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        s.source.volume = Mathf.Clamp(s.source.volume - amount, 0.1f, 1f);
    }
    
    // Fade in over specified time
    public void FadeIn(string name, float fadeTime)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        StartCoroutine(FadeIn(s, fadeTime));
    }
    
    // Fade out over specified time
    public void FadeOut(string name, float fadeTime)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        StartCoroutine(FadeOut(s, fadeTime));
    }

    private void BackgroundSounds()
    {
        Play("BGM");
        Play("THUNDERSTORM");
        Play("CRT_SCREEN");
        Play("NOTES_FLYING");
    }
    
    private IEnumerator FadeIn(Sound sound, float fadeTime)
    {
        sound.source.volume = 0f;
        sound.source.Play();
        
        float startVolume = 0f;
        float targetVolume = sound.volume;
        
        while (sound.source.volume < targetVolume)
        {
            sound.source.volume += (targetVolume - startVolume) * Time.deltaTime / fadeTime;
            yield return null;
        }
        
        sound.source.volume = targetVolume;
    }
    
    private IEnumerator FadeOut(Sound sound, float fadeTime)
    {
        float startVolume = sound.source.volume;
        
        while (sound.source.volume > 0)
        {
            sound.source.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
        
        sound.source.Stop();
        sound.source.volume = sound.volume; // Reset to original volume
    }

    private void PeriodicSoundHandler()
    {
        StartCoroutine(RandomAmbientSoundCoroutine());
    }

    private IEnumerator RandomAmbientSoundCoroutine()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(2, 11));

        int randInt = UnityEngine.Random.Range(1, 15);

        switch(randInt)
        {
            case 1:
                Play("ELEC_1");
                break;
            case 2:
                Play("ELEC_2");
                break;
            case 3:
                Play("ELEC_3");
                break;
            case 4:
                Play("ELEC_4");
                break;
            case 5:
                Play("COMPNOISE_1");
                break;
            case 6:
                Play("COMPNOISE_2");
                break;
            case 7:
                Play("COMPNOISE_3");
                break;
            case 8:
                Play("COMPNOISE_4");
                break;
            case 9:
                Play("COMPNOISE_5");
                break;
            case 10:
                Play("COMPNOISE_6");
                break;
            case 11:
                Play("DIAL_1");
                break;
            case 12:
                Play("DIAL_2");
                break;
            case 13:
                Play("DIAL_3");
                break;
            case 14:
                Play("DIAL_4");
                break;
            default:
                Play("ELEC_1");
                break;
        }
        StartCoroutine(RandomAmbientSoundCoroutine());
    }
}
