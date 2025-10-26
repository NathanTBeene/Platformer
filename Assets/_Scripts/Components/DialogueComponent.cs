using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueComponent : MonoBehaviour
{
    public event System.Action<GameObject> dialogueFinished;

    [Header("References")]
    public GameObject speaker;
    public DialogData dialogueData;
    public TextMeshProUGUI dialogueUI;
    public PanelPopIn panelPopIn;

    [Header("Settings")]
    public float textShowDuration = 2f;
    public bool autoClose = false;
    public float autoCloseDelay = 1f;

    [Header("Debug")]
    [Range(0f, 1f)]
    [SerializeField] private float maxVisibleCharacters = 0f;

    private void Start() {
        if (panelPopIn == null)
            panelPopIn = GetComponent<PanelPopIn>();
    }

    void OnValidate()
    {
        if (dialogueUI != null && dialogueUI.gameObject.activeInHierarchy)
            _updateVisibleCharacters();
    }

    private void _updateVisibleCharacters()
    {
        if (dialogueUI != null && dialogueUI.text != null)
        {
            int totalCharacters = dialogueUI.text.Length;
            int visibleCount = Mathf.FloorToInt(totalCharacters * maxVisibleCharacters);
            dialogueUI.maxVisibleCharacters = visibleCount;
        }
    }

    private void _setDialogueText()
    {
        if (dialogueUI == null) return;

        string text = dialogueData.GetNextDialogue();
        dialogueUI.text = text;
        dialogueUI.ForceMeshUpdate();
    }

    // An Async method to show dialogue for a set duration
    // If duration is -1, use the default displayDuration
    // Text is revealed letter by letter.
    public IEnumerator ShowDialogue(float duration = -1f, bool popIn = true)
    {
        // Set the dialogue text
        _setDialogueText();
        var displayedLine = dialogueData.CurrentLine;

        if (popIn)
            yield return panelPopIn.PopIn();


        // Reset and animate visible characters
        maxVisibleCharacters = 0f;
        _updateVisibleCharacters();

        float actualDuration = duration < 0 ? textShowDuration : duration;
        if (displayedLine != null && displayedLine.duration > 0f)
            actualDuration = displayedLine.duration;

        // Reveal text over time
        float revealProgress = 0f;
        float revealSpeed = 1f / actualDuration;
        while (revealProgress < 1f)
        {
            revealProgress += Time.deltaTime * revealSpeed;
            maxVisibleCharacters = Mathf.Clamp01(revealProgress);
            _updateVisibleCharacters();
            yield return null;
        }

        Debug.Log("Displayed Line: " + displayedLine);
        Debug.Log("Auto Next: " + (displayedLine != null ? displayedLine.autoNext.ToString() : "N/A"));
        Debug.Log("Has Next Dialogue: " + dialogueData.HasNextDialogue());
        // Check autonext on the line we just displayed
        if (displayedLine != null && displayedLine.autoNext && dialogueData.HasNextDialogue())
        {
            yield return new WaitForSeconds(displayedLine.autoDelay);
            yield return ShowDialogue(-1f, false);
        }
        else
        {
            if (autoClose)
            {
                yield return new WaitForSeconds(autoCloseDelay);
                yield return HideDialogue();
            }
            dialogueFinished?.Invoke(speaker);
        }

    }

    public IEnumerator HideDialogue()
    {
        // Wait for the text to finish revealing
        while (maxVisibleCharacters < 1f)
            yield return null;

        // Wait for a short duration before hiding
        yield return new WaitForSeconds(0.5f);
        yield return panelPopIn.PopOut();

        maxVisibleCharacters = 0f;
        _updateVisibleCharacters();
    }
}
