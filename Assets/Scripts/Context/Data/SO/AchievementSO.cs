using System.IO.Enumeration;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementSO", menuName = "Custom/Achievement")]
public class AchievementSO : ScriptableObject
{
    public AchievementID achievementID;
    public Sprite achievementIcon;
    public string title;
    public string description;
}

public enum AchievementID
{
    FirstBlood,
    PentaKill,
    TenThousandEarned,
    Total
}