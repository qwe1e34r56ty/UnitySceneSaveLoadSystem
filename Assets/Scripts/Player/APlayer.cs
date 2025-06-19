using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class APlayer : MonoBehaviour
{
    [SerializeField]
    public PlayerData playerData = new PlayerData();
    public PlayerStateInScene playerStateInScene;
    public PlayerStateInFloor playerStateInFloor;
    private GameContext gameContext;
    public bool isDontNeedSave = false;
    public bool isCreatedInFloorLoader = false;

    protected virtual void Start()
    {
        if (isDontNeedSave)
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
        }
        else
        {
            playerData = gameContext.saveData.playerData;
        }
        if (gameContext.IsSceneSaved(curSceneName))
        {
            this.playerStateInScene = gameContext.saveData.sceneDatas[curSceneName].playerStateInScene;

            // 받아온 상태 처리
            gameObject.transform.position = new Vector3(playerStateInScene.posX, playerStateInScene.posY, playerStateInScene.posZ);

            if (playerStateInScene.isPlayerExist == false)
            {
                Destroy(gameObject);
            }
        }
        else if (isCreatedInFloorLoader)
        {
            gameContext.playerStateInScene = playerStateInScene;
        }
        // 아니면 새로 생성하기
        else
        {
            playerStateInFloor = new();
            playerStateInScene = new();
            playerStateInScene.isPlayerExist = true;
            gameContext.playerStateInScene = playerStateInScene;
        }
        gameContext.playerStateInFloor = playerStateInFloor;
    }

    public virtual void Save()
    {
        if (playerStateInScene == null)
        {
            return;
        }
        if (isCreatedInFloorLoader)
        {
            playerStateInFloor.posX = gameObject.transform.position.x;
            playerStateInFloor.posY = gameObject.transform.position.y;
            playerStateInFloor.posZ = gameObject.transform.position.z;
        }
        else
        {
            playerStateInScene.posX = gameObject.transform.position.x;
            playerStateInScene.posY = gameObject.transform.position.y;
            playerStateInScene.posZ = gameObject.transform.position.z;
        }
    }

    protected virtual void OnDestroy()
    {
        if (gameContext.player == this)
        {
            Save();
        }
        if (gameContext != null && gameContext.player != null && gameContext.player == this)
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