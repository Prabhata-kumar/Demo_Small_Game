using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 1. Singleton Instance
    public static GameManager Instance { get; private set; }
    public ArtAssetHolder ScriptableObjectHolder;
    public GameState CurrentState { get; private set; }
    public LevelDatabase levelDatabase;

    // 3. Events (Actions) - Other scripts subscribe to these
    public static event Action<GameState> OnStateChanged;
    public static System.Action OnGameStart;
    private void Awake()
    {
        // Singleton Setup
        if (Instance == null)
        {
            Instance = this;
           // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        Application.targetFrameRate = 60;
    }

    private IEnumerator Start()
    {
        UpdateState(GameState.MainMenu);
        yield return new WaitForSeconds(1.5f); 
        OnGameStart?.Invoke();
    }

    // 4. State Machine Logic
    public void UpdateState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;

        // Logic for specific state entries
        switch (newState)
        {
            case GameState.MainMenu:
                Time.timeScale = 1f;
                break;
            case GameState.InGame:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
                Time.timeScale = 0f; // Freeze game logic
                break;
            case GameState.GameOver:
            case GameState.LevelComplete:
                Time.timeScale = 0f;
                break;
        }

        // Fire the event to notify UI, Sound, etc.
        OnStateChanged?.Invoke(newState);
    }

    // 5. Game Logic Helpers
    public void StartGame() => UpdateState(GameState.InGame);
    public void PauseGame() => UpdateState(GameState.Paused);
    public void ResumeGame() => UpdateState(GameState.InGame);
    public void GameOver() => UpdateState(GameState.GameOver);
    public void RestartLevel()
    {
        // Add logic to reload the scene here
        UpdateState(GameState.InGame);
    }
    private void OnApplicationQuit()
    {
        //
        SaveManager.Instance.SaveGame();
    }
}

// 2. State Definitions
public enum GameState
{
    MainMenu,
    InGame,
    Paused,
    GameOver,
    LevelComplete
}

public enum ArtAssetType
{
    Icone,
    Avater,
    Banner,
    WaterMark,
}

//000