using UnityEngine;
using System.Collections;

public class CameraControl 
{
    public Camera Cam { get { return Camera.main; } }
    public float OrthographicHeight { get { return Cam.orthographicSize * 2; } }
    public float OrthographicWidth { get { return OrthographicHeight *  Cam.aspect; } }
    
	public float LowerZoomLimit { get; private set; }
	public float UpperZoomLimit { get; private set; }
	public Vector2 XLimits { get; private set; }
	public Vector2 YLimits { get; private set; }

    /// <summary>
    /// Zoom value as a fraction between LowerZoomLimit and UpperZoomLimit. (0 = LowerZoomLimit | 1 = UpperZoomLimit)
    /// </summary>
    public float ZoomValue { get { return (Cam.orthographicSize - LowerZoomLimit) / (UpperZoomLimit - LowerZoomLimit); } }

    public Vector3 CamPosition { get { return Cam.transform.position; } }

    private Vector3 targetPos;
    
    public CameraControl()
    {
        
    }

    public CameraControl(CamControlMod mod)
    {
        LowerZoomLimit = mod.LowerZoomLimit;
        UpperZoomLimit = mod.UpperZoomLimit;
        XLimits = mod.XLimits;
        YLimits = mod.YLimits;

        if (mod.AllowPanZoom)
        {
            Game.EternalBehaviour.StartCoroutine(MoveToTarget());
        }
        
        Cam.orthographicSize = mod.StartZoom;
        Cam.orthographicSize = Mathf.Clamp(Cam.orthographicSize, LowerZoomLimit, UpperZoomLimit);
        Cam.transform.position = new Vector3(mod.CamStartPos.x, Cam.transform.position.y, mod.CamStartPos.y);
        targetPos = Cam.transform.position;
        Pan(Vector2.zero);
    }
    
    private IEnumerator MoveToTarget()
    {
        while(true)
        {
            yield return new WaitForEndOfFrame();
            Cam.transform.position = Vector3.Lerp(Cam.transform.position, targetPos, .9f);
        }
    }

    public void ForceTargetPostion(Vector3 position)
    {
        targetPos.x = position.x;
        targetPos.z = position.z;
        Pan(Vector2.zero);
    }
    
    public void Zoom(Vector2 delta)
    {
        float newSize = Cam.orthographicSize + delta.y;
            
        if (newSize < LowerZoomLimit)
        {
            newSize = LowerZoomLimit;
        }
        
        if (newSize > UpperZoomLimit)
        {
            newSize = UpperZoomLimit;
            targetPos.x = Mathf.Lerp(XLimits.x, XLimits.y, .5f);
            targetPos.z = Mathf.Lerp(YLimits.x, YLimits.y, .5f);
        }

        Cam.orthographicSize = newSize;
        Pan(Vector2.zero);
    }
    
    public void Pan(Vector2 delta)
    {
        if (Cam.orthographicSize >= UpperZoomLimit)
        {
            return;
        }

        var worldFrac = new Vector2(delta.x/Screen.width, delta.y/Screen.height);
        var worldDelta = new Vector2(worldFrac.x * OrthographicWidth, worldFrac.y * OrthographicHeight);
        targetPos.x -= worldDelta.x;
        targetPos.z -= worldDelta.y;
        
        var camPos = Cam.transform.position;
        var halfOrthoWidth = OrthographicWidth/2;
        var halfOrthoHeight = OrthographicHeight/2;
        
        if (targetPos.x - halfOrthoWidth < XLimits.x)
        {
            targetPos.x = XLimits.x + halfOrthoWidth;
        }
        else if (targetPos.x + halfOrthoWidth > XLimits.y)
        {
            targetPos.x = XLimits.y - halfOrthoWidth;
        }
        
        if (targetPos.z - halfOrthoHeight < YLimits.x)
        {
            targetPos.z = YLimits.x + halfOrthoHeight;
        }
        else if (targetPos.z + halfOrthoHeight > YLimits.y)
        {
            targetPos.z = YLimits.y - halfOrthoHeight;
        }
    }
}
