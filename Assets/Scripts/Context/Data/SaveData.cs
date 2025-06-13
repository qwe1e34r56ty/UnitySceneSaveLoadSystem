using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SceneBundle
{
    public PlayerStateInScene playerStateInScene;
    public Queue<NPCData> npcDataQueue = new();
}

[Serializable]
public class SaveData
{
    public string curSceneName;
    public PlayerData playerData;
    public Dictionary<string, SceneBundle> sceneBundles = new();
}