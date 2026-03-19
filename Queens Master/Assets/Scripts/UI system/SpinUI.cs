using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Firebase.Database;
using Firebase.Extensions;
using System;

public class SpinUI : UIScreen
{
    [Header("References")]
    public RectTransform spinner;
    [SerializeField] private Button spinButton;       // Assign in Inspector
    [SerializeField] private TextMeshProUGUI timerText; // Assign in Inspector
    [SerializeField] private Button closeButton;

    [Header("Reward Configuration")]
    public List<float> spinAngles;

    [Header("Settings")]
    [SerializeField] private float spinDuration = 3.5f;
    [SerializeField] private int extraFullCircles = 6;
    [SerializeField] private float cooldownMinutes = 15f; // Set to 15 for your requirement

    private bool isSpinning = false;
    private DatabaseReference dbReference;
    private string playerID = "Alpha_User"; // Use GameData.Instance.profileID if available

    private void Start()
    {
        closeButton.onClick.AddListener(CloseButton);
        spinButton.onClick.AddListener(AttemptSpin);

        // Initialize Firebase Reference correctly using RootReference
        dbReference = FirebaseDatabase.DefaultInstance.RootReference.Child("Users").Child(playerID);

        // Check if player is on cooldown immediately when opening the screen
        CheckCooldownStatus();
    }

    // --- STEP 1: CHECK COOLDOWN ---
    private void CheckCooldownStatus()
    {
        dbReference.Child("lastSpinTime").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || !task.Result.Exists)
            {
                SetButtonState(true, "Spin Now!");
                return;
            }

            long lastSpinTime = Convert.ToInt64(task.Result.Value);
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long cooldownMs = (long)(cooldownMinutes * 60 * 1000); // 15 * 60 * 1000 = 900,000ms
            long timePassed = currentTime - lastSpinTime;

            if (timePassed >= cooldownMs)
            {
                SetButtonState(true, "Spin Now!");
            }
            else
            {
                long remainingMs = cooldownMs - timePassed;
                StopAllCoroutines(); // Prevent double timers
                StartCoroutine(CooldownTimerRoutine(remainingMs));
            }
        });
    }

    // --- STEP 2: THE BUTTON CLICK ---
    public void AttemptSpin()
    {
        if (isSpinning) return;

        // Re-verify time with Firebase before allowing the spin
        dbReference.Child("lastSpinTime").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long cooldownMs = (long)(cooldownMinutes * 60 * 1000);

            if (task.Result.Exists)
            {
                long lastSpinTime = Convert.ToInt64(task.Result.Value);
                if (currentTime - lastSpinTime < cooldownMs)
                {
                    Debug.Log("<color=yellow>Still on Cooldown!</color>");
                    return; // EXIT: Do nothing if time hasn't passed
                }
            }

            // SUCCESS: Time has passed. Save NEW time and Spin!
            Dictionary<string, object> updates = new Dictionary<string, object>();
            updates["lastSpinTime"] = ServerValue.Timestamp;

            dbReference.UpdateChildrenAsync(updates).ContinueWithOnMainThread(saveTask =>
            {
                if (saveTask.IsCompleted)
                {
                    StartSpin();
                }
            });
        });
    }

    private void StartSpin()
    {
        if (spinAngles == null || spinAngles.Count == 0) return;

        int randomIndex = UnityEngine.Random.Range(0, spinAngles.Count);
        float targetAngle = spinAngles[randomIndex];

        StartCoroutine(SpinRoutine(targetAngle));
    }

    private IEnumerator SpinRoutine(float targetAngle)
    {
        isSpinning = true;
        SetButtonState(false, "Spinning...");

        float elapsed = 0;
        float startRotation = spinner.eulerAngles.z;
        float totalRotationToApply = (extraFullCircles * 360f) + (targetAngle - startRotation % 360f);
        float finalZ = startRotation + totalRotationToApply;

        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / spinDuration;
            float curve = 1f - Mathf.Pow(1f - t, 3f);
            float currentZ = Mathf.Lerp(startRotation, finalZ, curve);
            spinner.rotation = Quaternion.Euler(0, 0, currentZ);
            yield return null;
        }

        spinner.rotation = Quaternion.Euler(0, 0, targetAngle);
        isSpinning = false;

        // Refresh Cooldown to start the 15-min timer
        CheckCooldownStatus();
    }

    // --- HELPERS ---

    private void SetButtonState(bool interactable, string text)
    {
        spinButton.interactable = interactable;
        if (timerText != null) timerText.text = text;
    }

    private IEnumerator CooldownTimerRoutine(long remainingMs)
    {
        spinButton.interactable = false;
        float remainingSeconds = remainingMs / 1000f;

        while (remainingSeconds > 0)
        {
            TimeSpan t = TimeSpan.FromSeconds(remainingSeconds);
            // Formats as 15:00
            timerText.text = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);

            yield return new WaitForSeconds(1f);
            remainingSeconds -= 1f;
        }

        SetButtonState(true, "Spin Now!");
    }

    public void CloseButton()
    {
        UIManager.CloseAllScene?.Invoke(ScreenID);
    }
}