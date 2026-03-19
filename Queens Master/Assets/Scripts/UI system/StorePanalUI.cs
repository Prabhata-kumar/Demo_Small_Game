using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StorePanalUI : UIScreen
{
    [Header("Category: Special Offers")]
    [SerializeField] private Button limitedTimeBtn;
    [SerializeField] private TextMeshProUGUI limitedTimeBtnText; // Changed to UGUI
    [SerializeField] private Button themeBuyBtn;
    [SerializeField] private TextMeshProUGUI themeBuyBtnText; // Changed to UGUI

    [Header("Category: Remove Ads")]
    [SerializeField] private Button[] removeAdsButtons;
    [SerializeField] private TextMeshProUGUI[] removeAdsPriceTexts;

    [Header("Category: Coin Packs")]
    [SerializeField] private TextMeshProUGUI[] coinAmountTexts;
    [SerializeField] private Button[] coinPackButtons;
    [SerializeField] private TextMeshProUGUI[] coinPriceTexts;

    [Header("Visual Placeholders")]
    [SerializeField] private GameObject freeChaanl;
    [SerializeField] private GameObject LimitedTimeChaanl;
    [SerializeField] private GameObject TheamChaanl;

    private void Awake()
    {
        // 1. Assign Special Offers
        limitedTimeBtn.onClick.AddListener(() => OnPurchaseRequested("bundle_limited_time"));
        themeBuyBtn.onClick.AddListener(() => OnPurchaseRequested("theme_premium_dark"));

        // 2. Assign Remove Ads
        for (int i = 0; i < removeAdsButtons.Length; i++)
        {
            int index = i;
            removeAdsButtons[i].onClick.AddListener(() => OnPurchaseRequested($"remove_ads_tier_{index + 1}"));
        }

        // 3. Assign Coin Packs
        for (int i = 0; i < coinPackButtons.Length; i++)
        {
            int index = i;
            coinPackButtons[i].onClick.AddListener(() => OnPurchaseRequested($"coins_pack_{index + 1}"));
        }
    }

    private void Start()
    {
        StoreTextDisplay();
        RefreshStoreStatus();
    }

    public override void OnOpen()
    {
        base.OnOpen();
        RefreshStoreStatus();
    }

    private void OnPurchaseRequested(string productId)
    {
        Debug.Log($"<color=yellow>[Store]</color> Requesting purchase for: {productId}");

        // Mocking a successful purchase for testing visuals:
        if (productId.Contains("remove_ads"))
        {
            SaveManager.Instance.localData.hasRemovedAds = true;
            SaveManager.Instance.SaveGame(); // Save locally
            RefreshStoreStatus();
        }
    }

    private void RefreshStoreStatus()
    {
        // Accessing the SEPARATED LocalData
        bool adsRemoved = SaveManager.Instance.localData.hasRemovedAds;

        foreach (var btn in removeAdsButtons)
        {
            // If ads are removed, disable button and maybe change text
            btn.interactable = !adsRemoved;
            if (adsRemoved)
            {
                btn.GetComponentInChildren<TextMeshProUGUI>().text = "OWNED";
            }
        }
    }

    private void StoreTextDisplay()
    {
        // Special Offers
        limitedTimeBtnText.text = "$2.00";
        themeBuyBtnText.text = "$2.50";

        // Mocking Ads Prices
        string[] adPrices = { "$0.99", "$4.99", "$9.99" };
        for (int i = 0; i < removeAdsPriceTexts.Length; i++)
        {
            if (i < adPrices.Length) removeAdsPriceTexts[i].text = adPrices[i];
        }

        // Mocking Coin Pack Visuals
        string[] coinAmounts = { "500", "1200", "2500", "5000" };
        string[] coinPrices = { "$1.0", "$2.5", "$5.0", "$9.0" };

        for (int i = 0; i < coinPackButtons.Length; i++)
        {
            if (i < coinAmountTexts.Length) coinAmountTexts[i].text = coinAmounts[i] + " COINS";
            if (i < coinPriceTexts.Length) coinPriceTexts[i].text = coinPrices[i];
        }
    }
}