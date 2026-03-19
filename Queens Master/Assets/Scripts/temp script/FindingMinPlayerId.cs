using UnityEngine;
using Firebase.Database;
using System.Threading.Tasks;

public class FindingMinPlayer : MonoBehaviour
{
    private DatabaseReference leaderboardReference;

    private void Start()
    {
        leaderboardReference =
            FirebaseDatabase.DefaultInstance.GetReference("Leaderboard");
    }

    [ContextMenu("Find Min Star Player")]
    public async void FindSmallestStarCount()
    {
        Debug.Log("Searching for minimum starCount...");

        try
        {
            DataSnapshot snapshot =
                await leaderboardReference
                    .OrderByChild("starCount")
                    .LimitToFirst(1)
                    .GetValueAsync();

            if (!snapshot.Exists)
            {
                Debug.Log("No players found.");
                return;
            }

            foreach (DataSnapshot child in snapshot.Children)
            {
                LeaderboardData data =
                    JsonUtility.FromJson<LeaderboardData>(
                        child.GetRawJsonValue());

                Debug.Log(
                    $"Min Player Found\n" +
                    $"Player ID: {data.playerId}\n" +
                    $"Name: {data.playerName}\n" +
                    $"Star Count: {data.starCount}"
                );
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error finding minimum player: " + e.Message);
        }
    }
}
