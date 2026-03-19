using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CalendarDayItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private Image playedIndicator; // Use this for checkmarks/icons
    private Button dayButton;

    private void Awake() => dayButton = GetComponent<Button>();
    private void Start()
    {
        dayButton.onClick.AddListener(() => {
            // Handle day click logic here, e.g., open a detail panel or show stats
            Debug.Log($"Day {dayText.text} clicked!");
        });
    }
    public void Setup(int day, bool isToday, bool hasPlayed, bool hasFailed)
    {
        dayText.text = day.ToString();

        // Priority Logic for Colors
        if (isToday)
        {
            playedIndicator.sprite = GameManager.Instance.ScriptableObjectHolder.DateStatueTodaySpr;
        }
        else if (hasPlayed)
        {
            playedIndicator.sprite = GameManager.Instance.ScriptableObjectHolder.DateStatuePlayed;
            dayButton.interactable = false;
        }
        else if (hasFailed)
        {
            playedIndicator.sprite = GameManager.Instance.ScriptableObjectHolder.DateStatueFailedSpr;
        }
    }

}