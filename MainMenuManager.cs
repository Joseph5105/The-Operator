using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Main Scene"); // Replace with your actual scene name
    }

    public void OpenSettings()
    {
        Debug.Log("Settings clicked");
        // You can add settings panel logic here later
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}