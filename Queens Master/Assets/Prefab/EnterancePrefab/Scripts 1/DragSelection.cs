using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DragSelection : MonoBehaviour,
    IPointerEnterHandler,
    IPointerDownHandler,
    IPointerClickHandler
{
    [Header("UI References")]
    public Image coloreTile;
    public GameObject crossObject;
    public GameObject crownObject;
    public Image animImage;
    public Image image3dReference;
    public Image iconeImage;
    public Image whightHeart;
    public GameObject particalEffect;

    [Header("Logic")]
    public bool isCrown = false;

    [Header("Cross Colors")]
    public Color whiteCross = Color.white;
    public Color redCross = Color.red;

    [Header("Internal Setup")]
    public int row, colomn;
    [HideInInspector] public Image crownSprite;

    private Image crossImage;
    private Image ownCOmponent;
    private float doubleClickThreshold = 0.3f;
    private float lastClickTime = -1f;
    private float animationTime = 0.5f;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isSelected = false;
    private bool isRevealed = false;

    // OPTIMIZATION: Cache the transform to avoid internal C++ to C# calls
    private Transform cachedTransform;

    void Awake()
    {
        cachedTransform = transform; // Cache transform

        // 1. Robust Reference Assignment
        if (crossObject == null) crossObject = cachedTransform.GetChild(0).gameObject;
        if (crownObject == null) crownObject = cachedTransform.GetChild(1).gameObject;

        crossImage = crossObject.GetComponent<Image>();
        crownSprite = crownObject.GetComponent<Image>();
        ownCOmponent = GetComponent<Image>();

        // 2. Initial State
        crossObject.SetActive(false);
        crownObject.SetActive(false);
    }

    void Start()
    {
        originalScale = cachedTransform.localScale; // Use cached transform
        targetScale = originalScale * 1.1f;

        ResetTile();
    }

    // Standard Reset for pooling or level restart
    public void ResetTile()
    {
        // 1. Stop all current processes
        StopAllCoroutines();
        cachedTransform.DOKill(); // Use cached transform

        // 2. Reset Logic Flags
        isSelected = false;
        isRevealed = false;

        // 3. Reset Transform (Set scale to exactly 1)
        cachedTransform.localScale = Vector3.one; // Use cached transform

        // 4. Reset Visuals
        crossObject.SetActive(false);
        crownObject.SetActive(false);

        // OPTIMIZATION: Simplified null checks
        if (image3dReference) image3dReference.gameObject.SetActive(false);
        if (particalEffect) particalEffect.gameObject.SetActive(false);

        // 5. Reset Interaction
        // OPTIMIZATION: Removed redundant GetComponent (already done in Awake)
        ownCOmponent.raycastTarget = true;
        ownCOmponent.enabled = true;
        whightHeart.enabled = false;

        // Reset Color to white (so old Red Cross tints don't stay)
        ownCOmponent.color = Color.white;
    }

    // =========================
    // EVENT HANDLERS
    // =========================

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isRevealed) return;

        // Determine Drag Mode
        if (isSelected)
        {
            GamePlayManager.Instance.currentDragMode = DragMode.Erasing;
            HideCross();
        }
        else
        {
            GamePlayManager.Instance.currentDragMode = DragMode.Drawing;
            ShowWhiteCross();
            PlayAnimationCross(); // Feedback for first touch
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Only trigger if we are actively dragging
        // OPTIMIZATION: Check isRevealed first to avoid checking Input/EventData unnecessarily
        if (isRevealed) return;

        if (eventData.dragging || Input.GetMouseButton(0))
        {
            DragMode mode = GamePlayManager.Instance.currentDragMode;

            if (mode == DragMode.Drawing && !isSelected)
            {
                ShowWhiteCross();
                PlayAnimationCross();
            }
            else if (mode == DragMode.Erasing && isSelected)
            {
                HideCross();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // OPTIMIZATION: Store current time to avoid multiple property calls
        float currentTime = Time.time;

        if (currentTime - lastClickTime <= doubleClickThreshold)
        {
            // Double Click logic
            if (isCrown)
            {
                //ShowCrown();
                StartCoroutine(PlayCrown3dAnimation());
            }
            else
            {
                ShowRedCross();
            }
        }
        lastClickTime = currentTime;
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
        StartCoroutine(RedCrossAnimation());

        GamePlayManager.Instance.LoosTheHeart();

    }

    IEnumerator RedCrossAnimation()
    {
        isSelected = true;
        isRevealed = true;

        crossObject.SetActive(true);
        crownObject.SetActive(false);

        ownCOmponent.raycastTarget = false;
        coloreTile.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        whightHeart.DOFade(1f, 0);
        whightHeart.DOFade(0f, animationTime).OnComplete(() => {
            whightHeart.enabled = false;
            coloreTile.DOColor(whiteCross, animationTime / 2);
        });
        // OPTIMIZATION: Cache transform to avoid property access in DOTween chain
        Transform crossTransform = crossObject.transform;
        crossTransform.DOScale(targetScale, 0.1f).OnComplete(() => crossTransform.DOScale(originalScale, 0.1f));
        crossImage.DOColor(redCross, animationTime / 2);
    }

    void HideCross()
    {
        isSelected = false;
        crossObject.SetActive(false);
    }

    public void AutoFillCross()
    {
        if (!isRevealed && !isSelected)
        {
            ShowWhiteCross();
        }
    }

    // =========================
    // ANIMATIONS (Optimized)
    // =========================

    public IEnumerator PlayCrown3dAnimation()
    {
        isRevealed = true;
        isSelected = false;
        // OPTIMIZATION: Removed redundant/conflicting SetActive/raycast calls
        ownCOmponent.raycastTarget = false;
        coloreTile.raycastTarget = false;
        coloreTile.enabled = false;
        crownObject.SetActive(false);
        crossObject.SetActive(false);
        image3dReference.gameObject.SetActive(true);
        iconeImage.gameObject.SetActive(true);

        QueensGridCreator.Instance.AutoFillCrossAtCrown(row, colomn);
        particalEffect.SetActive(true);
        yield return new WaitForSeconds(0.8f);

        particalEffect.SetActive(false);
        if (!GamePlayManager.isGameWin)
        {
            image3dReference.gameObject.SetActive(false);
            iconeImage.gameObject.SetActive(false);

            coloreTile.enabled = true;
            crownObject.SetActive(true);

            PlayAnimationForCrown();
        }
    }

    public void PlayAnimationForCrown()
    {
        crownSprite.DOKill();
        // OPTIMIZATION: Cache transform for scale operation
        Transform crownTrans = crownSprite.transform;
        crownTrans.localScale = Vector3.zero;
        crownSprite.gameObject.SetActive(true);
        crownTrans.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);

        GamePlayManager.Instance.CheckGameOver();
    }

    public void PlayAnimationCross()
    {
        cachedTransform.DOKill(); // Use cached
        animImage.DOKill();

        // Tile "Pop"
        cachedTransform.DOScale(targetScale, 0.1f).OnComplete(() => cachedTransform.DOScale(originalScale, 0.1f));

        // Cross Graphic Fade
        animImage.enabled = true;
        // OPTIMIZATION: Cache transform for scale operation
        Transform animTrans = animImage.transform;
        animTrans.localScale = Vector3.zero;

        // Reset Alpha properly via DOFade
        animImage.DOFade(1f, 0);
        animTrans.DOScale(Vector3.one, animationTime);
        animImage.DOFade(0f, animationTime).OnComplete(() => animImage.enabled = false);
    }

    public void UnselectIfWhiteCross()
    {
        // Only clear if it's a white cross (isSelected) 
        // and NOT a revealed Crown or Red Cross (isRevealed)
        if (isSelected && !isRevealed)
        {
            isSelected = false;
            crossObject.SetActive(false);

            // Optional: Play a tiny scale down animation
            cachedTransform.DOKill(); // Use cached
            cachedTransform.DOScale(originalScale, 0.2f);
        }
    }
}