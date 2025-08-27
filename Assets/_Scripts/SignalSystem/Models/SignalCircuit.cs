using System.Linq;
using UnityEngine;

[System.Serializable]
public class SignalCircuit
{
  [Header("Circuit Settings")]
  [SerializeField] private string circuitName;
  [SerializeField] private Connection[] connections;
  [SerializeField] private bool isEnabled = true;

  public string CircuitName => circuitName;
  public bool IsEnabled => isEnabled;
  public Connection[] Connections => connections;

  public SignalCircuit(string name)
  {
    circuitName = name;
    connections = new Connection[0];
  }

  public void PropagateSignal(SignalNode sourceNode, bool signalState)
  {
    if (!isEnabled)
      return;

    // Get all connections where source node is the starting point.
    var outgoingConnections = connections.Where(c => c.source == sourceNode);
    Debug.Log($"SignalCircuit '{circuitName}': Propagating signal from '{sourceNode.gameObject.name}' to {outgoingConnections.Count()} connections.");

    foreach (var connection in outgoingConnections)
    {
      // Update signal state
      connection.isActive = signalState;

      var destinationLogicGate = connection.destination.GetComponent<LogicGate>();
      var destinationOutput = connection.destination.GetComponent<OutputNode>();

      if (destinationLogicGate)
      {
        EvaluateLogicGate(destinationLogicGate);
      }
      else if (destinationOutput)
      {
        destinationOutput.setState(signalState);
      }
    }
  }

  private void EvaluateLogicGate(LogicGate gate)
  {
    var incomingConnections = connections.Where(c => c.destination == gate);

    bool gateOutput = gate.EvaluateInputs(incomingConnections.ToArray());

    if (gate.isActive != gateOutput)
    {
      gate.setState(gateOutput);
      PropagateSignal(gate, gateOutput);
    }
  }

  public void AddConnection(Connection newConnection)
  {
    var connectionList = connections.ToList();
    connectionList.Add(newConnection);
    connections = connectionList.ToArray();
  }

  public void setEnabled(bool enabled) => isEnabled = enabled;
}