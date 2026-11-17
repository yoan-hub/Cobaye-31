using System.Collections;
using UnityEngine;

// Plays your messages one after another, can switch to fixed cameras along the way,
// then goes back to the player camera at the end.
public class MessageSequence : MonoBehaviour
{
    [System.Serializable]
    public class Step
    {
        public GameObject message;   // message1, message2... (shown during this step)
        public float duration = 3f;  // seconds on screen
        public Camera camera;        // fixed camera for this step. Leave empty = keep the current camera
    }

    public Camera playerCamera;                     // the camera the player normally uses
    public Step[] steps;                            // one line per message
    public GameObject textbox;                      // your Textbox panel, shown during the whole sequence
    public MonoBehaviour[] freezeDuringSequence;    // player movement / look scripts to switch off
    public GameObject[] hideDuringSequence;         // crosshair, objectives, bar... (things that are normally visible)
    public bool freezeTime = true;                  // also stops the robots while it plays
    public bool startWhenPlayerEnters = true;       // needs a Box Collider (Is Trigger) on this object
    public bool onlyOnce = true;

    bool running, played;

    void OnTriggerEnter(Collider other)
    {
        if (startWhenPlayerEnters && other.CompareTag("Player")) Play();
    }

    // Can also be called from anywhere else (a button, another script)
    public void Play()
    {
        if (running || (onlyOnce && played)) return;
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        running = true;
        played = true;

        foreach (MonoBehaviour m in freezeDuringSequence) m.enabled = false;
        foreach (GameObject g in hideDuringSequence) g.SetActive(false);
        if (textbox != null) textbox.SetActive(true);
        if (freezeTime) Time.timeScale = 0f;

        Camera current = playerCamera;

        foreach (Step s in steps)
        {
            if (s.camera != null && s.camera != current)
            {
                current.enabled = false;
                s.camera.enabled = true;
                current = s.camera;
            }

            if (s.message != null) s.message.SetActive(true);
            yield return new WaitForSecondsRealtime(s.duration);
            if (s.message != null) s.message.SetActive(false);
        }

        // back to the player
        if (current != playerCamera)
        {
            current.enabled = false;
            playerCamera.enabled = true;
        }

        if (textbox != null) textbox.SetActive(false);
        foreach (GameObject g in hideDuringSequence) g.SetActive(true);
        foreach (MonoBehaviour m in freezeDuringSequence) m.enabled = true;
        if (freezeTime) Time.timeScale = 1f;

        running = false;
    }

    // Never leave the game frozen if this gets turned off / the scene reloads mid-sequence
    void OnDisable()
    {
        if (running && freezeTime) Time.timeScale = 1f;
    }
}
