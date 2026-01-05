using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragSelection : MonoBehaviour,
    IPointerEnterHandler,
    IPointerDownHandler,
    IPointerClickHandler
{
    [Header("UI References")]
    public GameObject crossObject;   // X icon
    public GameObject crownObject;   // Crown icon

    [Header("Logic")]
    public bool isCrown = false;     // Is this tile actually a crown?

    [Header("Cross Colors")]
    public Color whiteCross = Color.white;
    public Color redCross = Color.red;

    private Image crossImage;

    public int row, colomn;

    [HideInInspector] public Image crownSprite;

    private bool isSelected = false; // Cross visible
    private bool isRevealed = false; // Crown visible

    private float lastClickTime = -1f;
    private float doubleClickThreshold = 0.3f;
    private Image ownCOmponent;

    
    void Awake()
    {
        // Auto assign children if not set
        if (crossObject == null && transform.childCount > 0)
            crossObject = transform.GetChild(0).gameObject;

        if (crownObject == null && transform.childCount > 1)
            crownObject = transform.GetChild(1).gameObject;

        crossImage = crossObject.GetComponent<Image>();
        crownSprite = crownObject.GetComponent<Image>();
        ownCOmponent = GetComponent<Image>();

        crossObject.SetActive(false);
        crownObject.SetActive(false);
    }

    // =========================
    // DRAG → WHITE CROSS ONLY
    // =========================
    // 1. OnPointerDown handles the FIRST touch (Start Dragging or Single Tap)
    public void OnPointerDown(PointerEventData eventData)
    {
        if (isRevealed) return;

        // Determine if we are starting a DRAW or an ERASE based on current state
        if (isSelected)
        {
            GamePlayManager.Instance.currentDragMode = DragMode.Erasing;
            HideCross(); // Acts as a single tap ERASE
        }
        else
        {
            GamePlayManager.Instance.currentDragMode = DragMode.Drawing;
            ShowWhiteCross(); // Acts as a single tap DRAW
        }
    }

    // 2. OnPointerEnter handles the PAINTING while dragging
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Check if the button is still held down and it's not a Crown/RedX
        if (Input.GetMouseButton(0) && !isRevealed)
        {
            DragMode mode = GamePlayManager.Instance.currentDragMode;

            if (mode == DragMode.Drawing)
            {
                ShowWhiteCross();
            }
            else if (mode == DragMode.Erasing)
            {
                HideCross();
            }
        }
    }

    // 3. OnPointerClick ONLY handles the DOUBLE CLICK for Crowns
    public void OnPointerClick(PointerEventData eventData)
    {
        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick <= doubleClickThreshold)
        {
            // Double Click Detected
            if (isCrown)
            {
                ShowCrown();
            }
            else
            {
                // If they double clicked an empty tile that wasn't a crown
                ShowRedCross();
            }
        }
        // REMOVED: The "else" block that toggled the white cross.
        // Why? Because OnPointerDown already did it!

        lastClickTime = Time.time;
    }

    // =========================
    // STATE FUNCTIONS
    // =========================
    void ShowWhiteCross()
    {
        isSelected = true;

        crossImage.color = whiteCross;
        crossObject.SetActive(true);
        crownObject.SetActive(false);
    }

    void ShowRedCross()
    {
        isSelected = true;
        isRevealed = true;

        crossImage.color = redCross;
        crossObject.SetActive(true);
        crownObject.SetActive(false);
       
        GamePlayManager.Instance.LoosTheHeart();
        ownCOmponent.raycastTarget = false;
    }

    void HideCross()
    {
        isSelected = false;
        crossObject.SetActive(false);
    }

    void ShowCrown()
    {
        isRevealed = true;
        isSelected = false;

        crownObject.SetActive(true);
        crossObject.SetActive(false);

        GamePlayManager.Instance.CardReavil();
        ownCOmponent.raycastTarget = false;

        QueensGridCreator.Instance.SingedUPtheGrides(row,colomn);
    }

    public void AutoFillCross()
    {
        // Don't auto-cross if the tile is already revealed (as a Crown or Red Cross)
        // and don't overwrite if the player has already placed a crown there
        if (!isRevealed)
        {
            ShowWhiteCross();
        }
    }
}
