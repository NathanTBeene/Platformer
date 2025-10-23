using UnityEngine;

public class FlyingBot : MonoBehaviour {
    [SerializeField] private DialogueComponent dialogueComponent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision detected with " + other.gameObject.name);
        if (other.gameObject.CompareTag("Player"))
        {
            // Handle collision with player
            if (dialogueComponent != null)
            {
                StartCoroutine(dialogueComponent.ShowDialogue());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        Debug.Log("Collision ended with " + other.gameObject.name);
        if (other.gameObject.CompareTag("Player"))
        {
            // Handle collision with player
            if (dialogueComponent != null)
            {
                StartCoroutine(dialogueComponent.HideDialogue());
            }
        }
    }
}
