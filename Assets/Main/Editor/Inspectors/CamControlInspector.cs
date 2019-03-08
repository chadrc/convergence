using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(CamControlMod))]
public class CamControlInspector : Editor
{
    private CamControlMod mod;
    float size = 2;

    void OnEnable()
    {
        mod = target as CamControlMod;
        size = (mod.XLimits.y * 2.0f) / 16.0f;
    }

    public override void OnInspectorGUI()
    {
        // Allow
        mod.AllowPanZoom = EditorGUILayout.Toggle("Allow Pan and Zoom", mod.AllowPanZoom);

        // Start Pos/Zoom Controls
        Vector2 tempPos = EditorGUILayout.Vector2Field("Start Camera Position", mod.CamStartPos);

        EditorGUILayout.LabelField("Start Camera Zoom [" + mod.LowerZoomLimit + ", " + mod.UpperZoomLimit + "]");
        float tempStartZoom = Mathf.Clamp(EditorGUILayout.Slider(mod.StartZoom, mod.LowerZoomLimit, mod.UpperZoomLimit), mod.LowerZoomLimit, mod.UpperZoomLimit);

        if (GUILayout.Button("Set Start Values From Main Camera"))
        {
            tempStartZoom = Camera.main.orthographicSize;
            tempPos = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.z);
        }

        if (GUILayout.Button("Set Camera to Start Values"))
        {
            Camera.main.orthographicSize = tempStartZoom;
            Camera.main.transform.position = new Vector3(tempPos.x, Camera.main.transform.position.y, tempPos.y);
        }

        if (GUILayout.Button("Set Start Values To Default"))
        {
            tempStartZoom = 8.0f;
            tempPos = Vector2.zero;
        }
        
        if (tempPos != mod.CamStartPos)
        {
            mod.CamStartPos = tempPos;
            Camera.main.transform.position = new Vector3(mod.CamStartPos.x, 10.0f, mod.CamStartPos.y);
        }

        if (tempStartZoom != mod.StartZoom)
        {
            mod.StartZoom = tempStartZoom;
            Camera.main.orthographicSize = mod.StartZoom;
        }

        // Zoom Controls
        //EditorGUILayout.LabelField("Zoom Range: [" + mod.LowerZoomLimit + ", " + mod.UpperZoomLimit + "]");
        //EditorGUILayout.MinMaxSlider(ref mod.LowerZoomLimit, ref mod.UpperZoomLimit, 5.0f, 50.0f);
        //mod.LowerZoomLimit = SnapToOnDecimal(mod.LowerZoomLimit);
        //mod.UpperZoomLimit = SnapToOnDecimal(mod.UpperZoomLimit);

        // Size/Limit Controls
        float tempSize = Mathf.Clamp(EditorGUILayout.FloatField("Limits Size [2.0, Inf]: ", size), 2.0f, float.MaxValue);
        if (tempSize < 0)
        {
            tempSize = 0;
        }
        if (tempSize != size)
        {
            size = tempSize;
            float halfWidth = (size * 16) / 2;
            float halfHeight = (size * 9) / 2;
            mod.XLimits = new Vector2(-halfWidth, halfWidth);
            mod.YLimits = new Vector2(-halfHeight, halfHeight);

            mod.UpperZoomLimit = halfHeight;
            mod.LowerZoomLimit = 8.0f;
        }
        EditorGUILayout.LabelField("XLimits: [" + mod.XLimits.x.ToString("0.0") + ", " + mod.XLimits.y.ToString("0.0") + "]");
        EditorGUILayout.LabelField("YLimits: [" + mod.YLimits.x.ToString("0.0") + ", " + mod.YLimits.y.ToString("0.0") + "]");
        EditorGUILayout.LabelField("World Width: " + (size * 16).ToString("0.0"));
        EditorGUILayout.LabelField("World Height: " + (size * 9).ToString("0.0"));
    }

    void OnSceneGUI()
    {
        // Move default handle out of sight
        //mod.transform.position = new Vector3(10000.0f, 0, 0);
        //Vector3 pos = Camera.main.transform.position;
        //pos.y = 0;
        //Vector3 targetPos = pos;
        //targetPos = Handles.PositionHandle(pos, Quaternion.identity);

        //targetPos.y = 10.0f;

        //float halfOrthoWidth = Camera.main.orthographicSize * Camera.main.aspect;
        //float halfOrthoHeight = Camera.main.orthographicSize;

        //// Check Limits
        //if (targetPos.x - halfOrthoWidth < mod.XLimits.x)
        //{
        //    targetPos.x = mod.XLimits.x + halfOrthoWidth;
        //}
        //else if (targetPos.x + halfOrthoWidth > mod.XLimits.y)
        //{
        //    targetPos.x = mod.XLimits.y - halfOrthoWidth;
        //}

        //if (targetPos.z - halfOrthoHeight < mod.YLimits.x)
        //{
        //    targetPos.z = mod.YLimits.x + halfOrthoHeight;
        //}
        //else if (targetPos.z + halfOrthoHeight > mod.YLimits.y)
        //{
        //    targetPos.z = mod.YLimits.y - halfOrthoHeight;
        //}

        //mod.CamStartPos = new Vector2(targetPos.x, targetPos.z);
        //Camera.main.transform.position = targetPos;
    }

    float SnapToOnDecimal(float val)
    {
        float newVal = val*10;

        newVal = Mathf.Round(newVal);

        return newVal / 10;
    }
}
