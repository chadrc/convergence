using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kills any unit within a radius (defined in tower info of Game), at a set interval
/// </summary>
[RequireComponent (typeof(SphereCollider))]
public class UnitAttacker : MonoBehaviour 
{
	TowerBehavior tower;
	LinkedList<UnitBehavior> visibleUnits = new LinkedList<UnitBehavior>();
	float timer;

	// Use this for initialization
	void Start () 
	{
		tower = GetComponent<TowerBehavior>();
		if (tower == null)
		{
			Debug.LogError("There is no TowerBehavior attached alongside UnitAttacker.");
			return;
		}
		var sphere = GetComponent<SphereCollider>();
		sphere.isTrigger = true;
//		sphere.radius = Game.GetGuardTowerAttackRadiusForLevel(tower.Level);

		tower.AttackedByUnit += OnUnitAttackTower;
		tower.ChangedFaction += OnTowerChangedFaction;
	}

	void OnDestroy()
	{
		tower.AttackedByUnit -= OnUnitAttackTower;
		tower.ChangedFaction -= OnTowerChangedFaction;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (timer >= 3.0f) //Game.GetGuardTowerAttackSpeedForLevel(tower.Level) && visibleUnits.Count > 0)
		{
			visibleUnits.First.Value.Kill();
			visibleUnits.RemoveFirst();
			timer = 0;
		}
		else
		{
			timer += Time.deltaTime;
		}
	}

	// Add unit to list for attacking if unit isn't apart of tower's faction
	void OnTriggerEnter(Collider col)
	{
		var unit = col.GetComponent<UnitBehavior>();
		if (unit != null && unit.Faction != tower.Faction)
		{
			visibleUnits.AddLast(unit);
		}
	}
	
	// Remove unit to list for attacking if unit isn't apart of tower's faction
	void OnTriggerExit(Collider col)
	{
		var unit = col.GetComponent<UnitBehavior>();
		if (unit != null && unit.Faction != tower.Faction)
		{
			visibleUnits.Remove(unit);
		}
	}
	
	// Remove unit to list for attacking if unit isn't apart of tower's faction
	void OnUnitAttackTower(UnitBehavior unit)
	{
		if (unit.Faction != tower.Faction)
		{
			visibleUnits.Remove(unit);
		}
	}

	// Clear list to start finding different faction
	void OnTowerChangedFaction(int faction)
	{
		visibleUnits.Clear();
	}
}
