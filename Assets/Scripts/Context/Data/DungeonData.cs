using System;
using System.Collections.Generic;

[Serializable]
public class DungeonData
{
    public List<FloorData> floorDataList = new List<FloorData>();
    public PlayerStateInDungeon playerStateInDungeon = new PlayerStateInDungeon();
}

[Serializable]
public class FloorData
{
    public Dictionary<string, Queue<NPCData>> npcDataQueues = new Dictionary<string, Queue<NPCData>>();
    public Dictionary<string, Queue<TileData>> tileDataQueues = new Dictionary<string, Queue<TileData>>();
    public PlayerStateInFloor playerStateInFloor = new PlayerStateInFloor();
}

[Serializable]
public class PlayerStateInDungeon
{
    public int curFloorIndex = 0;
}

[Serializable]
public class PlayerStateInFloor
{
    public float posX = 0;
    public float posY = 0;
    public float posZ = 0;
}