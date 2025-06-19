using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

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
        LoadNPC();
        LoadTile();
        LoadPlayer();
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

    #region private


    private void LoadTile()
    {
        PlayerStateInDungeon playerStateInDungeon = dungeonData.playerStateInDungeon;
        foreach (var kvp in dungeonData.floorDataList[playerStateInDungeon.curFloorIndex].npcDataQueues)
        {
            Queue<NPCData> queue = kvp.Value;
            while (queue.Count > 0)
            {
                NPCData npcData = queue.Dequeue();

                GameObject npcPrefab = Resources.Load<GameObject>(npcData.prefabPath);
                if (npcPrefab != null)
                {
                    GameObject npc = GameObject.Instantiate(npcPrefab);
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
    }

    private void LoadNPC()
    {
        PlayerStateInDungeon playerStateInDungeon = dungeonData.playerStateInDungeon;
        foreach (var kvp in dungeonData.floorDataList[playerStateInDungeon.curFloorIndex].tileDataQueues)
        {
            Queue<TileData> queue = kvp.Value;
            while (queue.Count > 0)
            {
                TileData tileData = queue.Dequeue();

                GameObject npcPrefab = Resources.Load<GameObject>(tileData.prefabPath);
                if (npcPrefab != null)
                {
                    GameObject tile = GameObject.Instantiate(npcPrefab);
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

    private void LoadPlayer()
    {
        PlayerStateInDungeon playerStateInDungeon = dungeonData.playerStateInDungeon;
        PlayerData playerData = gameContext.saveData.playerData;
        PlayerStateInFloor playerStateInFloor = dungeonData.floorDataList[playerStateInDungeon.curFloorIndex].playerStateInFloor;
        GameObject playerPrefab = Resources.Load<GameObject>(playerData.prefabPath);
        if (playerPrefab != null)
        {
            GameObject player = GameObject.Instantiate(playerPrefab);
            player.transform.position = new Vector3(playerStateInFloor.posX, playerStateInFloor.posY, playerStateInFloor.posZ);
            if (player.TryGetComponent<APlayer>(out APlayer _player))
            {
                _player.playerData = playerData;
                _player.playerStateInFloor = playerStateInFloor;
                _player.isCreatedInFloorLoader = true;
            }
        }
    }
    #endregion
}
