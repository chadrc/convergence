using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Tower behavior.
/// </summary>
[RequireComponent(typeof(ResourceGenerator))]
public class TowerBehavior : MonoBehaviour
{
    public static event Action<MovedUnitsInfo> UnitsMoved;

    public event Action<UnitBehavior> AttackedByUnit;
    public event Action<int> ChangedFaction;
    public event Action<TowerBehavior, TowerUpgrade> Upgraded;

    public int Index;
    public int Faction;
    public int StartingUnits;
    [Range(0, 3)]
    public int GraphicNumber;
    private float GraphicScale = 4.5f;
    public GameObject GraphicObj;
    public SpriteRenderer GraphicSprite { get { return GraphicObj.GetComponent<SpriteRenderer>(); } }

	public GameObject RangeGraphicObj { get; set; }

    [SerializeField]
    private TowerStats Stats;

    private UnitGroup stationedGroup;
    //private int stationedUnits;
    private SphereCollider atmosphereCollider;

    // Part of temporary scale fix
    private Vector3 initialScale;
    public float InitialGraphicScale { get; private set; }
    
    public TowerStats CurrentStats
    {
        get
        {
            return Stats;
        }

        set
        {
            Stats = value;
        }
    }
    public TowerStats DefaultStats { get; private set; }
    public bool IsUpgraded { get; private set; }
    public ResourceGenerator Generator { get; private set; }
    public OrbitMotion Orbit { get; private set; }
    public UnitGroup StationedGroup
    {
        get { return stationedGroup; }
    }
	public int StationedUnits
	{
		get { return stationedGroup.UnitCount; }
	}
	public float AtmosphereRange
	{
		get { return atmosphereCollider.radius; }
		set { atmosphereCollider.radius = value; }
	}

    // Unity Callbacks

    void Awake()
    {
        if(!enabled)
        {
            return;
        }

        // Temporary fixes for actual graphics being smaller than placeholder graphics
        InitialGraphicScale = GraphicObj.transform.localScale.x;
        SetGraphic();
        initialScale = new Vector3(InitialGraphicScale * GraphicScale, InitialGraphicScale * GraphicScale, 1);
        GraphicObj.transform.localScale = initialScale;
    }

	void Start () 
	{
		TowerController.InitializeTower(this);

        HideAtmosphere();
        stationedGroup = new UnitGroup(UnitController.GetUnitGroupID(), Faction, StartingUnits);
		atmosphereCollider = GetComponent<SphereCollider> ();
        if (atmosphereCollider == null)
		{
			atmosphereCollider = gameObject.AddComponent<SphereCollider> ();
		}
		atmosphereCollider.isTrigger = true;
        AtmosphereRange = 3.0f;

        Generator = GetComponent<ResourceGenerator> ();
		Orbit = GetComponent<OrbitMotion> ();

        DefaultStats = CurrentStats;
        SetStats();

        SetGraphic();
	}

    void Update ()
	{
	}

    //public bool DrawGizmos = false;
    void OnDrawGizmos()
    {
        //if (!DrawGizmos)
        //    return;

        Gizmos.color = Color.red;
        int resolution = 100;
        float angleStep = (360.0f / resolution) * Mathf.Deg2Rad;
        float x = Stats.AtmosphereRange * Mathf.Cos(0);
        float y = transform.position.y;
        float z = Stats.AtmosphereRange * Mathf.Sin(0);
        var startPos = new Vector3(x, y, z) + transform.position;
        //Gizmos.DrawSphere(startPos, .5f);
        var lastPos = startPos;
        for (int i = 1; i < resolution; i++)
        {
            float angle = i * angleStep;
            x = Stats.AtmosphereRange * Mathf.Cos(angle);
            y = transform.position.y;
            z = Stats.AtmosphereRange * Mathf.Sin(angle);
            var nextPos = new Vector3(x, y, z) + transform.position;
            Gizmos.DrawLine(lastPos, nextPos);
            lastPos = nextPos;
        }

        Gizmos.DrawLine(lastPos, startPos);
        var clr = Color.red;
        clr.a = .5f;
        Gizmos.color = clr;
        Gizmos.DrawSphere(transform.position, 2.0f);
    }

