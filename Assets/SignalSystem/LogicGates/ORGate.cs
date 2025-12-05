// OR Gate
using System.Linq;
using UnityEngine;

public class ORGate : LogicGate
{
    public override bool EvaluateLogic(SignalNode[] inputNodes)
    {

        if (inputNodes == null || inputNodes.Length == 0) return false;

        bool result = inputNodes.Any(node => node != null && node.isActive);

        this.input1 = inputNodes.Length > 0 && inputNodes[0] != null ? inputNodes[0].isActive : false;
        this.input2 = inputNodes.Length > 1 && inputNodes[1] != null ? inputNodes[1].isActive : false;
        this.output = result;

        return this.output;
    }
}
