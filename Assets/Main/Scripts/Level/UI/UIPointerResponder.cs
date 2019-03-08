using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;

public class UIPointerResponder : IPointerResponder
{
    public static System.Action<TowerButtonBehavior> PlayerSelectedTower;
    public static System.Action<TowerButtonBehavior> PlayerDeselectedTower;
    public static System.Action<TowerButtonBehavior> PlayerHoveredTower;
    public static System.Action<TowerButtonBehavior> PlayerUnhoveredTower;

    private TowerButtonBehavior firstSelect;
    private TowerButtonBehavior secondSelect;

    private TowerButtonBehavior selected;
    private UnitGroup curUnitGroup;

    public UIController Controller { get; set; }

    public UIPointerResponder()
    {

    }

    #region IPointerResponder Implemention

    public void OnBeginDrag(PointerEventData eventData)
    {
        var btn = Controller.GetTowerButton(eventData.position);

        //Debug.Log("Button: " + btn.Tower.Index + " | Faction: " + btn.Tower.Faction);
        if (btn != null && btn.Tower.Faction == FactionController.PlayerFaction)
        {
            firstSelect = btn;
            //Debug.Log("Drag Begin: " + btn.Tower.Index);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (firstSelect == null)
        {
            return;
        }

        var s = Controller.GetTowerButton(eventData.position);
        if (s != null)
        {
            //Debug.Log("Dragging: " + s.Tower.Index);

            // If pointer is over tower and it is not the same as the first selected tower
            if (s != firstSelect)// && secondSelect == null)
            {
                // If second is not selected yet
                if (secondSelect == null)
                {
                    secondSelect = s;
                    if (PlayerHoveredTower != null)
                    {
                        PlayerHoveredTower(secondSelect);
                    }
                }
                // Else if s is not already selected
                else if (s != secondSelect)
                {
                    if (PlayerUnhoveredTower != null)
                    {
                        PlayerUnhoveredTower(secondSelect);
                    }
                    secondSelect = s;
                    if (PlayerHoveredTower != null)
                    {
                        PlayerHoveredTower(secondSelect);
                    }
                }
            }
            else
            {
                // Unhover second selected tower
                if (secondSelect != null && PlayerUnhoveredTower != null)
                {
                    PlayerUnhoveredTower(secondSelect);
                    secondSelect = null;
                }
            }
        }
        else
        {
            // If pointer isn't over anything hide second tower's atmosphere if exists
            if (secondSelect != null)
            {
                if (PlayerUnhoveredTower != null)
                {
                    PlayerUnhoveredTower(secondSelect);
                }
            }
            secondSelect = null;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var btn = Controller.GetTowerButton(eventData.position);

        if (btn == null || btn.Tower.Faction != FactionController.PlayerFaction)
        {
            return;
        }

        if (firstSelect == null)
        {
            firstSelect = btn;
        }
        else
        {
            // If double click
            if (firstSelect == btn && eventData.clickCount % 2 == 0)
            {
                // If already selected, deselect
                if (selected == firstSelect)
                {
                    firstSelect.Deselect();
                    selected = null;
                }
                else
                {
                    // If nothing is selected, select
                    if (selected != null)
                    {
                        selected.Deselect();
                    }
                    firstSelect.Select();
                    selected = firstSelect;
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var btn = Controller.GetTowerButton(eventData.position);

        if (btn != null && btn.Tower.Faction == FactionController.PlayerFaction)
        {
            // Soft select tower and prepare units for movement
            firstSelect = btn;
            //firstSelect.Tower.ShowAtmosphere();

            curUnitGroup = UnitController.CreateUnitGroupForFaction(btn.Tower.Faction, (int)(btn.Tower.StationedUnits * Controller.UnitPercent));
            curUnitGroup.PrepareUnits(btn.Tower);

            if (PlayerSelectedTower != null)
            {
                PlayerSelectedTower(btn);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (firstSelect != null)
        {
            if (secondSelect != null)
            {
                // If two towers are selected and they are not the same tower, move units to second tower
                if (firstSelect != secondSelect)
                {
                    //Debug.Log("Sending Units: " + curUnitGroup.UnitCount);

                    TowerBehavior.MoveGroupFromTo(curUnitGroup, firstSelect.Tower, secondSelect.Tower);

                    curUnitGroup = null;
                }

                if (PlayerUnhoveredTower != null)
                {
                    PlayerUnhoveredTower(secondSelect);
                }
            }

            if (PlayerDeselectedTower != null)
            {
                PlayerDeselectedTower(firstSelect);
            }
        }

        // Reset unused units
        if (curUnitGroup != null)
        {
            curUnitGroup.UnprepareUnits();
            curUnitGroup = null;
        }

        firstSelect = null;
        secondSelect = null;
    }

    public void OnScroll(PointerEventData eventData)
    {

    }

    #endregion

    private void UpdateUnits(float unitPercent)
    {
        //Debug.Log("Updating Units");
        int total = (int)(firstSelect.Tower.StationedUnits * unitPercent);
        int newUnits = total - curUnitGroup.UnitCount;
        UnitController.AddUnitsToGroup(curUnitGroup, newUnits);
        curUnitGroup.PrepareUnits(firstSelect.Tower);
        //Debug.Log("Unit Count: " + curUnitGroup.UnitCount);
    }
}