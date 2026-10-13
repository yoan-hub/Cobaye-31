using UnityEngine;

// Put on any object with a Light component.
// Random flicker: min/max wait different. Steady strobe: set them to the same value.
[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    public float onIntensity = 3f;
    public float minWait = 0.05f;
    public float maxWait = 0.4f;

    Light lt;
    float timer;

    void Start()
    {
        lt = GetComponent<Light>();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        lt.intensity = lt.intensity > 0f ? 0f : onIntensity;   // flip on/off
        timer = Random.Range(minWait, maxWait);
    }
}
