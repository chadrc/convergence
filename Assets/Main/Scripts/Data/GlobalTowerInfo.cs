using UnityEngine;
using System.Collections;

public class GlobalTowerInfo : ScriptableObject
{
    // All Towers
    public int DefaultAttackStrength;
    public float DefaultMovementSpeed;
    public float DefaultUnitKillTime;

    public TowerUpgrade EnduranceUpgrade;
    public TowerUpgrade AtmosphereUpgrade;
    public TowerUpgrade UnitProductionUpgrade;

    public TowerGraphicAsset[] PlayerTowerGraphics;
    public TowerGraphicAsset[] EnemyTowerGraphics;
    public TowerGraphicAsset[] NeutralTowerGraphics;

    void Awake()
    {
        EnduranceUpgrade = new TowerUpgrade(EnduranceUpgrade.Cost, EnduranceUpgrade.Stats, TowerUpgradeType.Defense);
        AtmosphereUpgrade = new TowerUpgrade(AtmosphereUpgrade.Cost, AtmosphereUpgrade.Stats, TowerUpgradeType.Atmosphere);
        UnitProductionUpgrade = new TowerUpgrade(UnitProductionUpgrade.Cost, UnitProductionUpgrade.Stats, TowerUpgradeType.UnitProduction);
    }
}

[System.Serializable]
public class TowerGraphicAsset
{
    public Sprite Graphic;
    public float scaleFactor = 1.0f;
}