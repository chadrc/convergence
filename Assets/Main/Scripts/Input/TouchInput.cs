using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInput : MonoBehaviour, IPointerDownHandler 
{
    public static event System.Action DragBegin;
    public static event System.Action Draggin;
    public static event System.Action DragEnd;
    
    void Start()
    {
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Down");
    }
}
