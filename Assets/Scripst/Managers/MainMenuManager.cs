using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button startGameButton;
    public Button exitButton;

    private void Start()
    {
        startGameButton.onClick.AddListener(StartGame);
        exitButton.onClick.AddListener(Exit);
    }

    void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    void Exit()
    {
        Application.Quit();
    }
}
