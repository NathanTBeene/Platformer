using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FlyingBotMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject spriteObject;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2.5f; // Units per second
    [SerializeField] private Ease moveEase = Ease.Linear;
    [SerializeField] private float easeMagnitude = 1f;
    private bool wasMovingWhenPaused = false;

    [Header("Wait Settings")]
    [Header("If randomWait is false, uses minWaitTime for each waypoint.")]
    [SerializeField] private bool randomWait = false;
    [SerializeField] private float minWaitTime = 1f;
    [SerializeField] private float maxWaitTime = 2f;

    [Header("Waypoints need to be separate from the FlyingBot object.")]
    [SerializeField] private GameObject waypointContainer;
    [SerializeField] private int startingWaypointIndex = 0;
    [SerializeField] private List<Vector3> waypoints; // Add children as waypoints
    private int currentWaypointIndex = 0;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool isPaused = false;
    private Tween currentTween;

    [Header("Debug")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private Color gizmoColor = Color.green;
    [SerializeField] private float gizmoSize = 0.2f;

    private void Start()
    {
        _getWaypoints();
        currentWaypointIndex = startingWaypointIndex % waypoints.Count;
        _updateStartingPosition();
        if (waypoints.Count > 0)
        {
            _moveToNextWaypoint();
        }
    }

    private void _updateStartingPosition()
    {
        if (waypoints.Count == 0) return;
        transform.position = waypoints[startingWaypointIndex % waypoints.Count];
    }

    public void PauseMovement()
    {
        if (isPaused) return;
        isPaused = true;
        wasMovingWhenPaused = isMoving;
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Pause();
        }
        StopAllCoroutines();
    }

    public void ResumeMovement()
    {
        if (!isPaused) return;
        isPaused = false;

        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Play();
        }
        else if (wasMovingWhenPaused)
        {
            wasMovingWhenPaused = false;
            _moveToCurrentWaypoint();
        }
        else if (!isMoving)
        {
            StartCoroutine(WaitAtWaypoint());
        }

        _handleSpriteDirection();
    }

    public void StopMovement()
    {
        isPaused = true;
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }
        StopAllCoroutines();
        isMoving = false;
    }

    // Gets the direction vecetor towards the target waypoint
    private Vector3 _getMovementDirection()
    {
        if (waypoints.Count == 0) return Vector3.zero;

        Vector3 direction = targetPosition - transform.position;
        return direction;
    }

    private void _handleSpriteDirection()
    {
        Vector3 direction = _getMovementDirection();
        // If direction is positive, we are moving right
        // If direction is negative, we are moving left
        if (direction.x > 0)
        {
            // Facing right
            spriteObject.transform.DOScale(new Vector3(1, 1, 1), 0.2f);
        }
        else if (direction.x < 0)
        {
            // Facing left
            spriteObject.transform.DOScale(new Vector3(-1, 1, 1), 0.2f);
        }
    }

    private void _getWaypoints()
    {
        waypoints = new List<Vector3>();
        for (int i = 0; i < waypointContainer.transform.childCount; i++)
        {
            waypoints.Add(waypointContainer.transform.GetChild(i).position);
        }
    }

    private void _moveToCurrentWaypoint()
    {
        if (waypoints.Count == 0) return;

        targetPosition = waypoints[currentWaypointIndex];
        isMoving = true;
        _handleSpriteDirection();

        Vector3 distanceVector = _getMovementDirection();
        float distance = distanceVector.magnitude;

        float actualDuration = distance / moveSpeed;

        currentTween = transform.DOMove(targetPosition, actualDuration).SetEase(moveEase, easeMagnitude).OnComplete(() =>
        {
            isMoving = false;
            if (!isPaused) // Only start waiting if not paused
                StartCoroutine(WaitAtWaypoint());
        });
    }

    private void _moveToNextWaypoint()
    {
        if (waypoints.Count == 0) return;

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        targetPosition = waypoints[currentWaypointIndex];
        isMoving = true;
        _handleSpriteDirection();

        Vector3 distanceVector = _getMovementDirection();
        float distance = distanceVector.magnitude;

        float actualDuration = distance / moveSpeed;

        currentTween = transform.DOMove(targetPosition, actualDuration).SetEase(moveEase, easeMagnitude).OnComplete(() =>
        {
            isMoving = false;
            if (!isPaused) // Only start waiting if not paused
                StartCoroutine(WaitAtWaypoint());
        });
    }

    private IEnumerator WaitAtWaypoint()
    {
        float waitTime = minWaitTime;
        if (randomWait)
        {
            waitTime = Random.Range(minWaitTime, maxWaitTime);
        }
        yield return new WaitForSeconds(waitTime);

        if (!isPaused) // Only move to next waypoint if not paused
            _moveToNextWaypoint();
    }

    private void OnDrawGizmos()
    {
        if (waypointContainer == null) return;
        if(!Application.isPlaying)
            _getWaypoints();
        if (waypoints == null || waypoints.Count == 0) return;


        if (showGizmos)
        {
            Gizmos.color = gizmoColor;
            foreach (var point in waypoints)
            {
                Gizmos.DrawSphere(point, gizmoSize);
            }

            for (int i = 0; i < waypoints.Count; i++)
            {
                Vector3 currentPoint = waypoints[i];
                Vector3 nextPoint = waypoints[(i + 1) % waypoints.Count];
                Gizmos.DrawLine(currentPoint, nextPoint);
            }

            // Draw line from FlyingBot to target waypoint
            if (Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, targetPosition);
            }

            // Draw different color dot for current waypoint
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(waypoints[currentWaypointIndex], gizmoSize);
        }
    }

}
