using UnityEngine;
using Firebase;
using Firebase.Extensions;
using System;

public class FirebaseInitializer : MonoBehaviour
{
    public static bool IsReady { get; private set; }
    public static event Action OnFirebaseReady; // Others will listen to this
    public bool isDebugging = true;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                IsReady = true;
                if (isDebugging) Debug.Log("--- Firebase: Dependencies Verified ---");

                // This triggers AuthManager and SaveManager to start
                OnFirebaseReady?.Invoke();
            }
            else
            {
                Debug.LogError($"Firebase Failed: {task.Result}");
            }
        });
    }
}