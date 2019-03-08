using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(OrbitMotion))]
public class OrbitHandles : Editor
{

    private OrbitMotion orbMotion;

    //private float hRadius;
    //private float vRadius;

    private bool makeUniform;

    /// <summary>
    /// We are simply creating a reference to whatever object it is that we currently have selected. (Must have Orbitmotion.cs compenent)
    /// </summary>
    void OnEnable()
    {

        orbMotion = (OrbitMotion)target;


    }

    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();
 
        var startPos = orbMotion.StartPositionInOrbit;
        startPos = EditorGUILayout.Slider("Orbit Starting Position", startPos, 0, 1);
        orbMotion.StartPositionInOrbit = startPos;
        orbMotion.CalculatePositionEditor();

        //Kinda useless, but why not. Makes radii uniform. Sets vertical to horizontal radius. Need to hover over scene view to see results.
        makeUniform = EditorGUILayout.Toggle("Make radii uniform?", makeUniform);
        if (makeUniform == true)
        {
            //if (hRadius != vRadius)
                //vRadius = hRadius;
            //else
                //Debug.LogError("The two values are already equal.");

            makeUniform = false;
        }

        EditorUtility.SetDirty(orbMotion);

    }


    void OnSceneGUI()
    {

        var style = new GUIStyle();
        style.normal.textColor = Color.cyan;
        style.fontSize = 15;
        style.fontStyle = FontStyle.Bold;
        Handles.color = Color.magenta;

        float hRadius = orbMotion.HorizontalRadius;
        float vRadius = orbMotion.VerticalRadius;
        //Used for the handles positions in the scene.
        var horizontalPoint = new Vector3(hRadius, 0.0f, 0.0f);
        var verticalPoint = new Vector3(0.0f, 0.0f, vRadius);

        //The horizontal radius is set via changes made to this handle.
        hRadius = Handles.ScaleValueHandle(hRadius, horizontalPoint, Quaternion.identity, 7.5f, Handles.SphereCap, 1.0f);
        orbMotion.HorizontalRadius = hRadius;
        Handles.DrawLine(orbMotion.OrbitCenter.position, horizontalPoint);

        //The vertical radius is set via changes made to this handle.
        vRadius = Handles.ScaleValueHandle(vRadius, verticalPoint, Quaternion.identity, 7.5f, Handles.SphereCap, 1.0f);
        orbMotion.VerticalRadius = vRadius;
        if (orbMotion.OrbitCenter != null)
            Handles.DrawLine(orbMotion.OrbitCenter.position, verticalPoint);
        else
            Debug.Log("Quick! Assign a value to the public field \" Orbit Motion\" ");

        //Shows text at the points of our horizontal and vertical handles. Displays what they are and their respective values
        Handles.Label(horizontalPoint, "Horizontal Radius\n" + hRadius, style);
        Handles.Label(verticalPoint, "Vertical Radius\n" + vRadius, style);

        orbMotion.CalculatePositionEditor();
        //If any of the GUI has been changed in scene, set some variables.
        if (GUI.changed)
        {
            //orbMotion.transform.position = new Vector3(hRadius, orbMotion.transform.position.y, orbMotion.transform.position.z); //This is in here so it isn't happening outside of us intentionally editing the value via the scene.
            EditorUtility.SetDirty(orbMotion);
        }

    }



}
