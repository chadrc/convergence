using UnityEngine;

public class CamControlMod : MonoBehaviour
{
    public bool AllowPanZoom = false;
    public Vector2 CamStartPos;
    public float StartZoom;
    public float LowerZoomLimit = 8.0f;
    public float UpperZoomLimit = 20.0f;
    public Vector2 XLimits = new Vector2(-16f, 16f);
    public Vector2 YLimits = new Vector2(-9f, 9f);

	// Use this for initialization
	void Start () 
    {
	    UIController.InitializeCamControl(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    
    void OnDrawGizmos()
    {
        if (!AllowPanZoom)
        {
            return;
        }

        Camera cam = GameObject.Find("Camera").GetComponent<Camera>();
        var center = cam.transform.position;
        center.y = 0;
        float orthoH = cam.orthographicSize;
        float orthoW = orthoH * cam.aspect;
        
        var camBottomLeft = new Vector3(-orthoW, 0, -orthoH) + center;
        var camTopLeft = new Vector3(-orthoW, 0, orthoH) + center;
        var camTopRight = new Vector3(orthoW, 0, orthoH) + center;
        var camBottomRight = new Vector3(orthoW, 0, -orthoH) + center;
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(camBottomLeft, camTopLeft);
        Gizmos.DrawLine(camTopLeft, camTopRight);
        Gizmos.DrawLine(camTopRight, camBottomRight);
        Gizmos.DrawLine(camBottomRight, camBottomLeft);
        
        var bottomLeft = new Vector3(XLimits.x, 0, YLimits.x);
        var topLeft = new Vector3(XLimits.x, 0, YLimits.y);
        var topRight = new Vector3(XLimits.y, 0, YLimits.y);
        var bottomRight = new Vector3(XLimits.y, 0, YLimits.x);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(CamStartPos.x, 0, CamStartPos.y), 5.0f);
    }
}
