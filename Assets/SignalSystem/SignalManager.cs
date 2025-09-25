using System.Linq;
using UnityEngine;

public class SignalManager : MonoBehaviour
{

  public static SignalManager Instance;

  [Header("Circuit Settings")]
  [SerializeField] private SignalCircuit[] circuits;

  [Header("Tick Settings")]
  [SerializeField] private bool useTick = true;
  [SerializeField] private float ticks = 10f; // Ticks per second
  private float nextTickTime;
  private float tickInterval;

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }

    tickInterval = 1f / ticks;

    // Initialize all circuits with this MonoBehaviour as the coroutine runner
    if (circuits != null)
    {
      foreach (var circuit in circuits)
      {
        circuit.Initialize(this);
      }
    }
  }

  void Update()
  {
    if (useTick && Time.time >= nextTickTime)
    {
      foreach (var circuit in circuits.Where(c => c.IsEnabled))
      {
        circuit.Tick();
      }
      nextTickTime = Time.time + tickInterval;
    }
  }

  void OnDrawGizmos()
  {
    if (circuits == null || circuits.Length == 0) return;
    foreach (var circuit in circuits)
    {
      circuit.OnDrawGizmos();
    }
  }
}