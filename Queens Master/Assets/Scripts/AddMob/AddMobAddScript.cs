using UnityEngine;
using GoogleMobileAds.Api;
using TMPro;
using System;

public class AdMobAddScript : MonoBehaviour
{
  
    public static AdMobAddScript Instance;

#if UNITY_ANDROID
    string bannerID = "ca-app-pub-3940256099942544/6300978111";
    string interstitialID = "ca-app-pub-3940256099942544/1033173712";
    string rewardedID = "ca-app-pub-3940256099942544/5224354917";
    string nativeID = "ca-app-pub-3940256099942544/2247696110";
#elif UNITY_IOS
    string bannerID = "ca-app-pub-3940256099942544/2934735716";
    string interstitialID = "ca-app-pub-3940256099942544/4411468910";
    string rewardedID = "ca-app-pub-3940256099942544/1712485313";
    string nativeID = "ca-app-pub-3940256099942544/3986624511";
#else
    string bannerID = "unused";
    string interstitialID = "unused";
    string rewardedID = "unused";
    string nativeID = "unused";
#endif

    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        // Use Main Thread so UI updates (TextMeshPro) don't crash the app
       
        MobileAds.Initialize((InitializationStatus status) =>
        {
            Debug.Log("AdMob Initialized!");
            UpdateStatus("SDK Ready");

            // Optional: Pre-load ads so they are ready when needed
            LoadInterstitial();
            LoadRewardedAdd();
        });
    }

   /* private void Update()
    {
        // --- 1: BANNERS ---
        if (Input.GetKeyDown(KeyCode.Alpha1)) // Press '1'
        {
            LoadingTheBannerAdd();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) // Press '2'
        {
            DestroyingTheBannerAdd();
        }

        // --- 2: INTERSTITIALS (Full Screen) ---
        if (Input.GetKeyDown(KeyCode.Alpha3)) // Press '3'
        {
            // Smart Show: If not loaded, load it. If loaded, show it.
            if (interstitialAd != null && interstitialAd.CanShowAd())
            {
                ShowInterstitial();
            }
            else
            {
                UpdateStatus("Interstitial not ready. Requesting now...");
                LoadInterstitial();
            }
        }

        // --- 3: REWARDED (Video) ---
        if (Input.GetKeyDown(KeyCode.Alpha4)) // Press '4'
        {
            if (rewardedAd != null && rewardedAd.CanShowAd())
            {
                ShowReworedAdd();
            }
            else
            {
                UpdateStatus("Rewarded not ready. Requesting now...");
                LoadRewardedAdd();
            }
        }
    }*/

    private void UpdateStatus(string message)
    {
        
        Debug.Log(message);
    }

    #region BANNER METHODS
    public void LoadingTheBannerAdd()
    {
        if (bannerView != null) bannerView.Destroy();

        bannerView = new BannerView(bannerID, AdSize.Banner, AdPosition.Bottom);
        ListenToBannerEvents();

        AdRequest adRequest = new AdRequest();
        bannerView.LoadAd(adRequest);
        UpdateStatus("Loading Banner...");
    }

    void ListenToBannerEvents()
    {
        bannerView.OnBannerAdLoaded += () => UpdateStatus("Banner Loaded");
        bannerView.OnBannerAdLoadFailed += (LoadAdError error) => UpdateStatus("Banner Failed: " + error.GetMessage());
    }

    public void DestroyingTheBannerAdd()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
            UpdateStatus("Banner Destroyed");
        }
    }
    #endregion

    #region INTERSTITIAL METHODS
    public void LoadInterstitial()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        var adRequest = new AdRequest();
        InterstitialAd.Load(interstitialID, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                UpdateStatus("Interstitial Load Failed: " + error.GetMessage());
                return;
            }
            interstitialAd = ad;
            IntersticalEvent(interstitialAd);
            UpdateStatus("Interstitial Loaded");
        });
    }

    public void ShowInterstitial()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
        }
        else
        {
            UpdateStatus("Interstitial Not Ready");
            LoadInterstitial(); // Try to load again
        }
    }

    public void IntersticalEvent(InterstitialAd ad)
    {
        ad.OnAdFullScreenContentClosed += () => { LoadInterstitial(); };
        ad.OnAdFullScreenContentFailed += (AdError error) => { LoadInterstitial(); };
    }
    #endregion

    #region REWARDED METHODS
    public void LoadRewardedAdd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        var adRequest = new AdRequest();
        RewardedAd.Load(rewardedID, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                UpdateStatus("Rewarded Load Failed: " + error.GetMessage());
                return;
            }
            rewardedAd = ad;
            ReworedeAddEvent(rewardedAd);
            UpdateStatus("Rewarded Loaded");
        });
    }

    // Update this specific function in your AdMobAddScript
    public void ShowReworedAdd(Action onRewardSuccess)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log("Ad Finished! Granting Reward...");

                // This runs the code you passed from the WinningPanel
                onRewardSuccess?.Invoke();
            });
        }
        else
        {
            UpdateStatus("Rewarded Not Ready");
            LoadRewardedAdd();
        }
    }

    public void ReworedeAddEvent(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () => { LoadRewardedAdd(); };
        ad.OnAdFullScreenContentFailed += (AdError error) => { LoadRewardedAdd(); };
    }
    #endregion

    private void OnDestroy()
    {
        if (bannerView != null) bannerView.Destroy();
        if (interstitialAd != null) interstitialAd.Destroy();
        if (rewardedAd != null) rewardedAd.Destroy();
    }
}