using UnityEngine;
using System.Collections;

/// <summary>
///	Fixes Camera width so that entire map is always shown without showing camera background color.
/// </summary>
public class CameraAdjustmentBehavior : MonoBehaviour 
{
	private const float targetHorzSize = 20.5f;
	private Camera cam;

	// Use this for initialization
	void Start () 
	{
		cam = GetComponent<Camera>();
		float vert = targetHorzSize * Screen.height/Screen.width;
		cam.orthographicSize = vert;
	}
	
	// Update is called once per frame
	void Update () 
	{
		float vert = targetHorzSize * Screen.height/Screen.width;
		cam.orthographicSize = vert;
	}
}
