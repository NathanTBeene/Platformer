

using UnityEngine;

public class SaveData
{
    public static readonly string encryptionKey = "YourEncryptionKeyHere";
    public string sceneName;
    public Vector3 playerPosition;

    public SaveData(string sceneName, Vector3 playerPosition)
    {
        this.sceneName = sceneName;
        this.playerPosition = playerPosition;
    }

    public string ToJSON()
    {
        return JsonUtility.ToJson(this);
    }
}
