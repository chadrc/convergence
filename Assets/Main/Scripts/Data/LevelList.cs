using UnityEngine;
using System.Collections.Generic;

// Also representation of a campagin
public class LevelList : ScriptableObject
{
    public string Name;
    public string Description;
	public List<LevelData> Levels = new List<LevelData>();
}
