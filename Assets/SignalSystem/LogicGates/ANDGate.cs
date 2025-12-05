// AND Gate
public class ANDGate : LogicGate
{
    public override bool EvaluateLogic(SignalNode[] inputNodes)
    {
        if (inputNodes == null || inputNodes.Length < 2) return false;

        SignalNode inputA = inputNodes[0];
        SignalNode inputB = inputNodes[1];

        if (inputA == null || inputB == null) return false;

        this.input1 = inputA.isActive;
        this.input2 = inputB.isActive;
        this.output = inputA.isActive && inputB.isActive;

        return this.output;
    }
}
