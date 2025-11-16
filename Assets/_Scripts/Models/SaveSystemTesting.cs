using UnityEditor;
using UnityEngine;

public class SaveSystemTester : MonoBehaviour
{
    [Header("Test Controls")]
    public string testFileName = "test_save";

    [Header("Debug Info")]
    public bool saveExists;
    public string saveFolderPath;

    private void Update()
    {
        // Update debug info
        saveExists = SaveData.SaveExists(testFileName);
        saveFolderPath = Application.persistentDataPath + "/SaveData";

        // Keyboard shortcuts for testing
        if (Input.GetKeyDown(KeyCode.F5))
        {
            TestSave();
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            TestLoad();
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            TestDelete();
        }

        if (Input.GetKeyDown(KeyCode.F8))
        {
            SaveData.OpenSaveFolder();
        }
    }

    [ContextMenu("Test Save")]
    public void TestSave()
    {
        if (GameController.Instance != null)
        {
            // Use the actual game controller's save system
            GameController.Instance.SaveGame();
        }
        else
        {
            // Create simple mock data for testing
            Vector3 playerPos = new Vector3(1, 2, 3);
            string sceneName = "TestScene";

            SaveData testSave = new SaveData(sceneName, null);
            testSave.SaveToFile(testFileName);

            Debug.Log("Test save created!");
        }
    }

    [ContextMenu("Test Load")]
    public void TestLoad()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.LoadGame();
        }
        else
        {
            SaveData loadedData = SaveData.LoadFromFile(testFileName);
            if (loadedData != null)
            {
                Debug.Log($"Loaded save data - Scene: {loadedData.sceneName}");
                Debug.Log($"Save Point Position: {loadedData.savePointPosition}");
            }
            else
            {
                Debug.Log("No save data found!");
            }
        }
    }

    [ContextMenu("Test Delete")]
    public void TestDelete()
    {
        SaveData.DeleteSave(testFileName);
        Debug.Log("Save deleted!");
    }

    [ContextMenu("Open Save Folder")]
    public void OpenSaveFolder()
    {
        SaveData.OpenSaveFolder();
    }

    [ContextMenu("List All Saves")]
    public void ListAllSaves()
    {
        string[] saves = SaveData.GetAllSaveFiles();
        Debug.Log($"Found {saves.Length} save files: {string.Join(", ", saves)}");
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("Save System Tester", EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).box);

        if (GUILayout.Button("Test Save (F5)"))
            TestSave();

        if (GUILayout.Button("Test Load (F6)"))
            TestLoad();

        if (GUILayout.Button("Delete Save (F7)"))
            TestDelete();

        if (GUILayout.Button("Open Save Folder (F8)"))
            OpenSaveFolder();

        if (GUILayout.Button("List All Saves"))
            ListAllSaves();

        GUILayout.Label($"Save Exists: {saveExists}");
        GUILayout.Label($"Save Path: {saveFolderPath}");

        GUILayout.EndArea();
    }
}
