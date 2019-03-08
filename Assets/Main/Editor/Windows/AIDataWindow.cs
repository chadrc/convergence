using UnityEngine;
using UnityEditor;
using System.Collections;

public class AIDataWindow : EditorWindow {

    private string assetName;

    void OnGUI()
    {
        GUILayout.Label("Name of Asset", EditorStyles.boldLabel);
        assetName = EditorGUILayout.TextField("Text Field", assetName);

        if (assetName == null || assetName.Length == 0)
        {
            EditorGUILayout.HelpBox("Must name the AI Data before it can be created!", MessageType.Error);
        }

        else
        {
            if (GUILayout.Button("Create"))
            {
                TWEditorUtil.CreateScriptableAsset<AIData>("Assets/Main/Data/AI_Datas/" + assetName + ".asset");
            }
        }
    }
}
