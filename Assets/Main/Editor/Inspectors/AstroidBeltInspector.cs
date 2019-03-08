using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(AstroidBelt))]
public class AstroidBeltInspector : Editor
{
    private AstroidBelt obj;
    private SphereCollider innerCol;
    private SphereCollider outerCol;
    private bool displayGUIText = true;

    void OnEnable()
    {
        obj = target as AstroidBelt;
        outerCol = obj.OuterTriggerProxy.gameObject.GetComponent<SphereCollider>();
        innerCol = obj.InnerTriggerProxy.gameObject.GetComponent<SphereCollider>();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (EditorApplication.isPlaying)
        {
            return;
        }
        displayGUIText = EditorGUILayout.Toggle("Display GUI Text:", displayGUIText);
        obj.Init();
        obj.CalcPos();
    }

    void OnSceneGUI()
    {
        if (EditorApplication.isPlaying)
        {
            return;
        }
        // Text Style for GUI
        var style = new GUIStyle();
        style.normal.textColor = Color.white;
        Handles.color = Color.red;

        // Draw radius handles
        float objY = -obj.transform.eulerAngles.y;
        float objYRad = objY * Mathf.Deg2Rad;
        if (innerCol != null && outerCol != null)
        {
            // Can't scale the value if the value is zero
            if (innerCol.radius < 0.1f)
            {
                innerCol.radius = 0.1f;
            }
            var innerPoint = obj.CalcPoint(objYRad, innerCol.radius);
            innerCol.radius = Handles.ScaleValueHandle(innerCol.radius, innerPoint, Quaternion.identity, 5.0f, Handles.SphereCap, 1.0f);
            if (innerCol.radius > outerCol.radius)
            {
                innerCol.radius = outerCol.radius;
            }

            // Can't scale the value if the value is zero
            if (outerCol.radius < 0.1f)
            {
                outerCol.radius = 0.1f;
            }
            var outerPoint = obj.CalcPoint(objYRad, outerCol.radius);
            outerCol.radius = Handles.ScaleValueHandle(outerCol.radius, outerPoint, Quaternion.identity, 5.0f, Handles.SphereCap, 1.0f);
            if (outerCol.radius < innerCol.radius)
            {
                outerCol.radius = innerCol.radius;
            }

            if (displayGUIText)
            {
                Handles.Label(innerPoint, "Inner Radius\n" + innerCol.radius, style);
                Handles.Label(outerPoint, "Outer Radius\n" + outerCol.radius, style);
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(innerCol);
                EditorUtility.SetDirty(outerCol);
            }
        }
        
        // Draw Opening Handles
        foreach (var opening in obj.Openings)
        {
            float openingAngleAngle = ((opening.OpeningAngle + (opening.OpeningSize / 2)) + objY) * Mathf.Deg2Rad;
            var openingAnglePoint = obj.CalcPoint(openingAngleAngle, (outerCol.radius + innerCol.radius) / 2);
            if (openingAngleAngle < .1f)
            {
                openingAngleAngle = .1f;
            }

            opening.OpeningAngle = Handles.ScaleValueHandle(opening.OpeningAngle, openingAnglePoint, Quaternion.identity, 5.0f, Handles.SphereCap, 1.0f);

            if (opening.OpeningAngle < 0)
            {
                opening.OpeningAngle = 0;
            }
            else if (opening.OpeningAngle > 360.0f)
            {
                opening.OpeningAngle = 360.0f;
            }

            float openingSizeAngle = (opening.OpeningAngle + objY) * Mathf.Deg2Rad;
            var openingSizePoint = obj.CalcPoint(openingSizeAngle, (outerCol.radius + innerCol.radius) / 2);
            // Can't scale the value if the value is zero
            if (opening.OpeningSize < .1f)
            {
                opening.OpeningSize = 0.1f;
            }
            opening.OpeningSize = Handles.ScaleValueHandle(opening.OpeningSize, openingSizePoint, Quaternion.identity, 5.0f, Handles.SphereCap, 1.0f);

            if (opening.OpeningSize < 0)
            {
                opening.OpeningSize = 0;
            }
            else if (opening.OpeningSize > 360)
            {
                opening.OpeningSize = 360;
            }

            if (displayGUIText)
            {
                Handles.Label(openingAnglePoint, "Opening Angle\n" + opening.OpeningAngle, style);
                Handles.Label(openingSizePoint, "Opening Size\n" + opening.OpeningSize, style);
            }
        }

        // Draw starting rotation handle
        float angle = obj.StartRotationPercent * 360.0f;
        // Can't scale the value if the value is zero
        if (angle < 0.1f)
        {
            angle = 0.1f;
        }
        var percentPoint = obj.CalcPoint(angle * Mathf.Deg2Rad, 3.0f);
        obj.StartRotationPercent = Handles.ScaleValueHandle(angle, percentPoint, Quaternion.identity, 5.0f, Handles.SphereCap, 1.0f) / 360.0f;
        Handles.DrawSolidArc(obj.transform.position, Vector3.up, Vector3.right, -angle, 3.0f);
        if (obj.StartRotationPercent < 0)
        {
            obj.StartRotationPercent = 0;
        }
        else if (obj.StartRotationPercent > 1.0f)
        {
            obj.StartRotationPercent = 1.0f;
        }

        if (displayGUIText)
        {
            Handles.Label(percentPoint, "Start Percent\n" + obj.StartRotationPercent, style);
        }
    }
}
