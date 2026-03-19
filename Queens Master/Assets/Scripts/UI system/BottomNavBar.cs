using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BottomNavBar : UIScreen
{
    [SerializeField] Button homeButton;
    [SerializeField] Button storButton;

    [SerializeField] Image homeButtonbackground;
    [SerializeField] Image storButtonbackground;

    private void Start()
    {
        homeButton.onClick.AddListener(() => HomeButton());
        storButton.onClick.AddListener(() => StoreButton());
    }

    private void HomeButton()
    {
        UIManager.CloseAllScene?.Invoke(3);
        UIManager.ScreenNeedToShow?.Invoke(2);
    }

    private void StoreButton()
    {
        UIManager.CloseAllScene?.Invoke(2);
        UIManager.ScreenNeedToShow?.Invoke(3);
    }
}
