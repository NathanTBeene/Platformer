using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueComponent : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public DialogData dialogueData;
    public TextMeshProUGUI dialogueUI;
    public float textShowDuration = 2f;
    public PanelPopIn panelPopIn;

    private string currentText => dialogueData.GetCurrentDialogue() ?? "";

    [Header("Debug")]
    [Range(0f, 1f)]
    [SerializeField] private float maxVisibleCharacters = 0f;

    private void Start() {
        if (panelPopIn == null)
        {
            panelPopIn = GetComponent<PanelPopIn>();
        }
        _updateVisibleCharacters();
    }

    void OnValidate()
    {
        if (dialogueUI != null && dialogueUI.gameObject.activeInHierarchy)
        {
            _updateVisibleCharacters();
        }
    }

    private void _updateVisibleCharacters()
    {
        if (dialogueUI != null)
        {
            int totalCharacters = currentText.Length;
            int visibleCount = Mathf.FloorToInt(totalCharacters * maxVisibleCharacters);
            dialogueUI.maxVisibleCharacters = visibleCount;
        }
    }

    private void _updateGUIText()
    {
        if (dialogueUI != null)
        {
            dialogueUI.text = dialogueData.GetNextDialogue();
            dialogueUI.ForceMeshUpdate();
        }
    }

    // An Async method to show dialogue for a set duration
    // If duration is -1, use the default displayDuration
    // Text is revealed letter by letter.
    public IEnumerator ShowDialogue(float duration = -1f)
    {
        yield return panelPopIn.PopIn();
        _updateGUIText();

        maxVisibleCharacters = 0f;
        _updateVisibleCharacters();

        float actualDuration = duration < 0 ? textShowDuration : duration;

        float revealProgress = 0f;
        float revealSpeed = 1f / actualDuration;
        while (revealProgress < 1f)
        {
            revealProgress += Time.deltaTime * revealSpeed;
            maxVisibleCharacters = Mathf.Clamp01(revealProgress);
            _updateVisibleCharacters();
            yield return null;
        }
    }

    public IEnumerator HideDialogue()
    {
        Debug.Log("Hiding dialogue");
        yield return panelPopIn.PopOut();
        maxVisibleCharacters = 0f;
        _updateVisibleCharacters();
    }

    [ContextMenu("Test Show Dialogue")]
    private void _testShowDialogue()
    {
        dialogueUI.gameObject.SetActive(true);
        StartCoroutine(ShowDialogue());
    }

    [ContextMenu("Test Hide Dialogue")]
    private void _testHideDialogue()
    {
        HideDialogue();
    }
}
