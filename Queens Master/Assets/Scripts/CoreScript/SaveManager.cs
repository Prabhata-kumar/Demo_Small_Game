using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("Local Data")]
    public LocalData localData = new LocalData();
   
    [Header("Cloud Data")]
    public PlayerProgressData progressData = new PlayerProgressData();
    //public PlayerProfileData profileData = new PlayerProfileData();
    public LeaderboardData leaderboardData = new LeaderboardData();

    private string localPath;
    private DatabaseReference rootReference;

    private readonly string encKey = "p3S6v9y$B&E)H@Mc";
    private readonly string encIV = "8x/A?D(G+KbPeShV";

    public bool isDebuging = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        localPath = Path.Combine(Application.persistentDataPath, "settings.dat");
        rootReference = FirebaseDatabase.DefaultInstance.RootReference;

        LoadLocal();
        
        StartCoroutine(WaitAndLoad());
    }

    // ============================================================
    // LOCAL SAVE
    // ============================================================

    public void SaveLocal()
    {
        try
        {
            string json = JsonUtility.ToJson(localData, true);
            string encrypted = Encrypt(json);
            File.WriteAllText(localPath, encrypted);

            if (isDebuging)
            {
                Debug.Log("Local data saved.");
            }
        }
        catch (Exception e)
        {
            if (isDebuging)
            {
                Debug.LogError("Local save failed: " + e.Message);
            }
        }
    }


    public void SaveGame()
    {
        SaveToCloud();
        SaveLocal();
    }

    public void LoardGame()
    {

    }

    public void LoadLocal()
    {
        if (!File.Exists(localPath))
            return;

        try
        {
            string encrypted = File.ReadAllText(localPath);
            string json = Decrypt(encrypted);
            JsonUtility.FromJsonOverwrite(json, localData);

            if (isDebuging)
            {
                Debug.Log("Local data loaded.");
            }
        }
        catch (Exception e)
        {
            if (isDebuging) 
            { 
                Debug.LogError("Local load failed: " + e.Message);
            }
        }
    }

    // ============================================================
    // CLOUD SAVE
    // ============================================================

    public void SaveToCloud()
    {
        if (string.IsNullOrEmpty(progressData.playerId))
            return;

        progressData.lastPlayTimeTicks = DateTime.UtcNow.Ticks;

        UploadProgress();
        Debug.Log("upload in cloud");
        UploadLeaderboard();
    }

    private void UploadProgress()
    {
        string json = JsonUtility.ToJson(progressData, true);
        if (isDebuging)
        {
            Debug.Log(json);
        }
      
        rootReference.Child("Players")
            .Child(progressData.playerId)
            .SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
             /*   if (task.IsFaulted)
                    Debug.LogError("[CLOUD] ❌ Progress Upload Failed");
                else
                    Debug.Log("[CLOUD] ✅ Progress Upload Success");*/
            });
    }


    /*    private void UploadProfile()
        {
            rootReference.Child("Profiles")
                         .Child(profileData.playerId)
                         .SetRawJsonValueAsync(JsonUtility.ToJson(profileData));
        }*/

    private void UploadLeaderboard()
    {
        string json = JsonUtility.ToJson(leaderboardData, true);
        if (isDebuging)
        {
            Debug.Log(json);
        }

        rootReference.Child("Leaderboard")
            .Child(leaderboardData.playerId)
            .SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                /*if (task.IsFaulted)
                    Debug.LogError("[CLOUD] ❌ Leaderboard Upload Failed");
                else
                    Debug.Log("[CLOUD] ✅ Leaderboard Upload Success");*/
            });
    }


    private IEnumerator WaitAndLoad()
    {
        while (!FirebaseInitializer.IsReady)
            yield return null;

        LoadFromCloud();
    }


    public void LoadFromCloud()
    {
        if (string.IsNullOrEmpty(progressData.playerId))
        {
            Debug.LogError("[CLOUD] ❌ PlayerID EMPTY");
            return;
        }

        string id = progressData.playerId;

        // =========================
        // LOAD PROGRESS
        // =========================
        rootReference.Child("Players")
            .Child(id)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted) 
                {
                    if (isDebuging)
                    {
                        Debug.LogError("[CLOUD] ❌ Progress Load Failed"); return; 
                    }
                }
                if (!task.Result.Exists) 
                { 
                    if(isDebuging)
                    {
                        Debug.LogWarning("[CLOUD] ⚠ No Progress Found"); return;
                    }
                }

                string rawJson = task.Result.GetRawJsonValue();
                if (isDebuging)
                {
                    Debug.Log(rawJson);
                }
               
                JsonUtility.FromJsonOverwrite(rawJson, progressData);
                if (isDebuging)
                { 
                    Debug.Log($"[CLOUD] ✅ Progress Parsed | Coins:{progressData.coins}");
                }
            });

        // =========================
        // LOAD LEADERBOARD
        // =========================
        rootReference.Child("Leaderboard")
            .Child(id)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted && isDebuging) { Debug.LogError("[CLOUD] ❌ Leaderboard Load Failed"); return; }
                if (!task.Result.Exists && isDebuging) { Debug.LogWarning("[CLOUD] ⚠ No Leaderboard Found"); return; }

                string rawJson = task.Result.GetRawJsonValue();

                if (isDebuging)
                {
                    Debug.Log(rawJson);
                }
                JsonUtility.FromJsonOverwrite(rawJson, leaderboardData);

                if (isDebuging) { 
                    Debug.Log($"[CLOUD] ✅ Leaderboard Parsed | Stars:{leaderboardData.starCount}");
                }
            });
    }



    // ============================================================
    // ENCRYPTION
    // ============================================================

    private string Encrypt(string plainText)
    {
        byte[] key = Encoding.UTF8.GetBytes(encKey);
        byte[] iv = Encoding.UTF8.GetBytes(encIV);

        using Aes aes = Aes.Create();
        using ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);

        byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

        return Convert.ToBase64String(encryptedBytes);
    }

    private string Decrypt(string encryptedText)
    {
        byte[] key = Encoding.UTF8.GetBytes(encKey);
        byte[] iv = Encoding.UTF8.GetBytes(encIV);

        using Aes aes = Aes.Create();
        using ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);

        byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
        byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

        return Encoding.UTF8.GetString(decryptedBytes);
    }
}
