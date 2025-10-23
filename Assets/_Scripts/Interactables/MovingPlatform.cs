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
    private bool isAtTarget = false;

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
            if (state)
                _moveToTarget();
            else
                _returnToStart();
        }
    }

    private void _startMove()
    {
        if (isMoving) return;
        isMoving = true;
        StartCoroutine(MovementLoop());
    }

    private void _stopMove()
    {
        if (!isMoving) return;
        isMoving = false;
        _stopCurrentMovement();
    }

    private void _moveToTarget()
    {
        Debug.Log("Moving to target");
        if (isAtTarget) return;

        // Kill any current wtween and stop coroutines
        _stopCurrentMovement();

        isMoving = true;
        Vector3 targetPosition = _getTargetPosition();
        currentTween = transform.DOMove(targetPosition, movementDuration).SetEase(easeType).OnComplete(() =>
        {
            isAtTarget = true;
            isMoving = false;
        });
    }

    private void _returnToStart()
    {
        Debug.Log("Returning to start");
        if (!isAtTarget) return;

        _stopCurrentMovement();

        isMoving = true;
        currentTween = transform.DOMove(initialPosition, movementDuration).SetEase(easeType).OnComplete(() =>
        {
            isAtTarget = false;
            isMoving = false;
        });
    }

    private IEnumerator MovementLoop()
    {
        while (isMoving)
        {
            //Move to target
            Vector3 targetPosition = _getTargetPosition();
            currentTween = transform.DOMove(targetPosition, movementDuration).SetEase(easeType);
            yield return currentTween.WaitForCompletion();

            if (!isMoving) break;

            //Pause at target
            if (pauseDuration > 0f)
            {
                yield return new WaitForSeconds(pauseDuration);
            }

            if (!isMoving) break;

            //Return to start
            currentTween = transform.DOMove(initialPosition, movementDuration).SetEase(easeType);
            yield return currentTween.WaitForCompletion();

            if (!isMoving) break;

            //Pause at start
            if (pauseDuration > 0f)
            {
                yield return new WaitForSeconds(pauseDuration);
            }
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

    private void _stopCurrentMovement()
    {
        if (currentTween != null)
        {
            currentTween.Kill(true);
            currentTween = null;
        }

        StopAllCoroutines();
        transform.DOKill(true);
    }

    private void OnDisable() {
        if (outputNode)
        {
            outputNode.onStateChange -= onStateChange;
        }

        _stopMove();
    }
}
