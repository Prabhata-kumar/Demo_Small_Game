using Firebase.Auth;
using Firebase;
using System;
using UnityEngine;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance { get; private set; }
    public static event Action<string> OnAuthReady; // Changed to string for easier Temp ID passing

    public bool useTempId = true;
    public string tempPlayerId = "11a69f40-2788-40cd-a493-527cbab9f509";
    public bool isDebugging = true;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    private void OnEnable()
    {
        FirebaseInitializer.OnFirebaseReady += HandleFirebaseInitialized;
    }

    private void OnDisable()
    {
        FirebaseInitializer.OnFirebaseReady -= HandleFirebaseInitialized;
    }

    private void HandleFirebaseInitialized()
    {
        if (useTempId)
        {
            if (isDebugging) Debug.Log($"<color=cyan>[Auth]</color> Using Temporary ID: {tempPlayerId}");
            // Directly tell SaveManager to use this ID
            OnAuthReady?.Invoke(tempPlayerId);
        }
        else
        {
            // Normal Firebase Sign-in
            SignInAnonymously();
        }
    }

    private async void SignInAnonymously()
    {
        try
        {
            var auth = FirebaseAuth.DefaultInstance;
            AuthResult result = await auth.SignInAnonymouslyAsync();
            if (isDebugging) Debug.Log($"<color=green>[Auth]</color> Signed in: {result.User.UserId}");
            OnAuthReady?.Invoke(result.User.UserId);
        }
        catch (Exception e)
        {
            Debug.LogError($"[Auth] Sign-In Failed: {e.Message}");
        }
    }
}