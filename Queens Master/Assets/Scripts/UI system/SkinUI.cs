using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class SkinUI : UIScreen
{
    public static SkinUI Instance = new SkinUI();

    [SerializeField] Button closeBtn;
    [SerializeField] Button iconepanalBtn;
    [SerializeField] Button waterMarkPanalBtn;

    [SerializeField] GameObject iconePanal;
    [SerializeField] GameObject waterMarkPanal;

    private void Awake()
    {
        SelectOnStart();
    }
    private void OnEnable()
    {
        GameManager.OnGameStart -= SelectOnStart;
    }

    private void OnDisable()
    {
        GameManager.OnGameStart -= SelectOnStart;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        closeBtn.onClick.AddListener(CloseSettingsPanel);
        iconepanalBtn.onClick.AddListener(OpenIconePanal);
        waterMarkPanalBtn.onClick.AddListener(OpenWaterMarkPanal);
    }

    public override void OnOpen()
    {
        base.OnOpen();
        GameTimer.OnTimerResum?.Invoke();
        SelectOnStart();
    }

    public override void OnClose()
    {
        base.OnClose();
        GameTimer.OnTimerPlay?.Invoke();
    }

    public void CloseSettingsPanel()
    {
        UIManager.CloseAllScene?.Invoke(ScreenID);
    }

    public void OpenIconePanal()
    {
        iconePanal.SetActive(true);
        waterMarkPanal.SetActive(false);
    }

    public void OpenWaterMarkPanal()
    {
        iconePanal.SetActive(false);
        waterMarkPanal.SetActive(true);
    }

    public void SelectOnStart()
    {
        Debug.Log("111");
        GameManager.Instance.ScriptableObjectHolder.HandleAssetRequest(ArtAssetType.Icone, SaveManager.Instance.progressData.iconIndex);
        GameManager.Instance.ScriptableObjectHolder.HandleAssetRequest(ArtAssetType.WaterMark, SaveManager.Instance.progressData.watermarkIndex);
        EditLeaderBoard.autoButtonAct?.Invoke(ArtAssetType.Icone, SaveManager.Instance.progressData.iconIndex);
        EditLeaderBoard.autoButtonAct?.Invoke(ArtAssetType.WaterMark, SaveManager.Instance.progressData.watermarkIndex);
    }
}

  