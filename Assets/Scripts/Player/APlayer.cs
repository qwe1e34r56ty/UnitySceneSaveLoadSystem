using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class APlayer : MonoBehaviour
{
    [SerializeField]
    protected PlayerData playerData = new PlayerData();
    public PlayerData PlayerData => playerData;
    protected PlayerStateInScene playerStateInScene;
    private GameContext gameContext;
    public bool dontNeedSave = true;

    protected virtual void Start()
    {
        if (dontNeedSave)
        {
            return;
        }
        gameContext = DataManager.Instance.gameContext;
        gameContext.player = this;
        string curSceneName = SceneManager.GetActiveScene().name;
        // 해당 Scene 내용이 저장되어 있으면 saveData에서 상태 받아오기
        if (gameContext.saveData.playerData == null)
        {
            gameContext.saveData.playerData = playerData;
            Debug.Log("Asddfsd");
        }
        else
        {
            playerData = gameContext.saveData.playerData;
        }
        if (gameContext.IsSceneSaved(curSceneName))
        {
            this.playerStateInScene = gameContext.saveData.sceneBundles[curSceneName].playerStateInScene;

            // 받아온 상태 처리
            gameObject.transform.position = new Vector3(playerStateInScene.posX, playerStateInScene.posY, playerStateInScene.posZ);
            /*
             * 
             * 
             * 
             * 
             */

            if (playerStateInScene.isPlayerExist == false)
            {
                Destroy(gameObject);
            }
        }
        // 아니면 새로 생성하기
        else
        {
            playerStateInScene = new();
            playerStateInScene.isPlayerExist = true;
            gameContext.playerStateInScene = playerStateInScene;
        }
    }

    // Update에서 호출하지 말고 Save 지점에서만 호출할 방법 고민?
    public virtual void Save()
    {
        if (playerStateInScene == null)
        {
            return;
        }
        playerStateInScene.posX = gameObject.transform.position.x;
        playerStateInScene.posY = gameObject.transform.position.y;
        playerStateInScene.posZ = gameObject.transform.position.z;
    }

    protected virtual void OnDestroy()
    {
        Save();
        if (gameContext != null)
        {
            gameContext.player = null;
        }
    }

    [ContextMenu("RemoveFromSceneBundle")]
    public void RemoveFromSceneBundle()
    {
        if (playerStateInScene == null)
        {
            return;
        }
        playerStateInScene.isPlayerExist = false;
    }
}