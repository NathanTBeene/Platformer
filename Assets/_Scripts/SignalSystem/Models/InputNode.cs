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
  public static Action<InputNode> onInputOn;
  public static Action<InputNode> onInputOff;

  protected override void _emitOn()
  {
    var gameObject = this.gameObject;
    Debug.Log(gameObject.name + " emitted ON signal");
    onInputOn?.Invoke(this);
  }
  protected override void _emitOff()
  {
    var gameObject = this.gameObject;
    onInputOff?.Invoke(this);
  }
}