using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AchievementData
{
    public AchievementID achievementID;
    public bool unlocked;
    public AchievementData()
    {

    }
    public AchievementData(AchievementID achievementID, bool unlocked)
    {
        this.achievementID = achievementID;
        this.unlocked = unlocked;
    }
}