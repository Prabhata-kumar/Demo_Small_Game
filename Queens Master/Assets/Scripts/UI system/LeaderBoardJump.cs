using DG.Tweening;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardJump : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform viewport;
    [SerializeField] private RectTransform content;
    [SerializeField] private List<LeaderBoardIndexDetail> leaderboardSlots;

    [Header("Animation Settings")]
    [SerializeField] private float moveDuration = 0.25f;
    [SerializeField] private Vector3 scaleUp = new Vector3(1.15f, 1.15f, 1.15f);
    [SerializeField] private Vector3 originalSize = Vector3.one;
    [SerializeField] private Ease moveEase = Ease.OutQuart;

    private string _myPlayerId;
    public LeaderBoardIndexDetail _playerSlot;
    private DatabaseReference _dbRef;
    private int _currentGlobalStartRank = 1;

    private async void Start()
    {
        _myPlayerId = SaveManager.Instance?.leaderboardData?.playerId;
        await RefreshLeaderboard();
    }

    private async void OnEnable()
    {
        // Ensure visual order matches list order on enable
        RearrangeHierarchy();
        await RefreshLeaderboard();
    }

   /* private void Update()
    {
        // Debug Test: Press Space to simulate gain stars and jumping
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int currentStars = _playerSlot != null ? _playerSlot.CurrentStars : 0;
            StartCoroutine(UpdateLeaderBoardRoutine(currentStars + UnityEngine.Random.Range(5, 15)));
        }
    }*/

    public void CallBackFunction(int starCount)
    {
        Debug.Log("starCount " + starCount);
        StartCoroutine(UpdateLeaderBoardRoutine(starCount));
    }
    private IEnumerator UpdateLeaderBoardRoutine(int starCount)
    {

        // Safety: Timeout after 2 seconds if slot isn't found
        float timeout = Time.time + 2f;
        yield return new WaitUntil(() => _playerSlot != null || Time.time > timeout);

        if (_playerSlot == null) yield break;

        yield return new WaitForSeconds(0.5f);
        UpdateScoreAndRank(starCount);
    }

    public async Task RefreshLeaderboard()
    {
        _dbRef = FirebaseDatabase.DefaultInstance.GetReference("Leaderboard");
        var snapshot = await _dbRef.OrderByChild("starCount").GetValueAsync();

        if (!snapshot.Exists) return;

        var allPlayers = snapshot.Children
            .Select(c => JsonUtility.FromJson<LeaderboardData>(c.GetRawJsonValue()))
            .OrderByDescending(p => p.starCount)
            .ToList();

        int myGlobalIndex = allPlayers.FindIndex(p => p.playerId == _myPlayerId);
        if (myGlobalIndex == -1) return;

        // Window logic: Keep player visible near the bottom initially
        int offset = 2;
        int startIndex = Mathf.Max(0, myGlobalIndex - (leaderboardSlots.Count - 1 - offset));
        _currentGlobalStartRank = startIndex + 1;

        var displayList = allPlayers.Skip(startIndex).Take(leaderboardSlots.Count).ToList();

        RenderSlots(displayList, startIndex);
        PositionAtViewportBottom();
    }

    private void RenderSlots(List<LeaderboardData> players, int startRank)
    {
        _playerSlot = null;
        for (int i = 0; i < leaderboardSlots.Count; i++)
        {
            bool hasData = i < players.Count;
            leaderboardSlots[i].gameObject.SetActive(hasData);

            if (hasData)
            {
                leaderboardSlots[i].SetDetails(startRank + i + 1, players[i]);
                if (players[i].playerId == _myPlayerId)
                    _playerSlot = leaderboardSlots[i];
            }
        }
        RefreshSerialNumbers(_currentGlobalStartRank);
    }

    public void PositionAtViewportBottom()
    {
        Canvas.ForceUpdateCanvases();
        if (_playerSlot == null) return;

        RectTransform playerRect = _playerSlot.GetComponent<RectTransform>();

        // Target scroll to show player at the bottom area of viewport
        float targetY = -playerRect.anchoredPosition.y - viewport.rect.height + (playerRect.rect.height * 1.5f);
        float maxScroll = Mathf.Max(0, content.rect.height - viewport.rect.height);

        content.anchoredPosition = new Vector2(content.anchoredPosition.x, Mathf.Clamp(targetY, 0, maxScroll));
    }

    public void UpdateScoreAndRank(int newStars)
    {
        if (_playerSlot == null) return;

        // Find the index in the CURRENT visible list where the player should land
        int targetIdx = leaderboardSlots
            .OrderByDescending(s => s == _playerSlot ? newStars : s.CurrentStars)
            .ToList()
            .FindIndex(s => s == _playerSlot);

        ExecuteLeaderBoardJump(newStars, targetIdx);
    }

    public async void ExecuteLeaderBoardJump(int newStars, int finalTargetIndex)
    {
        if (_playerSlot == null) return;

        RectTransform playerRect = _playerSlot.GetComponent<RectTransform>();

        // 1. Highlight Player
        await playerRect.DOScale(scaleUp, 0.3f).SetEase(Ease.OutBack).AsyncWaitForCompletion();
        _playerSlot.UpdateOnlyScore(newStars);

        // 2. Step-by-Step Climb
        // We move the sibling index and then tween the scroll to follow
        while (playerRect.GetSiblingIndex() > finalTargetIndex)
        {
            int nextIdx = playerRect.GetSiblingIndex() - 1;
            playerRect.SetSiblingIndex(nextIdx);

            // Critical: Force Layout Group to calculate new positions immediately
            Canvas.ForceUpdateCanvases();

            // Update the Rank Numbers (1st, 2nd, etc) based on new positions
            RefreshSerialNumbers(_currentGlobalStartRank);

            // Scroll the viewport to keep the climbing player centered
            float targetScrollY = CalculateCenterY(playerRect);
            await content.DOAnchorPosY(targetScrollY, moveDuration).SetEase(moveEase).AsyncWaitForCompletion();
        }

        // 3. Landing
        await playerRect.DOScale(originalSize, 0.3f).SetEase(Ease.InQuad).AsyncWaitForCompletion();

        // Optional: Save to Firebase here so the rank is permanent
        // await _dbRef.Child(_myPlayerId).Child("starCount").SetValueAsync(newStars);
    }

    public void RefreshSerialNumbers(int baseRank)
    {
        // We iterate through the hierarchy because SetSiblingIndex changed the order
        for (int i = 0; i < content.childCount; i++)
        {
            Transform child = content.GetChild(i);
            if (child.gameObject.activeSelf && child.TryGetComponent<LeaderBoardIndexDetail>(out var slot))
            {
                slot.UpdateRankDisplay(baseRank + i);
            }
        }
    }

    private float CalculateCenterY(RectTransform target)
    {
        // Calculate the content position required to center the target in the viewport
        float itemCenterInContent = -target.anchoredPosition.y;
        float targetScrollPos = itemCenterInContent - (viewport.rect.height / 2f);

        float maxScroll = Mathf.Max(0, content.rect.height - viewport.rect.height);
        return Mathf.Clamp(targetScrollPos, 0, maxScroll);
    }

    public void RearrangeHierarchy()
    {
        if (leaderboardSlots == null) return;
        for (int i = 0; i < leaderboardSlots.Count; i++)
        {
            leaderboardSlots[i].transform.SetSiblingIndex(i);
        }
        Canvas.ForceUpdateCanvases();
    }
}