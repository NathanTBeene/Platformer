using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        if (GameController.Instance)
        {
            resumeButton.onClick.AddListener(OnResumeClicked);
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            quitButton.onClick.AddListener(OnQuitClicked);
        }
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