    // End Unity callbacks

    public void SetGraphic()
    {
        Sprite graphic = GraphicSprite.sprite;
        if (Faction == 1)
        {
            graphic = Game.TowerInfo.PlayerTowerGraphics[GraphicNumber].Graphic;
            GraphicScale = Game.TowerInfo.PlayerTowerGraphics[GraphicNumber].scaleFactor;
        }
        else if (Faction == 2)
        {
            graphic = Game.TowerInfo.EnemyTowerGraphics[GraphicNumber].Graphic;
            GraphicScale = Game.TowerInfo.EnemyTowerGraphics[GraphicNumber].scaleFactor;
        }
        else if (Faction == 0)
        {
            graphic = Game.TowerInfo.NeutralTowerGraphics[GraphicNumber].Graphic;
            GraphicScale = Game.TowerInfo.NeutralTowerGraphics[GraphicNumber].scaleFactor;
        }
        GraphicSprite.sprite = graphic;
        var scale = new Vector3(InitialGraphicScale * GraphicScale, InitialGraphicScale * GraphicScale, 1);
        GraphicObj.transform.localScale = scale;
    }

    // Sets all variables linked to stats data.
    public void SetStats()
    {
        stationedGroup.Endurance = Stats.Endurance;
        //AtmosphereRange = Stats.AtmosphereRange;
        AtmosphereRange = 3.0f;
        if (Faction == 0)
        {
            Generator.UnitsProducedPerSecond = 0;
        }
        else
        {
            Generator.UnitsProducedPerSecond = Stats.UnitsGeneratedPerSecond;
        }
    }

    public void ReplaceDefaultStats(TowerStats stats)
    {
        DefaultStats = stats;
    }

    public void DisableUnitGeneration()
    {
        Generator.UnitsProducedPerSecond = 0;
        Generator.enabled = false;
    }

    public void SetUnits(int count)
    {
        StationedGroup.UnitCount = count;
    }

    /// <summary>
    /// Makes atmosphere graphic visible.
    /// </summary>
    public void ShowAtmosphere()
    {
        RangeGraphicObj.SetActive(true);
    }

    /// <summary>
    /// Makes atmosphere graphic invisible
    /// </summary>
    public void HideAtmosphere()
    {
        RangeGraphicObj.SetActive(false);
    }

    /// <summary>
    /// Increases this towers unit count by one.
    /// </summary>
    public void IncStationedUnits()
    {
        stationedGroup.AddUnit();
    }

	/// <summary>
	/// Replaces the visual representation of the tower. Used for upgrading, faction switching, and tower swapping.
	/// </summary>
	/// <returns>The graphic that was replaced.</returns>
	/// <param name="graphic">The new graphic to use.</param>
	public GameObject ReplaceGraphic (GameObject graphic)
	{
		var old = GraphicObj;
		GraphicObj = graphic;
		GraphicObj.transform.SetParent(transform);
		GraphicObj.transform.localPosition = old.transform.localPosition;
        GraphicObj.transform.localScale = initialScale;
		return old;
	}

    /// <summary>
    /// Replaces the range graphic for the tower. Handles all resizeing and positioning.
    /// </summary>
    /// <param name="rangeGraphic">New graphic to use.</param>
    /// <returns>Old range graphic.</returns>
    public GameObject ReplaceRangeGraphic(GameObject rangeGraphic)
    {
        var old = RangeGraphicObj;
        RangeGraphicObj = rangeGraphic;
        RangeGraphicObj.transform.SetParent(transform);
        RangeGraphicObj.transform.localPosition = Vector3.zero;
        float rangeScale = 3.0f * .88f;
        RangeGraphicObj.transform.localScale = new Vector3(rangeScale, rangeScale, 1.0f);
        RangeGraphicObj.SetActive(false);
        return old;
    }

