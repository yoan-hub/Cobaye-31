using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Remembers the last level you played, so the lose screen can send you back to it.
// Nothing to put in a scene: it starts by itself.
public static class SceneMemory
{
    public static string lastLevel;
    static readonly HashSet<string> ignored = new HashSet<string>();   // scenes that are NOT levels (the lose screen)

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Reset()
    {
        lastLevel = null;
        ignored.Clear();
        SceneManager.sceneLoaded -= OnLoaded;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        SceneManager.sceneLoaded += OnLoaded;
        Save(SceneManager.GetActiveScene());
    }

    static void OnLoaded(Scene scene, LoadSceneMode mode)
    {
        Save(scene);
    }

    static void Save(Scene scene)
    {
        if (!ignored.Contains(scene.name)) lastLevel = scene.name;
    }

    // Called by LoseScreen so the lose screen itself is never remembered as a level
    public static void Ignore(string sceneName)
    {
        ignored.Add(sceneName);
    }
}
