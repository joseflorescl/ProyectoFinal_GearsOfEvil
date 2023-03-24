using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void ReloadGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ShowGameOver()
    {
        SceneManager.LoadScene("Game Over");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
