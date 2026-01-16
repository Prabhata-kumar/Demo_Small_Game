using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class FirebaseAutoTester : MonoBehaviour
{
    private DatabaseReference dbReference;
    private string[] names = { "Alpha", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot" };

    void Start()
    {
        // 1. Initialize Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Result == DependencyStatus.Available)
            {
                // IMPORTANT: Match the path in your image "Leader Board"
                dbReference = FirebaseDatabase.DefaultInstance.GetReference("Leader Board");
                Debug.Log("Firebase Ready. Starting Auto-Test...");

                StartCoroutine(AddRandomEntries(5));
            }
            else
            {
                Debug.LogError("Could not resolve dependencies: " + task.Result);
            }
        });
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(FetchLeaderBoardData());
        }
        else if(Input.GetKeyDown(KeyCode.C))
        {
            ClearFullLeaderboard();
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            DeleteSpecificUser("some-user-id");
        }
    }

    IEnumerator AddRandomEntries(int count)
    {
        for (int i = 0; i < count; i++)
        {
            string randomName = "User " + i;
            int randomScore = Random.Range(100, 1000);

            // Create a simple dictionary to hold the data
            Dictionary<string, object> entry = new Dictionary<string, object>();
            entry["User Name"] = randomName;
            entry["Scour"] = randomScore; // Matches the typo in your image

            // Push creates a unique ID so they don't overwrite each other
            dbReference.Push().UpdateChildrenAsync(entry).ContinueWithOnMainThread(task => {
                if (task.IsCompleted)
                {
                    Debug.Log("Added: " + randomName);
                }
            });

            yield return new WaitForSeconds(0.5f);
        }
    }

    public IEnumerator FetchLeaderBoardData()
    {
        // 1. Get the data from the specific "Leader Board" path
        var task = dbReference.GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError("Failed to fetch data: " + task.Exception);
        }
        else if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;

            // 2. Loop through every child (User) in the Leader Board
            foreach (DataSnapshot user in snapshot.Children)
            {
                // Using "Scour" to match your current database typo
                string name = user.Child("User Name").Value.ToString();
                string score = user.Child("Scour").Value.ToString();

                Debug.Log($"Fetched - Name: {name}, Score: {score}");
            }
        }
    }

    public void DeleteSpecificUser(string userId)
    {
        // Path: Leader Board -> User 0 (or unique ID)
        dbReference.Child(userId).RemoveValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                Debug.Log("User deleted successfully.");
            }
        });
    }

    public void ClearFullLeaderboard()
    {
        // This deletes everything under "Leader Board"
        dbReference.RemoveValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                Debug.Log("Leaderboard cleared!");
            }
        });
    }
}

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int score;
    public string PlayerID;

    public PlayerData(string name, int s, string id)
    {
        playerName = name;
        score = s;
        PlayerID = id;
    }
}