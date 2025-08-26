using System.Linq;
using UnityEngine;

public abstract class LogicGate : InputNode
{
  public abstract bool EvaluateInputs(Connection[] inputConnections);
}

// AND Gate
public class ANDGate : LogicGate
{
  public override bool EvaluateInputs(Connection[] inputConnections)
  {
    return inputConnections.All(c => c.isActive);
  }
}

// OR Gate
public class ORGate : LogicGate
{
  public override bool EvaluateInputs(Connection[] inputConnections)
  {
    return inputConnections.Any(c => c.isActive);
  }
}