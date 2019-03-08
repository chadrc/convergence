using UnityEngine;
using System.Collections;

/// <summary>
/// Simple class to hold tower upgrade information.
/// </summary>
[System.Serializable]
public class TowerUpgrade
{
    // Serialized for customization in editor
    [SerializeField]
    private int cost;
    [SerializeField]
    private TowerStats stats;

    private TowerUpgradeType type;

    public int Cost { get { return cost; } }
    public TowerStats Stats { get { return stats; } }
    public TowerUpgradeType Type { get { return type; } }

    public TowerUpgrade(int cost, TowerStats stats, TowerUpgradeType type)
    {
        this.cost = cost;
        this.stats = stats;
        this.type = type;
    }
}

/// <summary>
/// Enum to give names to upgrade. Mainly for Networking and Analytics
/// </summary>
public enum TowerUpgradeType : byte
{
    UnitProduction = 0,
    Defense,
    Atmosphere
}