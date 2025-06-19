using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class ATile : MonoBehaviour
{
    [SerializeField]
    public TileData tileData = new TileData();
    private GameContext gameContext;
    public bool isCreatedBySceneLoader = false;
    public bool isDontNeedSave = false;

    protected virtual void Awake()
    {
        gameContext = DataManager.Instance.gameContext;
    }

    protected virtual void Start()
    {
        if (isDontNeedSave)
        {
            return;
        }
        string curSceneName = SceneManager.GetActiveScene().name;
        // 해당 Scene 내용이 저장되어 있으면 saveData에서 상태 받아오기(전달 주체는 SceneLoader 지만 신경 쓸 필요 없음)
        if (tileData == null)
        {
            tileData = new TileData();
        }
        if (gameContext.IsSceneSaved(curSceneName))
        {
            if (isCreatedBySceneLoader == false)
            {
                Destroy(gameObject);
                return;
            }
            // 받아온 상태 처리
            gameObject.transform.position = new Vector3(tileData.posX, tileData.posY, tileData.posZ);
        }
        // 동기화 위해서 gameContext에 등록하기
        gameContext.RegisterTile(this, tileData);
    }

    // Update에서 호출하지 말고 Save 지점에서만 호출할 방법 고민?
    public virtual void Save()
    {
        tileData.posX = gameObject.transform.position.x;
        tileData.posY = gameObject.transform.position.y;
        tileData.posZ = gameObject.transform.position.z;
    }

    protected virtual void OnDestroy()
    {
        Save();
    }

    [ContextMenu("RemoveFromSave")]
    public void RemoveFromSave()
    {
        if (gameContext == null)
        {
            return;
        }
        gameContext.UnregisterTile(this);
    }
}
