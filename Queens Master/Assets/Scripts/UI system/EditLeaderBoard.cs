using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditLeaderBoard : UIScreen
{
    public static EditLeaderBoard Instance = new EditLeaderBoard();

    [SerializeField] Button closeBtn;
    [SerializeField] Button iconepanalBtn;
    [SerializeField] Button bannerMarkPanalBtn;

    [SerializeField]TMP_InputField playerNameInput;
    [SerializeField]TextMeshProUGUI playerNameDisplay; 

    [SerializeField] GameObject iconePanal;
    [SerializeField] GameObject bannnerMarkPanal;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static Action<ArtAssetType, int> OnRequestAsset;
    public static Action<ArtAssetType, Sprite> OnDeliverSprite;
    public static Action<ArtAssetType> offAllOthereThenThat;
   
    public static Action<ArtAssetType, int> autoButtonAct;

    private void Awake()
    {
        if(playerNameDisplay != null)
        {
            Debug.Log("Bug catch on start");
        }
    }
    private void OnEnable()
    {
        playerNameDisplay.text = SaveManager.Instance.leaderboardData.playerName;
        GameManager.OnGameStart += InitialCall;
    }

    private void OnDisable()
    {
        GameManager.OnGameStart -= InitialCall;
    }

    public override void OnOpen()
    {
        base.OnOpen();
        InitialCall();
    }

    void Start()
    {
        closeBtn.onClick.AddListener(CloseSettingsPanel);
        iconepanalBtn.onClick.AddListener(OpenIconePanal);
        bannerMarkPanalBtn.onClick.AddListener(OpenWaterMarkPanal);
        playerNameInput.onEndEdit.AddListener(OnFinishedTyping);
    }

  
    public void CloseSettingsPanel()
    {
        UIManager.CloseAllScene?.Invoke(ScreenID);
    }

    public void OpenIconePanal()
    {
        iconePanal.SetActive(true);
        bannnerMarkPanal.SetActive(false);
    }

    public void OpenWaterMarkPanal()
    {
        iconePanal.SetActive(false);
        bannnerMarkPanal.SetActive(true);
    }

    public void InitialCall()
    {
        if (SaveManager.Instance != null && GameManager.Instance.ScriptableObjectHolder != null)
        {
            playerNameDisplay.text = SaveManager.Instance.leaderboardData.playerName;
            GameManager.Instance.ScriptableObjectHolder.HandleAssetRequest(ArtAssetType.Avater, SaveManager.Instance.leaderboardData.avatarIndex);
            GameManager.Instance.ScriptableObjectHolder.HandleAssetRequest(ArtAssetType.Banner, SaveManager.Instance.leaderboardData.bannerIndex);
            /*            autoButtonAct?.Invoke(ArtAssetType.Avater, 5);
                        autoButtonAct?.Invoke(ArtAssetType.Banner, 5);*/
           
            autoButtonAct?.Invoke(ArtAssetType.Avater, SaveManager.Instance.leaderboardData.avatarIndex);
            autoButtonAct?.Invoke(ArtAssetType.Banner, SaveManager.Instance.leaderboardData.bannerIndex);
            Debug.Log("Requesting Assets...");
        }
    }

    private void OnFinishedTyping(string finalValue)
    {
        // 1. Clean up the text (remove accidental spaces)
        string cleanName = finalValue.Trim();

        if (!string.IsNullOrEmpty(cleanName))
        {
            // 2. Save to your CloudData
            SaveManager.Instance.leaderboardData.playerName = finalValue;
            SaveManager.Instance.leaderboardData.playerName = finalValue;
            SaveManager.Instance.SaveGame();

            Debug.Log($"Name saved to Cloud: {cleanName}");
            HomeScrenUI.Instance.NameUpdate();
        }
        else
        {
            Debug.LogWarning("User entered an empty name!");
        }
    }
}



