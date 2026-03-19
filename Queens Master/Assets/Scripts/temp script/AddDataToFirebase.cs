using UnityEngine;
using Firebase.Database;
using System;
using System.Collections.Generic;

public class AddDataToFirebase : MonoBehaviour
{
    private DatabaseReference rootReference;

    private void Start()
    {
        rootReference = FirebaseDatabase.DefaultInstance.RootReference;
        GenerateFakePlayers(200);
    }

    public void GenerateFakePlayers(int playerCount)
    {
        for (int i = 1; i <= playerCount; i++)
        {
            string id = Guid.NewGuid().ToString();

            // ----------------------------
            // PLAYER PROGRESS DATA
            // ----------------------------
            PlayerProgressData progress = new PlayerProgressData
            {
                playerId = id,
                currentLevel = UnityEngine.Random.Range(1, 100),
                coins = UnityEngine.Random.Range(0, 2000),
                hint = UnityEngine.Random.Range(0, 20),
                choose = UnityEngine.Random.Range(0, 20),
                weeklyChangeStreak = UnityEngine.Random.Range(0, 7),
                dailyChangeStreak = UnityEngine.Random.Range(0, 30),
                lastPlayTimeTicks = DateTime.UtcNow.Ticks,
                watermarkIndex = UnityEngine.Random.Range(0, 5),
                iconIndex = UnityEngine.Random.Range(0, 5),
               /* failedDates = new string,
                playedDates = new */
            };

            // ----------------------------
            // LEADERBOARD DATA
            // ----------------------------
            LeaderboardData leaderboard = new LeaderboardData
            {
                playerId = id,
                playerName = "Player_" + i,
                starCount = UnityEngine.Random.Range(0, 500),
                avatarIndex = UnityEngine.Random.Range(0, 5),
                bannerIndex = UnityEngine.Random.Range(0, 5)
            };

            UploadPlayer(progress, leaderboard);
        }

        Debug.Log(playerCount + " players generated and uploaded.");
    }

    private void UploadPlayer(PlayerProgressData progress,
                              LeaderboardData leaderboard)
    {
        // Upload to Players/{playerId}
        rootReference.Child("Players")
            .Child(progress.playerId)
            .SetRawJsonValueAsync(JsonUtility.ToJson(progress));

        // Upload to Leaderboard/{playerId}
        rootReference.Child("Leaderboard")
            .Child(leaderboard.playerId)
            .SetRawJsonValueAsync(JsonUtility.ToJson(leaderboard));
    }
}
