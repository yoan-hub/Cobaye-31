using UnityEngine;

// Put on an empty object. While the guard chases you: chase sound fades in,
// and the "other sounds" get quieter + muffled. Fades back out when the chase ends.
public class ChaseAudio : MonoBehaviour
{
    public GuardAI guard;                 // Robot-1
    public AudioSource chaseSound;        // your chase clip: tick Loop, untick Play On Awake
    public float chaseVolume = 1f;

    public AudioSource[] otherSounds;     // ambience, music... (NOT the chase sound)
    public float mutedCutoff = 800f;      // lower = more muffled
    [Range(0f, 1f)] public float mutedVolume = 0.5f;   // 0.5 = half volume while chased
    public float fadeSpeed = 2f;          // higher = faster fade

    AudioLowPassFilter[] filters;
    float[] baseVolumes;
    float amount;                         // 0 = normal, 1 = full chase

    void Start()
    {
        filters = new AudioLowPassFilter[otherSounds.Length];
        baseVolumes = new float[otherSounds.Length];

        for (int i = 0; i < otherSounds.Length; i++)
        {
            baseVolumes[i] = otherSounds[i].volume;
            filters[i] = otherSounds[i].GetComponent<AudioLowPassFilter>();
            if (filters[i] == null)
                filters[i] = otherSounds[i].gameObject.AddComponent<AudioLowPassFilter>();
        }

        chaseSound.loop = true;
        chaseSound.volume = 0f;
        chaseSound.Play();
    }

    void Update()
    {
        float target = guard.chasing ? 1f : 0f;
        amount = Mathf.MoveTowards(amount, target, fadeSpeed * Time.deltaTime);

        chaseSound.volume = amount * chaseVolume;

        for (int i = 0; i < otherSounds.Length; i++)
        {
            filters[i].cutoffFrequency = Mathf.Lerp(22000f, mutedCutoff, amount);
            otherSounds[i].volume = Mathf.Lerp(baseVolumes[i], baseVolumes[i] * mutedVolume, amount);
        }
    }
}
