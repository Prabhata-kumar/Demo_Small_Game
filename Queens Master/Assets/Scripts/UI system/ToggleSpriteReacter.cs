using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Toggle))]
public class ToggleSpriteReacter : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;

    public Toggle toggle;


    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        if (targetImage == null) targetImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        // Sync visual immediately on enable
        UpdateVisuals(toggle.isOn);
        toggle.onValueChanged.AddListener(UpdateVisuals);
    }

    private void OnDisable()
    {
        toggle.onValueChanged.RemoveListener(UpdateVisuals);
    }

    public void UpdateVisuals(bool value)
    {
        if (targetImage == null) return;
        targetImage.sprite = value ? onSprite : offSprite;
        toggle.isOn = value;
    }
}