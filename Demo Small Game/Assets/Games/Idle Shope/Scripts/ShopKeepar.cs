using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    [Header("Item Animations")]
    [SerializeField] private List<AnimationClip> items;

    private Animator animator;
    public int currentItemIndex = 0;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator not found in children!");
        }
    }

    void Update()
    {
        HandleRunning();
        HandleItemCycling();
    }

    // -----------------------------
    // RUN / IDLE LOGIC
    // -----------------------------
    private void HandleRunning()
    {
        bool isRunning = Input.GetKey(KeyCode.Space);
        animator.SetBool("run", isRunning);
    }

    // -----------------------------
    // ITEM SHOWCASE LOGIC
    // -----------------------------
    private void HandleItemCycling()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (items == null || items.Count == 0)
        {
            Debug.LogWarning("No item animations assigned!");
            return;
        }

        currentItemIndex = (currentItemIndex + 1) % items.Count;

        animator.SetInteger("itemIndex", currentItemIndex);
        animator.SetTrigger("showItem");
    }
}
