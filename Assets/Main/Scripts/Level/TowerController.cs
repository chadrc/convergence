using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// The tower controller for a level. Manages tower initialization, conversion, validation, and data querying.
/// </summary>
public class TowerController : MonoBehaviour
{
    public static event System.Action<TowerBehavior, int, int> TowerConverted;
    public static int TowerCount { get { return current.towers.Count; } }

    public static event System.Action DeletingInstance;

	private static TowerController current;

	public GameObject TowerFab;

    public GameObject[] FactionTowerGraphics;
    public GameObject[] FactionTowerRangeGraphics;
	//public FactionTowerFabGroup[] FactionTowers; 

	private List<TowerBehavior> towers = new List<TowerBehavior>();
    
	// Raise warning if more than one TowerController exists.
	void Awake () 
	{
		if (current != null)
		{
			Debug.LogWarning("Multiple TowerControllers created. Replacing current.");
		}
		current = this;
	}

	// null static current in preparation for next TowerController
	void OnDestroy()
	{
        if (DeletingInstance != null)
           DeletingInstance();

		current = null;
	}

	/// <summary>
	/// Initializes a tower, notifying and setting up any objects that need to know about towers
	/// </summary>
	/// <param name="tower">Tower to be initialized.</param>
	public static void InitializeTower(TowerBehavior tower)
	{
		current.towers.Add(tower);
        ValidateTowerFactionGraphic(tower);
        UIController.CreateTowerButtonForTower(tower);
    }

	/// <summary>
	/// Converts the tower to faction.
	/// </summary>
	/// <param name="tower">Tower.</param>
	/// <param name="faction">Faction.</param>
	public static void ConvertTowerToFaction(TowerBehavior tower, int faction)
	{
        // Do not convert if tower's faction is same as desired faction
		if (tower.Faction == faction) return;

        // Convert tower
        int oldFaction = tower.Faction;
        tower.Faction = faction;

        // Reset and validate graphics
        tower.SetStats();
        ValidateTowerFactionGraphic(tower);

        // Raise event
        if (TowerConverted != null)
        {
            TowerConverted(tower, oldFaction, faction);
        }

		MapControl.TowerControlChangeEventTrigger();
	}

	/// <summary>
	/// Creates appropriate graphic for a towers faction.
	/// </summary>
	/// <param name="tower">Tower to validate.</param>
	private static void ValidateTowerFactionGraphic(TowerBehavior tower)
	{
        //var graphic = CreateTowerGraphicForFaction(tower.Faction);
        var rangeGraphic = CreateTowerRangeGraphicForFaction(tower.Faction);
        tower.SetGraphic();
        //if (graphic != null)
        //{
        //    var oldGraphic = tower.ReplaceGraphic(graphic);
        //    if (oldGraphic != null)
        //    {
        //        oldGraphic.SetActive(false);
        //    }
        //    //GameObject.Destroy(oldGraphic);
        //}

        if (rangeGraphic != null)
        {
            var oldRangeGraphic = tower.ReplaceRangeGraphic(rangeGraphic);
            if (oldRangeGraphic != null)
            {
                oldRangeGraphic.SetActive(false);
            }
            //GameObject.Destroy(oldRangeGraphic);
        }
    }

	/// <summary>
	/// Creates the tower graphic based on given faction. Basic literal value conparison, to be changed when factions are properly implemented.
	/// </summary>
	/// <returns>The tower graphic for faction.</returns>
	/// <param name="faction">Faction to create graphic for.</param>
	private static GameObject CreateTowerGraphicForFaction(int faction)
	{
		if (faction < 0 || faction >= FactionController.FactionCount)
		{
			return null;
		}
		
		GameObject fab = current.FactionTowerGraphics[faction];
		var obj = GameObject.Instantiate(fab) as GameObject;
		return obj;
	}

    private static GameObject CreateTowerRangeGraphicForFaction(int faction)
    {
        if (faction < 0 || faction >= FactionController.FactionCount)
        {
            return null;
        }

        GameObject fab = current.FactionTowerRangeGraphics[faction];
        var obj = GameObject.Instantiate(fab) as GameObject;
        return obj;
    }

	/// <summary>
	/// Gets the number of units that a faction has in all of their towers.
	/// </summary>
	/// <returns>Unit count.</returns>
	/// <param name="faction">Faction to query.</param>
	public static int GetUnitCountFromTowersForFaction(int faction)
	{
		int count = 0;
		foreach(var t in current.towers)
		{
			if (t.Faction == faction)
			{
				count += t.StationedUnits;
			}
		}
		return count;
	}

	/// <summary>
	/// Gets the number of towers that a faction has.
	/// </summary>
	/// <returns>The tower count for faction.</returns>
	/// <param name="faction">Faction to query.</param>
	public static int GetTowerCountForFaction(int faction)
	{
		int count = 0;
		foreach(var t in current.towers)
		{
			if (t.Faction == faction)
			{
				count++;
			}
		}
		return count;
	}

	/// <summary>
	/// Gets the cummulative attack boost that a faction has in all of their towers.
	/// </summary>
	/// <returns>Total attack strength.</returns>
	/// <param name="faction">Faction to query.</param>
	public static int GetAttackBoostFromTowersForFaction(int faction)
	{
		int count = 0;
        //foreach (var t in current.towers)
        //{
        //    if (t.Faction == faction && t.Type == TowerType.AttackBoost)
        //    {
        //        count += Game.GetAttackBoostForTowerLevel(t.Level);
        //    }
        //}
        return count;
	}

	/// <summary>
	/// Gets the cummulative speed boost that a faction has in all of their towers.
	/// </summary>
	/// <returns>Total speed boost.</returns>
	/// <param name="faction">Faction to query.</param>
	public static float GetSpeedBoostFromTowersForFaction(int faction)
	{
		float count = 0;
        //foreach (var t in current.towers)
        //{
        //    if (t.Faction == faction && t.Type == TowerType.SpeedBoost)
        //    {
        //        count += Game.GetSpeedBoostForTowerLevel(t.Level);
        //    }
        //}
        return count;
	}

	public static List<TowerBehavior> GetTowersForFaction(int faction)
	{
		List<TowerBehavior> towersToSend = new List<TowerBehavior> ();
		foreach (TowerBehavior tower in current.towers) 
		{
			if(tower.Faction == faction)
				towersToSend.Add(tower);
		}

		return towersToSend;
	}

	public static List<TowerBehavior> GetAllTowers() 
	{
		return current.towers;
	}

    public static List<TowerBehavior>GetTowersNotOfFaction(int faction)
    {
        List<TowerBehavior> towersToSend = new List<TowerBehavior>();
        foreach(TowerBehavior tower in current.towers)
        {
            if (tower.Faction != faction)
                towersToSend.Add(tower);

        }

        return towersToSend;
    }

    public static List<UnitGroup> GetAllTowerUnitGroups()
    {
        var groups = new List<UnitGroup>();
        foreach (var t in current.towers)
        {
            groups.Add(t.StationedGroup);
        }
        return groups;
    }
}
