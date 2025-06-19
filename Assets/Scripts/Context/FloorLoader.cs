using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FloorLoader : MonoBehaviour
{
    GameContext gameContext;
    DungeonData dungeonData;

    protected void Awake()
    {
        gameContext = DataManager.Instance.gameContext;
        gameContext.DontSaveCurSceneBundle();
        dungeonData = gameContext.saveData.dungeonData;
        while (dungeonData.floorDataList.Count <= 5)
        {
            dungeonData.floorDataList.Add(new FloorData());
        }
        gameContext.LoadCurrentFloorData();
    }

    protected void Start()
    {
        LoadFloor();
    }

    protected void LoadFloor()
    {
        PlayerStateInDungeon playerStateInDungeon = dungeonData.playerStateInDungeon;
        foreach (var kvp in dungeonData.floorDataList[playerStateInDungeon.curFloorIndex].npcDataQueues)
        {
            Queue<NPCData> queue = kvp.Value;
            while (queue.Count > 0)
            {
                NPCData npcData = queue.Dequeue();

                GameObject prefab = Resources.Load<GameObject>(npcData.prefabPath);
                if (prefab != null)
                {
                    GameObject npc = GameObject.Instantiate(prefab);
                    npc.transform.position = new Vector3(npcData.posX, npcData.posY, npcData.posZ);

                    if (npc.TryGetComponent<ANPC>(out ANPC _npc))
                    {
                        _npc.npcData = npcData;
                    }
                }
                else
                {
                    Logger.LogError($"Prefab not found at path: {npcData.prefabPath}");
                }
            }
        }
        foreach (var kvp in dungeonData.floorDataList[playerStateInDungeon.curFloorIndex].tileDataQueues)
        {
            Queue<TileData> queue = kvp.Value;
            while (queue.Count > 0)
            {
                TileData tileData = queue.Dequeue();

                GameObject prefab = Resources.Load<GameObject>(tileData.prefabPath);
                if (prefab != null)
                {
                    GameObject tile = GameObject.Instantiate(prefab);
                    tile.transform.position = new Vector3(tileData.posX, tileData.posY, tileData.posZ);
                    if (tile.TryGetComponent<ATile>(out ATile _tile))
                    {
                        _tile.tileData = tileData;
                    }
                }
                else
                {
                    Logger.LogError($"Prefab not found at path: {tileData.prefabPath}");
                }
            }
        }
    }

    protected void ChangeFloor(int index)
    {
        int targetFloor = dungeonData.playerStateInDungeon.curFloorIndex + index;
        if (targetFloor < dungeonData.floorDataList.Count && targetFloor > -1)
        {
            gameContext.SaveCurrentFloor();
            gameContext.Save();
            gameContext.ClearBeforeLoad();
            dungeonData.playerStateInDungeon.curFloorIndex = targetFloor;
            LoadFloor();
        }
        else
        {
            Logger.Log($"[FloorLoader] targetFloor index out! [targetFloor : {targetFloor}]");
        }
    }

    public void DescendFloor()
    {
        ChangeFloor(1);
    }

    public void AscendFloor()
    {
        ChangeFloor(-1);
    }

    protected void OnDestroy()
    {
        gameContext.SaveCurrentFloor();
        gameContext.Save();
    }

    protected void OnApplicationQuit()
    {
        gameContext.SaveCurrentFloor();
        gameContext.Save();
    }
}
