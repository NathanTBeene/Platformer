using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DialogueTrigger : MonoBehaviour
{
    [Header("References")]
    public DialogueComponent dialogueComponent;
    public Collider2D triggerArea;

    [Header("Settings")]
    public bool triggerOnEnter = false;
    public float triggerCooldown = 2f;
    private bool canTrigger = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (dialogueComponent != null && triggerOnEnter && canTrigger)
            {
                canTrigger = false;
                StartCoroutine(TriggerCooldown());
                dialogueComponent.StartCoroutine(dialogueComponent.ShowDialogue());
            }
        }
    }

    private IEnumerator TriggerCooldown()
    {
        yield return new WaitForSeconds(triggerCooldown);
        canTrigger = true;
    }
}
