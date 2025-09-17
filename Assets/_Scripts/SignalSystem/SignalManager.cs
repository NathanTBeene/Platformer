using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.UIElements;

public class SignalManager : MonoBehaviour
{
  [SerializeField] private SignalCircuit[] circuits;
  [SerializeField] private bool showConnections = true;
  [SerializeField] private Color inputConnectionColor = Color.green;
  [SerializeField] private Color outputConnectionColor = Color.red;
  [SerializeField] private Color disabledConnectionColor = Color.gray;

  void OnEnable()
  {
    InputNode.onInputOn += HandleInputOn;
    InputNode.onInputOff += HandleInputOff;
    OutputNode.onOutputOn += HandleOutputOn;
    OutputNode.onOutputOff += HandleOutputOff;
  }

  void OnDisable()
  {
    InputNode.onInputOn -= HandleInputOn;
    InputNode.onInputOff -= HandleInputOff;
    OutputNode.onOutputOn -= HandleOutputOn;
    OutputNode.onOutputOff -= HandleOutputOff;
  }

  private void HandleInputOn(InputNode inputNode)
  {
    Debug.Log("SignalManager: Input On from " + inputNode.gameObject.name);

    // Get the circuit this node belongs to and delegate
    var circuit = circuits.FirstOrDefault(c => c.Connections.Any(conn => conn.source == inputNode));
    if (circuit != null)
    {
      Debug.Log($"SignalManager: Delegating Input On to circuit '{circuit.CircuitName}'");
      circuit.PropagateSignal(inputNode, true);
    }
    else
    {
      Debug.LogWarning("SignalManager: InputNode has no parent circuit assigned.");
    }
  }

  private void HandleInputOff(InputNode inputNode)
  {
    Debug.Log("SignalManager: Input Off from " + inputNode.gameObject.name);

    // Get the circuit this node belongs to and delegate
    var circuit = circuits.FirstOrDefault(c => c.Connections.Any(conn => conn.source == inputNode));
    if (circuit != null)
    {
      circuit.PropagateSignal(inputNode, false);
    }
    else
    {
      Debug.LogWarning("SignalManager: InputNode has no parent circuit assigned.");
    }
  }

  private void HandleOutputOn(OutputNode outputNode)
  {
    // Handle output on signal
  }

  private void HandleOutputOff(OutputNode outputNode)
  {
    // Handle output off signal
  }

  public void EnableCircuit(string circuitName)
  {
    var circuit = circuits.FirstOrDefault(c => c.CircuitName == circuitName);
    circuit?.setEnabled(true);
  }

  public void DisableCircuit(string circuitName)
  {
    var circuit = circuits.FirstOrDefault(c => c.CircuitName == circuitName);
    circuit?.setEnabled(false);
  }

  void OnDrawGizmos()
  {
    if (!showConnections || circuits == null) return;

    for (int i = 0; i < circuits.Length; i++)
    {
      var circuit = circuits[i];
      if (circuit?.Connections == null) continue;

      // Use secondary color if this is the last circuit in the list
      Color gizmoColor = (i == circuits.Length - 1)
        ? inputConnectionColor
        : (circuit.IsEnabled ? outputConnectionColor : disabledConnectionColor);

      Gizmos.color = gizmoColor;

      foreach (var connection in circuit.Connections)
      {
        if (connection.source != null && connection.destination != null)
        {
          Vector3 start = connection.source.transform.position;
          Vector3 end = connection.destination.transform.position;

          // Draw the line
          Gizmos.DrawLine(start, end);

          // Draw an arrow at the end
          DrawArrow(start, end);
        }
      }
    }
  }

  private void DrawArrow(Vector3 start, Vector3 end)
  {
    Vector3 direction = (end - start).normalized;
    Vector3 right = Vector3.Cross(direction, Vector3.up).normalized;
    Vector3 arrowHead1 = end - direction * 0.3f + right * 0.15f;
    Vector3 arrowHead2 = end - direction * 0.3f - right * 0.15f;
    
    Gizmos.DrawLine(end, arrowHead1);
    Gizmos.DrawLine(end, arrowHead2);
  }
}