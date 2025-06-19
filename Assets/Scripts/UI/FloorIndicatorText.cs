using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloorIndicatorText : MonoBehaviour
{
    // Start is called before the first frame update
    private TMP_Text floorIndicatorText = null;
    private DungeonData dungeonData = null;
    private void Start()
    {
        if(this.TryGetComponent<TMP_Text>(out TMP_Text floorIndicatorText))
        {
            this.floorIndicatorText = floorIndicatorText;
        }
        dungeonData = DataManager.Instance.gameContext.saveData.dungeonData;
    }

    // Update is called once per frame
    private void Update()
    {
        this.floorIndicatorText?.SetText($"Floor : {dungeonData.playerStateInDungeon.curFloorIndex}");
    }
}
