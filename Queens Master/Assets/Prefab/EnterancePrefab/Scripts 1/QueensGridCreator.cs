using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class QueensGridCreator : MonoBehaviour
{
    public static QueensGridCreator Instance;

    [Header("Grid Settings")]
    public GameObject tilePrefab;
    public float padding = 5f;
    public RectTransform gridContainer;
    public bool isAuto = false;

    [Header("Level Data")]
    public LevelDatabase levelDatabase;
    public TMPro.TextMeshProUGUI levelText;

    // This index follows your ScriptableObject list
    public int currentIndex;

    [Header("Internal References")]
    [SerializeField] private GridLayoutGroup layout;
    [SerializeField] private Transform gridalParent;

    private int[,] regionMap;
    private bool[,] crownMap;
    private DragSelection[,] allTileScripts;

    // Object Pool List
    private List<GameObject> tilePool = new List<GameObject>();

    // Values pulled from your LevelInput script
    private int getRow, getCol;
    private LevelType levelType;
    public GameTimer gameTimer;
    private int currentRandomness;
    public static System.Action LoadingGame;
    public static System.Action CleanAllCross;
    private void OnEnable() {
        LoadingGame += () => LoadLevelFromDatabase(currentIndex);
        
    }
    private void OnDisable() => LoadingGame -= () => LoadLevelFromDatabase(currentIndex);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (gridalParent == null) gridalParent = layout.transform.parent;
        if(gameTimer == null) gameTimer = GetComponentInParent<GameTimer>();
    }
    private void Start()
    {
        // Starts the game using the index you set in the inspector
        LoadLevelFromDatabase(currentIndex);
        RotationAnimation();
    }
    public void LevelUP()
    {
        currentIndex++;
        if (levelDatabase != null && currentIndex < levelDatabase.seeds.Count)
            LoadLevelFromDatabase(currentIndex);
        else
            Debug.Log("End of Database reached.");
    }

    private void RotationAnimation()
    {
        gridalParent.rotation = Quaternion.Euler(0, 0, 40);
        gridalParent.DORotate(Vector3.zero, 0.5f).SetEase(Ease.OutBack);
    }

    public void LoadLevelFromDatabase(int targetIndex)
    {
        Debug.Log("Attempting to load level at index: " + targetIndex + " 000");
        if (levelDatabase == null || targetIndex >= levelDatabase.seeds.Count) return;
        UpdateTextLevel();

        ApplyBorders();
        // Fetching data exactly as defined in your LevelInput script
        LevelInput currentLevelData = levelDatabase.seeds[targetIndex];
        TimerType(currentLevelData.levelType);

        getRow = currentLevelData.lRow;
        getCol = currentLevelData.lCol;
        levelType = currentLevelData.levelType;

        // Use the level number as seed for consistent generation
        Random.InitState(currentLevelData.levelNumber); 

        regionMap = new int[getRow, getCol];
        crownMap = new bool[getRow, getCol];
        Debug.Log( "targetIndex :"+ targetIndex + " getRow :" + getRow + " getCol :" + getCol + " regionMap " + regionMap + " crownMap " + crownMap);
        // Initialize map
        for (int r = 0; r < getRow; r++)
            for (int c = 0; c < getCol; c++)
                regionMap[r, c] = -1;

        // Core Generation
        PlaceCrownsBacktracking(0);
        GenerateLevelTypeCast(levelType);

        // Build Visuals
        BuildGrid();

        if (levelText != null) levelText.text = "Level " + currentLevelData.levelNumber;

        // Update GamePlayManager with the exact row count (Win Score)
        if (GamePlayManager.Instance != null)
            GamePlayManager.Instance.WinScore = getRow;
    }

    public void BuildGrid()
    {
        // 1. Reset all existing tiles in pool
        DeactivateAllTiles();

        if (GamePlayManager.Instance != null)
            GamePlayManager.Instance.crownSelections.Clear();

        // 2. Prepare the array for the exact current dimensions
        allTileScripts = new DragSelection[getRow, getCol];

        // 3. Layout Setup
        float totalWidth = gridContainer.rect.width;
        int maxDim = Mathf.Max(getRow, getCol);
        float cellSize = (totalWidth - (padding * (maxDim - 1))) / maxDim;

        layout.cellSize = new Vector2(cellSize, cellSize);
        layout.spacing = new Vector2(padding, padding);
        layout.constraintCount = getCol;

        // 4. Populate Grid using Pool
        int tileCounter = 0;
        for (int r = 0; r < getRow; r++)
        {
            for (int c = 0; c < getCol; c++)
            {
                GameObject tileObj;

                // If we have a tile in pool, reuse it. If not, make a new one.
                if (tileCounter < tilePool.Count)
                {
                    tileObj = tilePool[tileCounter];
                    tileObj.SetActive(true);
                }
                else
                {
                    tileObj = Instantiate(tilePrefab, transform);
                    tilePool.Add(tileObj);
                }

                DragSelection tileScript = tileObj.GetComponent<DragSelection>();
                tileScript.row = r;
                tileScript.colomn = c;

                // ResetTile handles scale=Vector3.one and stops old animations
                tileScript.ResetTile();

                allTileScripts[r, c] = tileScript;

                // Color/Sprite Assignment from Region Map
                int regionID = regionMap[r, c];
                if (regionID != -1 && GameManager.Instance.ScriptableObjectHolder.baseSqureSpr.Count > 0)
                {
                    //GamePlayManager.Instance.imageDatas[regionID % GamePlayManager.Instance.imageDatas.Count];
                    //ImageData data = GameManager.Instance.ScriptableObjectHolder.baseSqureSpr[regionID % GamePlayManager.Instance.imageDatas.Count];

                    tileScript.coloreTile.sprite = GameManager.Instance.ScriptableObjectHolder.baseSqureSpr[regionID % getRow]; 
                    tileScript.image3dReference.sprite = GameManager.Instance.ScriptableObjectHolder.baseSqureSpr[regionID % getRow];
                    tileScript.crownSprite.sprite = GameManager.Instance.ScriptableObjectHolder.iconeSpr[SaveManager.Instance.progressData.iconIndex];
                    tileScript.iconeImage.sprite = GameManager.Instance.ScriptableObjectHolder.iconeSpr[SaveManager.Instance.progressData.iconIndex];

                }
                
                // Answer Key logic
                tileScript.isCrown = crownMap[r, c];

                if (tileScript.isCrown && GamePlayManager.Instance != null)
                    GamePlayManager.Instance.crownSelections.Add(tileScript);

                tileCounter++;
            }
        }
    }
    private void DeactivateAllTiles()
    {
        foreach (GameObject tile in tilePool)
        {
            if (tile != null)
            {
                // Kill DOTween and Coroutines before hiding
                tile.GetComponent<DragSelection>().ResetTile();
                tile.SetActive(false);
            }
        }
    }

    // --- LOGIC GENERATION ---

    bool PlaceCrownsBacktracking(int row)
    {
        if (row >= getRow) return true;
        List<int> cols = new List<int>();
        for (int i = 0; i < getCol; i++) cols.Add(i);
        for (int i = 0; i < cols.Count; i++)
        {
            int temp = cols[i]; int rand = Random.Range(i, cols.Count);
            cols[i] = cols[rand]; cols[rand] = temp;
        }
        foreach (int col in cols)
        {
            if (IsSafe(row, col))
            {
                crownMap[row, col] = true;
                if (PlaceCrownsBacktracking(row + 1)) return true;
                crownMap[row, col] = false;
            }
        }
        return false;
    }

    bool IsSafe(int r, int c)
    {
        for (int i = 0; i < getRow; i++)
        {
            for (int j = 0; j < getCol; j++)
            {
                if (crownMap[i, j])
                {
                    if (j == c || i == r) return false;
                    if (Mathf.Abs(i - r) <= 1 && Mathf.Abs(j - c) <= 1) return false;
                }
            }
        }
        return true;
    }

    public void AutoFillCrossAtCrown(int crownR, int crownC)
    {
        if (!isAuto) return;
        StartCoroutine(CounterForSlow(crownR, crownC));
    }

    private IEnumerator CounterForSlow(int crownR, int crownC)
    {
        for (int r = 0; r < getRow; r++)
        {
            for (int c = 0; c < getCol; c++)
            {
                if (r == crownR && c == crownC) continue;
                if (r == crownR || c == crownC || (Mathf.Abs(r - crownR) <= 1 && Mathf.Abs(c - crownC) <= 1))
                {
                    allTileScripts[r, c].AutoFillCross();
                }
                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    void GenerateLevelTypeCast(LevelType nameTag)
    {
        switch (nameTag)
        {
            case LevelType.Medium: GenerateMediumRegions(); break;
            case LevelType.Hard: GenerateHardRegions(); break;
            case LevelType.Expert: GenerateExpertRegions(); break;
            case LevelType.special: GenerateMediumRegions(); break;
            case LevelType.superSpecial: GenerateHardRegions(); break;
            default: GenerateSnakeRegions(); break;
        }
    }

    // --- REGION ALGORITHMS ---

    void GenerateEasyRegions()
    {
        for (int r = 0; r < getRow; r++)
            for (int c = 0; c < getCol; c++)
                if (regionMap[r, c] == -1) regionMap[r, c] = FindNearestRegion(r, c);
    }

    int FindNearestRegion(int r, int c)
    {
        float minDist = float.MaxValue; int bestID = 0; int currentID = 0;
        for (int i = 0; i < getRow; i++)
        {
            for (int j = 0; j < getCol; j++)
            {
                if (crownMap[i, j])
                {
                    float d = Vector2.Distance(new Vector2(r, c), new Vector2(i, j));
                    if (d < minDist) { minDist = d; bestID = currentID; }
                    currentID++;
                }
            }
        }
        return bestID;
    }

    void GenerateMediumRegions()
    {
        List<Vector2Int> frontier = new List<Vector2Int>();
        SetupInitialRegions(frontier);
        while (frontier.Count > 0)
        {
            Vector2Int cell = frontier[0]; frontier.RemoveAt(0);
            if (regionMap[cell.x, cell.y] == -1)
            {
                List<int> neighbors = GetNeighborColors(cell.x, cell.y);
                if (neighbors.Count > 0)
                {
                    regionMap[cell.x, cell.y] = neighbors[0];
                    AddNeighborsToFrontier(cell.x, cell.y, frontier);
                }
            }
        }
    }

    void GenerateHardRegions()
    {
        List<Vector2Int> frontier = new List<Vector2Int>();
        SetupInitialRegions(frontier);

        while (frontier.Count > 0)
        {
            int randIdx = Random.Range(0, frontier.Count);
            Vector2Int cell = frontier[randIdx];
            frontier.RemoveAt(randIdx);

            if (regionMap[cell.x, cell.y] == -1)
            {
                List<int> possible = GetNeighborColors(cell.x, cell.y);
                if (possible.Count > 0)
                {
                    // DESIGN VARIATION: Instead of just picking the first neighbor,
                    // we pick a random one to create "interlocking" shapes.
                    regionMap[cell.x, cell.y] = possible[Random.Range(0, possible.Count)];
                    AddNeighborsToFrontier(cell.x, cell.y, frontier);
                }
            }
        }
    }

    void GenerateSnakeRegions()
    {
        List<Vector2Int> frontier = new List<Vector2Int>();
        SetupInitialRegions(frontier);

        while (frontier.Count > 0)
        {
            // Instead of random, we always pick the LAST added cell.
            // This creates "Snake" growth rather than "Blob" growth.
            int lastIdx = frontier.Count - 1;
            Vector2Int cell = frontier[lastIdx];
            frontier.RemoveAt(lastIdx);

            if (regionMap[cell.x, cell.y] == -1)
            {
                List<int> colors = GetNeighborColors(cell.x, cell.y);
                regionMap[cell.x, cell.y] = colors[Random.Range(0, colors.Count)];
                AddNeighborsToFrontier(cell.x, cell.y, frontier);
            }
        }
    }

    void GenerateExpertRegions()
    {
        List<Vector2Int> frontier = new List<Vector2Int>();
        SetupInitialRegions(frontier);
        while (frontier.Count > 0)
        {
            int randIdx = Random.Range(0, frontier.Count);
            Vector2Int cell = frontier[randIdx]; frontier.RemoveAt(randIdx);
            if (regionMap[cell.x, cell.y] == -1)
            {
                List<int> colors = GetNeighborColors(cell.x, cell.y);
                regionMap[cell.x, cell.y] = colors[Random.Range(0, colors.Count)];
                AddNeighborsToFrontier(cell.x, cell.y, frontier);
            }
        }
    }

    void AddNeighborsToFrontier(int r, int c, List<Vector2Int> frontier)
    {
        int[] dr = { 0, 0, 1, -1 }; int[] dc = { 1, -1, 0, 0 };
        for (int i = 0; i < 4; i++)
        {
            int nr = r + dr[i], nc = c + dc[i];
            if (nr >= 0 && nr < getRow && nc >= 0 && nc < getCol && regionMap[nr, nc] == -1)
            {
                Vector2Int pos = new Vector2Int(nr, nc);
                if (!frontier.Contains(pos)) frontier.Add(pos);
            }
        }
    }

    void SetupInitialRegions(List<Vector2Int> frontier)
    {
        int regionID = 0;
        for (int r = 0; r < getRow; r++)
            for (int c = 0; c < getCol; c++)
                if (crownMap[r, c])
                {
                    regionMap[r, c] = regionID++;
                    AddNeighborsToFrontier(r, c, frontier);
                }
    }

    List<int> GetNeighborColors(int r, int c)
    {
        List<int> colors = new List<int>();
        int[] dr = { 0, 0, 1, -1 }; int[] dc = { 1, -1, 0, 0 };
        for (int i = 0; i < 4; i++)
        {
            int nr = r + dr[i], nc = c + dc[i];
            if (nr >= 0 && nr < getRow && nc >= 0 && nc < getCol && regionMap[nr, nc] != -1)
                colors.Add(regionMap[nr, nc]);
        }
        return colors;
    }

    public void GiveSmartHint()
    {
        DragSelection target = null;

        // Search for a crown that hasn't been found yet
        for (int r = 0; r < getRow; r++)
        {
            for (int c = 0; c < getCol; c++)
            {
                if (allTileScripts[r, c].isCrown && !allTileScripts[r, c].crownObject.activeSelf)
                {
                    target = allTileScripts[r, c];
                    break;
                }
            }
            if (target != null) break;
        }

        if (target != null) StartCoroutine(HighlightHintZone(target.row, target.colomn));
    }

    private IEnumerator HighlightHintZone(int crowR, int crowC)
    {
        int regionID = regionMap[crowR, crowC];
        List<DragSelection> highlighted = new List<DragSelection>();

        for (int r = 0; r < getRow; r++)
        {
            for (int c = 0; c < getCol; c++)
            {
                // Check if tile is in the same row, col, or color region
                if (r == crowR || c == crowC || regionMap[r, c] == regionID)
                {
                    DragSelection tile = allTileScripts[r, c];
                    if (!tile.crownObject.activeSelf)
                    {
                        tile.GetComponent<Image>().DOColor(Color.yellow, 0.3f);
                        highlighted.Add(tile);
                    }
                }
            }
        }

        yield return new WaitForSeconds(1.2f); // Hint duration

        // Reset colors back to white
        foreach (var tile in highlighted) tile.GetComponent<Image>().DOColor(Color.white, 0.3f);
    }

    public void RevealOneCrown()
    {
        foreach (DragSelection tile in allTileScripts)
        {
            if (tile.isCrown && !tile.crownObject.activeSelf)
            {
                // We simulate a double click to trigger your existing crown logic
                tile.OnPointerClick(new PointerEventData(EventSystem.current) { clickCount = 2 });
                return; // Only reveal one per click
            }
        }
    }
    void ApplyBorders()
    {
        for (int r = 0; r < getRow; r++)
        {
            for (int c = 0; c < getCol; c++)
            {
                int currentRegion = regionMap[r, c];
                DragSelection tile = allTileScripts[r, c];

                // Show border if it's the edge of the grid OR the neighbor is a different color
                bool top = (r == 0 || regionMap[r - 1, c] != currentRegion);
                bool bottom = (r == getRow - 1 || regionMap[r + 1, c] != currentRegion);
                bool left = (c == 0 || regionMap[r, c - 1] != currentRegion);
                bool right = (c == getCol - 1 || regionMap[r, c + 1] != currentRegion);

            }
        }
    }

    public void UnselectAllWhiteCrosses()
    {
        if (allTileScripts == null) return;

        for (int r = 0; r < getRow; r++)
        {
            for (int c = 0; c < getCol; c++)
            {
                if (allTileScripts[r, c] != null)
                {
                    allTileScripts[r, c].UnselectIfWhiteCross();
                }
            }
        }
        Debug.Log("All temporary white crosses removed.");
    }

    public void TimerType(LevelType type)
    {
        if (type == LevelType.Easy)
        {
            gameTimer.StartGameWithCountdown(GameDifficulty.Normal);
            Debug.Log("Timer Started for Easy Level");
        }
        else
        {
            gameTimer.StartGameWithCountdown(GameDifficulty.Hard);
            Debug.Log("Timer Started for Hard Level");
        }
    }

    public void UpdateTextLevel()
    {
        if (SaveManager.Instance)
        {
            Debug.Log("Updating currentIndex from SaveManager: " + SaveManager.Instance.progressData.currentLevel);
            currentIndex = SaveManager.Instance.progressData.currentLevel;
        }
    }
}