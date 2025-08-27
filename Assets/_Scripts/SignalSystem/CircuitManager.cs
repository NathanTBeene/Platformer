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
  [SerializeField] private Color connectionColor = Color.green;

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

    foreach (var circuit in circuits)
    {
      if (circuit?.Connections == null) continue;

      Gizmos.color = circuit.IsEnabled ? connectionColor : Color.gray;
      
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