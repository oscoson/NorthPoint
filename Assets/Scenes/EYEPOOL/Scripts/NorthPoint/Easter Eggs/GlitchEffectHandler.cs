using UnityEngine;
using Augmenta;  // Required for AugmentaObject
public class GlitchEffectHandler : MonoBehaviour
{
    private SpriteRenderer glitchEffectObject;
    private AudioManager audioManager;
    private AudioVisualizer audioVisualizer;

    void Awake()
    {
        glitchEffectObject = gameObject.GetComponent<SpriteRenderer>();
        audioManager = FindAnyObjectByType<AudioManager>();
        audioVisualizer = FindAnyObjectByType<AudioVisualizer>();
    }

    void OnTriggerEnter(Collider other)
    {
        AugmentaObject augmenta = other.GetComponent<AugmentaObject>();
        if (augmenta == null)
        {
            return;
        }
        
        if(glitchEffectObject.enabled)
        {
            return;
        }
        else
        {
            glitchEffectObject.enabled = true;
            PlayGlitchSound();
            audioVisualizer.PlayVOClip(audioVisualizer.easterEggClips, true, 1);
        }
    }

    void OnTriggerExit(Collider other)
    {
        AugmentaObject augmenta = other.GetComponent<AugmentaObject>();
        if (augmenta == null)
        {
            return;
        }
        if(glitchEffectObject.enabled == false)
        {
            return;
        }
        else
        {
            glitchEffectObject.enabled = false;
        }
    }

    void PlayGlitchSound()
    {
        int randInt = Random.Range(1, 9);

        switch (randInt)
        {
            case 1:
                audioManager.PlayPoint("TVGLITCH_1", transform.position);
                break;
            case 2:
                audioManager.PlayPoint("TVGLITCH_2", transform.position);
                break;
            case 3:
                audioManager.PlayPoint("TVGLITCH_3", transform.position);
                break;
            case 4:
                audioManager.PlayPoint("TVGLITCH_4", transform.position);
                break;
            case 5:
                audioManager.PlayPoint("TVGLITCH_5", transform.position);
                break;
            case 6:
                audioManager.PlayPoint("TVGLITCH_6", transform.position);
                break;
            case 7:
                audioManager.PlayPoint("TVGLITCH_7", transform.position);
                break;
            case 8:
                audioManager.PlayPoint("TVGLITCH_8", transform.position);
                break;
            default:
                audioManager.PlayPoint("TVGLITCH_1", transform.position);
                break;
        }
    }
}
