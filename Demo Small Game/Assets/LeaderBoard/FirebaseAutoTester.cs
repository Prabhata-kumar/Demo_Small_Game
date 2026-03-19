using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Unity.VisualScripting;

public class FirebaseAutoTester : MonoBehaviour
{
    private DatabaseReference dbReference;
    // These will now be used as the "Parent Names" (the keys) in the database
    private string[] humanNames = { "Alpha", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot" };

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Result == DependencyStatus.Available)
            {
                dbReference = FirebaseDatabase.DefaultInstance.GetReference("Leader Board");
                Debug.Log("Firebase Ready.");

                // Start the test
                StartCoroutine(AddPlayersFromList());
            }
        });
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ClearFullLeaderboard();
        }
    }

    IEnumerator AddPlayersFromList()
    {
        foreach (string name in humanNames)
        {
            // 1. Create the data object
            PlayerEntry newPlayer = new PlayerEntry();
            //newPlayer.userName = name;
            newPlayer.score = Random.Range(100, 1000);
            newPlayer.diamondCount = Random.Range(0, 50);
            newPlayer.avatarIndex = Random.Range(1, 10);

            // 2. Convert to JSON
            string json = JsonUtility.ToJson(newPlayer);

            // 3. Set the data using the human name as the Parent Key
            // This replaces .Push() (random ID) with the actual name (Alpha, Bravo, etc.)
            dbReference.Child(name).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task => {
                if (task.IsCompleted) Debug.Log($"Successfully added: {name}");
            });

            yield return new WaitForSeconds(0.2f);
        }
    }

    public IEnumerator FetchLeaderBoardData()
    {
        var task = dbReference.GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsCompleted && !task.IsFaulted)
        {
            DataSnapshot snapshot = task.Result;
            foreach (DataSnapshot userSnap in snapshot.Children)
            {
                // Pulling data back out using the new field names
                string name = userSnap.Child("userName").Value.ToString();
                string diamonds = userSnap.Child("diamondCount").Value.ToString();
                Debug.Log($"Player: {name} | Diamonds: {diamonds}");
            }
        }
    }
    public void DeleteSpecificUser(string userId)
    {
        dbReference.Child(userId).RemoveValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                Debug.Log("User deleted successfully.");
            }
        });
    }
    public void ClearFullLeaderboard()
    {
        dbReference.RemoveValueAsync().ContinueWithOnMainThread(task => {

            if (task.IsCompleted)
            {
                Debug.Log("Leaderboard cleared!");
            }
        });
    }
}

// Use a class instead of a Dictionary for better organization
[System.Serializable]
public class PlayerEntry
{
    public string userName;
    public int score;
    public int diamondCount;
    public int avatarIndex;
}