using System.Threading.Tasks;
using UnityEngine;

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
    [SerializeField] private bool startOn = false;
    [SerializeField] private bool hideWireWhenOff = false;

    private bool canInteract = false;
    private bool isCoolingDown = false;
    private bool isPlayerInRange = false;

    async void Start()
    {
        if (!inputNode)
        {
            inputNode = GetComponent<InputNode>();
        }

        if (startOn)
        {
            await Task.Yield();
            await _switch_on();
            inputNode.setState(true);
        }
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
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canInteract = false;
            isPlayerInRange = false;
        }
    }

    private async void _on_interact()
    {
        if (!isPlayerInRange || !canInteract || isCoolingDown) return;

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
        isCoolingDown = true;
        Task.Delay((int)(switchCooldown * 1000)).ContinueWith(t =>
        {
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
