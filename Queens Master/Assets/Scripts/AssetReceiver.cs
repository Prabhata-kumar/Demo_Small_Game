using UnityEngine;
using UnityEngine.UI;

public class AssetReceiver : MonoBehaviour
{
    public Image displayImage;
    public ArtAssetType assetType;
    void OnEnable()
    {
        EditLeaderBoard.OnDeliverSprite += UpdateDisplay;
    }

    void OnDisable()
    {
        EditLeaderBoard.OnDeliverSprite -= UpdateDisplay;
    }

    void UpdateDisplay(ArtAssetType gettheAsset,Sprite receivedSprite)
    {
        if (displayImage != null && gettheAsset == assetType)
        {
            displayImage.overrideSprite = receivedSprite;
        }
    }
}