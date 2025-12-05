using System;
using UnityEngine;

// Base Signal Node
//
// This is the base signal node, meant to take a state and emit a signal
// based on it. To be overriden by Input and Output nodes.

public abstract class SignalNode : MonoBehaviour
{
  // Events
  public event Action<SignalNode> signalOn;
  public event Action<SignalNode> signalOff;
  public event Action<bool> onStateChange;

  public bool isActive = false;

  public abstract bool CanSend { get; }
  public abstract bool CanReceive { get; }

  public void setState(bool newState)
  {
    // If the state is the same, do nothing
    if (isActive == newState) return;

    isActive = newState;
    if (isActive)
      _emitOn();
    else
      _emitOff();

    onStateChange?.Invoke(isActive);
  }

  public bool getState()
  {
    return isActive;
  }


  protected virtual void _emitOn()
  {
    if (!CanSend) return;
    signalOn?.Invoke(this);
  }
  protected virtual void _emitOff()
  {
    if (!CanSend) return;
    signalOff?.Invoke(this);
  }


  public virtual void RecieveSignal(bool state)
  {
    if (!CanReceive) return;

    setState(state);
  }

  public void OnDrawGizmos()
  {
    Gizmos.color = isActive ? Color.green : Color.red;
    Gizmos.DrawSphere(transform.position, 0.1f);
  }

  [ContextMenu("Toggle State")]
  public void ToggleState()
  {
    setState(!isActive);
  }
}
