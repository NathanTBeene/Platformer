using System.Collections;
using UnityEngine;

public class AreaNodeController : MonoBehaviour
{
    private enum TriggerType
    {
        Once, // Trigger only once, a permanent change
        Continuous, // Trigger on enter and exit
        Pulse // Trigger for a short duration
    }

    private enum StartingState
    {
        Active,
        Inactive
    }

    [Header("References")]
    [SerializeField] private InputNode inputNode;
    [SerializeField] private Collider2D areaCollider;

    [Header("Settings")]
    [SerializeField] private StartingState startingState = StartingState.Inactive;
    [SerializeField] private TriggerType triggerType;
    [SerializeField] private float pulseDuration = 1f;
    private Coroutine triggerCoroutine;

    private bool hasTriggered = false;

    private void Start()
    {
        if (areaCollider == null)
        {
            areaCollider = GetComponent<Collider2D>();
        }

        // Set initial state
        if (inputNode)
        {
            bool initialState = (startingState == StartingState.Active);
            inputNode.setState(initialState);
        }
    }

    private void _triggerSwitch()
    {
        if (inputNode)
        {
            inputNode.setState(!inputNode.getState());
        }
    }

    private IEnumerator _triggerPulse()
    {
        if (inputNode)
        {
            inputNode.setState(true);
            yield return new WaitForSeconds(pulseDuration);
            inputNode.setState(false);
        }
    }

    private void _triggerEnterExit(bool isEntering)
    {
        if (inputNode)
        {
            inputNode.setState(isEntering);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            switch (triggerType)
            {
                case TriggerType.Once:
                    if (!hasTriggered)
                    {
                        _triggerSwitch();
                        hasTriggered = true;
                    }
                    break;
                case TriggerType.Continuous:
                    _triggerEnterExit(true);
                    break;
                case TriggerType.Pulse:
                    if (triggerCoroutine != null)
                    {
                        StopCoroutine(triggerCoroutine);
                    }
                    triggerCoroutine = StartCoroutine(_triggerPulse());
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerType == TriggerType.Continuous)
            {
                _triggerEnterExit(false);
            }
        }
    }
}
