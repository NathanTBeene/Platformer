using System;
using UnityEngine;

// Input Signal Node
//
// This node is attached to a GameObject that wants to emit a signal to the Signal Manager.
// Instead of a constant signal, the InputNode only cares about a singular trigger event.
// The SignalManager listens to this and handles propogation of events on it's own when it
// recieves the signal.

public class InputNode : SignalNode
{
  public override bool CanSend => true;
  public override bool CanReceive => false;
}