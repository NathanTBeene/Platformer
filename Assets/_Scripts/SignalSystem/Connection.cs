[System.Serializable]
public class Connection
{
  public InputNode source; // InputNode or LogicGate
  public OutputNode destination; // OutputNode or LogicGate
  public bool isActive;
  public float delay = 0f; // Delay in seconds
}