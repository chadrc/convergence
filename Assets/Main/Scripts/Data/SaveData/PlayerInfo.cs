using UnityEngine;
using System.Collections.Generic;

public static class PlayerInfo 
{
	private static IPlayerData data;

	public static string PlayerName { get { return data.Name; } set { data.Name = value; } }
	public static int MultiplayerWins { get { return data.MultiplayerWins; }  set { data.MultiplayerWins = value; } }
	public static int MultiplayerDefeats { get { return data.MultiplayerDefeats; } set { data.MultiplayerDefeats = value; } }

	static PlayerInfo()
	{
		
	}

	public static void LoadPlayerDataFromPlayerPrefs()
	{
		data = new PlayerPrefsPlayerData();
		data.Load();
	}

	public static void SavePlayerData()
	{
		data.Save();
	}

    public static void EditLevel(string levelName, bool victory, float completionTime)
    {
        data.EditLevel(levelName, victory, completionTime);
    }
}

public interface IPlayerData
{
	string Name { get; set; }
	int MultiplayerWins { get; set; }
	int MultiplayerDefeats { get; set; }
    List<LevelStats> LevelStatsList { get; set; }

	bool Save();
	bool Load();
    void EditLevel(string levelName, bool victory, float completionTime);
}

[System.Serializable]
public class PlayerStats
{
    public string Name;
    public int MultiplayerWins;
    public int MultiplayerDefeats;
    public LevelStats[] LevelStatsList;
}