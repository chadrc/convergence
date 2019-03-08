using UnityEngine;
using System.Collections;

[System.Serializable]
public class TowerStats
{
    public float AtmosphereRange;
    public float UnitsGeneratedPerSecond;
    public int Endurance;

    public TowerStats()
    {
    }

    public static TowerStats operator +(TowerStats stats1, TowerStats stats2)
    {
        var t = new TowerStats();
        t.AtmosphereRange = stats1.AtmosphereRange + stats2.AtmosphereRange;
        t.UnitsGeneratedPerSecond = stats1.UnitsGeneratedPerSecond + stats2.UnitsGeneratedPerSecond;
        t.Endurance = stats1.Endurance + stats2.Endurance;
        return t;
    }
}
