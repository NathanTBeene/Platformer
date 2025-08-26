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
  public static Action<OutputNode> onOutputOn;
  public static Action<OutputNode> onOutputOff;

  protected override void _emitOn()
  {
    var gameObject = this.gameObject;
    Debug.Log(gameObject.name + " recieved ON signal");
    onOutputOn?.Invoke(this);
  }
  protected override void _emitOff()
  {
    var gameObject = this.gameObject;
    Debug.Log(gameObject.name + " recieved OFF signal");
    onOutputOff?.Invoke(this);
  }
}