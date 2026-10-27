using System.Text;
using TMPro;
using UnityEngine;

// Put on the Canvas. Drag the TextMeshPro text for the TASKS into "text".
// Your own "Objectifs" title stays a normal text object, this only writes the tasks under it.
// Finished tasks disappear.
public class ObjectiveUI : MonoBehaviour
{
    public TMP_Text text;
    public string[] objectives = { "Ramasser la carte d'accès", "Activer le terminal", "Prendre l'ascenseur" };
    public Color accentColor = new Color(0.25f, 0.88f, 0.82f);   // color of the ">" marker
    public GameObject[] hideWhenAllDone;   // optional: drag your "Objectifs" title (and panel) here

    static ObjectiveUI instance;
    bool[] done;

    void Awake()
    {
        instance = this;
        done = new bool[objectives.Length];
        Refresh();
    }

    // index starts at 0 (first task = 0)
    public static void Complete(int index)
    {
        if (instance == null || index < 0 || index >= instance.done.Length) return;
        instance.done[index] = true;
        instance.Refresh();
    }

    void Refresh()
    {
        string accent = ColorUtility.ToHtmlStringRGB(accentColor);
        StringBuilder sb = new StringBuilder();
        int remaining = 0;

        for (int i = 0; i < objectives.Length; i++)
        {
            if (done[i]) continue;
            sb.AppendLine("<color=#" + accent + ">></color> " + objectives[i]);
            remaining++;
        }

        text.text = sb.ToString().TrimEnd();

        foreach (GameObject g in hideWhenAllDone)
            if (g != null) g.SetActive(remaining > 0);
    }
}