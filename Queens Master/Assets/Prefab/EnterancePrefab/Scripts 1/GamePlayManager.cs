using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayManager : UIScreen
{
    public static GamePlayManager Instance;

    [Header("Level Configuration")]
    //public List<ImageData> imageDatas;
    public int WinScore = 100;
    public int PlayerScore = 0;
    public List<DragSelection> crownSelections = new List<DragSelection>();

    [Header("Life System")]
    public List<GameObject> lifeIcons;
    private int lifeIndex = 0;

    [Header("UI References")]
    [SerializeField] Button settingBtn;
    [SerializeField] Button quitBtn;
    [SerializeField] Button skinBtn;
    [SerializeField] Button discoBtn;

    [SerializeField] Button cleanBtn;
    [SerializeField] Button hintBtn;
    [SerializeField] Button rainbowBtn;

    public DragMode currentDragMode = DragMode.None;

    // Events
    public static System.Action OnPlayerWin;
    public static System.Action OnPlayerLoss;
    public static System.Action ResetGame;
    public static bool isGameWin = false;

    public GameTimer gameTimer;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        OnPlayerWin += HandleWinLogic;
        OnPlayerLoss += HandleLossLogic;
        ResetGame += ResetGameLogic;
        gameTimer.ResetTimer();
    }

    private void OnDisable()
    {
        OnPlayerWin -= HandleWinLogic;
        OnPlayerLoss -= HandleLossLogic;
        ResetGame -= ResetGameLogic;
    }

    private void Start()
    {
      
        settingBtn.onClick.AddListener(SettingButton);
        quitBtn.onClick.AddListener(OnQuitTheGame);
        skinBtn.onClick.AddListener(SkinButton);
        discoBtn.onClick.AddListener(DiscoButton);
        cleanBtn.onClick.AddListener(CleanButton);
        hintBtn.onClick.AddListener(HintBotton);
        rainbowBtn.onClick.AddListener(RainbowButton);

        ResetLifeVisuals();
    }
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            currentDragMode = DragMode.None;
        }
    }

    public override void OnOpen()
    {
        base.OnOpen();
        AdMobAddScript.Instance.LoadingTheBannerAdd();
        AdMobAddScript.Instance.LoadInterstitial();
    }

    public void LoosTheHeart()
    {
        if (lifeIndex < lifeIcons.Count)
        {
            lifeIcons[lifeIndex].SetActive(false);
            lifeIndex++;

            if (lifeIndex >= lifeIcons.Count)
            {
                OnPlayerLoss?.Invoke();
            }
        }
    }

    public void ResetLifeVisuals()
    {
        foreach (GameObject icon in lifeIcons)
        {
            if (icon != null) icon.SetActive(true);
        }
        lifeIndex = 0;
    }

    // --- Game Logic ---

    public bool IsGameOver() => PlayerScore == WinScore;

    public void CheckGameOver()
    {
        PlayerScore++;
        StartCoroutine(isGameOverCheckCounter());
    }

    public IEnumerator isGameOverCheckCounter()
    {
        if (IsGameOver())
        {
            isGameWin = true;
            QueensGridCreator.Instance.currentIndex++;
            yield return new WaitForSeconds(1);

            for (int i = 0; i < crownSelections.Count; i++)
            {
                if (crownSelections[i] != null)
                    StartCoroutine(crownSelections[i].PlayCrown3dAnimation());
            }

            yield return new WaitForSeconds(2);
            gameTimer.StopTimer();
            OnPlayerWin?.Invoke();
            ResetGame?.Invoke();
        }
    }

    public void ResetGameLogic()
    {
        PlayerScore = 0;
        isGameWin = false;
        ResetLifeVisuals();
    }

    // --- Button Actions ---

    private void OnQuitTheGame()
    {
        UIManager.ScreenNeedToShow?.Invoke(2);
        UIManager.ScreenNeedToShow?.Invoke(1);
        QueensGridCreator.LoadingGame?.Invoke();
        quitBtn.gameObject.SetActive(false);
        UIManager.CloseAllScene?.Invoke(4);
        UIManager.CloseAllScene?.Invoke(15);
    }

    private void SettingButton()
    {
        UIManager.ScreenNeedToShow?.Invoke(4);
        quitBtn.gameObject.SetActive(true);
    }

    private void SkinButton() => UIManager.ScreenNeedToShow?.Invoke(14);
    private void DiscoButton() => Debug.Log("Disco Clicked");
    private void CleanButton() => QueensGridCreator.Instance.UnselectAllWhiteCrosses();
    private void RainbowButton() => QueensGridCreator.Instance.RevealOneCrown();
    public void HintBotton() => QueensGridCreator.Instance.GiveSmartHint();

    // --- Event Handlers ---

    public void HandleLossLogic()
    {
        UIManager.ScreenNeedToShow?.Invoke(20);
        Debug.Log("Player Loses!");
    }

    public void HandleWinLogic()
    {
        UIManager.ScreenNeedToShow?.Invoke(19);
        WinnigPanal.UpdateRewardLoad?.Invoke(gameTimer.starGet * 2 , gameTimer.CalculateFinalStars());
    }
}

/*[System.Serializable]
public class ImageData
{
    public Sprite colorTile, crowTile;
}*/

public enum TileType { NotSeleted = 0, CrownSelected = 1 }
public enum DragMode { None, Drawing, Erasing }

public enum GameDifficulty
{
    Normal,
    Hard
}
