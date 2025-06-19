using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SceneBundle
{
    public PlayerStateInScene playerStateInScene = new PlayerStateInScene();
    public Dictionary<string, Queue<NPCData>> npcDataQueues = new Dictionary<string, Queue<NPCData>>();
}

[Serializable]
public class SaveData
{
    public DifficultLevel difficultLevel = DifficultLevel.Easy;
    public string curSceneName;
    public int KillCount = 0;
    public Dictionary<AchievementID, AchievementData> achievements = new Dictionary<AchievementID, AchievementData>();
    public PlayerData playerData = null;
    public DungeonData dungeonData = new DungeonData();
    public Dictionary<string, SceneBundle> sceneBundles = new Dictionary<string, SceneBundle>();
}

public enum DifficultLevel
{
    Easy,
    Normal,
    Hard,
    Impossible
}