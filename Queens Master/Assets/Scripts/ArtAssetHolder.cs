using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ArtAssetHolder", menuName = "QueensMaster/ArtAssetHolder")]
public class ArtAssetHolder : ScriptableObject
{
    [Header("Sprites Lists")]
    public List<Sprite> iconeSpr;
    public List<Sprite> avaterSpr;
    public List<Sprite> bannerSpr;
    public List<Sprite> waterMarkSpr;
    public List<Sprite> rubbiesSpr;
    public List<Sprite> baseSqureSpr;
    public List <Sprite> d3SqureSpr;
    public Sprite DateStatueFailedSpr;
    public Sprite DateStatuePlayed;
    public Sprite DateStatueTodaySpr;
    public void HandleAssetRequest(ArtAssetType type, int index)
    {
        Sprite result = null;

        // Logic to pick the right list based on the Enum
        switch (type)
        {
            case ArtAssetType.Icone:
                if (index < iconeSpr.Count)
                {
                    result = iconeSpr[index];

                    SaveManager.Instance.progressData.iconIndex = index;
                }
                break;
            case ArtAssetType.Avater:
                if (index < avaterSpr.Count)
                {
                    result = avaterSpr[index];
                    SaveManager.Instance.leaderboardData.avatarIndex = index;
                }
                break;
            case ArtAssetType.Banner:
                if (index < bannerSpr.Count)
                {
                    result = bannerSpr[index];
                    SaveManager.Instance.leaderboardData.bannerIndex = index;
                }
                break;
            case ArtAssetType.WaterMark:
                if (index < waterMarkSpr.Count)
                {
                    result = waterMarkSpr[index];
                    SaveManager.Instance.progressData.watermarkIndex = index;
                }
                break;
        }

        if (result != null)
            EditLeaderBoard.OnDeliverSprite?.Invoke(type, result);
        else
            Debug.LogWarning($"Asset not found for Type: {type} at Index: {index}");
    }

    public Sprite AvterReturn(int indexAvt)
    {
        if (avaterSpr != null && indexAvt >= 0 && indexAvt < avaterSpr.Count)
        {
            return avaterSpr[indexAvt];
        }
        
        return avaterSpr[0];
    }

    public Sprite BannerReturn(int indexBnr)
    {
        // Fix: Check bannerSpr count, not avaterSpr
        if (bannerSpr != null && indexBnr >= 0 && indexBnr < bannerSpr.Count)
        {
            return bannerSpr[indexBnr];
        }
        
        return bannerSpr[0];
    }
}
