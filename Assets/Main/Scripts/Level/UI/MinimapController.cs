using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class MinimapController : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler
{
    public RectTransform CamAreaImageRect;
    public Camera MiniMapRenderCam;

    private CameraControl cam;
    private CamControlMod mod;

    private float maxH = 225.0f;
    private float maxW = 400.0f;

    private float maxWAspect;
    private float maxHAspect;

	// Use this for initialization
	void Awake ()
    {
        UIController.CamControlInitialized += OnCamControlInitialized;
        gameObject.SetActive(false);
	}

    void OnDestroy()
    {
        UIController.CamControlInitialized -= OnCamControlInitialized;
    }

    void OnCamControlInitialized(CameraControl ctrl)
    {
        gameObject.SetActive(true);
        cam = ctrl;
        maxWAspect = maxW;
        maxHAspect = maxWAspect * ((float)Screen.height / Screen.width);
        CamAreaImageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxW * cam.ZoomValue);
        CamAreaImageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxH * cam.ZoomValue);

        float limitHeight = cam.YLimits.y - cam.YLimits.x;
        float limitWidth = cam.XLimits.y - cam.XLimits.x;

        MiniMapRenderCam.orthographicSize = limitHeight / 2;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (cam == null)
        {
            return;
        }
        
        float wFrac = cam.OrthographicWidth / Mathf.Abs(cam.XLimits.y - cam.XLimits.x);
        float newHeight = maxHAspect * wFrac;
        if (newHeight > maxH)
        {
            newHeight = maxH;
        }

        float hFrac = cam.OrthographicHeight / Mathf.Abs(cam.YLimits.y - cam.YLimits.x);
        float newWidth = maxWAspect * hFrac;
        if (newWidth > maxW)
        {
            newWidth = maxW;
        }

        CamAreaImageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        CamAreaImageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        
        float posXFrac = Mathf.Abs((cam.CamPosition.x - cam.XLimits.x) / (cam.XLimits.y - cam.XLimits.x));
        float posYFrac = Mathf.Abs((cam.CamPosition.z - cam.YLimits.x) / (cam.YLimits.y - cam.YLimits.x));

        var newPos = new Vector2((posXFrac * maxW), (posYFrac * maxH));
        CamAreaImageRect.anchoredPosition = newPos;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ForcePos(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        ForcePos(eventData);
    }

    private void ForcePos(PointerEventData eventData)
    {        
        //Debug.Log("Pointer: " + eventData.position);

        Vector2 mmSpace = eventData.position;
        float trueWidth = ((RectTransform)transform).rect.width * (Screen.width / UIController.Scaler.referenceResolution.x);
        float trueHeight = ((RectTransform)transform).rect.height * (Screen.height / UIController.Scaler.referenceResolution.y);
        mmSpace.x -= (Screen.width - trueWidth);

        //Debug.Log("Scale Factor: " + (UIController.Scaler.referenceResolution.x / Screen.width));
        //Debug.Log("Screen Width: " + Screen.width);
        //Debug.Log("MiniMap Width: " + trueWidth);
        //Debug.Log("MinMapSpace: " + mmSpace);

        mmSpace.x /= trueWidth;
        mmSpace.y /= trueHeight;
        //Debug.Log("MiniMapSpace Normal: " + mmSpace);

        var worldSpace = new Vector3();
        worldSpace.x = Mathf.Lerp(cam.XLimits.x, cam.XLimits.y, mmSpace.x);
        worldSpace.z = Mathf.Lerp(cam.YLimits.x, cam.YLimits.y, mmSpace.y);
        cam.ForceTargetPostion(worldSpace);
    }
}
