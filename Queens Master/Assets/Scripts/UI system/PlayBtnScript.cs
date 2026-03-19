using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;
using System.Net.Http.Headers; // Added for Task

public class PlayBtnScript : MonoBehaviour
{
    [Header("UI Elements")]
    public Image btnImage;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI rarityText; // Fixed typo from 'rility'

    [Header("Difficulty Colors")]
    public Color expertColor; // Fixed typo from 'export'
    public Color hardColor;   // Fixed typo from 'heard'
    public Color easyColor;

    private int levelIndex = 0;

    // OnEnable should be a standard method, we start the Coroutine inside it
    public void OnEnable()
    {
        levelText.text = "";
        rarityText.text = "";
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        // Wait for 20 seconds as per your original logic
        yield return new WaitForSeconds(0.5f);
        InitialFunction();
    }

    public void InitialFunction()
    {
        // Safety check to ensure Instance exists
        if (SaveManager.Instance == null || GameManager.Instance == null) return;

        levelIndex = SaveManager.Instance.progressData.currentLevel;
        var levelType = GameManager.Instance.levelDatabase.seeds[levelIndex].levelType;

        if (levelType == LevelType.Hard)
        {
            rarityText.text = "HARD";
            btnImage.color = hardColor;
        }
        else if (levelType == LevelType.Expert)
        {
            rarityText.text = "EXPERT"; // Or "VERY HARD"
            btnImage.color = expertColor;
        }
        else
        {
            rarityText.text = "";
            btnImage.color = easyColor;
        }

        levelText.text = "LEVEL " + (levelIndex + 1); // +1 if you want Level 1 instead of Level 0

        Debug.Log(" levelIndex " + levelIndex + " btn");
    }
}