// OR Gate
using System.Linq;

public class ORGate : LogicGate
{
  public override bool EvaluateLogic(SignalNode[] inputNodes)
  {
    if (inputNodes.Length != 2) return false;
    SignalNode inputA = inputNodes[0];
    SignalNode inputB = inputNodes[1];

    this.input1 = inputA.isActive;
    this.input2 = inputB.isActive;
    this.output = inputA.isActive || inputB.isActive;

    return this.output;
  }
}