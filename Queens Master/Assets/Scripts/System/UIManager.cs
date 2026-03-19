using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Transactions;
using System;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Prefabs & References")]
    [SerializeField] private List<UIScreen> screenPrefabs;
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private GameObject dailyRewardPrefab;

    [SerializeField] private TMP_InputField playerScore;
    [SerializeField] private TMP_InputField playerName;
    [SerializeField] private TMP_InputField playerStart;
    [SerializeField] private Image profilePic;

    [SerializeField] Button home;         // Back / Main navigation
    [SerializeField] Button play;         // Primary action
    [SerializeField] Button setting;      // Global options
    [SerializeField] Button dailyReward;  // Time-based reward
    [SerializeField] Button shop;         // Monetization
    [SerializeField] Button tournament;   // Competitive mode
    [SerializeField] Button leaderBoard;  // Social proof
    [SerializeField] Button spine;        // Cosmetic / character / special feature


   /* // The Stack: Professional way to manage menu depth
    private Stack<UIScreen> uiStack = new Stack<UIScreen>();
    private List<UIScreen> spawnedScreens = new List<UIScreen>();*/

    public static Action LoadMainMenu;
    public static Action<int> CloseAllScene;
    public static Action<int> ScreenNeedToShow;

    private void OnEnable()
    {
        LoadMainMenu += LoadingMainMenu;
        ScreenNeedToShow += ShowScreen;
        CloseAllScene += CloseCurrent;
    }

    private void OnDisable()
    {
        LoadMainMenu -= LoadingMainMenu;
        ScreenNeedToShow -= ShowScreen;
        CloseAllScene -= CloseCurrent;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
            //ShowScreen(0);
            //ShowScreen(2);
            //LoadingMainMenu();
        }
        else { Destroy(gameObject); }
    }

    public void LoadingMainMenu()
    {
        Debug.Log("UI Manager: Loading Main Menu Screens.");
        ShowScreen(1);
        ShowScreen(2);
        //ShowScreen(15);
        //QueensGridCreator.LoadingGame?.Invoke();
    }

    // --- PUBLIC INTERFACE ---

     void ShowScreen(int index)
    {
        if (!InternetManager.Instance.IsOnline())
        {
            InterNetFilePanalWork();
            return;
        }

        for (int i = 0; i < screenPrefabs.Count; i++)
        {
            if(screenPrefabs[i].ScreenID == index)
            {
                UIScreen screen = screenPrefabs[i];
                screenPrefabs[i].OnOpen();
                screenPrefabs[i].IsPopup = true;
            }
        }
        Debug.Log($"UI Manager: Showing Screen ID {index}. 111");
    }

    public void CloseCurrent(int index)
    {
        for (int i = 0; i < screenPrefabs.Count; i++)
        {
            if (screenPrefabs[i].ScreenID == index)
            {
                UIScreen screen = screenPrefabs[i];
                screenPrefabs[i].OnClose();
                screenPrefabs[i].IsPopup = false;
            }
        }
    }

    // --- PRIORITY LOGIC ---

    public void TriggerDailyRewardSequence(bool isEligible)
    {
        if (isEligible)
        {
            // Pushing a priority popup directly to the top
            ShowScreen(3);
            Debug.Log("UI Manager: High Priority Daily Reward Forced.");
        }
    }

    // --- HUD UPDATES (Decoupled) ---

    public void UpdateHUD(int score, int moves)
    {
        // Internal logic to find the HUD screen and update text components
        // Only this manager knows WHICH text component displays the score
    }

    // --- TRANSITIONS ---

    public void PlayFade(System.Action onFadeComplete)
    {
        // Handle your black screen fade-in/out here
    }

    void InterNetFilePanalWork()
    {
        UIScreen screen = screenPrefabs[16];
        screenPrefabs[16].OnOpen();
        screenPrefabs[16].IsPopup = true;
    }
}

[System.Serializable]
public abstract class UIScreen : MonoBehaviour
{
    public int ScreenID;
    public bool IsPopup; // If true, doesn't hide the screen below it

    public virtual void OnOpen()
    {
        gameObject.SetActive(true);
    }

    public virtual void peek() => Debug.Log("Peeked " + ScreenID);

    public virtual void Push() => Debug.Log("Pushed " + ScreenID);
    public virtual void OnClose()
    {
        gameObject.SetActive(false);
    }
}