using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Switch : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputNode inputNode;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer indicatorSprite;
    [SerializeField] private Sprite indicatorON;
    [SerializeField] private Sprite indicatorOFF;

    [Header("Settings")]
    [SerializeField] private float switchCooldown = 0.2f;

    private bool canInteract = false;
    private bool isCoolingDown = false;

    void Start()
    {
        if (!inputNode)
        {
            inputNode = GetComponent<InputNode>();
        }

        _hook_signals();
        _switch_off();
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
        if (!inputNode.isActive && canInteract && !isCoolingDown)
        {
            inputNode.setState(true);
        }
        else if (inputNode.isActive && canInteract && !isCoolingDown)
        {
            inputNode.setState(false);
        }
    }

    private void _on_input_on(InputNode node)
    {
        if (node == inputNode)
            _switch_on();
    }

    private void _on_input_off(InputNode node)
    {
        if (node == inputNode)
            _switch_off();
    }

    private void _switch_on()
    {
        anim.SetBool("isOn", true);
        indicatorSprite.sprite = indicatorON;
        _start_cooldown();
    }

    private void _switch_off()
    {
        anim.SetBool("isOn", false);
        indicatorSprite.sprite = indicatorOFF;
        _start_cooldown();
    }

    private void _start_cooldown()
    {
        canInteract = false;
        isCoolingDown = true;
        Task.Delay((int)(switchCooldown * 1000)).ContinueWith(t =>
        {
            canInteract = true;
            isCoolingDown = false;
        });
    }

}
