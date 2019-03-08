using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// UnitGroup keeps track of a number of units.
/// </summary>
public class UnitGroup
{
    private int id;
    private int faction;
    private int unitCount;
    private int maxEndurance;
    private int endurance;

    public int Endurance
    {
        get
        {
            return endurance;
        }
        set
        {
            maxEndurance = value;
            endurance = value;
        }
    }
    public int Faction { get { return faction; } }
    public int UnitCount
    {
        get
        {
            return unitCount;
        }

        set
        {
            if (value >=0)
            {
                unitCount = value;
            }
        }
    }
    public bool Empty { get { return unitCount <= 0;  } }
    public int ID { get { return id; } }

    /// <summary>
    /// Create UnitGroup with initial size and sets its faction.
    /// </summary>
    /// <param name="id">Unique ID for this group.</param>
    /// <param name="faction">Faction for this group.</param>
    /// <param name="size">Initial number of units.</param>
    /// <param name="endurance">Initial endurance for this group.</param>
    public UnitGroup(int id, int faction, int size, int endurance=100)
    {
        this.id = id;
        this.faction = faction;
        unitCount = size;
        this.endurance = endurance;
        maxEndurance = endurance;
    }

    /// <summary>
    /// Adds one unit to this group.
    /// </summary>
    public void AddUnit()
    {
        unitCount++;
    }

    /// <summary>
    /// Subtracts one unit from this group. Does not allow count to fall below zero.
    /// </summary>
    public void SubtractUnit()
    {
        unitCount--;
        if (unitCount < 0)
        {
            unitCount = 0;
        }
    }

    /// <summary>
    /// Sets all units in this group to inactive.
    /// </summary>
    public void HideUnits()
    {
        var units = UnitController.GetUnitsForGroup(ID);
        foreach (var u in units)
        {
            u.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Calls Prepare method on all units in this group.
    /// </summary>
    /// <param name="origin"></param>
    public void PrepareUnits(TowerBehavior origin)
    {
        var units = UnitController.GetUnitsForGroup(ID);
        foreach(var u in units)
        {
            u.Prepare(origin);
        }
    }

    /// <summary>
    /// Calls Unprepare method on all units in this group.
    /// </summary>
    public void UnprepareUnits()
    {
        var units = UnitController.GetUnitsForGroup(ID);
        if (units == null)
        {
            return;
        }

        foreach (var u in units)
        {
            u.Unprepare();
        }
    }

    /// <summary>
    /// Calls MoveTo method on all units in this group.
    /// </summary>
    /// <param name="destination"></param>
    public void MoveUnits(TowerBehavior destination)
    {
        var units = UnitController.GetUnitsForGroup(ID);
        if (units == null)
        {
            return;
        }

        foreach (var u in units)
        {
            if (u != null)
            {
                u.MoveTo(destination);
            }
        }
    }

    /// <summary>
    /// Adds amount of units to this group.
    /// </summary>
    /// <param name="amount">Number of units to add.</param>
    public void AddUnits(int amount)
    {
        unitCount += amount;
    }

    /// <summary>
    /// Subtracts amount of units from this group. Does not allwo count ot fall below zero.
    /// </summary>
    /// <param name="amount">Number of units to subtract.</param>
    public void SubtractUnits(int amount)
    {
        unitCount -= amount;
        if (unitCount < 0)
        {
            unitCount = 0;
        }
    }

    /// <summary>
    /// Calculates whether a unit will be subtracted from this group based on damage taken.
    /// See documentation for full metrics.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns>True if a unit was subtracted, false otherwise.</returns>
    public bool Damage(int amount)
    {
        endurance -= amount;
        if (endurance <= 0)
        {
            SubtractUnit();
            endurance = maxEndurance + endurance;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Uninitializes this group.
    /// </summary>
    public void Destory()
    {
        unitCount = 0;
        endurance = 0;
    }
}
