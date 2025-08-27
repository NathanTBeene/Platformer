// OR Gate
using System.Linq;

public class ORGate : LogicGate
{
  public override bool EvaluateInputs(Connection[] inputConnections)
  {
    return inputConnections.Any(c => c.isActive);
  }
}