using System;
using UnityEngine;

// Output Signal Node
//
// This node is attached to a GameObject that wants to recieve a signal from the SignalManager.
// Instead of a constant signal, the OutputNode only cares about a singular trigger event.
// The SignalManager listens to this and handles propogation of events on it's own when it
// emits the signal.

public class OutputNode : SignalNode
{
  public override bool CanSend => false;
  public override bool CanReceive => true;
}