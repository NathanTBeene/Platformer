using System.Collections;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


public class WireManager : MonoBehaviour
{
  [SerializeField] private Vector3[] points;
  [SerializeField] private LineRenderer lineRenderer;

  [SerializeField] private float width = 0.1f;

  [Range(0, 1)]
  [SerializeField] private float currentFill = 0.5f;

  [Header("Debug")]
  public float debugFill = 0.5f;
  public float debugDuration = 2f;
  private Coroutine fillCoroutine;

  private void Start()
  {
    _getPoints();
    lineRenderer.positionCount = points.Length;
    lineRenderer.SetPositions(points);
  }

  private void _getPoints()
  {
    points = new Vector3[transform.childCount];
    for (int i = 0; i < transform.childCount; i++)
    {
      points[i] = transform.GetChild(i).position;
    }
  }

  private void Update()
  {
    lineRenderer.startWidth = width;
    lineRenderer.endWidth = width;

    lineRenderer.material.SetFloat("_FillAmount", currentFill);
  }

  private void LateUpdate()
  {
    _getPoints();
    lineRenderer.SetPositions(points);
  }

  public IEnumerator OnFill(float duration, float targetFill)
  {
    float startTime = Time.time;
    float initialFill = currentFill;
    float elapsed = 0f;

    while (elapsed < duration)
    {
      elapsed += Time.deltaTime;
      currentFill = Mathf.Lerp(initialFill, targetFill, elapsed / duration);
      yield return null;
    }
    currentFill = targetFill;

    Debug.Log($"Fill complete in {Time.time - startTime} seconds");
  }

  public void StartFill(float duration, float targetFill)
  {
    if (fillCoroutine != null)
    {
      StopCoroutine(fillCoroutine);
    }
    fillCoroutine = StartCoroutine(OnFill(duration, targetFill));
  }

  private void OnDrawGizmos()
  {
    _getPoints();
    Gizmos.color = Color.red;

    // Draw thicker lines by drawing multiple offset lines
    float thickness = 0.1f;
    int segments = 5;
    for (int i = 0; i < points.Length - 1; i++)
    {
      Vector3 dir = (points[i + 1] - points[i]).normalized;
      Vector3 normal = Vector3.Cross(dir, Camera.current.transform.forward).normalized * thickness;
      for (int j = -segments / 2; j <= segments / 2; j++)
      {
        Gizmos.DrawLine(points[i] + normal * j / segments, points[i + 1] + normal * j / segments);
      }
    }

    // Draw circles at each point
    float radius = 0.1f;
    for (int i = 0; i < points.Length; i++)
    {
      Gizmos.DrawSphere(points[i], radius);
    }
  }
}

[CustomEditor(typeof(WireManager))]
public class WireManagerEditor : Editor
{
  public override void OnInspectorGUI()
  {
    DrawDefaultInspector();

    WireManager wireManager = (WireManager)target;
    if (GUILayout.Button("Fill Wire"))
    {
      wireManager.StartFill(wireManager.debugDuration, wireManager.debugFill);
    }
  }
}