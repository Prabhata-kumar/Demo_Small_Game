using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AssetRequester : MonoBehaviour
{
    public ArtAssetType assetType;
    public int assetIndex;
    public Image selectedImg;
    private Button myButton;

    private void Awake()
    {
        // Assign this here so it's ready BEFORE any events fire
        myButton = GetComponent<Button>();
    }

    private void OnEnable()
    {
        EditLeaderBoard.offAllOthereThenThat += ResetAllSelected;
        EditLeaderBoard.autoButtonAct += AutoRequestArt;
    }

    private void OnDisable()
    {
        EditLeaderBoard.offAllOthereThenThat -= ResetAllSelected;
        EditLeaderBoard.autoButtonAct -= AutoRequestArt;
    }

    void Start()
    {
        myButton.onClick.AddListener(ExecuteSelection);
    }

    public void AutoRequestArt(ArtAssetType type, int index)
    {
        if (type == assetType && index == assetIndex)
        {
            Debug.Log($"Auto-selecting asset {index} of type {type}");
            ExecuteSelection();
        }
    }

    // Created a helper function so you don't repeat code
    private void ExecuteSelection()
    {
        EditLeaderBoard.offAllOthereThenThat?.Invoke(assetType);

        if (GameManager.Instance != null && GameManager.Instance.ScriptableObjectHolder != null)
        {
            GameManager.Instance.ScriptableObjectHolder.HandleAssetRequest(assetType, assetIndex);
        }

        myButton.interactable = false;
        selectedImg.gameObject.SetActive(true);
    }

    public void ResetAllSelected(ArtAssetType assetSeteledType)
    {
        if (assetType != assetSeteledType) return;
        myButton.interactable = true;
        selectedImg.gameObject.SetActive(false);
    }
}