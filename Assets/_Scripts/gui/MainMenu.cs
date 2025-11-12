using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private OutputNode outputNode;

    private void OnEnable()
    {
        startButton.onClick.AddListener(_onStartButtonClicked);
        exitButton.onClick.AddListener(_onExitButtonClicked);
    }

    private void _onStartButtonClicked()
    {
        if (!outputNode.isActive) return;
        Debug.Log("Start Button Clicked - Load Game Scene");
        GameController.Instance.Change2DScene("Level1", true, true);
        GameController.Instance.UnloadScene("MainMenu");
    }

    private void _onExitButtonClicked()
    {
        if (!outputNode.isActive) return;
        Debug.Log("Exit Button Clicked - Quit Application");
        // Add logic to quit the application
        Application.Quit();
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveListener(_onStartButtonClicked);
        exitButton.onClick.RemoveListener(_onExitButtonClicked);
    }
}
