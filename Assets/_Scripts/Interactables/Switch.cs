using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Switch : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputNode inputNode;
    [SerializeField] private GameObject sprite;

    [Header("Offsets")]
    [SerializeField] private float moveTime = 0.5f;
    [SerializeField] private Vector3 onPosition;
    [SerializeField] private Vector3 offPosition;

    private bool canInteract = false;

    void Start()
    {
        if (!inputNode)
        {
            inputNode = GetComponent<InputNode>();
        }
        if (!sprite)
        {
            sprite = transform.GetChild(0).gameObject;
        }

        _hook_signals();
        _sprite_off();
    }

    private void _hook_signals()
    {
        InputManager.onInteract += _on_interact;
        InputNode.onInputOn += _on_input_on;
        InputNode.onInputOff += _on_input_off;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canInteract = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canInteract = false;
        }
    }

    private void _on_interact()
    {
        if (!inputNode.isActive && canInteract)
        {
            inputNode.setState(true);
        }
        else if (inputNode.isActive && canInteract)
        {
            inputNode.setState(false);
        }
    }

    private void _on_input_on(InputNode node)
    {
        if (node == inputNode)
            _sprite_on();
    }

    private void _on_input_off(InputNode node)
    {
        if (node == inputNode)
            _sprite_off();
    }

    private void _sprite_on()
    {
        sprite.transform.DOLocalMove(onPosition, moveTime);
    }

    private void _sprite_off()
    {
        sprite.transform.DOLocalMove(offPosition, moveTime);
    }

}
