using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Circuit is a single line of connection from x inputs to y outputs.
// This is where propagation logic happens.

// Every tick it attempts to propagate signals from active inputs to outputs through logic gates.

[System.Serializable]
public class SignalCircuit
{
  [Header("Circuit Settings")]
  [SerializeField] private string circuitName;
  [SerializeField] private Connection[] connections;
  [SerializeField] private Color circuitColor = Color.red;
  [SerializeField] private bool isEnabled = true;

  [Header("Debug")]
  [SerializeField] private bool showGizmos = true;

  public string CircuitName => circuitName;
  public bool IsEnabled => isEnabled;
  public Connection[] Connections => connections;


  private HashSet<Connection> activeConnections = new HashSet<Connection>();
  private MonoBehaviour coroutineRunner;

  public void Initialize(MonoBehaviour runner)
  {
    coroutineRunner = runner;
  }

  public void Tick()
  {
    if (!isEnabled || connections == null || connections.Length == 0)
      return;

    if (coroutineRunner == null)
    {
      Debug.LogError("SignalCircuit: Coroutine runner not set. Call Initialize() with a MonoBehaviour.");
      return;
    }

    // First, get all connections that have an InputNode as source
    // These are the bottom level for signal propagation.
    // All other connections will either be a LogicGate or an OutputNode.
    Connection[] inputConnections = connections.Where(c => c.source != null && c.source is InputNode).ToArray();

    // Process each connection synchronously
    foreach (var conn in inputConnections)
    {
      if (!ValidateConnection(conn)) continue;
      if (activeConnections.Contains(conn)) continue; // Prevent re-entrance
      coroutineRunner.StartCoroutine(PropagateSignalAsync(conn));
    }
  }

  private bool ValidateConnection(Connection conn)
  {
    if (conn == null) return false;
    if (conn.source == null || conn.destination == null) return false;
    return true;
  }

  private IEnumerator PropagateSignalAsync(Connection inputConnection)
  {
    if (inputConnection == null) yield break;
    if (inputConnection.source == null || inputConnection.destination == null) yield break;
    if (activeConnections.Contains(inputConnection)) yield break; // Prevent re-entrance

    activeConnections.Add(inputConnection);

    try
    {
      if (inputConnection.delay > 0f)
      {
        yield return new WaitForSeconds(inputConnection.delay);
      }


      // If the destination is an OutputNode, we just set its state.
      if (inputConnection.destination is OutputNode outputNode)
      {
        outputNode.RecieveSignal(inputConnection.source.isActive);
      }
      // If the destination is a LogicGate, we need to evaluate its logic
      else if (inputConnection.destination is LogicGate logicGate)
      {
        // Get all connections that feed into this gate
        var gateInputConnections = connections.Where(c => c.destination == logicGate).ToArray();

        // Get ALL source nodes (InputNodes AND LogicGates) that feed into this gate
        var allInputSources = gateInputConnections.Select(c => c.source).ToArray();

        // Evaluate the logic of the gate based on ALL its inputs
        bool result = logicGate.EvaluateLogic(allInputSources);

        // Set the state of the gate
        logicGate.RecieveSignal(result);

        // Now propagate the signal from this gate to its outputs
        var gateOutputConnections = connections.Where(c => c.source == logicGate).ToArray();
        foreach (var outConn in gateOutputConnections)
        {
          if (!ValidateConnection(outConn)) continue;
          coroutineRunner.StartCoroutine(PropagateSignalAsync(outConn));
        }
      }
    }
    finally
    {
      activeConnections.Remove(inputConnection);
    }
  }

  public void OnDrawGizmos()
  {
    if (!showGizmos) return;
    if (!isEnabled || connections == null || connections.Length == 0)
    {
      return;
    }
    ;

    foreach (var conn in connections)
    {
      if (conn.source == null || conn.destination == null) continue;

      Gizmos.color = circuitColor;
      Gizmos.DrawLine(conn.source.transform.position, conn.destination.transform.position);
      Gizmos.DrawCube(conn.source.transform.position, Vector3.one * 0.1f);
      Gizmos.DrawCube(conn.destination.transform.position, Vector3.one * 0.1f);
    }
  }
}