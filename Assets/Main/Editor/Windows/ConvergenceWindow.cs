using UnityEngine;
using UnityEditor;
using System.Collections;

public class ConvergenceWindow : EditorWindow
{
    Vector2 scrollPos;
    bool update = true;
    SimData data;
    int page = 0;
    int framesPerPage = 25;

    void OnGUI()
    {
        if (update)
        {
            // Find level controller to obtain the data and prefab
            var levelCtrl = GameObject.FindObjectOfType<LevelController>();
            //Debug.Log("Level Ctrl: " + levelCtrl);
            var levelData = levelCtrl.CurrentLevel;
            //Debug.Log("Level Data: " + levelData);
            var levelObj = levelData.Prefab;
            //Debug.Log("Level Object: " + levelObj);
            data = ConverganceCalc.SimulateForConvergences(levelObj);
            update = false;
        }

        if (data == null)
        {
            return;
        }

        var firstFrame = data.Frames[0];
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Time", GUILayout.Width(100));
        foreach (var o in firstFrame)
        {
            var tower = o.Key.gameObject.GetComponent<TowerBehavior>();
            string text = "";
            if (tower == null)
            {
                text = "N/A";
            }
            else
            {
                text = tower.Index.ToString();
            }
            EditorGUILayout.LabelField(text, GUILayout.Width(100));
        }
        EditorGUILayout.EndHorizontal();
        int pageCount = data.Frames.Count / framesPerPage;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int i=page*framesPerPage; i<(page*framesPerPage)+framesPerPage; i++)
        {
            if (i >= data.Frames.Count)
            {
                break;
            }
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.LabelField(data.Frames[i].Time.ToString("#.##"), GUILayout.Width(100));

            foreach (var o in data.Frames[i])
            {
                EditorGUILayout.LabelField((o.Value % 360).ToString("#.##"), GUILayout.Width(100));
            }

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("<"))
        {
            if (page > 0)
            {
                page--;
            }
        }

        if (GUILayout.Button(">"))
        {
            if (page < pageCount)
            {
                page++;
            }
        }
        EditorGUILayout.EndHorizontal();
    }
}
