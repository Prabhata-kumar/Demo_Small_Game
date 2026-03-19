using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Required for the smooth chase effect

public class ScrolleController : MonoBehaviour
{
    [Header("UI References")]
    public ScrollRect scrollRect;
    public RectTransform viewport;
    public RectTransform content;

    [Header("The Moving Object")]
    public RectTransform playerRow; // The "pkp" object

    [Header("Movement Settings")]
    public float duration = 0.7f;
    public Ease moveEase = Ease.OutQuint; // Smooth, high-speed entry
    public Vector3 scaleUp = new Vector3(1.25f,1.25f,1.25f), originalSize = Vector3.one;

    void Update()
    {
        // Press Space to move the player to a new random rank and center them
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Example: Move to a random position between 2nd and 10th place
            int randomRank = Random.Range(1, 10);
            MoveAndCenter(randomRank);
        }
    }

    public void MoveAndCenter(int newIndex)
    {
        if (playerRow == null) return;

        // 1. Change the hierarchy position (The "Rank")
        playerRow.SetSiblingIndex(newIndex);
        playerRow.DOScale(scaleUp,0.5f).SetEase(moveEase).OnComplete(()  => 
        {
            Canvas.ForceUpdateCanvases();

            // 3. Calculate the Y needed to put the playerRow in the center of the Viewport
            // Formula: -(Item Position) - (Half of Viewport Height)
            float targetY = -playerRow.anchoredPosition.y - (viewport.rect.height / 2f);

            // 4. Clamp the value so the scroll doesn't go past the very top or bottom
            float maxScroll = content.rect.height - viewport.rect.height;
            targetY = Mathf.Clamp(targetY, 0, maxScroll);

            // 5. Use DOTween to smoothly slide the content to that position
            content.DOAnchorPosY(targetY, duration).SetEase(moveEase).OnComplete(() => playerRow.DOScale(originalSize, 0.5f).SetEase(moveEase));
        } );
        // 2. IMPORTANT: Force the UI to calculate the new Y position immediately
        
    }
}