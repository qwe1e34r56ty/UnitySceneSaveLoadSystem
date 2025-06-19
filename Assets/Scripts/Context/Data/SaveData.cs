using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public DifficultLevel difficultLevel = DifficultLevel.Easy;
    public string curSceneName;
    public int KillCount = 0;
    public Dictionary<AchievementID, AchievementData> achievements = new Dictionary<AchievementID, AchievementData>();
    public PlayerData playerData = null;
    public DungeonData dungeonData = new DungeonData();
    public Dictionary<string, SceneData> sceneDatas = new Dictionary<string, SceneData>();
}

public enum DifficultLevel
{
    Easy,
    Normal,
    Hard,
    Impossible
}