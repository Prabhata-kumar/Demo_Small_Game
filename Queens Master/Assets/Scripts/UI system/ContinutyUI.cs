using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using TMPro;

public class ContinutyUI : UIScreen
{
    [Header("Top Streak Line (W T F S S M T)")]
    public List<Image> topDayDots; // The dots in the horizontal line
    public Color activeDotColor = Color.blue;
    public Color inactiveDotColor = Color.gray;

    [Header("Milestone Grid (3 Days, 7 Days, etc)")]
    public List<MilestoneItem> milestones; // Assign objects with the MilestoneItem script
    public Sprite achievedMilestoneSprite; // The colorful Ruby
    public Sprite lockedMilestoneSprite;   // The gray shape
    Color32 achievedJuil = new Color32(255, 255, 255, 250); // Bright, solid green
    Color32 lockedjuil = new Color32(0, 0, 0, 75);   // Faded, semi-transparent green
    [Header("Stat Texts")]
    public TextMeshProUGUI streakCountText;
    public TextMeshProUGUI totalLevelsText;
    public TextMeshProUGUI winRateText;
    public TextMeshProUGUI maxStreakText;

    [Header("Animation Settings")]
    public float popDuration = 0.5f;
    public float staggerDelay = 0.1f;

    public Button closeBtn;

    private void Start()
    {
       
        closeBtn.onClick.AddListener(ClosePanel);
    }

    private void OnEnable()
    {
        GameManager.OnGameStart += UIsetUp;
    }

    private void OnDisable()
    {

        GameManager.OnGameStart -= UIsetUp;
    }

    public override void OnOpen()
    {
        base.OnOpen();
        UIsetUp();
    }

    public void UIsetUp()
    {
        SetupUI(SaveManager.Instance.progressData.choose, SaveManager.Instance.progressData.currentLevel, Random.Range(60, 91), SaveManager.Instance.progressData.choose);
    }

    public void SetupUI(int currentStreak, int totalLevels, float winRate, int maxStreak)
    {
        // 1. Update Stats
        streakCountText.text = currentStreak.ToString();
        totalLevelsText.text = totalLevels.ToString();
        winRateText.text = (winRate).ToString() + "%";
        maxStreakText.text = maxStreak.ToString();

        // 2. Animate Top Day Line (Dynamic)
        // This assumes the dots represent the last 7 days. 
        // Logic: if current streak >= dot index, light it up.
        for (int i = 0; i < topDayDots.Count; i++)
        {
            topDayDots[i].sprite = (i < currentStreak % 7) ? achievedMilestoneSprite : lockedMilestoneSprite;
        }

        // 3. Update & Animate Milestones
        StartCoroutine(AnimateMilestones(currentStreak));
    }

    private IEnumerator AnimateMilestones(int currentStreak)
    {
        foreach (var milestone in milestones)
        {
            bool isAchieved = currentStreak >= milestone.daysRequired;

            // Set visuals
            milestone.iconImage.color = isAchieved ? achievedJuil : lockedjuil;
           // Reset for animation
           milestone.transform.localScale = Vector3.zero;
        }

        // Staggered Pop Animation
        foreach (var milestone in milestones)
        {
            milestone.transform.DOScale(1f, popDuration).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(staggerDelay);
        }
    }

    public void ClosePanel()
    {
        transform.DOKill();
        UIManager.CloseAllScene?.Invoke(ScreenID);
    }
}

[System.Serializable]
public class MilestoneItem
{
    public Transform transform;
    public Image iconImage;
    public TextMeshProUGUI label;
    public int daysRequired; // e.g., 3, 7, 15, 30...
}