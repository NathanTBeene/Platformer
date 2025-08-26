using System;
using UnityEngine;

// Base Signal Node
//
// This is the base signal node, meant to take a state and emit a signal
// based on it. To be overriden by Input and Output nodes.

public abstract class SignalNode : MonoBehaviour
{
  public bool isActive = false;

  public void setState(bool newState)
  {
    isActive = newState;
    if (isActive)
      _emitOn();
    else
      _emitOff();
  }

  protected abstract void _emitOn();
  protected abstract void _emitOff();
}