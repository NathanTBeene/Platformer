using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private OutputNode outputNode;

    [SerializeField] private InputNode continueInputNode;
    [SerializeField] private Button continueButton;
    private void OnEnable()
    {
        startButton.onClick.AddListener(_onStartButtonClicked);
        continueButton.onClick.AddListener(_onContinueButtonClicked);
        exitButton.onClick.AddListener(_onExitButtonClicked);

    }

    private void Start()
    {
        if (continueButton != null && GameController.Instance.HasSaveData())
        {
            continueButton.gameObject.SetActive(true);
            continueInputNode.setState(true);
        } else {
            continueButton.gameObject.SetActive(false);
            continueInputNode.setState(false);
        }
    }

    private void _onStartButtonClicked()
    {
        if (!outputNode.isActive) return;
        GameController.Instance.StartNewGame();
    }

    private void _onContinueButtonClicked()
    {
        if (!outputNode.isActive) return;
        GameController.Instance.ContinueGame();
    }

    private void _onExitButtonClicked()
    {
        if (!outputNode.isActive) return;
        Application.Quit();
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveListener(_onStartButtonClicked);
        exitButton.onClick.RemoveListener(_onExitButtonClicked);
        continueButton.onClick.RemoveListener(_onContinueButtonClicked);
    }
}
