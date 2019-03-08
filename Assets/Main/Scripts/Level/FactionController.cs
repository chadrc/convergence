using UnityEngine;
using System.Collections;

/// <summary>
/// Faction controller to hold total number of factions
/// </summary>
public class FactionController : MonoBehaviour 
{
	public static int NeutralFaction { get; private set; }
	public static int PlayerFaction { get; private set; }
    public static int OtherFaction1 { get; private set; }

    private static FactionController current;

	public int NumberOfFactions = 3;

	public static int FactionCount {
		get { return current.NumberOfFactions; }
	}

    static FactionController()
    {
        SetDefaultFactions();
    }

    public static void SetDefaultFactions()
    {
        NeutralFaction = 0;
        PlayerFaction = 1;
        OtherFaction1 = 2;
    }

    public static void SetNetworkClientFactions()
    {
        PlayerFaction = 2;
        OtherFaction1 = 1;
    }

    // Raise warning if more than one UIController exists.
    void Awake () 
	{
		if (current != null)
		{
			Debug.LogWarning("Multiple UIControllers created. Replacing current.");
		}
		current = this;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
	
	// null static current in preparation for next UnitController
	void OnDestroy()
	{
		current = null;
	}

	/// <summary>
	/// Gets the attack strength for faction, adding the amount from towers and the default amount;
	/// </summary>
	/// <returns>The attack strength for faction.</returns>
	/// <param name="faction">Faction to query.</param>
	public static int GetAttackStrengthForFaction(int faction)
	{
		return TowerController.GetAttackBoostFromTowersForFaction(faction) + Game.GetDefaultAttackStrength();
	}

	/// <summary>
	/// Gets the speed for faction, adding the amount from towers and the default amount;
	/// </summary>
	/// <returns>The attack strength for faction.</returns>
	/// <param name="faction">Faction to query.</param>
	public static float GetSpeedForFaction(int faction)
	{
		return TowerController.GetSpeedBoostFromTowersForFaction(faction) + Game.GetDefaultUnitSpeed();
	}
}
