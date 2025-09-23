using System.Linq;
using UnityEngine;

public abstract class LogicGate : SignalNode
{

  public override bool CanSend => true;
  public override bool CanReceive => true;

  public bool input1 = false;
  public bool input2 = false;
  public bool output = false;

  public abstract bool EvaluateLogic(SignalNode[] inputNodes);
}



