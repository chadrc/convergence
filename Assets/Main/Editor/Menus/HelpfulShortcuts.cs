using UnityEngine;
using UnityEditor;


static class HelpfulShortcuts
{
    [MenuItem("Tools/Clear Console %#x")] // SHIFT + C
    static void ClearConsole()
    {

        //Allows for clearing of the console window if it is the current window being focused on.
        if (EditorWindow.focusedWindow.GetType().ToString() == "UnityEditor.ConsoleWindow")
        {
            // This simply does "LogEntries.Clear()" the long way:
            var logEntries = System.Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
            var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            clearMethod.Invoke(null, null);
        }

    }

}

