﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CurSceneNameText : MonoBehaviour
{
    // Start is called before the first frame update
    private TMP_Text curSceneNameText = null;
    private void Start()
    {
        if (this.TryGetComponent<TMP_Text>(out TMP_Text curSceneNameText))
        {
            this.curSceneNameText = curSceneNameText;
            this.curSceneNameText?.SetText($"{SceneManager.GetActiveScene().name}");
        }
        else
        {
            Logger.Log($"[CurSceneNameText] in {gameObject.name} object : TMP_Text component not found");
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}
