using DG.Tweening;
using UnityEngine;

public class RevealingWindow : MonoBehaviour
{
    enum Direction
    {
        Horizontal,
        Vertical
    }

    [Header("References")]
    [SerializeField] private OutputNode outputNode;
    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;

    [Header("Settings")]
    [SerializeField] private Direction direction = Direction.Horizontal;
    [SerializeField] private float slideDistance = 2f;
    [SerializeField] private float slideDuration = 1f;
    [SerializeField] private Ease easeType = Ease.InOutSine;
    [SerializeField] private bool autoStart = true;

    private void OnEnable()
    {
        if (!outputNode)
        {
        outputNode = GetComponent<OutputNode>();
        }

        if (outputNode)
        {
        outputNode.onStateChange += _onStateChange;
        }
    }

    private void OnDisable()
    {
        if (outputNode)
        {
        outputNode.onStateChange -= _onStateChange;
        }
    }

    private void Start()
    {
        // Only auto start if there is no output node
        if (autoStart)
        {
        _openWindow();
        }
    }

    private void _onStateChange(bool state)
    {
        if (state)
        {
        _openWindow();
        }
        else
        {
        _closeWindow();
        }
    }

    private void _openWindow()
    {
        if (direction == Direction.Horizontal)
        {
            _openHorizontal();
        }
        else
        {
            _openVertical();
        }
    }

    private void _openHorizontal()
    {
        leftDoor.transform.DOMoveX(leftDoor.transform.position.x - slideDistance, slideDuration).SetEase(easeType);
        rightDoor.transform.DOMoveX(rightDoor.transform.position.x + slideDistance, slideDuration).SetEase(easeType);
    }

    private void _openVertical()
    {
        leftDoor.transform.DOMoveY(leftDoor.transform.position.y + slideDistance, slideDuration).SetEase(easeType);
        rightDoor.transform.DOMoveY(rightDoor.transform.position.y - slideDistance, slideDuration).SetEase(easeType);
    }

    private void _closeWindow()
    {
        if (direction == Direction.Horizontal)
        {
            _closeHorizontal();
        }
        else
        {
            _closeVertical();
        }
    }

    private void _closeVertical()
    {
        leftDoor.transform.DOMoveY(leftDoor.transform.position.y - slideDistance, slideDuration).SetEase(easeType);
        rightDoor.transform.DOMoveY(rightDoor.transform.position.y + slideDistance, slideDuration).SetEase(easeType);
    }

    private void _closeHorizontal()
    {
        leftDoor.transform.DOMoveX(leftDoor.transform.position.x + slideDistance, slideDuration).SetEase(easeType);
        rightDoor.transform.DOMoveX(rightDoor.transform.position.x - slideDistance, slideDuration).SetEase(easeType);
    }
}
