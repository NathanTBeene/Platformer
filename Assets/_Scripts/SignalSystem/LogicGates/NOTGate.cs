// AND Gate
using System.Linq;

public class NOTGate : LogicGate
{
  public override bool EvaluateInputs(Connection[] inputConnections)
  {
    return inputConnections.Length == 1 ? !inputConnections[0].isActive : false;
  }
}