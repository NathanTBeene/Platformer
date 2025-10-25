using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Switch : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WireManager wire;
    [SerializeField] private InputNode inputNode;
    [SerializeField] private Animator anim;
    [SerializeField] private PowerIndicator powerIndicator;

    [Header("Settings")]
    [SerializeField] private float switchCooldown = 0.2f;
    [SerializeField] private float wireFillDuration = 0.5f;

    private bool canInteract = false;
    private bool isCoolingDown = false;

    void Start()
    {
        if (!inputNode)
        {
            inputNode = GetComponent<InputNode>();
        }
        // _switch_off();
    }

    void OnEnable()
    {
        InputManager.onInteract += _on_interact;
    }

    void OnDisable()
    {
        InputManager.onInteract -= _on_interact;
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

    private async void _on_interact()
    {
        if (!inputNode.isActive && canInteract && !isCoolingDown)
        {
            await _switch_on();
            inputNode.setState(true);
        }
        else if (inputNode.isActive && canInteract && !isCoolingDown)
        {
            await _switch_off();
            inputNode.setState(false);
        }
    }

    private async Task _switch_on()
    {
        anim.SetBool("isOn", true);
        powerIndicator.TurnOn();
        _start_cooldown();
        await _fill_wire(true);
    }

    private async Task _switch_off()
    {
        anim.SetBool("isOn", false);
        powerIndicator.TurnOff();
        _start_cooldown();
        await _fill_wire(false);
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

    private async Task _fill_wire(bool toOn)
    {
        if (wire != null)
        {
            if (toOn)
                await wire.PowerOn(wireFillDuration);
            else
                // Power off should be really fast.
                await wire.PowerOff(wireFillDuration * 0.5f);
        }
    }
}
