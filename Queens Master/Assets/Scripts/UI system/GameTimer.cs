using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Optimized structure without changing any existing function names
public class GameTimer : MonoBehaviour
{
    public static Action OnTimerResum, OnTimerPlay;

    [SerializeField] private TextMeshProUGUI timerText, startCountText;
    [SerializeField] private GameObject smallTimeObj, largeTimerObj;
    [SerializeField] private Button noThanksBtn;
    [SerializeField] private List<float> heardTimerList, normalTimerList;
    [SerializeField] private List<int> heardStarList, normalStartList;
    [SerializeField] private GameObject winParticalEffect;

    public int starGet;
    private float elapsedTime;
    private bool isRunning, timerVisible;
    public GameDifficulty currentDifficulty;

    private List<float> activeTimerList;
    private List<int> activeStarList;

    private void OnEnable()
    {
        OnTimerResum += PauseTimer;
        OnTimerPlay += ResumeTimer;
        if (noThanksBtn) noThanksBtn.onClick.AddListener(OnNoThanksClicked);
        if (winParticalEffect) winParticalEffect.SetActive(false);
        if (largeTimerObj) largeTimerObj.SetActive(false);
        if (smallTimeObj) smallTimeObj.SetActive(false);
    }

    private void OnDisable()
    {
        OnTimerResum -= PauseTimer;
        OnTimerPlay -= ResumeTimer;
        if (noThanksBtn) noThanksBtn.onClick.RemoveListener(OnNoThanksClicked);
    }

    private void Update()
    {
        if (!isRunning) return;
        elapsedTime += Time.deltaTime;
        UpdateTimerText();
        StartTimerRoutine();
        DispalySmallObject(currentDifficulty);
    }

    public void StartGameWithCountdown(GameDifficulty difficulty)
    {
        ResetTimer();
        currentDifficulty = difficulty;
        if (difficulty == GameDifficulty.Hard)
        {
            activeTimerList = heardTimerList;
            activeStarList = heardStarList;
        }
        else
        {
            activeTimerList = normalTimerList;
            activeStarList = normalStartList;
        }
        isRunning = true;
    }

    private void OnNoThanksClicked()
    {
        if (largeTimerObj) largeTimerObj.SetActive(false);
        timerVisible = true;
        if (smallTimeObj) smallTimeObj.SetActive(true);
    }

    private void StartTimerRoutine()
    {
        if (!isRunning || activeTimerList == null || activeStarList == null || activeTimerList.Count < 3 || activeStarList.Count < 3) return;
        if (elapsedTime <= activeTimerList[0]) starGet = activeStarList[0];
        else if (elapsedTime <= activeTimerList[1]) starGet = activeStarList[1];
        else starGet = activeStarList[2];
        if (startCountText) startCountText.text = starGet.ToString();
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        isRunning = false;
        timerVisible = false;
        starGet = 0;
        if (timerText) timerText.text = "00:00";
        if (startCountText) { startCountText.text = "0"; startCountText.color = Color.white; }
        if (smallTimeObj) smallTimeObj.SetActive(false);
        if (largeTimerObj) largeTimerObj.SetActive(false);
    }

    public void StopTimer()
    {
        isRunning = false;
        if (winParticalEffect) winParticalEffect.SetActive(true);
        if (startCountText) startCountText.color = Color.yellow;
    }

    public int CalculateFinalStars()
    {
        return starGet;
    }

    public void DispalySmallObject(GameDifficulty difficulty)
    {
        if (timerVisible || activeTimerList == null || activeTimerList.Count == 0) return;
        if (elapsedTime > activeTimerList[0])
        {
            if (largeTimerObj) largeTimerObj.SetActive(false);
            if (smallTimeObj) smallTimeObj.SetActive(true);
            timerVisible = true;
        }
        else
        {
            if (largeTimerObj) largeTimerObj.SetActive(true);
        }
    }

    private void UpdateTimerText()
    {
        if (!timerText) return;
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void PauseTimer() => isRunning = false;
    private void ResumeTimer() => isRunning = true;
}