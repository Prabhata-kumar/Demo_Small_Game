using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinnigPanal : UIScreen
{
    [Header("Buttons")]
    [SerializeField] Button playBtn;
    [SerializeField] Button homeBtn;
    [SerializeField] Button addBtn;

    [Header("Reward Texts")]
    [SerializeField] TextMeshProUGUI wincoinText;
    [SerializeField] TextMeshProUGUI winstarText;
    [SerializeField] TextMeshProUGUI claimText;

    public LeaderBoardJump leaderBoard;

    private int baseCoins;
    private int baseStars;
    private bool isDoubled = false;
    private bool rewardClaimed = false;

    private int multipleOfReward;

    public static System.Action<int, int> UpdateRewardLoad;

    private void OnEnable()
    {
        UpdateRewardLoad += UpdateReward;
        isDoubled = false;
        rewardClaimed = false;
        addBtn.interactable = true;

        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
        UpdateRewordBtn();
    }

    private void OnDisable()
    {
        UpdateRewardLoad -= UpdateReward;
        transform.DOKill();
    }

    void Start()
    {
        playBtn.onClick.AddListener(OnPlayButton);
        homeBtn.onClick.AddListener(OnHomeButton);
        addBtn.onClick.AddListener(OnClaimAdd);
    }

    public override void OnOpen()
    {
        base.OnOpen();

        leaderBoard.CallBackFunction(baseStars);
        SaveManager.Instance.progressData.currentLevel++;


    }
    public void UpdateRewordBtn()
    {
        // 2 and 3 appear often (3 times each)
        // 4 and 5 are rare (1 time each)
        int[] weights = { 2, 2, 2, 3, 3, 3, 4, 5 };

        int randomIndex = UnityEngine.Random.Range(0, weights.Length);
        multipleOfReward = weights[randomIndex];

        claimText.text = multipleOfReward + "X";
    }

    void UpdateReward(int coins, int stars)
    {
        baseCoins = coins;
        baseStars = SaveManager.Instance.leaderboardData.starCount + stars;

        AnimateText(wincoinText, baseCoins);
        //AnimateText(winstarText, baseStars); there is should rubi

        //leaderBoard.UpdateScoreAndRank(stars);
        //leaderBoard.StartCoroutine(leaderBoard.UpdateLeaderBoardRoutine(baseStars));
        leaderBoard.CallBackFunction(baseStars);
    }

    private void OnClaimAdd()
    {
        if (isDoubled) return;

        // We tell the AdMob script: "Show the ad, and IF it finishes, run ApplyMultiplier"
        AdMobAddScript.Instance.ShowReworedAdd(ApplyMultiplier);
    }

    // Separate the logic so it only runs when the ad is actually watched
    private void ApplyMultiplier()
    {
        isDoubled = true;
        addBtn.interactable = false;

        baseCoins *= multipleOfReward;
        baseStars *= multipleOfReward;

        // Use your existing DOTween animation
        AnimateText(wincoinText, baseCoins);
        AnimateText(winstarText, baseStars);

        Debug.Log("Multiplier applied successfully!");
    }

    public void AddingInCloud()
    {
        if (rewardClaimed) return;
        rewardClaimed = true;
        SaveManager.Instance.progressData.coins += baseCoins;
        SaveManager.Instance.SaveGame();
    }

    private void AnimateText(TextMeshProUGUI textElement, int targetValue)
    {
        int startValue = 0;
        int.TryParse(textElement.text, out startValue);

        DOTween.To(() => startValue,
          x =>
          {
              startValue = x;
              textElement.text = x.ToString();
          },
          targetValue,
          1.5f).SetEase(Ease.OutCubic);
    }



    private void OnPlayButton()
    {
        AddingInCloud();
        UIManager.ScreenNeedToShow?.Invoke(15);
        QueensGridCreator.LoadingGame?.Invoke();
        UIManager.CloseAllScene?.Invoke(ScreenID);
        GamePlayManager.ResetGame?.Invoke();
    }

    private void OnHomeButton()
    {
        AddingInCloud();
        AdMobAddScript.Instance.DestroyingTheBannerAdd();

        UIManager.ScreenNeedToShow?.Invoke(2);
        UIManager.ScreenNeedToShow?.Invoke(1);
        QueensGridCreator.LoadingGame?.Invoke();
        UIManager.CloseAllScene?.Invoke(ScreenID);
        UIManager.CloseAllScene?.Invoke(15);
    }
}