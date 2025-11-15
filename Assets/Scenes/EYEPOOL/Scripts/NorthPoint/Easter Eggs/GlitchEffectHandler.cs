using UnityEngine;
using Augmenta;  // Required for AugmentaObject
public class GlitchEffectHandler : MonoBehaviour
{
    private SpriteRenderer glitchEffectObject;

    void Awake()
    {
        glitchEffectObject = gameObject.GetComponent<SpriteRenderer>();
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
}
