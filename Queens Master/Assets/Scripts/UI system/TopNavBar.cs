using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TopNavBar : UIScreen
{
   [SerializeField] Button coinBtton;
   [SerializeField] Button topLeaderBoard;
   [SerializeField] Button settingPanal;

    public TextMeshProUGUI coinCount;
    public TextMeshProUGUI stats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1);  
        coinBtton.onClick.AddListener(() => StoreButton());
        topLeaderBoard.onClick.AddListener(() => TopLeaderBoard());
        settingPanal.onClick.AddListener(() => SettingButton());
        UpdateCoinUI();
    }

    public void UpdateCoinUI()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.progressData != null)
        {
            coinCount.text = SaveManager.Instance.progressData.coins.ToString();
            stats.text = SaveManager.Instance.leaderboardData.starCount.ToString();
        }
    }

    // Update UI when the screen opens
    public override void OnOpen()
    {
        base.OnOpen();
        UpdateCoinUI();
    }

    private void StoreButton()
    {
        UIManager.CloseAllScene?.Invoke(2);
        UIManager.ScreenNeedToShow?.Invoke(3);
    }
    private void TopLeaderBoard()
    {
        UIManager.ScreenNeedToShow?.Invoke(5);
    }
    private void SettingButton()
    {
        UIManager.ScreenNeedToShow?.Invoke(4);
    }

}
