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

  private void HandleInputOn(InputNode inputNode)
  {
    Debug.Log("SignalManager: Input On from " + inputNode.gameObject.name);
    // Handle input on signal
    PropagateSignal(inputNode, true);
  }

  private void HandleInputOff(InputNode inputNode)
  {
    Debug.Log("SignalManager: Input Off from " + inputNode.gameObject.name);
    // Handle input off signal
    PropagateSignal(inputNode, false);
  }

  private void HandleOutputOn(OutputNode outputNode)
  {
    // Handle output on signal
  }

  private void HandleOutputOff(OutputNode outputNode)
  {
    // Handle output off signal
  }

  private void PropagateSignal(InputNode sourceNode, bool signalState)
  {
    // Get all connections where the object is the source.
    var outgoingConnections = connections.Where(c => c.source == sourceNode);
    Debug.Log($"Propagating signal from {sourceNode.gameObject.name} to {outgoingConnections.Count()} connections.");

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
    var incomingConnections = connections.Where(c => c.destination == gate);
    Debug.Log($"Evaluating {gate.gameObject.name} with {incomingConnections.Count()} incoming connections.");

    // Apply gate logic
    bool gateOutput = gate.EvaluateInputs(incomingConnections.ToArray());

    // If gate output has changed, propogate further.
    if (gate.isActive != gateOutput)
    {
      gate.isActive = gateOutput;

      PropagateSignal(gate, gateOutput);
    }
  }
}