using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HomeScrenUI : UIScreen
{
    public static HomeScrenUI Instance = new HomeScrenUI();
    [Header("Buttons")]
    [SerializeField] Button editableLeaderboardBtn;                //0
    [SerializeField] Button tournamentBtn;                 //1
    [SerializeField] Button dailyRewardBtn;                //2
    [SerializeField] Button spinBtn;                       //3
    [SerializeField] Button stackBtn;
    [SerializeField] Button profileBtn;                    //4
    [SerializeField] Button playButton;

    [SerializeField] TextMeshProUGUI playerName;

    [SerializeField] List<GameObject> panalList;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    private void OnEnable()
    {
        GameManager.OnGameStart += UpdateNameUI;
    }
    private void OnDisable()
    {
        GameManager.OnGameStart -= UpdateNameUI;
    }

    private void Start()
    {
        //yield return new WaitForSeconds(1);
        if (panalList.Count > 0)
        {
            foreach (var panal in panalList)
            {
                panal.SetActive(false);
            }
        }
        // Assign listeners to buttons
        editableLeaderboardBtn.onClick.AddListener(() => EditLiderBoard());
        tournamentBtn.onClick.AddListener(() => StartCoroutine(TournamentBoard()));
        dailyRewardBtn.onClick.AddListener(() => DailyReworButton());
        spinBtn.onClick.AddListener(() => SpinButton());
        playButton.onClick.AddListener(OnPlayButton);
        stackBtn.onClick.AddListener(OnStackButton);
        //UpdateNameUI();
    }

    public void UpdateNameUI()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.progressData != null)
        {
            Debug.Log("Updating Coin UI: " + SaveManager.Instance.progressData.coins);
            playerName.text = SaveManager.Instance.leaderboardData.playerName;
            GameManager.Instance.ScriptableObjectHolder.HandleAssetRequest(ArtAssetType.Avater, SaveManager.Instance.leaderboardData.avatarIndex);
            GameManager.Instance.ScriptableObjectHolder.HandleAssetRequest(ArtAssetType.Banner, SaveManager.Instance.leaderboardData.bannerIndex);
            //SkinUI.Instance.SelectOnStart();
            //EditLeaderBoard.Instance.InitialCall();
        }
    }

    // Update UI when the screen opens
    public override void OnOpen()
    {
        base.OnOpen();
        //UpdateNameUI();
    }

    private void OnPlayButton()
    {
        //need to fix this function error 4004 
        
        UIManager.ScreenNeedToShow?.Invoke(15);
        QueensGridCreator.LoadingGame?.Invoke();
        UIManager.CloseAllScene?.Invoke(2);
        UIManager.CloseAllScene?.Invoke(1);
    }
    private void DailyReworButton()
    {
        Debug.Log("Daily Reward Button Clicked");
        UIManager.ScreenNeedToShow?.Invoke(6);
    }

    private void SpinButton()
    {
        UIManager.ScreenNeedToShow?.Invoke(7);
    }

    private void OnStackButton()
    {
        UIManager.ScreenNeedToShow?.Invoke(22);
    }
    public void EditLiderBoard() 
    {
        Debug.Log("editable button clicked");
        UIManager.ScreenNeedToShow?.Invoke(21);
    } 

    public IEnumerator TournamentBoard()
    {
        // need to fix erroer 404 

        Debug.Log("Tournament Board Open After 2 Second");
        yield return new WaitForSeconds(2f);
        UIManager.ScreenNeedToShow?.Invoke(15);
        QueensGridCreator.LoadingGame?.Invoke();
        UIManager.CloseAllScene?.Invoke(2);
        UIManager.CloseAllScene?.Invoke(1);
    }

    public void NameUpdate()
    {
        if(SaveManager.Instance == null && playerName == null) { return; }
        playerName.text = SaveManager.Instance.leaderboardData.playerName;
    }
}