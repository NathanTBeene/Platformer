using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FlyingBotMovement : MonoBehaviour
{
    [SerializeField] private float moveDuration = 2f;
    [SerializeField] private Ease moveEase = Ease.Linear;
    [SerializeField] private GameObject waypointContainer;
    [SerializeField] private List<Vector3> waypoints; // Add children as waypoints
    [SerializeField] private float waitTime = 1f;
    private int currentWaypointIndex = 0;
    private Vector3 targetPosition;
    private bool isMoving = false;

    [Header("Debug")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private Color gizmoColor = Color.green;
    [SerializeField] private float gizmoSize = 0.2f;

    private void Start()
    {
        _getWaypoints();
        if (waypoints.Count > 0)
        {
            _moveToNextWaypoint();
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

    private void _moveToNextWaypoint()
    {
        if (waypoints.Count == 0) return;

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        targetPosition = waypoints[currentWaypointIndex];
        isMoving = true;

        transform.DOMove(targetPosition, moveDuration).SetEase(moveEase, 1).OnComplete(() =>
        {
            isMoving = false;
            StartCoroutine(WaitAtWaypoint());
        });
    }

    private IEnumerator WaitAtWaypoint()
    {
        yield return new WaitForSeconds(waitTime);
        _moveToNextWaypoint();
    }
    
    private void OnDrawGizmos()
    {
        if (waypointContainer == null) return;
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
        }
    }

}
