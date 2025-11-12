using System.Collections;
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

    private bool canInteract = false;
    private bool isCoolingDown = false;
    private bool isPlayerInRange = false;

    void Start()
    {
        if (!inputNode)
        {
            inputNode = GetComponent<InputNode>();
        }

        if (startOn)
        {
            StartCoroutine(_switch_on());
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

    private void _on_interact()
    {
        if (!isPlayerInRange || !canInteract || isCoolingDown) return;

        if (!inputNode.isActive && canInteract && !isCoolingDown)
        {
            StartCoroutine(_switch_on());
        }
        else if (inputNode.isActive && canInteract && !isCoolingDown)
        {
            StartCoroutine(_switch_off());
        }
    }

    private IEnumerator _switch_on()
    {
        anim.SetBool("isOn", true);
        powerIndicator.TurnOn();
        StartCoroutine(_start_cooldown());
        yield return _fill_wire(true);
        inputNode.setState(true);
    }

    private IEnumerator _switch_off()
    {
        anim.SetBool("isOn", false);
        powerIndicator.TurnOff();
        StartCoroutine(_start_cooldown());
        yield return _fill_wire(false);
        inputNode.setState(false);
    }

    private IEnumerator _start_cooldown()
    {
        isCoolingDown = true;
        yield return new WaitForSeconds(switchCooldown);
        isCoolingDown = false;
    }

    private IEnumerator _fill_wire(bool toOn)
    {
        if (wire != null)
        {
            if (toOn)
                yield return wire.PowerOn(wireFillDuration);
            else
                // Power off should be really fast.
                yield return wire.PowerOff(wireFillDuration * 0.5f);
        }
    }
}
