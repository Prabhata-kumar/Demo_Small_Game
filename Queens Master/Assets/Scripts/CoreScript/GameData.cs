using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LocalData
{
    public static LocalData Instance { get; private set; }

    public bool musicEnabled = true;
    public bool sfxEnabled = true;
    public bool notificationsEnabled = true;
    public bool tuterialCompleted = false;
    public bool hasRemovedAds = false;
    public bool vibrationEnabled = true;
    public float musicVolume = 0.8f;
    public float sfxVolume = 0.8f;
    public string language = "en";
}

[Serializable]
public class PlayerProgressData
{
    public string playerId;
    public int currentLevel;
    public int coins;
    public int hint;
    public int choose;

    public int weeklyChangeStreak;
    public int dailyChangeStreak;
    public long lastPlayTimeTicks;

    public int watermarkIndex;
    public int iconIndex;

    public int[] failedDates = new int[] { };
    public int[] playedDates = new int[] { };
    public int[] purchangeIcon = new int[] { };
    public int[] purchangeWatermark = new int[] { };
}

[Serializable]
public class LeaderboardData
{
    public string playerId;
    public string playerName;
    public int starCount;     // ranking value
    public int avatarIndex;
    public int bannerIndex;
}

