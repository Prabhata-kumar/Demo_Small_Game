using UnityEngine;
using UnityEngine.UI;

public class AfterGamePanal : UIScreen
{
    [SerializeField] Button playButton;
    [SerializeField] Button homeButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playButton.onClick.AddListener(OnPlayButton);   
        homeButton.onClick.AddListener(OnHome);
    }

    private void OnPlayButton()
    {
        UIManager.ScreenNeedToShow?.Invoke(15);
        QueensGridCreator.LoadingGame?.Invoke();
        UIManager.CloseAllScene?.Invoke(17);
    }

    private void OnHome()
    {
        UIManager.ScreenNeedToShow?.Invoke(2);
        UIManager.ScreenNeedToShow?.Invoke(1);
        QueensGridCreator.LoadingGame?.Invoke();
        UIManager.CloseAllScene?.Invoke(2);
    }

    
}
