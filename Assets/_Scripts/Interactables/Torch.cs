using System.Collections;
using UnityEngine;

public class Torch : MonoBehaviour
{
    private enum StartingState
    {
        On,
        Off
    }

    [Header("References")]
    [SerializeField] private SpriteRenderer torchSprite;
    [SerializeField] private Animator anim;
    [SerializeField] private InputNode inputNode;
    [SerializeField] private WireManager wire;

    [Header("Settings")]
    [SerializeField] private StartingState startingState = StartingState.Off;
    [SerializeField] private float wireFillDuration = 0.5f;

    private void OnEnable() {
        if (inputNode != null)
        {
            inputNode.onStateChange += SetState;
        }
    }

    private void OnDisable() {
        if (inputNode != null)
        {
            inputNode.onStateChange -= SetState;
        }
    }

    private void Start()
    {
        if (startingState == StartingState.On)
        {
            StartCoroutine(_turnOn());
        }
        else
        {
            StartCoroutine(_turnOff());
        }
    }

    private IEnumerator _turnOn()
    {
        anim.SetBool("isOn", true);
        yield return _fill_wire(true);
        inputNode.setState(true);
    }

    private IEnumerator _turnOff()
    {
        anim.SetBool("isOn", false);
        yield return _fill_wire(false);
        inputNode.setState(false);
    }

    private IEnumerator _fill_wire(bool toOn)
    {
        if (wire != null)
        {
            if (toOn)
                yield return wire.PowerOn(wireFillDuration);
            else
                yield return wire.PowerOff(wireFillDuration * 0.5f);
        }
    }

    public void SetState(bool state)
    {
        if (state)
        {
            _turnOn();
        }
        else
        {
            _turnOff();
        }
    }

}
