using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class ExplosionQuickTimePointerResponder : IPointerResponder
{
    private UIController controller;
    private int[] towerClickCounts;
    private UIPointerResponder uiResponder;

    public UIController Controller { get; set; }

    public List<int> TowerClicks
    {
        get
        {
            return new List<int>(towerClickCounts);
        }
    }

    public ExplosionQuickTimePointerResponder(UIPointerResponder uiResponder = null)
    {
        this.uiResponder = (uiResponder == null) ? new UIPointerResponder() : uiResponder;
        Reset();
    }

    public void Reset()
    {
        var towers = TowerController.GetAllTowers();
        towerClickCounts = new int[towers.Count];
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        uiResponder.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        uiResponder.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        uiResponder.OnEndDrag(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var btn = Controller.GetTowerButton(eventData.position);
        if (btn != null)
        {
            //Debug.Log("Tower " + btn.Tower.Index);
            towerClickCounts[btn.Tower.Index]++;
            //Debug.Log("Clicked " + towerClickCounts[btn.Tower.Index] + " times.");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        uiResponder.OnPointerDown(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        uiResponder.OnPointerUp(eventData);
    }
    
    public void OnScroll(PointerEventData eventData)
    {
        
    }
}
