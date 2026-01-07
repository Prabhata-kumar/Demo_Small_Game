using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TicTackToTile : MonoBehaviour, IPointerDownHandler
{
    [Header("Settings")]
    public int index;

    [Header("References")]
    public Image tileImage; // The icon (X or O)
    public Image coreImage; // The background/button itself

     public bool isSelected = false;

    private void Awake()
    {
        // Fallback: If not assigned in Inspector, find them
        if (coreImage == null) coreImage = GetComponent<Image>();
        if (tileImage == null) tileImage = transform.GetChild(0).GetComponent<Image>();

        // Ensure the icon is invisible at the start
        tileImage.sprite = null;
        tileImage.color = new Color(1, 1, 1, 0);
    }

    private void OnEnable()
    {
        TikTackToManager.TileInteraction += TileResponse;
    }

    private void OnDisable()
    {
        TikTackToManager.TileInteraction -= TileResponse;
    }

    public void ResetTile()
    {
        isSelected = false;
        tileImage.sprite = null;
        tileImage.color = new Color(1, 1, 1, 0); // Hide the image
        coreImage.raycastTarget = true;
    }

    public void TileResponse(bool getValue)
    {
        coreImage.raycastTarget = getValue;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Guard clause: Don't allow clicking if already selected
        if (isSelected) return;

        isSelected = true;
        coreImage.raycastTarget = false;

        // Update visual
        tileImage.sprite = TikTackToManager.Instance.OnTileSelected(index);
        tileImage.color = Color.white; // Make it visible
    }
}