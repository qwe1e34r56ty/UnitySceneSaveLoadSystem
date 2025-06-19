using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoSingleton<DataManager>
{
    [SerializeField] private string saveDataDir;
    [SerializeField] private string defaultSaveDataPath;
    [SerializeField] private string achivementSODir;
    [SerializeField] private List<string> abortSceneNameList = new();
    private HashSet<string> abortSceneNames = new();
    [SerializeField] private AchievementUnlockUI achievementUnlockUI;

    public GameContext gameContext;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        gameContext = new GameContext(saveDataDir, defaultSaveDataPath, achivementSODir, achievementUnlockUI);

        foreach(string name in abortSceneNameList)
        {
            abortSceneNames.Add(name);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    protected override void OnDestroy()
    {
        if (abortSceneNames.Contains(SceneManager.GetActiveScene().name))
        {
            return;
        }
        gameContext.SaveCurrentScene();
        gameContext.Save();
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
        gameContext.Save();
        gameContext.ClearBeforeLoad();
        gameContext.dontSaveCurSceneBundle = false;
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

    [Conditional("UNITY_EDITOR")]
    [ContextMenu("AddKillCountHundred")]
    public void AddKillCountHundred()
    {
        gameContext.AddKillCount(100);
    }

    [Conditional("UNITY_EDITOR")]
    [ContextMenu("SubKillCountHundred")]
    public void SubKillCountHundred()
    {
        gameContext.AddKillCount(-100);
    }

    [Conditional("UNITY_EDITOR")]
    [ContextMenu("ResetSave")]
    public void ResetSave()
    {
        gameContext.ResetSave();
    }

    [Conditional("UNITY_EDITOR")]
    [ContextMenu("ResetSave")]
    public void ClearBeforeLoad()
    {
        gameContext.ClearBeforeLoad();
    }
}
