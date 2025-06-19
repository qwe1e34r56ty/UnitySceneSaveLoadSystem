using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class GameContext
{
    private readonly string saveDataPath;
    private readonly string defaultSaveDataPath;
    private readonly string achievementSODir;
    private readonly AchievementUnlockUI achievementUnlockUI;

    public SaveData saveData;

    private Dictionary<AchievementID, AchievementSO> achievementSOs = 
        new Dictionary<AchievementID, AchievementSO>();

    #region Save Option
    public bool dontSaveCurSceneBundle = false;
    #endregion  

    #region Object In Scene
    public APlayer player = null;
    public HashSet<ANPC> npcs = new HashSet<ANPC>();
    public HashSet<ATile> tiles = new HashSet<ATile>();
    #endregion

    #region Temp Scene Data
    public PlayerStateInScene playerStateInScene = new PlayerStateInScene();
    public Dictionary<ANPC, NPCData> npcDatas = new Dictionary<ANPC, NPCData>();
    public Dictionary<ATile, TileData> tileDatas = new Dictionary<ATile, TileData>();
    #endregion

    #region Temp Floor Data
    public PlayerStateInFloor playerStateInFloor = new PlayerStateInFloor();
    #endregion


    #region API
    public void SelectDifficultLevel(DifficultLevel difficultLevel)
    {
        saveData.difficultLevel = difficultLevel;
    }
    
    public DifficultLevel GetDifficultLevel()
    {
        return saveData.difficultLevel;
    }

    public void AddKillCount(int count)
    {
        int tmp = saveData.KillCount;
        saveData.KillCount += count;
        if (tmp < 1 && saveData.KillCount >= 1)
        {
            Unlock(AchievementID.FirstBlood);
        }
        if (tmp < 5 && saveData.KillCount >= 5)
        {
            Unlock(AchievementID.PentaKill);
        }
    }

    public void ClearCurSceneBundle()
    {
        saveData.sceneDatas.Remove(saveData.curSceneName);
    }

    public void DontSaveCurSceneBundle()
    {
        dontSaveCurSceneBundle = true;
    }

    public void Unlock(AchievementID achievementID)
    {
        AchievementData achievementData;
        if (!saveData.achievements.ContainsKey(achievementID))
        {
            achievementData = new AchievementData(achievementID, false);
            saveData.achievements.Add(achievementID, achievementData);
        }
        else
        {
            achievementData = saveData.achievements[achievementID];
        }
        if (achievementData.unlocked == false)
        {
            if (achievementSOs.TryGetValue(achievementID, out AchievementSO achievementSO))
            {
                achievementData.unlocked = true;
                achievementUnlockUI.Unlock(achievementSO);
            }
        }
    }
    #endregion

    public GameContext(string saveDataPath,
        string defaultSaveDataPath,
        string achievementSODir,
        AchievementUnlockUI achievementUnlockUI)
    {
        this.saveDataPath = saveDataPath;
        this.defaultSaveDataPath = defaultSaveDataPath;
        this.achievementSODir = achievementSODir;
        this.achievementUnlockUI = achievementUnlockUI;
        InitializeAchievements();
        if (File.Exists(saveDataPath))
        {
            Load();
        }
        else if (File.Exists(defaultSaveDataPath))
        {
            LoadDefault();
            Save();
        }
        else
        {
            saveData = new SaveData();
            Save();
        }
    }

    public void RegisterNPC(ANPC anpc, NPCData data)
    {
        npcDatas[anpc] = data;
    }

    public void UnregisterNPC(ANPC anpc)
    {
        npcDatas.Remove(anpc);
    }

    public void RegisterTile(ATile atile, TileData data)
    {
        tileDatas[atile] = data;
    }

    public void UnregisterTile(ATile atile)
    {
        tileDatas.Remove(atile);
    }


    private void InitializeAchievements()
    {
        AchievementSO[] loadedSOs = Resources.LoadAll<AchievementSO>(achievementSODir);
        for(int i = 0; i < loadedSOs.Length; i++)
        {
            achievementSOs[loadedSOs[i].achievementID] = loadedSOs[i];
        }
    }

    public void Load()
    {
        string json = File.ReadAllText(saveDataPath);
        saveData = JsonConvert.DeserializeObject<SaveData>(json);
    }

    public void LoadDefault()
    {
        string json = File.ReadAllText(defaultSaveDataPath);
        saveData = JsonConvert.DeserializeObject<SaveData>(json);
    }

    public void Save()
    {
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        File.WriteAllText(saveDataPath, json);
    }

    public void ResetSave()
    {
        LoadDefault();
        Save();
    }

    public void ClearBeforeLoad()
    {
        if (player != null)
        {
            GameObject.Destroy(player.gameObject);
        }
        foreach (ANPC aNPC in npcDatas.Keys)
        {
            if (aNPC != null)
            {
                GameObject.Destroy(aNPC.gameObject);
            }
        }
        foreach (ATile aTile in tileDatas.Keys)
        {
            if (aTile != null)
            {
                GameObject.Destroy(aTile.gameObject);
            }
        }
        npcDatas.Clear();
        tileDatas.Clear();
    }
    public void SetCurrentScene(string sceneName)
    {
        saveData.curSceneName = sceneName;
    }

    #region Scene Data Manipulation Method
    public void SaveCurrentScene()
    {
        if (player != null)
        {
            player.Save();
        }
        foreach (ANPC aNPC in npcDatas.Keys)
        {
            if (aNPC != null)
            {
                aNPC.Save();
            }
        }
        foreach (ATile aTile in tileDatas.Keys)
        {
            if (aTile != null)
            {
                aTile.Save();
            }
        }
        if (!dontSaveCurSceneBundle)
        {
            Dictionary<string, Queue<NPCData>> npcDataQueues = new Dictionary<string, Queue<NPCData>>();
            foreach(KeyValuePair<ANPC, NPCData> pair in npcDatas)
            {
                string prefabPath = pair.Value.prefabPath;
                if (!npcDataQueues.ContainsKey(prefabPath))
                {
                    npcDataQueues.Add(prefabPath, new Queue<NPCData>());
                }
                npcDataQueues[prefabPath].Enqueue(pair.Value);
            }
            Dictionary<string, Queue<TileData>> tileDataQueues = new Dictionary<string, Queue<TileData>>();
            foreach (KeyValuePair<ATile, TileData> pair in tileDatas)
            {
                string prefabPath = pair.Value.prefabPath;
                if (!tileDataQueues.ContainsKey(prefabPath))
                {
                    tileDataQueues.Add(prefabPath, new Queue<TileData>());
                }
                tileDataQueues[prefabPath].Enqueue(pair.Value);
            }
            var bundle = new SceneData
            {
                playerStateInScene = playerStateInScene,
                npcDataQueues = npcDataQueues,
                tileDataQueues = tileDataQueues
            };
            if (saveData.sceneDatas.ContainsKey(saveData.curSceneName))
            {
                saveData.sceneDatas[saveData.curSceneName] = bundle;
            }
            else
            {
                saveData.sceneDatas.Add(saveData.curSceneName, bundle);
            }
        }
    }

    public void LoadCurrentSceneData()
    {
        string sceneName = saveData.curSceneName;

        if (saveData.sceneDatas.TryGetValue(sceneName, out var bundle))
        {
            playerStateInScene = bundle.playerStateInScene ?? new PlayerStateInScene();
            npcDatas.Clear();

            Logger.Log($"[GameContext] Loaded existing SceneBundle for: {sceneName}");
        }
        else
        {
            Logger.Log($"[GameContext] No saved SceneBundle found for: {sceneName}, initialized empty runtime state.");
        }
    }

    public bool IsSceneSaved(string sceneName)
    {
        if (saveData.sceneDatas.ContainsKey(sceneName))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region Floor Data Manipulation Method
    public void SaveCurrentFloor()
    {
        if (player != null)
        {
            player.Save();
        }
        foreach (ANPC aNPC in npcDatas.Keys)
        {
            if (aNPC != null)
            {
                aNPC.Save();
            }
        }
        foreach (ATile aTile in tileDatas.Keys)
        {
            if (aTile != null)
            {
                aTile.Save();
            }
        }
        int curFloorIndex = saveData.dungeonData.playerStateInDungeon.curFloorIndex;

        while (saveData.dungeonData.floorDataList.Count <= curFloorIndex)
        {
            saveData.dungeonData.floorDataList.Add(new FloorData());
        }

        Dictionary<string, Queue<NPCData>> npcDataQueues = new();
        foreach (KeyValuePair<ANPC, NPCData> pair in npcDatas)
        {
            string prefabPath = pair.Value.prefabPath;
            if (!npcDataQueues.ContainsKey(prefabPath))
            {
                npcDataQueues[prefabPath] = new Queue<NPCData>();
            }
            npcDataQueues[prefabPath].Enqueue(pair.Value);
        }
        Dictionary<string, Queue<TileData>> tileDataQueues = new Dictionary<string, Queue<TileData>>();
        foreach (KeyValuePair<ATile, TileData> pair in tileDatas)
        {
            string prefabPath = pair.Value.prefabPath;
            if (!tileDataQueues.ContainsKey(prefabPath))
            {
                tileDataQueues.Add(prefabPath, new Queue<TileData>());
            }
            tileDataQueues[prefabPath].Enqueue(pair.Value);
        }

        saveData.dungeonData.floorDataList[curFloorIndex].npcDataQueues = npcDataQueues;
        saveData.dungeonData.floorDataList[curFloorIndex].tileDataQueues = tileDataQueues;
        saveData.dungeonData.floorDataList[curFloorIndex].playerStateInFloor = playerStateInFloor;
    }

    public void LoadCurrentFloorData()
    {
        int curFloorIndex = saveData.dungeonData.playerStateInDungeon.curFloorIndex;
        if (saveData.dungeonData.floorDataList.Count > curFloorIndex)
        {
            playerStateInFloor = saveData.dungeonData.floorDataList[curFloorIndex].playerStateInFloor ?? new PlayerStateInFloor();
            npcDatas.Clear();
            tileDatas.Clear();

            Logger.Log($"[GameContext] Loaded existing PlayerStateInFloor for: Floor {curFloorIndex}");
        }
        else
        {
            Logger.Log($"[GameContext] No saved PlayerStateInFloor found for: Floor {curFloorIndex}, initialized empty runtime state.");
        }
    }
    #endregion
}
