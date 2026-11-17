using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Put on the Canvas. Drag the full-screen BLACK Image (last child of the Canvas) into "black".
public class ScreenFade : MonoBehaviour
{
    public Image black;
    public float fadeInTime = 1.5f;   // fade from black when the scene starts
    public float fadeOutTime = 1f;    // fade to black before loading another scene
    public bool fadeInOnStart = true;
    public float startDelay = 0.4f;   // stays black this long when the scene starts (hides the loading hitch)

    static ScreenFade instance;
    Coroutine current;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (fadeInOnStart) Run(FadeInAtStart());
        else SetAlpha(0f);
    }

    IEnumerator FadeInAtStart()
    {
        SetAlpha(1f);
        yield return new WaitForSecondsRealtime(startDelay);
        yield return Fade(1f, 0f, fadeInTime);
    }

    // Screen goes black instantly, then fades back in. Used by the hiding zones.
    public static void FadeInNow(float time)
    {
        if (instance != null) instance.Run(instance.Fade(1f, 0f, time));
    }

    // Hook this to a UI Button's OnClick, type the scene name in the box
    public void FadeAndLoad(string sceneName)
    {
        FadeAndLoad(sceneName, 0f);
    }

    // holdBlack = seconds to stay fully black before loading (lets a sound finish)
    public void FadeAndLoad(string sceneName, float holdBlack)
    {
        Run(FadeOutThenLoad(sceneName, holdBlack));
    }

    // For other scripts: fades to black, then lets them continue (used by SceneTransition)
    public IEnumerator FadeOutRoutine()
    {
        if (current != null) StopCoroutine(current);
        yield return Fade(black.color.a, 1f, fadeOutTime);
    }

    void Run(IEnumerator routine)
    {
        if (current != null) StopCoroutine(current);
        current = StartCoroutine(routine);
    }

    IEnumerator FadeOutThenLoad(string sceneName, float holdBlack)
    {
        yield return Fade(black.color.a, 1f, fadeOutTime);
        if (holdBlack > 0f) yield return new WaitForSecondsRealtime(holdBlack);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator Fade(float from, float to, float time)
    {
        float t = 0f;
        while (t < time)
        {
            t += Mathf.Min(Time.unscaledDeltaTime, 0.05f);   // a lag spike can't swallow the fade
            SetAlpha(Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t / time)));   // eases in and out
            yield return null;
        }
        SetAlpha(to);
    }

    void SetAlpha(float a)
    {
        black.color = new Color(0f, 0f, 0f, a);
    }
}