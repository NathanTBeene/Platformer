using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;

    private void OnEnable()
    {
        startButton.onClick.AddListener(_onStartButtonClicked);
        optionsButton.onClick.AddListener(_onOptionsButtonClicked);
        exitButton.onClick.AddListener(_onExitButtonClicked);
    }

    private void _onStartButtonClicked()
    {
        Debug.Log("Start Button Clicked - Load Game Scene");
        GameController.Instance.Change2DScene("PrototypeLevel", true, true);
        GameController.Instance.UnloadScene("MainMenu");
    }

    private void _onOptionsButtonClicked()
    {
        Debug.Log("Options Button Clicked - Open Options Menu");
        // Add logic to open the options menu
    }

    private void _onExitButtonClicked()
    {
        Debug.Log("Exit Button Clicked - Quit Application");
        // Add logic to quit the application
        Application.Quit();
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveListener(_onStartButtonClicked);
        optionsButton.onClick.RemoveListener(_onOptionsButtonClicked);
        exitButton.onClick.RemoveListener(_onExitButtonClicked);
    }
}
