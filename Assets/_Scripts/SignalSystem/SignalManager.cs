using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class SignalManager : MonoBehaviour
{
  [SerializeField] private Connection[] connections;
  private Queue<GameObject> propagationQueue = new Queue<GameObject>();

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

  private void HandleInputOn(GameObject inputObj)
  {
    // Handle input on signal
    PropagateSignal(inputObj, true);
  }

  private void HandleInputOff(GameObject inputObj)
  {
    // Handle input off signal
    PropagateSignal(inputObj, false);
  }

  private void HandleOutputOn(GameObject outputObj)
  {
    // Handle output on signal
  }

  private void HandleOutputOff(GameObject outputObj)
  {
    // Handle output off signal
  }

  private void PropagateSignal(GameObject sourceObj, bool signalState)
  {
    // Get all connections where the object is the source.
    var outgoingConnections = connections.Where(c => c.source == sourceObj);

    foreach (var connection in outgoingConnections)
    {
      // Update signal state
      connection.isActive = signalState;

      // Get destination component.
      var destinationInput = connection.destination.GetComponent<LogicGate>();
      var destinationOutput = connection.destination.GetComponent<OutputNode>();

      if (destinationInput != null)
      {
        // This is a logic gate - evaluate.
        EvaluateLogicGate(destinationInput);
      }
      else if (destinationOutput != null)
      {
        // This is an output node - propagate the signal.
        destinationOutput.setState(signalState);
      }
    }
  }

  private void EvaluateLogicGate(LogicGate gate)
  {
    // Get all connections feeding INTO this gate.
    var incomingConnections = connections.Where(c => c.destination == gate.gameObject);

    // Apply gate logic
    bool gateOutput = gate.EvaluateInputs(incomingConnections.ToArray());

    // If gate output has changed, propogate further.
    if (gate.currentOutput != gateOutput)
    {
      gate.currentOutput = gateOutput;

      PropagateSignal(gate.gameObject, gateOutput);
    }
  }
}