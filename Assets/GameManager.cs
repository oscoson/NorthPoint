using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SpriteRenderer[] greenLights;
    public SpriteRenderer[] redLights;
    public SpriteRenderer[] yellowLights;
    public SpriteRenderer[] purpleLights;

    public void ActivateLights(string color, int index)
    {
        switch(color)
        {
            case "Green":
                greenLights[index].color = new Color(0, 255, 30);
                break;
            case "Red":
                redLights[index].color = Color.red;
                break;
            case "Yellow":
                yellowLights[index].color = new Color(255, 184, 0);
                break;
            case "Purple":
                purpleLights[index].color = new Color(113, 0 , 255);
                break;
        }
    }

    public void DeactivateAllLights()
    {
        foreach (SpriteRenderer light in greenLights)
        {
            light.color = new Color(0, 43, 5);
        }
        foreach (SpriteRenderer light in redLights)
        {
            light.color = new Color(83, 10, 0);
        }
        foreach (SpriteRenderer light in yellowLights)
        {
            light.color = new Color(61, 44, 0);
        }
        foreach (SpriteRenderer light in purpleLights)
        {
            light.color = new Color(54, 23, 93);
        }
    }
}
