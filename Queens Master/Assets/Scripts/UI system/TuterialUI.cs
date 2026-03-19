using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TuterialUI : UIScreen
{
    public Button closeBtn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        closeBtn.onClick.AddListener(ClosePanel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClosePanel()
    {
        transform.DOKill();
        UIManager.CloseAllScene?.Invoke(ScreenID);
    }
}
