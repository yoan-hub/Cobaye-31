using UnityEngine;
using UnityEngine.UI;

// Put on the Canvas (not on the Slider itself). Drag the UI Slider into "bar".
// Finds every GuardAI in the scene automatically and shows the highest danger.
public class DangerBar : MonoBehaviour
{
    public Slider bar;
    public bool drain = false;        // false = bar fills up as you get caught, true = bar drains
    public bool alwaysShow = true;    // true = bar is always on screen, false = only shows while in danger

    GuardAI[] guards;

    void Start()
    {
        bar.minValue = 0f;
        bar.maxValue = 1f;
        guards = FindObjectsByType<GuardAI>(FindObjectsSortMode.None);
    }

    void Update()
    {
        float danger = 0f;
        foreach (GuardAI g in guards)
            danger = Mathf.Max(danger, g.danger);

        bar.gameObject.SetActive(alwaysShow || danger > 0.01f);
        bar.value = drain ? 1f - danger : danger;
    }
}