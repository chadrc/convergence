using UnityEngine;
using System.Collections;

public static class Game
{
    public static event System.Action<LevelData> SelectedLevelChanged;
    public static event System.Action<LevelList> LevelListChanged;

    private static LevelList currentList;
    private static GlobalTowerInfo towerInfo;
    private static int currentLevelIndex = -1;

    public static GlobalTowerInfo TowerInfo { get { return towerInfo; } }
    public static LevelList CurrentCampaign { get { return currentList; } }
    public static LevelData CurrentLevel { get { return IndexInCurrentList(currentLevelIndex) ? currentList.Levels[currentLevelIndex] : null; } }
	public static bool Initialized { get { return towerInfo != null; } }
    
    public static MonoBehaviour EternalBehaviour { get; set; }

	public static void Initialize(GameInitializationBehavior behavior)
	{
		if (towerInfo == null)
		{
			towerInfo = behavior.TowerInfo;
		}
	}

    public static bool HasLevelList()
    {
        return currentList != null;
    }

    public static bool IndexInCurrentList(int index)
    {
        return (index >= 0 && index < currentList.Levels.Count);
    }

    public static bool HasNextLevelInList()
    {
        return IndexInCurrentList(currentLevelIndex + 1);
    }

    public static void SetCurrentLevelToNextInList()
    {
        if (HasNextLevelInList())
        {
            currentLevelIndex++;
        }
    }

    public static void SetLevelList(LevelList list)
    {
        if (currentList != list)
        {
            if (LevelListChanged != null)
            {
                LevelListChanged(list);
            }
            currentList = list;
        }
    }

	public static void SetSelectedLevel(int index)
	{
        if (currentList == null || index < 0 || index >= currentList.Levels.Count)
        {
            currentLevelIndex = -1;
            return;
        }

        currentLevelIndex = index;
		if (SelectedLevelChanged != null)
		{
			SelectedLevelChanged (currentList.Levels[index]);
		}
	}

	public static bool HasSelectedLevel()
	{
        return currentLevelIndex != -1;
	}

    // Getter funcitons to access towerInfo without the ability to change it

    public static int GetDefaultAttackStrength()
    {
        return towerInfo.DefaultAttackStrength;
    }

    public static float GetDefaultUnitSpeed ()
	{
		return towerInfo.DefaultMovementSpeed;
	}
}

/// <summary>
/// Denotes a towers function on the field. Sub-classing TowerBehavior wasn't used because the subclasses wouldn't have had any custom behavior or data.
/// </summary>
public enum TowerType
{
	Resource = 0,
	Guard,
	AttackBoost,
	SpeedBoost
}

/// <summary>
/// Used to group together tower prefabs in the inspector.
/// </summary>
[System.Serializable]
public class FactionTowerFabGroup
{
	public GameObject ResourceTower;
	public GameObject GuardTower;
	public GameObject AttackBoostTower;
	public GameObject SpeedBoostTower;
}