using UnityEngine;
using UnityEngine.Rendering;

// Put on the camera that shows the game (your MainCamera).
// The shake is only applied while the frame is drawn, so it doesn't fight your player controller.
[RequireComponent(typeof(Camera))]
public class CameraShake : MonoBehaviour
{
    public bool shakeOnStart = true;
    public float startDelay = 0f;     // extra seconds before the shake begins (on top of the black screen at the start)
    public float intensity = 0.15f;   // how far the camera moves (meters)
    public float rotation = 2f;       // how much it twists (degrees)
    public float duration = 3.5f;     // the shake fades out over this time

    Camera cam;
    float delayLeft, timeLeft, strength;
    Vector3 savedPos;
    Quaternion savedRot;
    bool applied;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += OnBegin;   // URP / HDRP
        RenderPipelineManager.endCameraRendering += OnEnd;
    }

    void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBegin;
        RenderPipelineManager.endCameraRendering -= OnEnd;
        Restore();
    }

    void Start()
    {
        if (!shakeOnStart) return;

        Shake();

        // wait for the black screen at the start of the scene, so the shake isn't wasted while nothing is visible
        ScreenFade fade = FindFirstObjectByType<ScreenFade>();
        if (fade != null && fade.fadeInOnStart) delayLeft += fade.startDelay;
    }

    // Can also be called from other scripts: GetComponent<CameraShake>().Shake();
    public void Shake()
    {
        delayLeft = startDelay;
        timeLeft = duration;
        Debug.Log("CameraShake: started on " + name);
    }

    void Update()
    {
        float dt = Time.unscaledDeltaTime;

        if (delayLeft > 0f)
        {
            delayLeft -= dt;
            strength = 0f;
            return;
        }

        if (timeLeft > 0f)
        {
            timeLeft -= dt;
            float t = Mathf.Clamp01(timeLeft / duration);
            strength = t * t;            // strong at first, then fades out smoothly
        }
        else strength = 0f;
    }

    void OnBegin(ScriptableRenderContext context, Camera c) { if (c == cam) Apply(); }
    void OnEnd(ScriptableRenderContext context, Camera c) { if (c == cam) Restore(); }

    void OnPreCull() { Apply(); }        // built-in render pipeline
    void OnPostRender() { Restore(); }

    void Apply()
    {
        if (strength <= 0f || applied) return;

        savedPos = transform.localPosition;
        savedRot = transform.localRotation;
        transform.localPosition += Random.insideUnitSphere * intensity * strength;
        transform.localRotation *= Quaternion.Euler(Random.insideUnitSphere * rotation * strength);
        applied = true;
    }

    void Restore()
    {
        if (!applied) return;

        transform.localPosition = savedPos;
        transform.localRotation = savedRot;
        applied = false;
    }
}