    /// <summary>
    /// Upgrades tower by adding the stats in given upgrade to current stats.
    /// </summary>
    /// <param name="upgrade">Upgrade to consider.</param>
    /// <returns>False if tower has already been upgraded or tower doesn't have enough units to upgrade with.</returns>
    public bool UpgradeBy(TowerUpgrade upgrade)
    {
        return UpgradeTo(new TowerUpgrade(upgrade.Cost, CurrentStats + upgrade.Stats, upgrade.Type));
    }

    /// <summary>
    /// Upgrades tower by setting its stats to the upgrades stats.
    /// </summary>
    /// <param name="upgrade">Upgrade to consider.</param>
    /// <returns>False if tower has already been upgraded or tower doesn't have enough units to upgrade with.</returns>
    public bool UpgradeTo(TowerUpgrade upgrade)
    {
        if (IsUpgraded || StationedUnits < upgrade.Cost)
        {
            return false;
        }
        CurrentStats = upgrade.Stats;
        StationedGroup.SubtractUnits(upgrade.Cost);
        IsUpgraded = true;
        SetStats();
        if (Upgraded != null)
        {
            Upgraded(this, upgrade);
        }
        return true;
    }

    /// <summary>
    /// Removes upgrade by setting towers stats to default and setting upgraded flag to false.
    /// </summary>
    public void RemoveUpgrade()
    {
        CurrentStats = DefaultStats;
        IsUpgraded = false;
    }

	/// <summary>
	/// Called when a unit is to enter/attack this tower. 
	/// See documenation for full explaination and metrics.
	/// If unit is friendly, stationed units is increased.
	/// Raises ChangedFaction and AttackedByUnit events.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void UnitEntered(UnitBehavior unit)
	{
		if (unit.Faction == Faction)
		{
            // Friendly units are transfered to this towers group
            unit.TransferGroup(stationedGroup);
            unit.gameObject.SetActive(false);
		}
		else
		{
            // Hostile units damage this towers stationed unit group
            int strength = FactionController.GetAttackStrengthForFaction(unit.Faction);
            stationedGroup.Damage(strength);

            // Change tower's faction if last unit was killed
            if (stationedGroup.Empty)
            {
                TowerController.ConvertTowerToFaction(this, unit.Faction);
                if (ChangedFaction != null)
                {
                    ChangedFaction(Faction);
                    SetGraphic();
                }
            }

            unit.ImpactKill();

            if (AttackedByUnit != null)
            {
                AttackedByUnit(unit);
            }
		}
	}

    // Replaced MoveUnits
    /// <summary>
    /// Sets a unit group's unit's to move toward a destination tower from an origin tower.
    /// </summary>
    /// <param name="group">Unit group to move.</param>
    /// <param name="origin">Tower were group should move from.</param>
    /// <param name="destination">Tower were group will move toward.</param>
    public static void MoveGroupFromTo(UnitGroup group, TowerBehavior origin, TowerBehavior destination)
    {
        origin.stationedGroup.SubtractUnits(group.UnitCount);
        group.MoveUnits(destination);
        var info = new MovedUnitsInfo(origin, destination, group.UnitCount);
        if (UnitsMoved != null)
        {
            UnitsMoved(info);
        }
    }
}

/// <summary>
/// Data class for holding info from when units are moved between towers.
/// </summary>
public class MovedUnitsInfo
{
    public readonly TowerBehavior From;
    public readonly TowerBehavior To;
    public readonly int NumberOfUnits;
    public readonly int FromFaction;
    public readonly int ToFaction;

    public MovedUnitsInfo(TowerBehavior from, TowerBehavior to, int numberOfUnits)
    {
        From = from;
        To = to;
        FromFaction = from.Faction;
        ToFaction = to.Faction;
        NumberOfUnits = numberOfUnits;
    }
}