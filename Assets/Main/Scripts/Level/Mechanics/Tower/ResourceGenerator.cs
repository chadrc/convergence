using UnityEngine;
using System.Collections;

/// <summary>
/// Generates units for the attached tower
/// </summary>
[RequireComponent (typeof(TowerBehavior))]
public class ResourceGenerator : MonoBehaviour 
{
	private float genTimer = 0;
	private TowerBehavior tower;
    private int genMax = 20;
    public int TotalProduced { get; private set; }
	public float UnitsProducedPerSecond { get; set; }

	// Use this for initialization
	void Start () 
	{
		tower = GetComponent<TowerBehavior>();
		if (tower == null)
		{
			Debug.LogError("There is no TowerBehavior attached alongside ResourceGenerator.");
		}
	}
	
	// Generate units if stationed units is less than genMax
	void Update () 
	{
		if (tower.StationedUnits < genMax)
		{
			genTimer += Time.deltaTime;
			float spawnInterval = 1f / UnitsProducedPerSecond; // Calculate interval. Eventualy will be a constant.
			if (genTimer >= spawnInterval)
			{
				tower.IncStationedUnits();
                TotalProduced++;
				genTimer = 0;
			}
		}
	}
}
