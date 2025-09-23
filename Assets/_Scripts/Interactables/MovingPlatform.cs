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

    private void Start()
    {
        initialPosition = transform.position;
        if (!outputNode)
        {
            Debug.LogWarning("No output node assigned, defaulting to OutputNode component on this object.");
            outputNode = GetComponent<OutputNode>();
        }

        if (autoStart)
        {
            StartCoroutine(MovePlatform());
        }
    }

    private void _onOutputOn(OutputNode sourceNode)
    {
        if (sourceNode.gameObject == gameObject)
        {
            // Start moving
            StopAllCoroutines();
            StartCoroutine(MovePlatform());
        }
    }

    // Move platform coroutine
    // Moves the platform back and forth between two points
    // If isLooping is true, it will loop indefinitely
    // Otherwise it will only move when triggered
    private IEnumerator MovePlatform()
    {
        Vector3 targetPosition = initialPosition + (movementDirection == Direction.Horizontal ? Vector3.right : Vector3.up) * movementDistance;
        Vector3 startPosition = initialPosition;

        do
        {
            yield return transform.DOMove(targetPosition, movementDuration).SetEase(easeType).WaitForCompletion();
            yield return new WaitForSeconds(pauseDuration);

            // Swap start and target positions for return trip
            Vector3 temp = startPosition;
            startPosition = targetPosition;
            targetPosition = temp;

        } while (isLooping);
    }
}
