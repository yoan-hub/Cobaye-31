using UnityEngine;

// Put on an empty object that starts DISABLED (box unticked).
// In your Game Creator action list, add "Set Active -> true" on this object just BEFORE your Load Scene action,
// then a Wait of the same length as Fade Out Time on the ScreenFade. The screen fades to black first.
public class FadeOutTrigger : MonoBehaviour
{
    void OnEnable()
    {
        // ignore the normal enabling that happens while the scene loads
        if (Time.timeSinceLevelLoad > 0.5f) ScreenFade.FadeOutNow();
    }

    // Can also be called from a button or another script
    public void Go()
    {
        ScreenFade.FadeOutNow();
    }
}