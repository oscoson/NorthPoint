using UnityEngine;

public class AudioVisualizer : MonoBehaviour
{
    [Header("References")]
    public AudioSource audioSource;
    public Transform[] bars;
 
    [Header("Settings")]
    public FrequencyFocusWindow frequencyFocusWindow;
    public float amplification = 1.0f;
    public float baseHeight = 0.0f;
    public FFTWindow fftWindow;
    public bool useDecibels;
    [SerializeField] private float voiceOverClipCountdown;
    [SerializeField] private float ambienceVoiceOverCountdown;
    private float ambienceVoiceOverTimer;
    private float voiceOverClipDuration;

    [Header("State")]
    public bool isPlaying = false;
    public float[] spectrumData;

    [Header("VO Audio Clip Triggers")]
    public AudioClip[] introClips;
    public AudioClip[] ambienceVOClips;
    public AudioClip[] easterEggClips;
    public AudioClip[] energyBallCaptureClips;
    public AudioClip[] energyBallDropClips;
    public AudioClip[] fullConduitClips;
    public AudioClip[] gameCompleteClips;
 
    void Awake()
    {
        // Must be a power of 2 number, between 64 and 8192
        spectrumData = new float[4096];
        ambienceVoiceOverTimer = ambienceVoiceOverCountdown;
        voiceOverClipDuration = voiceOverClipCountdown;
    }
    
    void Start()
    {
        PlayVOClip(introClips);
    }
 
    void Update()
    {
        VOTimer();
        AmbienceVOCountdown();

        audioSource.GetSpectrumData(spectrumData, 0, fftWindow);
        var blockSize = spectrumData.Length / bars.Length / (int)frequencyFocusWindow;
        for (int i = 0; i < bars.Length; ++i)
        {
            float sum = 0;
            for (int j = 0; j < blockSize; j++)
            {
                sum += spectrumData[i * blockSize + j];
            }
            sum /= blockSize;
            float amplitude = Mathf.Clamp(sum, 1e-7f, 1f);
            var scale = bars[i].localScale;
            if (useDecibels)
            {
                scale.y = -Mathf.Log10(amplitude) * amplification / 200;
            }
            else
            {
                scale.y = sum * amplification + baseHeight;
            }
            bars[i].localScale = scale;
        }
    }

    public void PlayVOClip(AudioClip[] audioClips, bool isEasterEgg = false, int index = 0)
    {
        if(!isPlaying)
        {
            isPlaying = true;
            if(!isEasterEgg)
            {
                audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
                audioSource.Play();
            }
            else
            {
                audioSource.clip = audioClips[index];
                audioSource.Play();
            }       
        }

    }

    private void AmbienceVOCountdown()
    {
        if(!isPlaying)
        {
            ambienceVoiceOverCountdown -= Time.deltaTime;
        }
        if(ambienceVoiceOverCountdown <= 0f)
        {
            ambienceVoiceOverCountdown = ambienceVoiceOverTimer;
            PlayVOClip(ambienceVOClips);
        }
    }

    private void VOTimer()
    {
        voiceOverClipCountdown -= Time.deltaTime;
        if(voiceOverClipCountdown <= 0f)
        {
            isPlaying = false;
            voiceOverClipCountdown = voiceOverClipDuration;
        }
    }


}

public enum FrequencyFocusWindow
{
    Entire = 1,
    FirstHalf = 2,
    FirstQuarter = 4,
    FirstEight = 8,
    FirstSixteenth = 16
}
