using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NoInternetPanal : UIScreen
{
    public Button crossButton;
    public Transform messageBox;
    private Vector3 originalSize = new Vector3(0.85f, 0.85f, 0.85f);
    private float timer = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        crossButton.onClick.AddListener(CloseButton);
    }

    private void OnEnable()
    {
        messageBox.localScale = Vector3.zero;
        PopUpAnimation();
    }

    public void CloseButton()
    {
        if (!InternetManager.Instance.IsOnline())
        {
            UIManager.CloseAllScene?.Invoke(ScreenID);
            GameManager.OnGameStart?.Invoke();
            return;
        }
        
    }

    public void PopUpAnimation()
    {
        // Stop any running animations on this object to prevent glitches
        messageBox.DOKill();

        messageBox.localScale = Vector3.zero;

        // Using a Sequence makes it easier to add sounds or other effects later
        DOTween.Sequence()
            .Append(messageBox.DOScale(Vector3.one, timer).SetEase(Ease.OutQuad))
            .Append(messageBox.DOScale(originalSize, timer / 2f).SetEase(Ease.InOutSine))
            .SetUpdate(true); // SetUpdate(true) ensures it works even if Time.timeScale is 0
    }
}
