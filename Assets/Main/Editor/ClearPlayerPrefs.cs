using UnityEngine;
using UnityEditor;
using System.Collections;

public class ClearPlayerPrefs
{
    [MenuItem("Tools/Clear PlayerPrefs")]
    private static void NewMenuOption()
    {
        PlayerPrefs.DeleteAll();
    }
}
