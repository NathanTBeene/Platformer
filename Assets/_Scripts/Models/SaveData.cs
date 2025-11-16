using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public static readonly string encryptionKey = "PlatformerSaveKey123";
    private static readonly string SAVE_EXTENSION = ".sav";
    private static readonly string SAVE_FOLDER = "SaveData";

    // Save Point Data
    public string sceneName;
    public Vector3 savePointPosition;


    public SaveData()
    {
        // Default constructor for JSON serialization
    }

    public SaveData(string sceneName, SavePoint savePoint)
    {
        if (savePoint != null)
        {
            this.sceneName = sceneName;
            this.savePointPosition = savePoint.GetRespawnPosition();
        }
    }

    public string ToJSON()
    {
        return JsonUtility.ToJson(this);
    }

    public static string Encrypt(string plainText, string key)
    {
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] encryptedBytes = new byte[plainBytes.Length];

        for (int i = 0; i < plainBytes.Length; i++)
        {
            encryptedBytes[i] = (byte)(plainBytes[i] ^ keyBytes[i % keyBytes.Length]);
        }

        return System.Convert.ToBase64String(encryptedBytes);
    }

    public static string Decrypt(string encryptedText, string key)
    {
        byte[] encryptedBytes = System.Convert.FromBase64String(encryptedText);
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] decryptedBytes = new byte[encryptedBytes.Length];

        for (int i = 0; i < encryptedBytes.Length; i++)
        {
            decryptedBytes[i] = (byte)(encryptedBytes[i] ^ keyBytes[i % keyBytes.Length]);
        }

        return Encoding.UTF8.GetString(decryptedBytes);
    }

    private static string GetSaveFolderPath()
    {
        return Path.Combine(Application.persistentDataPath, SAVE_FOLDER);
    }

    private static void EnsureSaveFolderExists()
    {
        string folderPath = GetSaveFolderPath();
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }

    public void SaveToFile(string fileName)
    {
        string json = ToJSON();
        string encryptedData = Encrypt(json, encryptionKey);

        #if UNITY_WEBGL && !UNITY_EDITOR
            PlayerPrefs.SetString(fileName, encryptedData);
            PlayerPrefs.Save();
        #else
            EnsureSaveFolderExists();
            string filePath = Path.Combine(GetSaveFolderPath(), fileName + SAVE_EXTENSION);
            File.WriteAllText(filePath, encryptedData);
        #endif
    }

    public static SaveData LoadFromFile(string fileName)
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
            // Load from PlayerPrefs for WebGL builds
            if (PlayerPrefs.HasKey(fileName))
            {
                string encryptedData = PlayerPrefs.GetString(fileName);
                string json = Decrypt(encryptedData, encryptionKey);
                return JsonUtility.FromJson<SaveData>(json);
            }
        #else
            // Load from file system for desktop builds
            string filePath = Path.Combine(GetSaveFolderPath(), fileName + SAVE_EXTENSION);
            if (File.Exists(filePath))
            {
                string encryptedData = File.ReadAllText(filePath);
                string json = Decrypt(encryptedData, encryptionKey);
                return JsonUtility.FromJson<SaveData>(json);
            }
        #endif

        return null;
    }

    public static void DeleteSave(string fileName)
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
            if (PlayerPrefs.HasKey(fileName))
            {
                PlayerPrefs.DeleteKey(fileName);
                PlayerPrefs.Save();
            }
        #else
            string filePath = Path.Combine(GetSaveFolderPath(), fileName + SAVE_EXTENSION);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        #endif
    }

    public static bool SaveExists(string fileName)
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
            return PlayerPrefs.HasKey(fileName);
        #else
            string filePath = Path.Combine(GetSaveFolderPath(), fileName + SAVE_EXTENSION);
            return File.Exists(filePath);
        #endif
    }

    // Utility method to get all save files
    public static string[] GetAllSaveFiles()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
            // For web builds, you'd need to manually track save file names
            // This is a limitation of PlayerPrefs
            return new string[0];
        #else
            string savePath = GetSaveFolderPath();
            if (Directory.Exists(savePath))
            {
                string[] files = Directory.GetFiles(savePath, "*.sav");
                for (int i = 0; i < files.Length; i++)
                {
                    files[i] = Path.GetFileNameWithoutExtension(files[i]);
                }
                return files;
            }
            return new string[0];
        #endif
    }

    public static void OpenSaveFolder()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE_WIN
            string savePath = GetSaveFolderPath();
            EnsureSaveFolderExists();

            Debug.Log($"Opening save folder at: {savePath}");

            if (Directory.Exists(savePath))
            {
                // Use the full path with quotes to handle spaces
                System.Diagnostics.Process.Start("explorer.exe", $"\"{savePath}\"");
            }
            else
            {
                Debug.LogError($"Save folder does not exist at: {savePath}");
            }
        #else
            Debug.Log("Opening save folder is only supported on Windows.");
        #endif
    }

    // Add this debug method to see where saves are actually stored
    public static void LogSavePath()
    {
        string persistentPath = Application.persistentDataPath;
        string savePath = GetSaveFolderPath();

        Debug.Log($"Application.persistentDataPath: {persistentPath}");
        Debug.Log($"Save folder path: {savePath}");
        Debug.Log($"Save folder exists: {Directory.Exists(savePath)}");

        if (Directory.Exists(savePath))
        {
            string[] files = Directory.GetFiles(savePath);
            Debug.Log($"Files in save folder: {files.Length}");
            foreach (string file in files)
            {
                Debug.Log($"  - {Path.GetFileName(file)}");
            }
        }
    }
}
