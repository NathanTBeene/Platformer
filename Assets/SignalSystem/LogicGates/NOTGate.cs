// NOT Gate
public class NOTGate : LogicGate
{
    public override bool EvaluateLogic(SignalNode[] inputNodes)
    {
        if (inputNodes == null || inputNodes.Length == 0) return false;

        SignalNode inputA = inputNodes[0];
        if (inputA == null) return false;

        this.input1 = inputA.isActive;
        this.output = !inputA.isActive;

        return this.output;
    }
}
