using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// salle1 -> fade to black -> explosion (while black) -> load the next scene.
// The next scene fades in from black by itself (ScreenFade) and shakes (CameraShake on its camera).
// Put on an object with a Box Collider (Is Trigger) so it starts when the player walks in,
// or tick "Start When Turned On" and let one of your Game Creator actions switch the object on.
public class SceneTransition : MonoBehaviour
{
    public string sceneName = "salle-1-destroyed";   // exact scene name, must be in the Build Profiles scene list

    [Header("Explosion sound (optional, use either one, or none)")]
    public AudioSource explosionSource;               // an AudioSource already in the scene (e.g. one of your sounds)
    public AudioClip explosionClip;                   // or drag a clip in here
    [Range(0f, 1f)] public float clipVolume = 1f;

    public float holdBlack = 1.5f;                    // seconds the screen stays black (the explosion plays here)
    public MonoBehaviour[] freezeDuringTransition;    // player movement / look scripts to switch off
    public bool startWhenPlayerEnters = true;
    public bool startWhenTurnedOn = false;

    bool started;

    void OnTriggerEnter(Collider other)
    {
        if (startWhenPlayerEnters && other.CompareTag("Player")) Go();
    }

    void OnEnable()
    {
        // ignore the normal enabling that happens while the scene loads
        if (startWhenTurnedOn && Time.timeSinceLevelLoad > 0.5f) Go();
    }

    public void Go()
    {
        if (started) return;
        started = true;
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        foreach (MonoBehaviour m in freezeDuringTransition) m.enabled = false;

        // 1. fade to black
        ScreenFade fade = FindFirstObjectByType<ScreenFade>();
        if (fade != null) yield return fade.FadeOutRoutine();

        // 2. explosion while it's black
        if (explosionSource != null)
        {
            explosionSource.ignoreListenerPause = true;
            explosionSource.Play();
        }
        else if (explosionClip != null)
        {
            Vector3 pos = Camera.main != null ? Camera.main.transform.position : transform.position;
            AudioSource.PlayClipAtPoint(explosionClip, pos, clipVolume);
        }

        yield return new WaitForSecondsRealtime(holdBlack);

        // 3. load the next scene
        SceneManager.LoadScene(sceneName);
    }
}
