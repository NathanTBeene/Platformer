// NOT Gate
public class NOTGate : LogicGate
{
  public override bool EvaluateLogic(SignalNode[] inputNodes)
  {
    if (inputNodes.Length != 1) return false;
    SignalNode inputA = inputNodes[0];

    this.input1 = inputA.isActive;
    this.output = !inputA.isActive;

    return this.output;
  }
}
