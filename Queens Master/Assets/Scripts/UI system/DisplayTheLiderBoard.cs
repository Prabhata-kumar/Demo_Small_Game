using Firebase;
using Firebase.Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayTheLeaderboard : UIScreen
{
    [Header("UI References")]
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private List<LeaderBoardIndexDetail> leaderboardSlots;

    private DatabaseReference leaderboardReference;

    private async void Awake()
    {
        await InitializeFirebase();
        Initialize();
    }

    private async Task InitializeFirebase()
    {
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();

        if (dependencyStatus != DependencyStatus.Available)
        {
            Debug.LogError("Firebase dependency error: " + dependencyStatus);
            return;
        }

        leaderboardReference =
            FirebaseDatabase.DefaultInstance.GetReference("Leaderboard");
    }

    private void Initialize()
    {
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        _ = FetchTopPlayers();
    }

    private async Task FetchTopPlayers()
    {
        try
        {
            DataSnapshot snapshot =
                await leaderboardReference
                    .OrderByChild("starCount")
                    .LimitToLast(leaderboardSlots.Count)
                    .GetValueAsync();

            if (!snapshot.Exists)
            {
                playerCountText.text = "Top Players: 0";
                return;
            }

            List<LeaderboardData> players = new();

            foreach (var child in snapshot.Children)
            {
                if (child.GetRawJsonValue() == null)
                    continue;

                LeaderboardData data =
                    JsonUtility.FromJson<LeaderboardData>(
                        child.GetRawJsonValue());

                if (data != null)
                    players.Add(data);
            }

            // Firebase returns ascending order
            players = players
                .OrderByDescending(p => p.starCount)
                .ToList();

            playerCountText.text = $"Top Players: {players.Count}";

            AssignToUI(players);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Leaderboard fetch failed:");
            Debug.LogError(e);
        }
    }

    private void AssignToUI(List<LeaderboardData> players)
    {
        for (int i = 0; i < leaderboardSlots.Count; i++)
        {
            if (i < players.Count)
            {
                leaderboardSlots[i].SetDetails(i + 1, players[i]);
                leaderboardSlots[i].gameObject.SetActive(true);
            }
            else
            {
                leaderboardSlots[i].gameObject.SetActive(false);
            }
        }
    }
}
