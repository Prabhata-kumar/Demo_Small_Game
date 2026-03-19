using UnityEngine;
using System;

public class TimeCheckController : MonoBehaviour
{
    public PlayerProgressData data;

    // Tracking variables for the "Lock"
    public double lastKnownSystemTicks;
    public float lastKnownRealtime;
    public const float DriftToleranceSeconds = 10f; // Allow 10 seconds of lag

    void Start()
    {
        // Initialize the lock on startup
        lastKnownSystemTicks = DateTime.UtcNow.Ticks;
        lastKnownRealtime = Time.realtimeSinceStartup;

        Debug.Log("Time Lock Initialized.");
    }

    void Update()
    {
        // Manual Debug Check (Press "D")
        if (Input.GetKeyDown(KeyCode.D))
        {
            ValidateAndProcessDay();
        }

        // Timer Check (Press "T") - Checks if time was manipulated in the last 5 mins
        if (Input.GetKeyDown(KeyCode.T))
        {
            CheckForTimeManipulation();
        }
    }

    private void CheckForTimeManipulation()
    {
        // Calculate how much time the System Clock says has passed
        double systemTimePassed = (DateTime.UtcNow.Ticks - lastKnownSystemTicks) / TimeSpan.TicksPerSecond;

        // Calculate how much time the Hardware Timer says has passed
        float realTimePassed = Time.realtimeSinceStartup - lastKnownRealtime;

        // If System Time jumped ahead but Real Time didn't...
        if (Math.Abs(systemTimePassed - realTimePassed) > DriftToleranceSeconds)
        {
            Debug.LogError($"[CHEAT DETECTED] System jumped {systemTimePassed}s, but only {realTimePassed}s passed in-game!");
            // ACTION: Revert the system clock influence or block the reward.
        }
        else
        {
            Debug.Log($"[TIME SECURE] Drift is safe: {Math.Abs(systemTimePassed - realTimePassed):F2}s");
        }
    }

    private void ValidateAndProcessDay()
    {
        // 1. Run the manipulation check first
        CheckForTimeManipulation();

        DateTime today = DateTime.UtcNow.Date;
        DateTime lastPlayDate = new DateTime(data.lastPlayTimeTicks, DateTimeKind.Utc).Date;

        // 2. Logic for New Day
        if (today > lastPlayDate)
        {
            // NEW DAY LOGIC
            int daysMissed = (today - lastPlayDate).Days;

            if (daysMissed == 1)
            {
                data.dailyChangeStreak++;
                Debug.Log("Streak Continued!");
            }
            else
            {
                data.dailyChangeStreak = 1;
                Debug.Log("Streak Reset to 1 (Day missed).");
            }

            data.lastPlayTimeTicks = DateTime.UtcNow.Ticks;
            //SecureDataManager.SaveData(data); // Save the encrypted file
        }
        else
        {
            Debug.Log("Still the same day. No rewards yet.");
        }
    }
}