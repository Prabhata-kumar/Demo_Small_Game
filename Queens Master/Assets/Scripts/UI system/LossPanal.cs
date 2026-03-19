using UnityEngine;
using UnityEngine.UI;

public class LossPanal : UIScreen
{
    [SerializeField] Button restartBtn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        restartBtn.onClick.AddListener(OnRestartButton);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnRestartButton()
    {
        UIManager.ScreenNeedToShow?.Invoke(15);
        QueensGridCreator.LoadingGame?.Invoke();
        UIManager.CloseAllScene?.Invoke(ScreenID);
        GamePlayManager.ResetGame?.Invoke();
    }

    private void AddButton()
    {

    }
}
