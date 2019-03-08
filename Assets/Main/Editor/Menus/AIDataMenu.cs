using UnityEngine;
using UnityEditor;
using System.Collections;

public class AIDataMenu : MonoBehaviour
{    
    [MenuItem("Convergence/Create/AI Data")]
    public static void ShowAIDataWindow()
    {
        // Get existing open window or if none, make a new one:
        var window = (AIDataWindow)EditorWindow.GetWindow(typeof(AIDataWindow));
        window.Show();
    }
}
	

