using System.Collections;
using System.Collections.Generic;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public enum SceneType
    {
        Scene2D,
        Scene3D,
        SceneGUI
    }

    [System.Serializable]
    public class SceneConf
    {
        public string name;
        public SceneReference reference;
        public SceneType type;
    }

    [System.Serializable]
    public class GameScenesConfig
    {
        public SceneConf[] scenes2D = new SceneConf[0];
        public SceneConf[] scenes3D = new SceneConf[0];
        public SceneConf[] scenesGUI = new SceneConf[0];

        public SceneReference SceneExists(string sceneName, SceneType sceneType)
        {
            SceneConf[] scenesArray = sceneType switch
            {
                SceneType.Scene2D => scenes2D,
                SceneType.Scene3D => scenes3D,
                SceneType.SceneGUI => scenesGUI,
                _ => null
            };

            if (scenesArray == null) return null;

            foreach (var scene in scenesArray)
            {
                if (scene.name == sceneName)
                {
                    return scene.reference;
                }
            }
            return null;
        }
    }

    public static GameController Instance;
    public EventSystem EventSystemInstance;
    public bool IsPaused = false;

    [SerializeField] private GameScenesConfig gameScenesConfig;

    [SerializeField] private string initialScene2D;
    [SerializeField] private string initialScene3D;
    [SerializeField] private string initialSceneGUI;

    private List<SceneReference> _current2DScenes = new List<SceneReference>();
    private List<SceneReference> _current3DScenes = new List<SceneReference>();
    private List<SceneReference> _currentGUIScenes = new List<SceneReference>();

    private SavePoint lastSavePoint;
    private const string SAVE_FILE_NAME = "gamesave";

    private void OnEnable() {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Find the EventSystem in the scene or create one if it doesn't exist
        EventSystem eventSystem = FindFirstObjectByType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystem = eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
            DontDestroyOnLoad(eventSystemGO);
            EventSystemInstance = eventSystem;
        }
        else
        {
            DontDestroyOnLoad(eventSystem.gameObject);
            EventSystemInstance = eventSystem;
        }
    }

    private void Start()
    {
        if (!HasMainMenuLoaded())
        {
            if (!string.IsNullOrEmpty(initialScene2D))
                Change2DScene(initialScene2D, true, true);
            if (!string.IsNullOrEmpty(initialScene3D))
                Change3DScene(initialScene3D, true, true);
            if (!string.IsNullOrEmpty(initialSceneGUI))
                ChangeGUIScene(initialSceneGUI, true, true);
        }

    }

    public void Change2DScene(string sceneName, bool visible = true, bool standalone = false)
    {
        SceneReference sceneRef = gameScenesConfig.SceneExists(sceneName, SceneType.Scene2D);
        if (sceneRef != null)
        {
            if (standalone)
            {
                foreach (var loadedScene in _current2DScenes)
                {
                    SceneManager.UnloadSceneAsync(loadedScene.Path);
                }
                _current2DScenes.Clear();
            }

            StartCoroutine(PreloadScene(sceneRef, visible));
            _current2DScenes.Add(sceneRef);
        }
        else
        {
            Debug.LogWarning($"Scene '{sceneName}' does not exist in 2D scenes configuration.");
        }
    }

    public void Change3DScene(string sceneName, bool visible = true, bool standalone = false)
    {
        SceneReference sceneRef = gameScenesConfig.SceneExists(sceneName, SceneType.Scene3D);
        if (sceneRef != null)
        {
            if (standalone)
            {
                foreach (var loadedScene in _current3DScenes)
                {
                    SceneManager.UnloadSceneAsync(loadedScene.Path);
                }
                _current3DScenes.Clear();
            }

            StartCoroutine(PreloadScene(sceneRef, visible));
            _current3DScenes.Add(sceneRef);
        }
        else
        {
            Debug.LogWarning($"Scene '{sceneName}' does not exist in 3D scenes configuration.");
        }
    }

    public void ChangeGUIScene(string sceneName, bool visible = true, bool standalone = false)
    {
        SceneReference sceneRef = gameScenesConfig.SceneExists(sceneName, SceneType.SceneGUI);
        if (sceneRef != null)
        {
            if (standalone)
            {
                foreach (var loadedScene in _currentGUIScenes)
                {
                    SceneManager.UnloadSceneAsync(loadedScene.Path);
                }
                _currentGUIScenes.Clear();
            }

            StartCoroutine(PreloadScene(sceneRef, visible));
            _currentGUIScenes.Add(sceneRef);
        }
        else
        {
            Debug.LogWarning($"Scene '{sceneName}' does not exist in GUI scenes configuration.");
        }
    }

    // An async method to preload a scene in the background
    // Default is to load it as visible
    private IEnumerator PreloadScene(SceneReference sceneRef, bool visible = true)
    {
        // Always load scenes additively for preloading
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneRef.Path, LoadSceneMode.Additive);

        yield return asyncLoad;

        _checkForDuplicateEventSystem();
        _checkForDuplicateAudioListener();
        Scene loadedScene = SceneManager.GetSceneByPath(sceneRef.Path);

        if (!visible)
        {
            GameObject[] rootObjects = loadedScene.GetRootGameObjects();
            foreach (GameObject obj in rootObjects)
            {
                obj.SetActive(false);
            }
        }
    }

    public void UnloadScene(string name)
    {
        SceneReference sceneRef = gameScenesConfig.SceneExists(name, SceneType.Scene2D);
        if (sceneRef != null && _current2DScenes.Contains(sceneRef))
        {
            SceneManager.UnloadSceneAsync(sceneRef.Path);
            _current2DScenes.Remove(sceneRef);
            return;
        }

        sceneRef = gameScenesConfig.SceneExists(name, SceneType.Scene3D);
        if (sceneRef != null && _current3DScenes.Contains(sceneRef))
        {
            SceneManager.UnloadSceneAsync(sceneRef.Path);
            _current3DScenes.Remove(sceneRef);
            return;
        }

        sceneRef = gameScenesConfig.SceneExists(name, SceneType.SceneGUI);
        if (sceneRef != null && _currentGUIScenes.Contains(sceneRef))
        {
            SceneManager.UnloadSceneAsync(sceneRef.Path);
            _currentGUIScenes.Remove(sceneRef);
            return;
        }

        Debug.LogWarning($"Scene '{name}' is not currently loaded and cannot be unloaded.");
    }

    private void _checkForDuplicateEventSystem()
    {
        var eventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        if (eventSystems.Length > 1)
        {
            for (int i = 0; i < eventSystems.Length; i++)
            {
                if (eventSystems[i] != EventSystemInstance)
                {
                    Destroy(eventSystems[i].gameObject);
                }
            }
        }
    }

    private void _checkForDuplicateAudioListener()
    {
        var audioListeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        if (audioListeners.Length > 1)
        {
            for (int i = 0; i < audioListeners.Length; i++)
            {
                if (audioListeners[i] != Camera.main.GetComponent<AudioListener>())
                {
                    Destroy(audioListeners[i]);
                }
            }
        }
    }

    public void TogglePauseGame()
    {
        // Check if we are on the main menu
        if (SceneManager.GetActiveScene().name == "MainMenu") return;

        if (IsPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        // Additvely add a pause menu scene
        ChangeGUIScene("PauseMenu", true, false);
        Time.timeScale = 0f;
        IsPaused = true;
    }

    public void ResumeGame()
    {
        UnloadScene("PauseMenu");
        Time.timeScale = 1f;
        IsPaused = false;
    }

    public void BackToMainMenu()
    {
        var current2DScenes = new List<SceneReference>(_current2DScenes);
        var current3DScenes = new List<SceneReference>(_current3DScenes);
        var currentGUIScenes = new List<SceneReference>(_currentGUIScenes);

        foreach (var scene in current2DScenes)
        {
            string sceneName = _getSceneNameFromReference(scene);
            UnloadScene(sceneName);
        }

        foreach (var scene in current3DScenes)
        {
            string sceneName = _getSceneNameFromReference(scene);
            UnloadScene(sceneName);
        }

        foreach (var scene in currentGUIScenes)
        {
            string sceneName = _getSceneNameFromReference(scene);
            UnloadScene(sceneName);
        }

        // Load the main menu scene
        ChangeGUIScene("MainMenu", true, true);
        if (IsPaused)
        {
            Time.timeScale = 1f;
            IsPaused = false;
        }
    }

    private string _getSceneNameFromReference(SceneReference sceneRef)
    {
        string path = sceneRef.Path;
        int lastSlashIndex = path.LastIndexOf('/');
        int lastDotIndex = path.LastIndexOf('.');

        if (lastSlashIndex >= 0 && lastDotIndex > lastSlashIndex)
        {
            return path.Substring(lastSlashIndex + 1, lastDotIndex - lastSlashIndex - 1);
        }

        return path; // Fallback to full path if parsing fails
    }

    public void SetLastSavePoint(SavePoint savePoint)
    {
        lastSavePoint = savePoint;
    }

    // Check if a save point exists
    // And check if it's somewhere in the currently loaded scenes
    public bool SavePointExists()
    {
        if (lastSavePoint == null) return false;

        Scene savePointScene = lastSavePoint.gameObject.scene;
        foreach (var sceneRef in _current2DScenes)
        {
            Scene loadedScene = SceneManager.GetSceneByPath(sceneRef.Path);
            if (loadedScene == savePointScene)
            {
                return true;
            }
        }
        foreach (var sceneRef in _current3DScenes)
        {
            Scene loadedScene = SceneManager.GetSceneByPath(sceneRef.Path);
            if (loadedScene == savePointScene)
            {
                return true;
            }
        }
        foreach (var sceneRef in _currentGUIScenes)
        {
            Scene loadedScene = SceneManager.GetSceneByPath(sceneRef.Path);
            if (loadedScene == savePointScene)
            {
                return true;
            }
        }

        return false;
    }

    public void SaveGame()
    {
        if (lastSavePoint == null)
        {
            Debug.LogWarning("No save point set. Cannot save game.");
            return;
        }

        string sceneName = lastSavePoint.gameObject.scene.name;

        // Get current player position
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 playerPos = player != null ? player.transform.position : Vector3.zero;

        // Get currently loaded scenes
        List<string> scenes2D = GetSceneNames(_current2DScenes);
        List<string> scenes3D = GetSceneNames(_current3DScenes);
        List<string> scenesGUI = GetSceneNames(_currentGUIScenes);

        SaveData saveData = new SaveData(playerPos, scenes2D, scenes3D, scenesGUI, sceneName, lastSavePoint);
        saveData.SaveToFile(SAVE_FILE_NAME);

        Debug.Log("Game saved.");
    }

    public void LoadGame()
    {
        SaveData saveData = SaveData.LoadFromFile(SAVE_FILE_NAME);
        if (saveData == null)
        {
            Debug.Log("No save data found.");
            return;
        }

        StartCoroutine(LoadGameCoroutine(saveData));
    }

    private IEnumerator LoadGameCoroutine(SaveData saveData)
    {
        UnloadAllScenes();
        yield return null; // Wait a frame for scenes to unload

        // Load all scenes
        foreach (var sceneName in saveData.loadedScenes2D)
        {
            Change2DScene(sceneName, true, false);
            yield return null;
        }
        foreach (var sceneName in saveData.loadedScenes3D)
        {
            Change3DScene(sceneName, true, false);
            yield return null;
        }
        foreach (var sceneName in saveData.loadedScenesGUI)
        {
            ChangeGUIScene(sceneName, true, false);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f); // Wait for scenes to load

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = saveData.savePointPosition;
        }

        Debug.Log("Game loaded.");
    }

    private bool HasMainMenuLoaded()
    {
        // Check if MainMenu is in the currently loaded GUI scenes
        foreach (var sceneRef in _currentGUIScenes)
        {
            string sceneName = _getSceneNameFromReference(sceneRef);
            if (sceneName == "MainMenu")
                return true;
        }
        return false;
    }

    private List<string> GetSceneNames(List<SceneReference> sceneRefs)
    {
        List<string> sceneNames = new List<string>();
        foreach (var sceneRef in sceneRefs)
        {
            string sceneName = _getSceneNameFromReference(sceneRef);
            sceneNames.Add(sceneName);
        }
        return sceneNames;
    }

    private void UnloadAllScenes()
    {
        var scenes2D = new List<SceneReference>(_current2DScenes);
        var scenes3D = new List<SceneReference>(_current3DScenes);
        var scenesGUI = new List<SceneReference>(_currentGUIScenes);

        foreach (var scene in scenes2D)
            UnloadScene(_getSceneNameFromReference(scene));
        foreach (var scene in scenes3D)
            UnloadScene(_getSceneNameFromReference(scene));
        foreach (var scene in scenesGUI)
            UnloadScene(_getSceneNameFromReference(scene));
    }

    public void RespawnAtLastSavePoint()
    {
        if (lastSavePoint != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = lastSavePoint.GetRespawnPosition();
            }
        }
    }

    public void StartNewGame()
    {
        StartCoroutine(StartNewGameCoroutine());
    }

    private IEnumerator StartNewGameCoroutine()
    {
        Change2DScene("Level1", false, true);

        yield return new WaitForSeconds(0.5f); // Wait for scene to load

        MakeLoadedScenesVisible();

        UnloadScene("MainMenu");
    }

    public void ContinueGame()
    {
        StartCoroutine(ContinueGameCoroutine());
    }

    private IEnumerator ContinueGameCoroutine()
    {
        SaveData saveData = SaveData.LoadFromFile(SAVE_FILE_NAME);
        if (saveData == null)
        {
            Debug.Log("No save data found.");
            yield return StartNewGameCoroutine();
            yield break;
        }

        // Load all scenes in background
        foreach (var sceneName in saveData.loadedScenes2D)
        {
            Change2DScene(sceneName, false, false);
            yield return new WaitForSeconds(0.1f);
        }
        foreach (var sceneName in saveData.loadedScenes3D)
        {
            Change3DScene(sceneName, false, false);
            yield return new WaitForSeconds(0.1f);
        }
        foreach (var sceneName in saveData.loadedScenesGUI)
        {
            if (sceneName == "MainMenu") continue; // Skip MainMenu
            ChangeGUIScene(sceneName, false, false);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f); // Wait for scenes to load

        // Position player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = saveData.savePointPosition;
        }

        MakeLoadedScenesVisible();

        UnloadScene("MainMenu");
    }

    private void MakeLoadedScenesVisible()
    {
        // Make all 2D scenes visible
        foreach (var sceneRef in _current2DScenes)
        {
            Scene scene = SceneManager.GetSceneByPath(sceneRef.Path);
            if (scene.isLoaded)
            {
                GameObject[] rootObjects = scene.GetRootGameObjects();
                foreach (GameObject obj in rootObjects)
                {
                    obj.SetActive(true);
                }
            }
        }

        // Make all 3D scenes visible
        foreach (var sceneRef in _current3DScenes)
        {
            Scene scene = SceneManager.GetSceneByPath(sceneRef.Path);
            if (scene.isLoaded)
            {
                GameObject[] rootObjects = scene.GetRootGameObjects();
                foreach (GameObject obj in rootObjects)
                {
                    obj.SetActive(true);
                }
            }
        }

        // Make all GUI scenes visible (except MainMenu)
        foreach (var sceneRef in _currentGUIScenes)
        {
            Scene scene = SceneManager.GetSceneByPath(sceneRef.Path);
            string sceneName = _getSceneNameFromReference(sceneRef);

            if (scene.isLoaded && sceneName != "MainMenu")
            {
                GameObject[] rootObjects = scene.GetRootGameObjects();
                foreach (GameObject obj in rootObjects)
                {
                    obj.SetActive(true);
                }
            }
        }
    }

    public bool HasSaveData()
    {
        return SaveData.SaveExists(SAVE_FILE_NAME);
    }



    // Editor testing methods
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
