using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;

public class CalenderPanel : UIScreen
{
    [Header("UI References")]
    [SerializeField] private GameObject dayPrefab;
    [SerializeField] private GameObject emptyPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private TextMeshProUGUI monthText;
    [SerializeField] private TextMeshProUGUI yearText;
    [SerializeField] private GameObject loadingOverlay;

    [Header("Pooling")]
    private Stack<GameObject> dayPool = new Stack<GameObject>();
    private Stack<GameObject> emptyPool = new Stack<GameObject>();
    private List<GameObject> activeItems = new List<GameObject>();

    private DatabaseReference dbRef;
    private DateTime currentViewingDate;
    private DateTime actualToday;
    private bool isInitialized = false;

    // Use your WaitAndLoad logic to start the process
    void Start()
    {
        if (loadingOverlay != null) loadingOverlay.SetActive(true);
        StartCoroutine(WaitAndLoad());
    }

    private IEnumerator WaitAndLoad()
    {
        // Wait for Firebase to be ready (from your SaveManager logic)
        // while (!FirebaseInitializer.IsReady) yield return null; 

        // Initialize Firebase
        var task = Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Result == Firebase.DependencyStatus.Available)
        {
            dbRef = FirebaseDatabase.DefaultInstance.RootReference;
            SyncTimeWithServer();
        }
    }

    public void SyncTimeWithServer()
    {
        // Get server timestamp to prevent local time cheating
        dbRef.Child("server_time_check").SetValueAsync(ServerValue.Timestamp).ContinueWithOnMainThread(task => {
            dbRef.Child("server_time_check").GetValueAsync().ContinueWithOnMainThread(readTask => {
                if (readTask.IsCompleted && !readTask.IsFaulted)
                {
                    long timestamp = (long)readTask.Result.Value;
                    actualToday = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
                    currentViewingDate = actualToday;
                    isInitialized = true;

                    if (loadingOverlay != null) loadingOverlay.SetActive(false);
                    RefreshCalendar();
                }
            });
        });
    }

    public void RefreshCalendar()
    {
        if (!isInitialized) return;

        ReturnAllToPool();

        DateTime firstOfMonth = new DateTime(currentViewingDate.Year, currentViewingDate.Month, 1);
        int daysInMonth = DateTime.DaysInMonth(currentViewingDate.Year, currentViewingDate.Month);

        int dayOffset = (int)firstOfMonth.DayOfWeek;
        Debug.Log($"Refreshing Calendar: {currentViewingDate:MMMM yyyy} - Days: {daysInMonth}, Offset: {dayOffset}");

        // 1. Fill Empty Slots for the start of the month
        for (int i = 0; i < dayOffset; i++)
        {
            GetItemFromPool(emptyPool, emptyPrefab);
        }

        // 2. Fill Actual Days
        for (int i = 1; i <= daysInMonth; i++)
        {
            GameObject dayObj = GetItemFromPool(dayPool, dayPrefab);
            CalendarDayItem dayItem = dayObj.GetComponent<CalendarDayItem>();

            DateTime dateBeingChecked = new DateTime(currentViewingDate.Year, currentViewingDate.Month, i);

            // Link to your PlayerProgressData Lists
            bool isToday = (dateBeingChecked.Date == actualToday.Date);
            bool hasPlayed = CheckIfPlayerPlayed(dateBeingChecked);
            bool hasFailed = CheckIfPlayerFailed(dateBeingChecked, hasPlayed);

            dayItem.Setup(i, isToday, hasPlayed, hasFailed);
        }

        monthText.text = currentViewingDate.ToString("MMMM").ToUpper();
        yearText.text = currentViewingDate.Year.ToString();
    }

    // --- DATA CHECKERS ---

    private bool CheckIfPlayerPlayed(DateTime date)
    {
        string dateKey = date.ToString("yyyy-MM-dd");
        // Accessing the list from your PlayerProgressData
        return true;// SaveManager.Instance.progressData.playedDates.Contains(dateKey);
    }

    private bool CheckIfPlayerFailed(DateTime date, bool played)
    {
        if (played) return false;

        string dateKey = date.ToString("yyyy-MM-dd");
        // It's a failure if it's in the failed list OR if the date is in the past and they didn't play
        /*if ( SaveManager.Instance.progressData.failedDates.Contains(dateKey)) return true;
*/
        return date.Date < actualToday.Date;
    }

    // --- POOLING SYSTEM ---

    private GameObject GetItemFromPool(Stack<GameObject> pool, GameObject prefab)
    {
        GameObject obj = (pool.Count > 0) ? pool.Pop() : Instantiate(prefab, container);
        obj.SetActive(true);
        obj.transform.SetAsLastSibling();
        activeItems.Add(obj);
        return obj;
    }

    private void ReturnAllToPool()
    {
        foreach (var item in activeItems)
        {
            item.SetActive(false);
            if (item.GetComponent<CalendarDayItem>() != null)
                dayPool.Push(item);
            else
                emptyPool.Push(item);
        }
        activeItems.Clear();
    }

    // --- NAVIGATION ---

    public void ShowNextMonth() { currentViewingDate = currentViewingDate.AddMonths(1); RefreshCalendar(); }
    public void ShowPreviousMonth() { currentViewingDate = currentViewingDate.AddMonths(-1); RefreshCalendar(); }
}

  public enum DayStatus
    {
        Future,
        Played,
        Failed,
        Today
    }