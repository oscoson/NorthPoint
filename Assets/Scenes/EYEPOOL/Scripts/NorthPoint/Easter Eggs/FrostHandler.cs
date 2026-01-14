using System.Collections.Generic;
using UnityEngine;
using Augmenta;

public class FrostHandler : MonoBehaviour
{
    public HashSet<AugmentaObject> usersInZone = new();
    // Stretch to one sprite instead
    public GameObject[] frostEffectObjects;
    public Color frostEffectObjectOne;
    public Color frostEffectObjectTwo;
    public Color frostEffectObjectThree;
    public Color frostEffectObjectFour;
    public Color frostEffectObjectFive;
    public Color frostEffectObjectSix;

    private AudioManager audioManager;
    private AudioVisualizer audioVisualizer;

    void Awake()
    {
        frostEffectObjectOne = frostEffectObjects[0].GetComponent<SpriteRenderer>().color;
        frostEffectObjectTwo = frostEffectObjects[1].GetComponent<SpriteRenderer>().color;
        frostEffectObjectThree = frostEffectObjects[2].GetComponent<SpriteRenderer>().color;
        frostEffectObjectFour = frostEffectObjects[3].GetComponent<SpriteRenderer>().color;
        frostEffectObjectFive = frostEffectObjects[4].GetComponent<SpriteRenderer>().color;
        frostEffectObjectSix = frostEffectObjects[5].GetComponent<SpriteRenderer>().color;

        audioManager = FindAnyObjectByType<AudioManager>();
        audioVisualizer = FindAnyObjectByType<AudioVisualizer>();
    }

    void Update()
    {
        if(usersInZone.Count > 0)
        {
            // Apply frost effect
            frostEffectObjectOne.a = Mathf.MoveTowards(frostEffectObjectOne.a, 1f, 0.5f * Time.deltaTime);
            frostEffectObjectTwo.a = Mathf.MoveTowards(frostEffectObjectTwo.a, 1f, 0.5f * Time.deltaTime);
            frostEffectObjectThree.a = Mathf.MoveTowards(frostEffectObjectThree.a, 1f, 0.5f * Time.deltaTime);
            frostEffectObjectFour.a = Mathf.MoveTowards(frostEffectObjectFour.a, 1f, 0.5f * Time.deltaTime);
            frostEffectObjectFive.a = Mathf.MoveTowards(frostEffectObjectFive.a, 1f, 0.5f * Time.deltaTime);
            frostEffectObjectSix.a = Mathf.MoveTowards(frostEffectObjectSix.a, 1f, 0.5f * Time.deltaTime);
        }
        else
        {
            // Remove frost effect
            frostEffectObjectOne.a = Mathf.MoveTowards(frostEffectObjectOne.a, 0f, 0.5f * Time.deltaTime);
            frostEffectObjectTwo.a = Mathf.MoveTowards(frostEffectObjectTwo.a, 0f, 0.5f * Time.deltaTime);
            frostEffectObjectThree.a = Mathf.MoveTowards(frostEffectObjectThree.a, 0f, 0.5f * Time.deltaTime);
            frostEffectObjectFour.a = Mathf.MoveTowards(frostEffectObjectFour.a, 0f, 0.5f * Time.deltaTime);
            frostEffectObjectFive.a = Mathf.MoveTowards(frostEffectObjectFive.a, 0f, 0.5f * Time.deltaTime);
            frostEffectObjectSix.a = Mathf.MoveTowards(frostEffectObjectSix.a, 0f, 0.5f * Time.deltaTime);
        }

        frostEffectObjects[0].GetComponent<SpriteRenderer>().color = frostEffectObjectOne;
        frostEffectObjects[1].GetComponent<SpriteRenderer>().color = frostEffectObjectTwo;
        frostEffectObjects[2].GetComponent<SpriteRenderer>().color = frostEffectObjectThree;
        frostEffectObjects[3].GetComponent<SpriteRenderer>().color = frostEffectObjectFour;
        frostEffectObjects[4].GetComponent<SpriteRenderer>().color = frostEffectObjectFive;
        frostEffectObjects[5].GetComponent<SpriteRenderer>().color = frostEffectObjectSix;
    }

    void OnTriggerEnter(Collider other)
    {
        AugmentaObject augmenta = other.GetComponent<AugmentaObject>();
        if (augmenta == null)
        {
            return;
        }
        else
        {
            
            usersInZone.Add(augmenta);
            if(usersInZone.Count == 1)
            {
                PlayFrostSound();
                audioVisualizer.PlayVOClip(audioVisualizer.easterEggClips, true, 0);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        AugmentaObject augmenta = other.GetComponent<AugmentaObject>();
        if (augmenta == null)
        {
            return;
        }
        else
        {
            usersInZone.Remove(augmenta);
        }
    }

    void PlayFrostSound()
    {
        int randInt = Random.Range(1, 6);

        switch(randInt)
        {
            case 1:
                audioManager.PlayPoint("WINDOW_FROST_1", transform.position);
                break;
            case 2:
                audioManager.PlayPoint("WINDOW_FROST_2", transform.position);
                break;
            case 3:
                audioManager.PlayPoint("WINDOW_FROST_3", transform.position);
                break;
            case 4:
                audioManager.PlayPoint("WINDOW_FROST_4", transform.position);
                break;
            case 5:
                audioManager.PlayPoint("WINDOW_FROST_5", transform.position);
                break;
            default:
                audioManager.PlayPoint("WINDOW_FROST_1", transform.position);
                break;
        }
    }
}
