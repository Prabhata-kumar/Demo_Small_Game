using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileScript : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IBeginDragHandler,
    IEndDragHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public TileType tileType = TileType.NotSeleted;

    [Header("UI")]
    [SerializeField] private Image tileImage;
    [SerializeField] private Image markImage;

    [Header("Sprites")]
    [SerializeField] private Sprite xSprite;
    [SerializeField] private Sprite crownSprite;
    [SerializeField] private Sprite redCrossSprite;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color dragHighlightColor = new Color(1f, 1f, 1f, 0.6f);

    private float lastTapTime;
    private const float doubleTapThreshold = 0.3f;

    private static bool isDragging = false;

    private void Awake()
    {
        if (!tileImage)
            tileImage = GetComponent<Image>();

        tileImage.color = normalColor;
        markImage.gameObject.SetActive(false);
    }

    // ---------------- INPUT ----------------

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        Highlight();
        ShowX();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        ResetColor();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetColor();

        if (isDragging) return;

        if (Time.time - lastTapTime < doubleTapThreshold)
            HandleDoubleTap();
        else
            HandleSingleTap();

        lastTapTime = Time.time;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging)
        {
            Highlight();
            ShowX();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDragging)
            ResetColor();
    }

    // ---------------- LOGIC ----------------

    private void HandleSingleTap()
    {
        ShowX();
    }

    private void HandleDoubleTap()
    {
        if (tileType == TileType.CrownSelected)
            ShowCrown();
        else
            ShowRedCross();
    }

    // ---------------- VISUALS ----------------

    private void Highlight()
    {
        tileImage.color = dragHighlightColor;
    }

    private void ResetColor()
    {
        tileImage.color = normalColor;
    }

    private void ShowX()
    {
        markImage.gameObject.SetActive(true);
        markImage.sprite = xSprite;
    }

    private void ShowCrown()
    {
        markImage.gameObject.SetActive(true);
        markImage.sprite = crownSprite;
    }

    private void ShowRedCross()
    {
        markImage.gameObject.SetActive(true);
        markImage.sprite = redCrossSprite;
    }
}

