using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public string prefabPath;
}

[Serializable]
public class PlayerStateInScene
{
    public bool isPlayerExist;
    public float posX;
    public float posY;
    public float posZ;
}