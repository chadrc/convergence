using UnityEngine;
using UnityEditor;
using System.Collections;

public class RuntimeMonitor : EditorWindow
{
    Vector2 scrollPos;
    void OnGUI()
    {
        if (EditorApplication.isPlaying)
        {
            Render();
        }
        else
        {
            EditorGUILayout.LabelField("Editor not running");
        }
    }

    void Render()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        var unitCtrl = GameObject.FindObjectOfType<UnitController>();
        var towerCtrl = GameObject.FindObjectOfType<TowerController>();
        if (unitCtrl == null || towerCtrl == null)
        {
            EditorGUILayout.PrefixLabel("No Unit Controller");
            return;
        }
        EditorGUILayout.LabelField("Unit Groups", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("ID");
        EditorGUILayout.LabelField("Faction");
        EditorGUILayout.LabelField("Count");
        EditorGUILayout.EndHorizontal();

        foreach (var group in unitCtrl.Groups)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(group.ID.ToString());
            EditorGUILayout.LabelField(group.Faction.ToString());
            EditorGUILayout.LabelField(group.UnitCount.ToString());
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Towers", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Tower Index");
        EditorGUILayout.LabelField("Faction");
        EditorGUILayout.LabelField("Count");
        EditorGUILayout.EndHorizontal();
        var towerGroups = TowerController.GetAllTowers();
        foreach (var t in towerGroups)
        {
            var group = t.StationedGroup;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(t.Index.ToString());
            EditorGUILayout.LabelField(group.Faction.ToString());
            EditorGUILayout.LabelField(group.UnitCount.ToString());
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
        Repaint();
    }
}
