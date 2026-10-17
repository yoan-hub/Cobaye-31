using UnityEngine;

// Put on the Player (needs a CharacterController).
// Loops one long walking clip while you move, pauses it when you stop.
public class FootstepLoop : MonoBehaviour
{
    public AudioClip walkLoop;                  // your 4 second walking clip
    [Range(0f, 1f)] public float volume = 0.6f;
    public float normalSpeed = 6f;              // your player's walk speed (steps speed up/slow down around this)
    [Range(0.5f, 1.5f)] public float speedMultiplier = 1f;   // below 1 = slower steps, above 1 = faster

    CharacterController controller;
    AudioSource source;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        source = gameObject.AddComponent<AudioSource>();
        source.clip = walkLoop;
        source.loop = true;
        source.volume = volume;
        source.spatialBlend = 0f;   // 2D, always heard clearly
        source.playOnAwake = false;
    }

    void Update()
    {
        Vector3 v = controller.velocity;
        v.y = 0f;

        bool moving = controller.isGrounded && v.magnitude > 0.2f;

        if (moving)
        {
            source.pitch = Mathf.Clamp(v.magnitude / normalSpeed, 0.7f, 1.3f) * speedMultiplier;
            if (!source.isPlaying) source.Play();
        }
        else if (source.isPlaying)
        {
            source.Pause();
        }
    }
}
