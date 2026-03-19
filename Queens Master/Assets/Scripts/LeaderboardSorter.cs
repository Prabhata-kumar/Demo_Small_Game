using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Linq;

public class LeaderboardSorter : MonoBehaviour
{
    private DatabaseReference dbReference;

    void Start()
    {
        dbReference = FirebaseDatabase.GetInstance("https://fir-leader-board-default-rtdb.firebaseio.com/")
            .GetReference("Leader Board");

        GetTopPlayers();
    }

    [ContextMenu("Get Top 50 Players")]
    public void GetTopPlayers()
    {
        // 1. Ask Firebase for data sorted by startCount
        // 2. LimitToLast(50) gets the 50 highest values
        dbReference.OrderByChild("Leader Board").LimitToLast(50)
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {

                if (task.IsFaulted)
                {
                    Debug.LogError("Firebase Error: " + task.Exception);
                    return;
                }

              /*  if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    List<PlayerEntry> topPlayers = new List<PlayerEntry>();

                    // Firebase returns data from Smallest to Largest
                    // So we loop through and then reverse the list
                    foreach (DataSnapshot child in snapshot.Children)
                    {
                        string json = child.GetRawJsonValue();
                        PlayerEntry player = JsonUtility.FromJson<PlayerEntry>(json);
                        topPlayers.Add(player);
                    }

                    // Reverse so index 0 is the #1 player (highest stars)
                    topPlayers.Reverse();

                    DisplayLeaderboard(topPlayers);
                }*/
            });
    }

   /* void DisplayLeaderboard(List<PlayerEntry> players)
    {
        Debug.Log("--- TOP PLAYERS ---");
        for (int i = 0; i < players.Count; i++)
        {
            Debug.Log($"#{i + 1}: {players[i].userName} - Stars: {players[i].startCount}");
        }
    }*/
}