using Google.MiniJSON;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelUI : UIScreen
{


    [Header("UI Components")]
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle sfxToggle;
    [SerializeField] private Toggle vibrationToggle;
    [SerializeField] private Toggle notificationsToggle;

    [SerializeField] private ToggleSpriteReacter musicToggleSpr;
    [SerializeField] private ToggleSpriteReacter sfxToggleSpr;
    [SerializeField] private ToggleSpriteReacter vibrationToggleSpr;
    [SerializeField] private ToggleSpriteReacter notificationsToggleSpr;




    [SerializeField] private Button crossButton;


    private void Start()
    {
        if (crossButton != null)
            crossButton.onClick.AddListener(CloseSettingsPanel);

        InitializeUI();
    }

    private void OnEnable()
    {
        GameManager.OnGameStart += SettingInisiligingAssigningValues;
    }

    private void OnDisable()
    {
        GameManager.OnGameStart -= SettingInisiligingAssigningValues;
        RemoveAllToggleListeners();
    }

    public override void OnOpen()
    {
        base.OnOpen();
        InitializeUI();
        SettingInisiligingAssigningValues();
        GameTimer.OnTimerResum?.Invoke();

    }
    public override void OnClose()
    {
        base.OnClose();
        GameTimer.OnTimerPlay?.Invoke();
    }
    private void InitializeUI()
    {
        // 1. IMPORTANT: Remove listeners so the visual update doesn't trigger "SaveGame" repeatedly during setup
        //RemoveAllToggleListeners();

        if (SaveManager.Instance != null && SaveManager.Instance.localData != null)
        {
            var data = SaveManager.Instance.localData;

            // 2. Sync Toggle States to Data
            /*musicToggle.isOn = data.musicEnabled;
            sfxToggle.isOn = data.sfxEnabled;
            vibrationToggle.isOn = data.vibrationEnabled;
            notificationsToggle.isOn = data.notificationsEnabled;*/

            // 3. Force Apply sound states immediately

            SoundManager.Instance.musicSourceController(musicToggle.isOn);
            SoundManager.Instance.SetSfxEnabled(sfxToggle.isOn);
            //Debug.Log($"<color=cyan><b>[SettingsSync]</b></color> UI synced with SaveData. Music: {data.musicEnabled}");
        }

        // 4. Re-add listeners for player interaction
        AddAllToggleListeners();
    }

    private void SetMusic(bool isOn)
    {
        Debug.Log($"<color=orange><b>[SettingsLogic]</b></color> Music set to: {isOn}");
        SoundManager.Instance.musicSourceController(isOn);
        SaveManager.Instance.localData.musicEnabled = isOn;
        SaveManager.Instance.SaveGame();
    }

    private void SetSFX(bool isOn)
    {
        Debug.Log($"<color=orange><b>[SettingsLogic]</b></color> SFX set to: {isOn}");
        SoundManager.Instance.SetSfxEnabled(isOn);
        SaveManager.Instance.localData.sfxEnabled = isOn;
        SaveManager.Instance.SaveGame();
    }

    private void SetVibration(bool isOn)
    {
        Debug.Log($"<color=orange><b>[SettingsLogic]</b></color> Vibration set to: {isOn}");
        SaveManager.Instance.localData.vibrationEnabled = isOn;
        if (isOn) Handheld.Vibrate();
        SaveManager.Instance.SaveGame();
    }

    private void SetNotifications(bool isOn)
    {
        Debug.Log($"<color=orange><b>[SettingsLogic]</b></color> Notifications set to: {isOn}");
        SaveManager.Instance.localData.notificationsEnabled = isOn;
        SaveManager.Instance.SaveGame();
    }

    private void AddAllToggleListeners()
    {
        musicToggle.onValueChanged.AddListener(SetMusic);
        sfxToggle.onValueChanged.AddListener(SetSFX);
        vibrationToggle.onValueChanged.AddListener(SetVibration);
        notificationsToggle.onValueChanged.AddListener(SetNotifications);
    }

    private void RemoveAllToggleListeners()
    {
        musicToggle.onValueChanged.RemoveAllListeners();
        sfxToggle.onValueChanged.RemoveAllListeners();
        vibrationToggle.onValueChanged.RemoveAllListeners();
        notificationsToggle.onValueChanged.RemoveAllListeners();
    }

    
    public void CloseSettingsPanel() => UIManager.CloseAllScene?.Invoke(ScreenID);

    public void SettingInisiligingAssigningValues()
    {
        musicToggleSpr.UpdateVisuals(SaveManager.Instance.localData.musicEnabled);
        sfxToggleSpr.UpdateVisuals(SaveManager.Instance.localData.sfxEnabled);
        vibrationToggleSpr.UpdateVisuals(SaveManager.Instance.localData.vibrationEnabled);
        notificationsToggleSpr.UpdateVisuals(SaveManager.Instance.localData.notificationsEnabled);

        Debug.Log("UI UX update in setting");
    }
}