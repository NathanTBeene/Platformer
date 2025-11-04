using System.Collections;
using DG.Tweening;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public enum Direction
    {
        Horizontal,
        Vertical
    }

    [Header("References")]
    [SerializeField] private OutputNode outputNode;
    [SerializeField] private PowerIndicator powerIndicator;

    [Header("Settings")]
    [SerializeField] private bool autoStart = true;
    [SerializeField] private bool isLooping = true;
    [SerializeField] private Direction movementDirection = Direction.Horizontal;
    [SerializeField] private float movementDistance = 3f;
    [SerializeField] private float movementDuration = 2f;
    [SerializeField] private float pauseDuration = 0.5f;

    [Header("Debug")]
    [SerializeField] private Ease easeType = Ease.Linear;

    private Vector3 initialPosition;
    private Tween currentTween;
    private bool isMoving = false;

    private void OnEnable() {
        if (!outputNode)
        {
            outputNode = GetComponent<OutputNode>();
        }

        if (outputNode)
        {
            outputNode.onStateChange += onStateChange;
        }
    }

    private void Start()
    {
        initialPosition = transform.position;

        // Only auto start if there is no output node
        if (autoStart && (outputNode == null))
        {
            _startMove();
        }
    }

    private void onStateChange(bool state)
    {
        if (isLooping)
        {
            if (state)
                _startMove();
            else
                _stopMove();
        }
        else
        {
            // Single cycle movement
            // Move to target on true, return to start on false
        }

        if (state)
            powerIndicator?.TurnOn();
        else
            powerIndicator?.TurnOff();
    }

    private void _startMove()
    {
        // Prevent multiple starts
        if (isMoving) return;

        // Start movement loop
        isMoving = true;
        StartCoroutine(MovementLoop());
    }

    private void _stopMove()
    {
        isMoving = false;

        // Await current movement to finish
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.OnComplete(() =>
            {
                currentTween = null;
            });
        }
    }


    private enum JumpPoint
    {
        Start,
        Target
    }

    private JumpPoint lastJumpPoint = JumpPoint.Start;

    // Loop will always wait until the current movement is complete before stopping
    // If stopped in the middle, it will start from the current position on next start
    private IEnumerator MovementLoop()
    {
        bool moveToTarget = lastJumpPoint == JumpPoint.Start;

        while (isMoving)
        {
            if (moveToTarget)
            {
                Vector3 targetPosition = _getTargetPosition();
                currentTween = transform.DOMove(targetPosition, movementDuration).SetEase(easeType);
                yield return currentTween.WaitForCompletion();

                // Update last jump point
                lastJumpPoint = JumpPoint.Target;

                if (!isMoving) break;

                //Pause at target
                if (pauseDuration > 0f)
                    yield return new WaitForSeconds(pauseDuration);
            }
            else
            {
                currentTween = transform.DOMove(initialPosition, movementDuration).SetEase(easeType);
                yield return currentTween.WaitForCompletion();

                // Update last jump point
                lastJumpPoint = JumpPoint.Start;

                if (!isMoving) break;

                //Pause at start
                if (pauseDuration > 0f)
                    yield return new WaitForSeconds(pauseDuration);
            }

            // Toggle direction
            moveToTarget = !moveToTarget;
        }
    }

    private Vector3 _getTargetPosition()
    {
        Vector3 targetPosition = initialPosition;

        switch (movementDirection)
        {
            case Direction.Horizontal:
                targetPosition += new Vector3(movementDistance, 0f, 0f);
                break;
            case Direction.Vertical:
                targetPosition += new Vector3(0f, movementDistance, 0f);
                break;
        }
        return targetPosition;
    }

    private void OnDisable() {
        if (outputNode)
        {
            outputNode.onStateChange -= onStateChange;
        }

        _stopMove();
    }
}
