using System.Collections;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


public class WireManager : MonoBehaviour
{
  [SerializeField] private Vector3[] points;
  [SerializeField] private LineRenderer lineRenderer;

  [SerializeField] private float width = 0.1f;

  [Range(0, 2)]
  [SerializeField] private float currentFill = 0.5f;

  [Header("Wire Colors")]
  [SerializeField] private Color powerColor = Color.red;
  [SerializeField] private Color emptyColor = Color.black;
  
  [Header("Power State")]
  [SerializeField] private bool isPowered = false;

  [Header("Debug")]
  public float debugFill = 0.5f;
  public float debugDuration = 2f;

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
    lineRenderer.material.SetFloat("_FillAmount", currentFill);
    lineRenderer.material.SetColor("_EmptyColor", emptyColor);
    lineRenderer.material.SetColor("_WireColor", powerColor);
    lineRenderer.startWidth = width;
    lineRenderer.endWidth = width;
  }

  // Power turning on should go from fill 0 to fill 1
  public async Task PowerOn(float duration = 1f)
  {
    StopAllCoroutines();
    await StartCoroutineAsync(_fill(0f, 1f, duration, true));
  }

  // Power turning off should go from fill 1 to fill 2
  public async Task PowerOff(float duration = 1f)
  {
    StopAllCoroutines();
    await StartCoroutineAsync(_fill(1f, 2f, duration, false));
  }

  private Task StartCoroutineAsync(IEnumerator coroutine)
  {
    var tcs = new TaskCompletionSource<bool>();
    StartCoroutine(WrapCoroutine(coroutine, tcs));
    return tcs.Task;
  }

  private IEnumerator WrapCoroutine(IEnumerator coroutine, TaskCompletionSource<bool> tcs)
  {
    yield return StartCoroutine(coroutine);
    tcs.SetResult(true);
  }

  private IEnumerator _fill(float startFill, float endFill, float duration, bool powerState = false)
  {
    float elapsed = 0f;
    while (elapsed < duration)
    {
      elapsed += Time.deltaTime;
      currentFill = Mathf.Lerp(startFill, endFill, elapsed / duration);
      yield return null;
    }
    currentFill = endFill;
    isPowered = powerState;
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
  public override async void OnInspectorGUI()
  {
    DrawDefaultInspector();

    WireManager wireManager = (WireManager)target;
    
    GUILayout.Space(10);
    GUILayout.Label("Power Controls", EditorStyles.boldLabel);
    
    GUILayout.BeginHorizontal();
    if (GUILayout.Button("Turn Power ON"))
    {
      await wireManager.PowerOn(wireManager.debugDuration);
    }
    if (GUILayout.Button("Turn Power OFF"))
    {
      await wireManager.PowerOff(wireManager.debugDuration);
    }
    GUILayout.EndHorizontal();
  }
}