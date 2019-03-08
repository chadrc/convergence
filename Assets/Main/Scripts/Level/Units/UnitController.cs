using UnityEngine;
using System.Collections.Generic;

public class UnitController : MonoBehaviour 
{
	private static UnitController current;
    private static int groupCount;

    public int UnitPoolCount = 500;
	public GameObject[] FactionUnitPrefabs;

    private List<int> currentPullCounts = new List<int>();
    private List<List<UnitBehavior>> unitPool = new List<List<UnitBehavior>>();
    private List<UnitGroup> groups = new List<UnitGroup>();
    private Dictionary<int, List<UnitBehavior>> groupUnitsMap = new Dictionary<int, List<UnitBehavior>>();

    public List<UnitGroup> Groups
    {
        get
        {
            var temp = new List<UnitGroup>();
            foreach(var g in groups)
            {
                if (g.Empty)
                {
                    groupUnitsMap.Remove(g.ID);
                }
                else
                {
                    temp.Add(g);
                }
            }
            groups = temp;
            return groups;
        }
    }
	
	// Raise warning if more than one UnitController exists.
	void Awake ()
	{
		if (current != null)
		{
			Debug.LogWarning("Multiple UnitControllers created. Replacing current.");
		}
		current = this;
	}

    void Start()
    {
        // Start at 1 to skip neutral faction
        // And add empty list so faction numbers line up properly
        unitPool.Add(new List<UnitBehavior>());
        currentPullCounts.Add(0);

        // Setup other faction tracking
        for (int i=1; i<FactionController.FactionCount; i++)
        {
            unitPool.Add(new List<UnitBehavior>());
            currentPullCounts.Add(0);

            if (i >= FactionUnitPrefabs.Length)
            {
                Debug.LogError("Missing unit prefab for faction: " + i);
                continue;
            }
            var fab = FactionUnitPrefabs[i];
            if (fab == null)
            {
                Debug.LogError("Missing unit prefab for faction: " + i);
                continue;
            }

            if (fab.GetComponent<UnitBehavior>() == null)
            {
                Debug.LogError("Unit prefab on UnitController has no UnitBehavior attached.");
                continue;
            }

            var unitParent = new GameObject();
            unitParent.name = "Unit Pool " + i;
            unitParent.transform.SetParent(transform);

            for (int j = 0; j < UnitPoolCount; j++)
            {
                var unitObj = GameObject.Instantiate(fab) as GameObject;
                var unit = unitObj.GetComponent<UnitBehavior>();
                unit.gameObject.SetActive(false);
                unit.transform.SetParent(unitParent.transform);
                unitPool[i].Add(unit);
            }
        }
    }

	// null static current in preparation for next UnitController
	void OnDestroy()
	{
		current = null;
	}

    private UnitBehavior GetUnitGraphicForFaction(int faction)
    {
        if (faction == 0)
        {
            Debug.LogWarning("Requesting units for faction 0 (i.e. Neutral Faction). Not possible with current design.");
            return null;
        }
        if (faction >= FactionUnitPrefabs.Length)
        {
            Debug.LogError("Unit graphic for faction number '" + faction + "' does not exist.");
            return null;
        }
        var unit = unitPool[faction][currentPullCounts[faction]];
        currentPullCounts[faction]++;
        if (currentPullCounts[faction] >= unitPool[faction].Count)
        {
            currentPullCounts[faction] = 0;
        }
        return unit;
    }

	/// <summary>
	/// Creates the unit with appropriate data and graphics for given faction.
	/// </summary>
	/// <returns>The created unit.</returns>
	/// <param name="faction">Faction to create unit for.</param>
	public static UnitBehavior CreateUnitForFaction(int faction)
	{
		if (faction <= 0 || faction >= current.FactionUnitPrefabs.Length)
		{
            Debug.LogError("Attempting to retrieve unti for non-existant faction: " + faction);
			return null;
		}
        
        var behavior = current.GetUnitGraphicForFaction(faction);
        if (behavior == null)
        {
            Debug.LogWarning("No unit retrieved for faction: " + faction);
            return null; 
        }
        behavior.gameObject.SetActive(true);
		return behavior;
	}

    /// <summary>
    /// Creates a group of units for specified faction. Will send units to a destination if specified.
    /// </summary>
    /// <param name="faction">Faction for units.</param>
    /// <param name="size">Number of units to create.</param>
    /// <returns>Unit Group created.</returns>
    public static UnitGroup CreateUnitGroupForFaction(int faction, int size)
    {
        var group = new UnitGroup(GetUnitGroupID(), faction, size);

        var units = new List<UnitBehavior>();
        for (int i=0; i<size; i++)
        {
            var unit = CreateUnitForFaction(faction);
            unit.SetGroup(group);
            units.Add(unit);
        }

        current.groupUnitsMap.Add(group.ID, units);
        current.groups.Add(group);
        return group;
    }

    public static void AddUnitsToGroup(UnitGroup group, int numUnits)
    {
        var units = current.groupUnitsMap[group.ID];
        group.AddUnits(numUnits);
        for (int i = 0; i < numUnits; i++)
        {
            var unit = CreateUnitForFaction(group.Faction);
            unit.SetGroup(group);
            units.Add(unit);
        }
    }

    public static List<UnitBehavior> GetUnitsForGroup(int id)
    {
        if (current.groupUnitsMap.ContainsKey(id))
        {
            return current.groupUnitsMap[id];
        }
        return null;
    }

    public static int GetUnitGroupID()
    {
        return ++groupCount;
    }

    public static int GetUnitCountForFaction(int faction)
    {
        int unitsFromTowers = TowerController.GetUnitCountFromTowersForFaction(faction);
        int unitsFromGroups = GetFieldUnitCountForFaction(faction);
        return unitsFromGroups + unitsFromTowers;
    }

    public static int GetFieldUnitCountForFaction(int faction)
    {
        int unitsFromGroups = 0;
        var groups = current.Groups;
        foreach (var group in groups)
        {
            if (group.Faction == faction)
            {
                unitsFromGroups += group.UnitCount;
            }
        }
        return unitsFromGroups;
    }

    public static void DestoryAllUnitGroups()
    {
        var groups = current.Groups;
        foreach(var group in groups)
        {
            var units = current.groupUnitsMap[group.ID];
            foreach(var u in units)
            {
                u.ImpactKill();
            }
        }
    }
}
