using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerPrefsPlayerData : IPlayerData
{
    private const string DisplayNameKey = "DisplayName";
    private const string MultiplayerWinsKey = "MultiplayerWins";
    private const string MultiplayerDefeatsKey = "MultiplayerDefeats";

    public string name;
    public int multiplayerWins;
    public int multiplayerDefeats;
    public List<LevelStats> levelStatsList = new List<LevelStats>();
    public Dictionary<string, LevelStats> levelTable = new Dictionary<string, LevelStats>();

    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }
    }
    public int MultiplayerWins
    {
        get
        {
            return multiplayerWins;
        }
        set
        {
            multiplayerWins = value;
        }
    }
    public int MultiplayerDefeats
    {
        get
        {
            return multiplayerDefeats;
        }
        set
        {
            multiplayerDefeats = value;
        }
    }
    public List<LevelStats> LevelStatsList
    {
        get
        {
            return levelStatsList;
        }
        set
        {
            levelStatsList = value;
        }
    }

    public PlayerPrefsPlayerData()
    {

    }

    public bool Save()
    {
        var ps = new PlayerStats();
        ps.Name = Name;
        ps.MultiplayerWins = MultiplayerWins;
        ps.MultiplayerDefeats = MultiplayerDefeats;

        var lsl = new LevelStats[levelTable.Count];
        int i = 0;
        foreach(var l in levelTable.Values)
        {
            lsl[i] = l;
            i++;
        }
        ps.LevelStatsList = lsl;

        string dataStr = JsonUtility.ToJson(ps);
        PlayerPrefs.SetString("PlayerStats", dataStr);
        //Debug.Log("Saved Data: " + dataStr);

        //Debug.Log("PlayerData: " + dataStr);
        //PlayerPrefs.SetString(DisplayNameKey, Name);
        //PlayerPrefs.SetInt(MultiplayerWinsKey, MultiplayerWins);
        //PlayerPrefs.SetInt(MultiplayerDefeatsKey, MultiplayerDefeats);
        return true;
    }

    public bool Load()
    {
        string dataStr = PlayerPrefs.GetString("PlayerStats");
        PlayerStats ps;
        if (dataStr == "")
        {
            ps = new PlayerStats();
        }
        else
        {
            ps = JsonUtility.FromJson<PlayerStats>(dataStr);
        }

        Name = ps.Name;
        MultiplayerWins = ps.MultiplayerWins;
        MultiplayerDefeats = ps.MultiplayerDefeats;
        levelTable.Clear();
        if (ps.LevelStatsList == null)
        {
            return true;
        }
        foreach(var l in ps.LevelStatsList)
        {
            levelTable.Add(l.LevelName, l);
        }

        //Name = PlayerPrefs.GetString(DisplayNameKey);
        //MultiplayerWins = PlayerPrefs.GetInt(MultiplayerWinsKey);
        //MultiplayerDefeats = PlayerPrefs.GetInt(MultiplayerDefeatsKey);
        return true;
    }

    public void EditLevel(string levelName, bool victory, float completionTime)
    {
        LevelStats ls = null;
        if (levelTable.ContainsKey(levelName))
        {
            ls = levelTable[levelName];
        }
        else
        {
            ls = new LevelStats();
            ls.LevelName = levelName;
            levelTable.Add(levelName, ls);
        }

        if (victory)
        {
            ls.Victories++;
        }
        else
        {
            ls.Defeats++;
        }

        if (completionTime < ls.CompletionTime)
        {
            ls.CompletionTime = completionTime;
        }
    }
}