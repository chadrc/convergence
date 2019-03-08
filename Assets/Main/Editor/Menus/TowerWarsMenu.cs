using UnityEngine;
using System.Collections;
using UnityEditor;

public class TowerWarsMenu : MonoBehaviour 
{
	[MenuItem ("Convergence/Create/Level Data")]
	static void CreateLevelData()
	{
		TWEditorUtil.CreateScriptableAsset<LevelData>("Assets/Main/Data/Levels/Data/Level.asset");
	}

	[MenuItem ("Convergence/Create/Global Tower Info")]
	static void CreateGlobalTowerInfo()
	{
		TWEditorUtil.CreateScriptableAsset<GlobalTowerInfo>("Assets/Main/Data/GlobalTowerInfo.asset", true, false);
	}

	[MenuItem ("Convergence/Create/Level List")]
	static void CreateLevelList()
	{
		TWEditorUtil.CreateScriptableAsset<LevelList>("Assets/Main/Data/Levels/Lists/LevelList.asset");
	}

    [MenuItem("Convergence/Runtime Monitor")]
    static void RuntimeWindow()
    {
        // Get existing open window or if none, make a new one:
        var window = (RuntimeMonitor)EditorWindow.GetWindow(typeof(RuntimeMonitor));
        window.Show();
    }

    [MenuItem("Convergence/Convergence Window")]
    static void ConvergenceWindow()
    {
        // Get existing open window or if none, make a new one:
        var window = (ConvergenceWindow)EditorWindow.GetWindow(typeof(ConvergenceWindow));
        window.Show();
    }
}
