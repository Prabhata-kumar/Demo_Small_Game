using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelDatabase))]
public class LevelDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector so you still see your list
        DrawDefaultInspector();

        LevelDatabase database = (LevelDatabase)target;

        GUILayout.Space(20);
        GUILayout.Label("Mass Generation Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate 1000 Levels"))
        {
            GenerateLevels(database, 1000);
        }

        if (GUILayout.Button("Clear All Levels"))
        {
            database.seeds.Clear();
            EditorUtility.SetDirty(database);
        }
    }

    void GenerateLevels(LevelDatabase db, int amount)
    {
        Undo.RecordObject(db, "Generate Rubber-Band Progression");

        // Start from the current count so you can add more levels later
        int startCount = db.seeds.Count;

        for (int i = 0; i < amount; i++)
        {
            LevelInput newLevel = new LevelInput();
            int currentIdx = startCount + i + 1; // Correctly offsets if list isn't empty
            newLevel.levelNumber = currentIdx;

            // 1. DETERMINE TYPE (The Heartbeat)
            if (currentIdx % 15 == 0) newLevel.levelType = LevelType.Expert;
            else if (currentIdx % 10 == 0) newLevel.levelType = LevelType.Hard;
            else if (currentIdx % 5 == 0) newLevel.levelType = LevelType.special;
            else newLevel.levelType = LevelType.Easy;

            // 2. DEFINE BASE SIZE BY RANGE (Your Polished Logic)
            int baseRow, baseCol;

            if (currentIdx < 8) { baseRow = 4; baseCol = 4; }
            else if (currentIdx < 20) { baseRow = 5; baseCol = 5; }
            else if (currentIdx < 50) { baseRow = 5; baseCol = 6; }
            else if (currentIdx < 100) { baseRow = 6; baseCol = 6; }
            else if (currentIdx < 400) { baseRow = 7; baseCol = 8; }
            else if (currentIdx < 800) { baseRow = 8; baseCol = 9; }
            else { baseRow = 9; baseCol = 10; }

            // 3. APPLY THE RUBBER-BAND EFFECT WITH A SAFETY CLAMP
            if (newLevel.levelType == LevelType.Easy)
            {
                // Shrink by 1, but NEVER go below 4x4
                newLevel.lRow = Mathf.Max(4, baseRow - 1);
                newLevel.lCol = Mathf.Max(4, baseCol - 1);
            }
            else
            {
                newLevel.lRow = baseRow;
                newLevel.lCol = baseCol;
            }

            db.seeds.Add(newLevel);
        }

        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();
        Debug.Log($"Queen Master Synchronized: {amount} levels added.");
    }
}