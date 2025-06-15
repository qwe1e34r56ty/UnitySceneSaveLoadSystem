using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoSingleton<SceneLoader>
{
    GameContext gameContext;
    private static SceneLoader instance;
    [SerializeField] private List<string> abortSceneNameList = new();
    private HashSet<string> abortSceneNames = new();

    protected override void Awake()
    {
        base.Awake();
    }

    protected void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        foreach (string name in abortSceneNameList)
        {
            abortSceneNames.Add(name);
        }
        gameContext = DataManager.Instance.gameContext;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (abortSceneNames.Contains(scene.name))
        {
            return;
        }

        if (gameContext.IsSceneSaved(scene.name))
        {
            if (!gameContext.saveData.sceneBundles.TryGetValue(scene.name, out var sceneBundle))
            {
                Logger.LogWarning($"SceneBundle not found for scene: {scene.name}");
                return;
            }

            foreach (var kvp in sceneBundle.npcDataQueues)
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
                            _npc.isCreatedBySceneLoader = true;
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
    }

}
