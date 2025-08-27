[System.Serializable]
public class Connection
{
  public SignalNode source; // InputNode or LogicGate
  public SignalNode destination; // OutputNode or LogicGate
  public bool isActive;
  public float delay = 0f; // Delay in seconds
}