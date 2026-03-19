using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Firebase.Database;
using Firebase.Extensions;
using System.Threading.Tasks;

public class AnimatedLeaderBoard : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Transform leaderBoardParent;

    public LeaderBoardIndexDetail playerBoard;
    public List<LeaderBoardIndexDetail> leaderBoardItems = new List<LeaderBoardIndexDetail>();

    // Your specific Player ID for testing
    public string myPlayerId = "fdabe86a-eb28-44a0-9458-66c97a175368";
    private DatabaseReference dbRef;
/*
    private void Start()
    {
        // Path must match your Firebase screenshot exactly
        dbRef = FirebaseDatabase.DefaultInstance.GetReference("LeaderBoard");

        FetchExistingRows();
        FetchLeaderBoardData();
    }

    public void PlayerOneValueAssigne()
    {

    }

    private void FetchExistingRows()
    {
        leaderBoardItems.Clear();
        foreach (Transform child in leaderBoardParent)
        {
            if (child.TryGetComponent(out LeaderBoardIndexDetail detail))
            {
                leaderBoardItems.Add(detail);
            }
        }
    }

    public async void FetchLeaderBoardData()
    {
        Debug.Log("<color=cyan>[Leaderboard] Async Fetching...</color>");
        try
        {
            // Fetching top 30 based on startCount
            DataSnapshot snapshot = await dbRef.OrderByChild("startCount").LimitToLast(30).GetValueAsync();

            if (snapshot.Exists)
            {
                List<CloudData> allPlayers = new List<CloudData>();

                foreach (var child in snapshot.Children)
                {
                    // Map JSON to Class variables
                    CloudData data = JsonUtility.FromJson<CloudData>(child.GetRawJsonValue());
                    data.playerId = child.Key;
                    allPlayers.Add(data);
                }

                // Sort: Highest stars first. 
                // Ties are handled by Firebase's default key sorting
                allPlayers = allPlayers.OrderByDescending(p => p.start).ToList();

                ApplyDataToRows(allPlayers);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Fetch Error: {e.Message}");
        }
    }

    private void ApplyDataToRows(List<CloudData> players)
    {
        // Loop through the UI rows you have in the Hierarchy
        for (int i = 0; i < leaderBoardItems.Count; i++)
        {
            if (i < players.Count)
            {
                CloudData pData = players[i];

                // Set unique details for THIS specific index
                leaderBoardItems[i].SetDetails(
                    i + 1,
                    pData.selectedAvaterIndex,
                    pData.selectedBannerIndex,
                    pData.playerName,
                    pData.start
                );

                leaderBoardItems[i].playerId = pData.playerId;
                leaderBoardItems[i].starScore = pData.start; // Store score locally for animations
                leaderBoardItems[i].gameObject.SetActive(true);

                // Highlight YOUR row if it matches your ID
                if (pData.playerId == myPlayerId)
                {
                    leaderBoardItems[i].GetComponent<Image>().color = Color.yellow;
                }
            }
            else
            {
                leaderBoardItems[i].gameObject.SetActive(false);
            }
        }
    }

    // THE RANKING UPDATE FUNCTION
    public async void UpdatePlayerRanking(int addedStars)
    {
        // Find your row in the current list
        LeaderBoardIndexDetail playerRow = leaderBoardItems.Find(x => x.playerId == myPlayerId);

        if (playerRow != null)
        {
            int oldScore = playerRow.starScore;
            int newScore = oldScore + addedStars;

            // 1. Update Cloud
            await dbRef.Child(myPlayerId).Child("startCount").SetValueAsync(newScore);

            // 2. Animate Score & Swap
            DOTween.To(() => oldScore, x => {
                playerRow.starScore = x;
                playerRow.playerScore.text = x.ToString();
                CheckForRankUp(playerRow);
            }, newScore, 1.5f).SetEase(Ease.OutQuad);
        }
    }

    private void CheckForRankUp(LeaderBoardIndexDetail playerRow)
    {
        int currentIndex = leaderBoardItems.IndexOf(playerRow);

        if (currentIndex > 0)
        {
            LeaderBoardIndexDetail above = leaderBoardItems[currentIndex - 1];

            // Only swap if your score is STRICTLY HIGHER than the one above
            // This ensures if you have 25 and they have 25, you stay behind them
            if (playerRow.starScore > above.starScore)
            {
                // Logic Swap
                leaderBoardItems[currentIndex] = above;
                leaderBoardItems[currentIndex - 1] = playerRow;

                // Visual Swap in Hierarchy
                playerRow.transform.SetSiblingIndex(currentIndex - 1);

                // Update Rank Numbers
                playerRow.playerSlNo.text = (currentIndex).ToString();
                above.playerSlNo.text = (currentIndex + 1).ToString();

                playerRow.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
                FocusOnPlayer(playerRow);
            }
        }
    }

    private void FocusOnPlayer(LeaderBoardIndexDetail playerRow)
    {
        if (scrollRect == null) return;
        int index = leaderBoardItems.IndexOf(playerRow);
        float targetPos = 1f - ((float)index / (leaderBoardItems.Count - 1));
        scrollRect.DOVerticalNormalizedPos(targetPos, 0.3f);
    }*/
}