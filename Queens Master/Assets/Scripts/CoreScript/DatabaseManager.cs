using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using System;

public class DatabaseManager : MonoBehaviour
{
    /*public static DatabaseManager Instance;
    private DatabaseReference dbReference;
    public bool isFirebaseReady { get; private set; }

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }

        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Result == DependencyStatus.Available)
            {
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;
                isFirebaseReady = true;
                Debug.Log("<color=cyan>[Cloud]</color> Firebase Ready.");
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
    }

    public void SaveToCloud(string userId, PlayerEntry entry)
    {
        if (!isFirebaseReady)
        {
            Debug.LogWarning("Firebase not ready yet. Skipping save.");
            return;
        }

        string json = JsonUtility.ToJson(entry);
        // Use "Leader Board" to match your Firebase screenshots exactly
        FirebaseDatabase.DefaultInstance.GetReference("Leader Board")
            .Child(userId)
            .SetRawJsonValueAsync(json);
    }

    public void GetCloudData(string userId, Action<string> onDataLoaded)
    {
        if (!isFirebaseReady)
        {
            onDataLoaded?.Invoke(null);
            return;
        }

        FirebaseDatabase.DefaultInstance.GetReference("LeaderBoard")
            .Child(userId).GetValueAsync().ContinueWithOnMainThread(task => {
                if (task.IsCompleted && task.Result.Exists)
                {
                    onDataLoaded?.Invoke(task.Result.GetRawJsonValue());
                }
                else
                {
                    onDataLoaded?.Invoke(null);
                }
            });
    }*/
}