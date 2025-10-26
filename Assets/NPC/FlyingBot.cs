using System.Collections;
using DG.Tweening;
using UnityEngine;

public class FlyingBot : MonoBehaviour {
    [SerializeField] private DialogueComponent dialogueComponent;
    [SerializeField] private FlyingBotMovement movementComponent;
    [SerializeField] private GameObject spriteObject;

    [SerializeField] private float dialogueCooldown = 2.5f;

    private bool canSpeak = true;
    private Coroutine cooldownCoroutine = null;


    private void OnEnable() {
        dialogueComponent.dialogueFinished += _onDialogueFinished;
    }

    private void OnDisable()
    {
        dialogueComponent.dialogueFinished -= _onDialogueFinished;
    }

    private void _onDialogueFinished(GameObject speaker)
    {
        if (speaker != gameObject) return;

        if (cooldownCoroutine != null)
            StopCoroutine(cooldownCoroutine);

        cooldownCoroutine = StartCoroutine(DialogueCooldown());
        ResumeMovement();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!canSpeak) return;
            canSpeak = false;
            PauseMovement();
            TriggerDialogue();
        }
    }

    public void TriggerDialogue()
    {
        if (dialogueComponent == null) return;
        _facePlayer();
        dialogueComponent.StartCoroutine(dialogueComponent.ShowDialogue());
    }

    public void PauseMovement()
    {
        if (movementComponent == null) return;
        movementComponent.PauseMovement();
    }

    public void ResumeMovement()
    {
        if (movementComponent == null) return;
        movementComponent.ResumeMovement();
    }

    public void StopMovement()
    {
        if (movementComponent == null) return;
        movementComponent.StopMovement();
    }

    public void _facePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector3 directionToPlayer = player.transform.position - transform.position;
        if (directionToPlayer.x > 0)
        {
            // Face right
            spriteObject.transform.DOScaleX(1f, 0.2f);
        }
        else
        {
            // Face left
            spriteObject.transform.DOScaleX(-1f, 0.2f);
        }
    }

    private IEnumerator DialogueCooldown()
    {
        yield return new WaitForSeconds(dialogueCooldown);
        canSpeak = true;
    }
}
