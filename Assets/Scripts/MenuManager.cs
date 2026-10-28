using UnityEngine;
using UnityEngine.SceneManagement; 

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadSceneAsync(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("Son sahnedesin! Daha ileri gidilemez.");
        }
    }

    public void BackButton()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int prevSceneIndex = currentSceneIndex - 1;

        if (prevSceneIndex >= 0)
        {
            SceneManager.LoadSceneAsync(prevSceneIndex);
        }
    }

    public void QuitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}