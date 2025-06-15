using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSaveSceneButton : MonoBehaviour
{
    [SerializeField]
    private string defaultNextScene;
    private void Start()
    {
        GameContext gameContext = DataManager.Instance.gameContext;
        if (gameObject.TryGetComponent<Button>(out Button button))
        {
            button.onClick.AddListener(() => SceneManager.LoadScene(gameContext.saveData.curSceneName ?? defaultNextScene));
        }
    }
}
