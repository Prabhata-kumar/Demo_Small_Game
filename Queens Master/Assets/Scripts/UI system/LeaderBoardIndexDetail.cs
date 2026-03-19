using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardIndexDetail : MonoBehaviour
{
    [Header("UI References")]
    public Image playerIcon;
    public Image bannerImg;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerRankText;
    public TextMeshProUGUI playerScoreText;

    private string playerId;
    public int CurrentStars { get; private set; } // Added this so FindIndex can work

    public void SetDetails(int rank, LeaderboardData data)
    {
        playerId = data.playerId;

        playerRankText.text = rank.ToString();
        playerNameText.text = data.playerName;
        playerScoreText.text = data.starCount.ToString();
        CurrentStars = data.starCount;
        
        playerIcon.sprite = GameManager.Instance
            .ScriptableObjectHolder
            .AvterReturn(data.avatarIndex);

        bannerImg.sprite = GameManager.Instance
            .ScriptableObjectHolder
            .BannerReturn(data.bannerIndex);
    }

    public void UpdateOnlyScore(int newScore)
    {
        playerScoreText.text = newScore.ToString();
        CurrentStars = newScore;
        SaveManager.Instance.leaderboardData.starCount = CurrentStars;
        SaveManager.Instance.SaveGame();
    }

    public void UpdateRankDisplay(int newRank)
    {
        playerRankText.text = newRank.ToString();
    }
}
