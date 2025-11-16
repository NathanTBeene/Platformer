using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        if (!GameController.Instance)
        {
            Debug.LogError("GameController instance not found in the scene.");
            return;
        }

        resumeButton.onClick.AddListener(OnResumeClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnDestroy()
    {
        resumeButton.onClick.RemoveListener(OnResumeClicked);
        mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
        quitButton.onClick.RemoveListener(OnQuitClicked);
    }

    private void OnResumeClicked()
    {
        GameController.Instance.TogglePauseGame();
    }

    private void OnMainMenuClicked()
    {
        GameController.Instance.BackToMainMenu();
    }

    private void OnQuitClicked()
    {
        Application.Quit();
    }
}
