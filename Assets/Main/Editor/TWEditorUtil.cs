using UnityEngine;
using UnityEditor;
using System.Collections;

public static class TWEditorUtil 
{
	public static T CreateScriptableAsset<T>(string path, bool uniqueAsset=false, bool focusAfterCreation=true) where T : ScriptableObject
	{
		var a = ScriptableObject.CreateInstance<T>();
		string assetPath = path;
		if (!uniqueAsset)
		{
			assetPath = AssetDatabase.GenerateUniqueAssetPath(path);
		}

		AssetDatabase.CreateAsset(a, assetPath);
		AssetDatabase.Refresh();
		if (focusAfterCreation)
		{
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = a;
		}
		return a;
	}
}
