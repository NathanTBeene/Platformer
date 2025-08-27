using System.Linq;
using UnityEngine;

public abstract class LogicGate : InputNode
{
  public abstract bool EvaluateInputs(Connection[] inputConnections);
}



