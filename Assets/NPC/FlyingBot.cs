using System.Collections;
using UnityEngine;

public class FlyingBot : MonoBehaviour {
    [SerializeField] private DialogueComponent dialogueComponent;
    [SerializeField] private FlyingBotMovement movementComponent;

    // If the dialogue has been shown for a certain duration, hide it and resume movement
    // Only show dialogue after a cooldown period
    [SerializeField] private float dialogueDisplayDuration = 2f;
    [SerializeField] private float dialogueCooldownDuration = 1f;

    private Coroutine dialogueCoroutine;
    private Coroutine cooldownCoroutine;
    private bool isInCooldown = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision detected with " + other.gameObject.name);
        if (other.gameObject.CompareTag("Player") && !isInCooldown)
        {
                _showDialogueAndPauseMovement();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Collision ended with " + other.gameObject.name);
        if (other.gameObject.CompareTag("Player"))
        {
                _hideDialogueAndResumeMovement();
        }
    }

    private void _showDialogueAndPauseMovement()
    {
        if (!gameObject.activeInHierarchy) return;

        if (dialogueComponent != null && movementComponent != null)
        {
            // Stop any existing timers
            if (dialogueCoroutine != null)
            {
                StopCoroutine(dialogueCoroutine);
            }
            movementComponent.PauseMovement();

            dialogueComponent.StartCoroutine(dialogueComponent.ShowDialogue());

            // start auto-hide coroutine
            dialogueCoroutine = StartCoroutine(_dialogueDisplayCoroutine());
        }
    }

    private void _hideDialogueAndResumeMovement()
    {
        if (!gameObject.activeInHierarchy) return;

        if (dialogueComponent != null && movementComponent != null)
        {
            if (dialogueCoroutine != null)
            {
                StopCoroutine(dialogueCoroutine);
                dialogueCoroutine = null;
            }

            if (cooldownCoroutine != null)
            {
                StopCoroutine(cooldownCoroutine);
                cooldownCoroutine = null;
            }

            dialogueComponent.StartCoroutine(dialogueComponent.HideDialogue());
            movementComponent.ResumeMovement();

            cooldownCoroutine = StartCoroutine(_dialogueCooldownCoroutine());
        }
    }

    private IEnumerator _dialogueDisplayCoroutine()
    {
        yield return new WaitForSeconds(dialogueDisplayDuration);

        _hideDialogueAndResumeMovement();
    }

    private IEnumerator _dialogueCooldownCoroutine()
    {
        isInCooldown = true;
        yield return new WaitForSeconds(dialogueCooldownDuration);
        isInCooldown = false;
        cooldownCoroutine = null;
    }
}
