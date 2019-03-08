using UnityEngine;
using System.Collections;

[System.Serializable]
public class LevelStats
{
    public string LevelName;
    public float CompletionTime = float.MaxValue;
    public int Victories;
    public int Defeats;

    public int Attempts { get { return Victories + Defeats; } }

    public LevelStats()
    {

    }
}
