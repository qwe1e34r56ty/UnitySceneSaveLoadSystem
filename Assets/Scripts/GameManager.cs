using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private string saveDataDir;
    [SerializeField] private string defaultSaveDataPath;
    [SerializeField] private List<string> abortSceneNameList = new();
    private HashSet<string> abortSceneNames = new();

    public GameContext gameContext;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
        gameContext = new GameContext(saveDataDir, defaultSaveDataPath);

        foreach(string name in abortSceneNameList)
        {
            abortSceneNames.Add(name);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    protected override void OnApplicationQuit()
    {
        if (abortSceneNames.Contains(SceneManager.GetActiveScene().name))
        {
            return;
        }
        gameContext.SaveCurrentScene();
            gameContext.Save();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (abortSceneNames.Contains(SceneManager.GetActiveScene().name))
        {
            return;
        }
        gameContext.SetCurrentScene(scene.name);
        gameContext.LoadCurrentSceneData();
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (abortSceneNames.Contains(SceneManager.GetActiveScene().name))
        {
            return;
        }
        gameContext.SaveCurrentScene();
    }

    [ContextMenu("ClearCurSceneBundle")]
    public void ClearCurSceneBundle()
    {
        gameContext.ClearCurSceneBundle();
    }

    [ContextMenu("DontSaveCurSceneBundle")]
    public void DontSaveCurSceneBundle()
    {
        gameContext.DontSaveCurSceneBundle();
    }
}
