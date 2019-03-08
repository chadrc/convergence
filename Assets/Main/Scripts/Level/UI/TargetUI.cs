using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TargetUI : MonoBehaviour
{
    public Image TopArrow;
    public Image BottomArrow;
    public Image RightArrow;
    public Image LeftArrow;

    public float outTime = .5f;
    public float rotateTime = 1.0f;
    public float rotation = -90;

    private TowerButtonBehavior btn;

    private RectTransform rect;

    Coroutine currentRoutine;

    // Use this for initialization
    void Awake()
    {
        rect = transform as RectTransform;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (btn != null)
        {
            rect.position = btn.transform.position;
        }
    }

    public void SetToTowerButton(TowerButtonBehavior btn)
    {
        gameObject.SetActive(true);
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            Reset();
        }
        this.btn = btn;
        rect.position = btn.transform.position;
        currentRoutine = StartCoroutine(Open(btn.Size*1.5f, Quaternion.Euler(new Vector3(0, 0, rotation))));
    }

    public void Hide()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }
        currentRoutine = StartCoroutine(Close());
    }

    void Reset()
    {
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
        rect.rotation = new Quaternion();
    }

    IEnumerator Open(float targetSize, Quaternion targetRot)
    {
        float timer = 0;
        float startSize = rect.rect.width;
        while (timer < outTime)
        {
            timer += Time.deltaTime;
            float size = Mathf.Lerp(startSize, targetSize, timer / outTime);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
            yield return new WaitForEndOfFrame();
        }

        timer = 0;
        var startRot = rect.rotation;
        while (timer < rotateTime)
        {
            timer += Time.deltaTime;
            rect.rotation = Quaternion.Slerp(startRot, targetRot, timer / rotateTime);
            yield return new WaitForEndOfFrame();
        }
        rect.rotation = startRot;
    }

    IEnumerator Close()
    {
        float timer = 0;
        float startSize = rect.rect.width;
        while (timer < outTime)
        {
            timer += Time.deltaTime;
            float size = Mathf.Lerp(startSize, 0, timer / outTime);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
        Reset();
        currentRoutine = null;
    }
}
