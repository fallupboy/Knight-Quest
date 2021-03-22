using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadMenuScene()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void LoadHowToPlayScene()
    {
        SceneManager.LoadScene("How To Play Menu");
    }

    public void LoadStartLevel()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void LoadNextScene()
    {
        var currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene + 1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
