using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class ANPC : MonoBehaviour
{
    [SerializeField]
    public NPCData npcData = new NPCData();
    private GameContext gameContext;
    public bool isCreatedBySceneLoader = false;
    public bool dontNeedSave = true;

    protected virtual void Awake()
    {
        gameContext = DataManager.Instance.gameContext;
    }

    protected virtual void Start()
    {
        if (dontNeedSave)
        {
            return;
        }
        string curSceneName = SceneManager.GetActiveScene().name;
        // 해당 Scene 내용이 저장되어 있으면 saveData에서 상태 받아오기(전달 주체는 SceneLoader 지만 신경 쓸 필요 없음)
        if (npcData == null)
        {
            npcData = new NPCData();
        }
        if (gameContext.IsSceneSaved(curSceneName))
        {
            if (isCreatedBySceneLoader == false)
            {
                Destroy(gameObject);
                return;
            }
            // 받아온 상태 처리
            gameObject.transform.position = new Vector3(npcData.posX, npcData.posY, npcData.posZ);

            /*
             * 
             *
             * 
             * 
             */
        }
        // 동기화 위해서 gameContext에 등록하기
        gameContext.RegisterNPC(this, npcData);
    }

    // Update에서 호출하지 말고 Save 지점에서만 호출할 방법 고민?
    public virtual void Save()
    {
        npcData.posX = gameObject.transform.position.x;
        npcData.posY = gameObject.transform.position.y;
        npcData.posZ = gameObject.transform.position.z;
    }

    protected virtual void OnDestroy()
    {
        Save();
    }

    [ContextMenu("RemoveFromSceneBundle")]
    public void RemoveFromSceneBundle()
    {
        if (gameContext == null)
        {
            return;
        }
        gameContext.UnregisterNPC(this);
    }
}
