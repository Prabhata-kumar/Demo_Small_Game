using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragSelection : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerClickHandler
{
    [Header("UI References")]
    public GameObject crossObject;   // The X child
    public GameObject crownObject;   // The Crown child

    [Header("Status")]
    public bool isSelected = false;  // Is the X visible?
    public bool isRevealed = false;  // Is the Crown visible?
    public bool isCrown = false;     // Is this actually a correct spot?

    private float lastClickTime;
    private float doubleClickThreshold = 0.3f; // Adjust for "feel"

    [HideInInspector]
    public Image crownSprite;

    void Awake()
    {
        // Auto-assign children if not set in inspector
        if (transform.childCount >= 2)
        {
            crossObject = transform.GetChild(0).gameObject;
            crownObject = transform.GetChild(1).gameObject;
        }

        crossObject.SetActive(false);
        crownObject.SetActive(false);

        crownSprite = crownObject.GetComponent<Image>();
    }

    // SMOOTH DRAGGING: Highlighting with X
    public void OnPointerEnter(PointerEventData eventData)
    {
        // If we are dragging/holding mouse, and it's not a crown yet
        if ((eventData.dragging || Input.GetMouseButton(0)) && !isRevealed)
        {
            SetCross(true);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Required to catch the click
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick <= doubleClickThreshold && isCrown)
        {
            // DOUBLE TAP: Reveal Crown
            ToggleCrown();
        }
        else if (timeSinceLastClick <= doubleClickThreshold && !isCrown)
        {
            // Double click on non-crown does nothing
        }
        else
        {
            // SINGLE TAP: Toggle Cross
            if (!isRevealed)
            {
                SetCross(!isSelected);
            }
        }

        lastClickTime = Time.time;
    }

    private void SetCross(bool state)
    {
        isSelected = state;
        crossObject.SetActive(state);

        // If we turn on a cross, make sure crown is off
        if (state)
        {
            isRevealed = false;
            crownObject.SetActive(false);
        }
    }

    private void ToggleCrown()
    {
        isRevealed = !isRevealed;
        crownObject.SetActive(isRevealed);

        // If we turn on a crown, the cross MUST be hidden
        if (isRevealed)
        {
            isSelected = false;
            crossObject.SetActive(false);
        }
    }
}