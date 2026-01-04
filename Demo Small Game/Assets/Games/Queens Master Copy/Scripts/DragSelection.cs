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
    public void OnPointerEnter(PointerEventData eventData)
    {
        if ((eventData.dragging || Input.GetMouseButton(0)) && !isRevealed)
        {
            ShowWhiteCross();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Required for click timing
    }

    // =========================
    // CLICK LOGIC
    // =========================
    public void OnPointerClick(PointerEventData eventData)
    {
        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick <= doubleClickThreshold)
        {
            // DOUBLE TAP
            if (isCrown)
            {
                ShowCrown();
            }
            else
            {
                ShowRedCross();
            }
        }
        else
        {
            // SINGLE TAP → TOGGLE WHITE CROSS
            if (!isRevealed)
            {
                if (isSelected)
                    HideCross();
                else
                    ShowWhiteCross();
            }
        }

        lastClickTime = Time.time;
    }

    // =========================
    // STATE FUNCTIONS
    // =========================
    void ShowWhiteCross()
    {
        isSelected = true;
        isRevealed = false;

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
