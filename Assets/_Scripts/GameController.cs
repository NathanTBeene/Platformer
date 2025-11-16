using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public static GameController Instance;
    [SerializeField] private EventSystem eventSystemInstance;

    [SerializeField] private string[] scenes;

    public bool IsPaused { get; private set; } = false;

    private SavePoint lastSavePoint;
    private const string SAVE_FILE_NAME = "gamesave";

    void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(eventSystemInstance.gameObject);
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            LoadMainMenu();
        }

    }

    // Removes duplicate EventSystems and ensures one persistent EventSystem exists
    private void _ensureEventSystem()
    {
        EventSystem[] eventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);

        if (eventSystems.Length > 1)
        {
            for (int i = 0; i < eventSystems.Length; i++)
            {
                if (eventSystems[i] != eventSystemInstance)
                {
                    Destroy(eventSystems[i].gameObject);
                }
            }
        }
        else if (eventSystems.Length == 0)
        {
            Instantiate(eventSystemInstance);
            DontDestroyOnLoad(eventSystemInstance.gameObject);
        }
    }

    #region Scene Management

    public void LoadMainMenu()
    {
        SetGameTime(true);
        StartCoroutine(LoadScene("MainMenu", false));
    }

    public void StartNewGame()
    {
        SetGameTime(true);
        lastSavePoint = null;
        StartCoroutine(LoadScene("Level1", false));
    }

    public IEnumerator LoadScene(string sceneName, bool isAdditive)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
        _ensureEventSystem();
    }

    public void UnloadScene(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
    }

    public void BackToMainMenu()
    {
        LoadMainMenu();
    }

    public void ContinueGame()
    {
        LoadGame();
    }

    #endregion

    #region Time Management

    public void ToggleGameTime()
    {
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
    }

    public void SetGameTime(bool playing)
    {
        IsPaused = !playing;
        Time.timeScale = playing ? 1f : 0f;
    }

    #endregion


    #region Pause Menu Management

    public void TogglePauseGame()
    {
        if (IsPaused)
        {
            HidePauseMenu();
            SetGameTime(true);
        }
        else
        {
            ShowPauseMenu();
            SetGameTime(false);
        }
    }

    public void ShowPauseMenu()
    {
        Debug.Log("Showing Pause Menu...");
        StartCoroutine(_loadPauseMenuAsync());
    }

    public void HidePauseMenu()
    {
        Debug.Log("Hiding Pause Menu...");
        StartCoroutine(_unloadPauseMenuAsync());
    }

    private IEnumerator _loadPauseMenuAsync()
    {
        if (!_isSceneLoaded("PauseMenu"))
        {
            yield return LoadScene("PauseMenu", true);
        }
    }

    private IEnumerator _unloadPauseMenuAsync()
    {
        if (_isSceneLoaded("PauseMenu"))
        {
            UnloadScene("PauseMenu");
            yield return null;
        }
    }

    #endregion


    #region Save System

    public void SetLastSavePoint(SavePoint savePoint)
    {
        lastSavePoint = savePoint;
    }

    public void SaveGame()
    {
        if (lastSavePoint == null)
        {
            Debug.LogWarning("No save point set. Cannot save game.");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        SaveData saveData = new SaveData(SceneManager.GetActiveScene().name, lastSavePoint);

        saveData.SaveToFile(SAVE_FILE_NAME);
        Debug.Log("Game saved.");
    }

    public void LoadGame()
    {
        SaveData saveData = SaveData.LoadFromFile(SAVE_FILE_NAME);
        if (saveData == null)
        {
            Debug.LogWarning("No save data found.");
            StartNewGame();
            return;
        }

        StartCoroutine(_loadGameCoroutine(saveData));
    }

    private IEnumerator _loadGameCoroutine(SaveData saveData)
    {
        SetGameTime(true);

        // Load the saved scene
        yield return LoadScene(saveData.sceneName, false);

        // Wait one frame for the scene to load
        yield return null;

        // Move player to the saved position
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = saveData.savePointPosition;
            Physics2D.SyncTransforms();
        }


        Debug.Log("Game loaded.");
    }

    public void RespawnAtLastSavePoint()
    {
        if (lastSavePoint != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = lastSavePoint.GetRespawnPosition();
            Physics2D.SyncTransforms();
        }
        else
        {
            Debug.LogWarning("No save point set. Cannot respawn.");
        }
    }

    public bool HasSaveData()
    {
        return SaveData.SaveExists(SAVE_FILE_NAME);
    }
    #endregion

    #region Utility

    public bool _isInMainMenu()
    {
        return SceneManager.GetActiveScene().name == "MainMenu";
    }

    private bool _isSceneLoaded(string sceneName)
    {
        return SceneManager.GetSceneByName(sceneName).isLoaded;
    }
    #endregion

    #if UNITY_EDITOR
    [ContextMenu("Test Save Game")]
    private void TestSaveGame()
    {
        SaveGame();
    }

    [ContextMenu("Test Load Game")]
    private void TestLoadGame()
    {
        LoadGame();
    }

    [ContextMenu("Log Save Path")]
    private void LogSavePath()
    {
        SaveData.LogSavePath();
    }

    [ContextMenu("Open Save Folder")]
    private void OpenSaveFolder()
    {
        SaveData.OpenSaveFolder();
    }

    [ContextMenu("Delete All Saves")]
    private void DeleteAllSaves()
    {
        string[] saves = SaveData.GetAllSaveFiles();
        foreach (string save in saves)
        {
            SaveData.DeleteSave(save);
        }
        Debug.Log($"Deleted {saves.Length} save files");
    }
    #endif

}
