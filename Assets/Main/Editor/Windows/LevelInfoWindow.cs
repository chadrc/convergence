using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LevelInfoWindow : EditorWindow
{
    bool valid = true;
    LevelController lvlCtrl = null;
    GameObject levelFab = null;
    LevelData levelData = null;
    FactionController facCtrl = null;
    TowerController towerCtrl = null;
    UnitController unitCtrl = null;

    List<string> errorList = new List<string>();
    bool displayConvergences = false;
    bool displayTowerGraphics = false;
    bool displayTowerRangeGraphics = false;
    bool displayUnitGraphics = false;
    bool displayAllTowers = false;
    Vector2 leftScroll;
    Vector2 rightScroll;
    Dictionary<string, bool> displayTowerBools = new Dictionary<string, bool>();

    [MenuItem("Convergence/Level Info")]
    public static void OpenLevelInfoWindow()
    {
        var window = (LevelInfoWindow)EditorWindow.GetWindow(typeof(LevelInfoWindow));
        window.minSize = new Vector2(800, 600);
        window.Show();
    }

    void OnEnable()
    {
        Validate();
        //if (valid)
        //{
        //    var towers = levelFab.GetComponentsInChildren<TowerBehavior>();
        //    foreach(var t in towers)
        //    {
        //        displayTowerBools.Add(t.gameObject.name, false);
        //    }
        //}
    }

    void OnGUI()
    {
        if (valid)
        {
            DisplayLevelInfo();
        }
        else
        {
            EditorGUILayout.LabelField("Please resolve errors.");
            foreach (var e in errorList)
            {
                EditorGUILayout.LabelField(e);
            }
            if (GUILayout.Button("Validate"))
            {
                Validate();
            }
        }
    }

    private void Validate()
    {
        errorList.Clear();
        lvlCtrl = GameObject.FindObjectOfType<LevelController>();
        valid = true;
        if (lvlCtrl == null)
        {
            errorList.Add("Error: No Level Controller in scene.");
        }
        else
        {
            levelData = lvlCtrl.CurrentLevel;
            if (levelData == null)
            {
                errorList.Add("Error: No Level Data in level controller.");
                valid = false;
            }
            else
            {
                Debug.Log(levelData.Prefab.name);
                levelFab = GameObject.Find(levelData.Prefab.name);
                if (levelFab == null)
                {

                    errorList.Add("Error: Level Prefab not in scene");
                    valid = false;
                }
                else
                {
                    facCtrl = levelFab.GetComponentInChildren<FactionController>();
                    if (facCtrl == null)
                    {
                        errorList.Add("Error: No Faction Controller in level.");
                        valid = false;
                    }

                    towerCtrl = levelFab.GetComponentInChildren<TowerController>();
                    if (towerCtrl == null)
                    {
                        errorList.Add("Error: No Tower Controller in level.");
                        valid = false;
                    }

                    unitCtrl = levelFab.GetComponentInChildren<UnitController>();
                    if (unitCtrl == null)
                    {
                        errorList.Add("Error: No Unit Controller in level.");
                        valid = false;
                    }
                }
            }
        }
    }

    private void DisplayLevelInfo()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical(GUILayout.Width(400));
        leftScroll = EditorGUILayout.BeginScrollView(leftScroll);
        DisplayRightSide();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(GUILayout.Width(400));
        rightScroll = EditorGUILayout.BeginScrollView(rightScroll);
        DisplayLeftSide();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    private void DisplayRightSide()
    {
        EditorGUILayout.LabelField("General Information", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Name: " + levelData.Name);
        EditorGUILayout.LabelField("Description: " + levelData.Discription);
        EditorGUILayout.LabelField("Prefab Name: " + levelData.Prefab.name);
        displayConvergences = EditorGUILayout.Foldout(displayConvergences, "Convergences");
        if (displayConvergences)
        {
            int count = 1;
            foreach (var c in levelData.convergences)
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.LabelField("Occurrence " + count);
                EditorGUI.indentLevel = 2;
                EditorGUILayout.LabelField("First Occurence: " + c.FirstTime);
                EditorGUILayout.LabelField("IntervalTime: " + c.IntervalTime);
                string towersInvolved = "[";
                foreach (var t in c.TowersInvolved)
                {
                    towersInvolved += t + ", ";
                }
                towersInvolved = towersInvolved.Substring(0, towersInvolved.Length - 2) + "]";
                EditorGUILayout.LabelField("Towers Involved: " + towersInvolved);
                count++;
            }
        }
        EditorGUI.indentLevel = 0;

        // Faction Controller Stuff
        EditorGUILayout.LabelField("Faction Count: " + facCtrl.NumberOfFactions);
        var towers = levelFab.GetComponentsInChildren<TowerBehavior>();

        // Unit Controller Stuff
        EditorGUILayout.LabelField("Unit Pool Count: " + unitCtrl.UnitPoolCount);
        displayUnitGraphics = EditorGUILayout.Foldout(displayUnitGraphics, "Unit Prefabs By Faction");
        if (displayUnitGraphics)
        {
            int fCount = 0;
            foreach (var p in unitCtrl.FactionUnitPrefabs)
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.LabelField("Faction " + (fCount++) + " Unit Fab: " + (p == null ? "NULL" : p.name));
            }
        }
        EditorGUI.indentLevel = 0;

        // Tower Controller Stuff

        displayTowerGraphics = EditorGUILayout.Foldout(displayTowerGraphics, "Tower Graphics By Faction");
        if (displayTowerGraphics)
        {
            int tgCount = 0;
            foreach (var t in towerCtrl.FactionTowerGraphics)
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.LabelField("Faction " + (tgCount++) + " Tower Graphic: " + (t == null ? "NULL" : t.name));
            }
        }
        EditorGUI.indentLevel = 0;

        displayTowerRangeGraphics = EditorGUILayout.Foldout(displayTowerRangeGraphics, "Tower Range Graphics By Faction");
        if (displayTowerRangeGraphics)
        {
            int tgCount = 0;
            foreach (var t in towerCtrl.FactionTowerRangeGraphics)
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.LabelField("Faction " + (tgCount++) + " Tower Range Graphic: " + (t == null ? "NULL" : t.name));
            }
        }
        EditorGUI.indentLevel = 0;

        EditorGUILayout.LabelField("Level Metrics", EditorStyles.boldLabel);
        int[] factionTowerCount = new int[facCtrl.NumberOfFactions];
        int[] factionUnitCount = new int[facCtrl.NumberOfFactions];
        foreach( var t in towers)
        {
            factionTowerCount[t.Faction]++;
            factionUnitCount[t.Faction] += t.StartingUnits;
        }

        for (int i=0; i<facCtrl.NumberOfFactions; i++)
        {
            EditorGUI.indentLevel = 0;
            EditorGUILayout.LabelField("Faction " + i);
            EditorGUI.indentLevel = 1;
            EditorGUILayout.LabelField("Number of Towers: " + factionTowerCount[i]);
            EditorGUILayout.LabelField("Number of Units: " + factionUnitCount[i]);
        }
        EditorGUI.indentLevel = 0;
    }

    private void DisplayLeftSide()
    {
        EditorGUILayout.LabelField("Towers");
        var towers = levelFab.GetComponentsInChildren<TowerBehavior>();
        EditorGUILayout.LabelField("Tower Count: " + towers.Length);
        //displayAllTowers = EditorGUILayout.Foldout(displayAllTowers, "Towers");
        if (true)
        {
            var indexSet = new HashSet<int>();
            foreach (var t in towers)
            {
                EditorGUI.indentLevel = 0;
                string indexStr = "Index: " + t.Index;
                if (indexSet.Contains(t.Index))
                {
                    indexStr += " (Duplicate index)";
                }
                else
                {
                    indexSet.Add(t.Index);
                }

                if (!displayTowerBools.ContainsKey(t.gameObject.name))
                {
                    displayTowerBools.Add(t.gameObject.name, false);
                }
                displayTowerBools[t.gameObject.name] = EditorGUILayout.Foldout(displayTowerBools[t.gameObject.name], t.gameObject.name);
                EditorGUI.indentLevel = 1;
                if (!displayTowerBools[t.gameObject.name])
                {
                    continue;
                }
                EditorGUILayout.LabelField(indexStr);

                string facStr = "Faction: " + t.Faction;
                if (t.Faction < 0 || t.Faction > facCtrl.NumberOfFactions)
                {
                    facStr += " (Invalid Faction)";
                }

                EditorGUILayout.LabelField(facStr);
                EditorGUILayout.LabelField("Starting Units: " + t.StartingUnits);
                EditorGUILayout.LabelField("Atmosphere Range: " + t.CurrentStats.AtmosphereRange);
                EditorGUILayout.LabelField("Units Per Second: " + t.CurrentStats.UnitsGeneratedPerSecond);
                EditorGUILayout.LabelField("Endurance: " + t.CurrentStats.Endurance);
            }
        }

        EditorGUI.indentLevel = 0;
    }
}
