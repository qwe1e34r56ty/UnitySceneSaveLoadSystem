using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AchievementUnlockUI : MonoSingleton<AchievementUnlockUI>
{
    private Queue<AchievementSO> unlocked = new();

    [SerializeField] private GameObject panel;
    [SerializeField] private Image achievementIcon;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private float minScale;
    [SerializeField] private float maxScale;
    [SerializeField] private float bufferTime;
    [SerializeField] private float scalingTime;
    [SerializeField] private float maintainTime;
    [SerializeField] private float waitTime;

    private WaitForSeconds waitBufferTimeSecond;
    private WaitForSeconds waitForMaintainTime;
    private WaitForSeconds waitForWaitTime;

    private bool isShowing = false;

    protected override void Awake()
    {
        base.Awake();
        waitBufferTimeSecond = new WaitForSeconds(bufferTime);
        waitForMaintainTime = new WaitForSeconds(maintainTime);
        waitForWaitTime = new WaitForSeconds(waitTime); ;
        panel?.SetActive(false);
        DontDestroyOnLoad(this);
    }

    public void Unlock(AchievementSO achievementSO)
    {
        unlocked.Enqueue(achievementSO);
        Logger.Log(achievementSO.title);
    }

    protected void Update()
    {
        if (!isShowing && unlocked.Count > 0)
        {
            AchievementSO next = unlocked.Dequeue();
            StartCoroutine(ShowAchievement(next));
        }
    }

    private IEnumerator ShowAchievement(AchievementSO achievement)
    {
        isShowing = true;

        title.text = achievement.title;
        description.text = achievement.description;
        achievementIcon.sprite = achievement.achievementIcon;

        panel.SetActive(true);
        panel.transform.localScale = Vector3.one * minScale;

        yield return waitBufferTimeSecond;
        float t = 0f;
        while (t < scalingTime)
        {
            t += Time.deltaTime;
            float scale = Mathf.Lerp(minScale, maxScale, t / scalingTime);
            panel.transform.localScale = Vector3.one * scale;
            yield return null;
        }

        panel.transform.localScale = Vector3.one * maxScale;

        yield return waitForMaintainTime;

        panel.SetActive(false);

        yield return waitForWaitTime;

        isShowing = false;
    }
}