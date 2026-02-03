using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Animator gameStateAnimator;
    public SpriteRenderer[] greenLights;
    public SpriteRenderer[] redLights;
    public SpriteRenderer[] yellowLights;
    public SpriteRenderer[] purpleLights;
    
    private Color baseGreenLight;
    private Color baseRedLight;
    private Color baseYellowLight;
    private Color basePurpleLight;

    private AudioVisualizer audioVisualizer;

    void Awake()
    {
        audioVisualizer = FindAnyObjectByType<AudioVisualizer>();
        baseGreenLight = greenLights[0].color;
        baseRedLight = redLights[0].color;
        baseYellowLight = yellowLights[0].color;
        basePurpleLight = purpleLights[0].color;
    }
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
        if(index == 4 && FindAnyObjectByType<BallSpawner>().GetBallsLeftToSpawn() > 0 && FindAnyObjectByType<BallSpawner>().GetBalls() > 1)
        {
            audioVisualizer.PlayVOClip(audioVisualizer.fullConduitClips);
        }
    }

    public void DeactivateAllLights()
    {
        foreach (SpriteRenderer light in greenLights)
        {
            light.color = baseGreenLight;
        }
        foreach (SpriteRenderer light in redLights)
        {
            light.color = baseRedLight;
        }
        foreach (SpriteRenderer light in yellowLights)
        {
            light.color = baseYellowLight;
        }
        foreach (SpriteRenderer light in purpleLights)
        {
            light.color = basePurpleLight;
        }
        audioVisualizer.PlayVOClip(audioVisualizer.gameCompleteClips);
    }

    public void SetGameState(string state)
    {
        gameStateAnimator.SetTrigger(state);
    }
}
