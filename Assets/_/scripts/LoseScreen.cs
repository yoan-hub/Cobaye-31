using UnityEngine;
using UnityEngine.SceneManagement;

// Put on an empty object in your LOSE SCREEN scene (the "loser" scene).
// Hook the buttons' OnClick to: Retry (Continuer), GoToMenu (Menu), Quit (Quitter).
public class LoseScreen : MonoBehaviour
{
    public string menuScene = "menu";         // name of your main menu scene
    public string fallbackScene = "salle1";   // used only if the game doesn't know where you died

    void Awake()
    {
        SceneMemory.Ignore(gameObject.scene.name);   // the lose screen is not a level
        Time.timeScale = 1f;                         // in case you died during a frozen moment
        Cursor.lockState = CursorLockMode.None;      // mouse for the buttons
        Cursor.visible = true;
    }

    // Continuer: back to the start of the level you died in
    public void Retry()
    {
        string target = string.IsNullOrEmpty(SceneMemory.lastLevel) ? fallbackScene : SceneMemory.lastLevel;
        Load(target);
    }

    // Menu
    public void GoToMenu()
    {
        Load(menuScene);
    }

    // Quitter
    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;   // so the button also works in the editor
#endif
    }

    void Load(string sceneName)
    {
        ScreenFade fade = FindFirstObjectByType<ScreenFade>();
        if (fade != null) fade.FadeAndLoad(sceneName);
        else SceneManager.LoadScene(sceneName);
    }
}
