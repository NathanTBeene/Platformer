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

    [SerializeField] private GameScenesConfig gameScenesConfig;

    [SerializeField] private string initialScene2D;
    [SerializeField] private string initialScene3D;
    [SerializeField] private string initialSceneGUI;

    private List<SceneReference> _current2DScenes = new List<SceneReference>();
    private List<SceneReference> _current3DScenes = new List<SceneReference>();
    private List<SceneReference> _currentGUIScenes = new List<SceneReference>();

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
        if (!string.IsNullOrEmpty(initialScene2D))
            Change2DScene(initialScene2D, true, true);
        if (!string.IsNullOrEmpty(initialScene3D))
            Change3DScene(initialScene3D, true, true);
        if (!string.IsNullOrEmpty(initialSceneGUI))
            ChangeGUIScene(initialSceneGUI, true, true);
    }

    public void Change2DScene(string sceneName, bool visible = true, bool standalone = false)
    {
        SceneReference sceneRef = gameScenesConfig.SceneExists(sceneName, SceneType.Scene2D);
        if (sceneRef != null)
        {
            StartCoroutine(PreloadScene(sceneRef, visible));
            if (standalone)
            {
                foreach (var loadedScene in _current2DScenes)
                {
                    SceneManager.UnloadSceneAsync(loadedScene.Path);
                }
                _current2DScenes.Clear();
            }
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
            StartCoroutine(PreloadScene(sceneRef, visible));
            if (standalone)
            {
                foreach (var loadedScene in _current3DScenes)
                {
                    SceneManager.UnloadSceneAsync(loadedScene.Path);
                }
                _current3DScenes.Clear();
            }
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
            StartCoroutine(PreloadScene(sceneRef, visible));
            if (standalone)
            {
                foreach (var loadedScene in _currentGUIScenes)
                {
                    SceneManager.UnloadSceneAsync(loadedScene.Path);
                }
                _currentGUIScenes.Clear();
            }
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
}
