[System.Serializable]
public class Connection
{
  public string connectionName;
  public SignalNode source; // InputNode or LogicGate
  public SignalNode destination; // OutputNode or LogicGate
  public float delay = 0f; // Delay in seconds
}