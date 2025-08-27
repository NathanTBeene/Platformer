// AND Gate
using System.Linq;

public class ANDGate : LogicGate
{
  public override bool EvaluateInputs(Connection[] inputConnections)
  {
    return inputConnections.All(c => c.isActive);
  }
}