using UnityEngine;
using UnityEngine.SceneManagement;

public class MonScript : MonoBehaviour
{
    public string nomDeLaScene = "stage1";

    public void ChangeLaScene()
    {
        SceneManager.LoadScene(nomDeLaScene);
    }
}