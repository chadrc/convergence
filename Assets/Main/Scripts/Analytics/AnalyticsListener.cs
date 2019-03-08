using UnityEngine;
using System.Collections;

public class AnalyticsListener : MonoBehaviour
{
    private float[] unitPercentTimes = new float[4];
    private int currentUnitPercentChoice = 0;
    private int enduranceUpgradeCount = 0;
    private int productionUpgradeCount = 0;
    private int unitPercentChangeCount = 0;
    private bool convergenceDefeat = false;
    private bool levelQuit = false;
    private int unitsDestroyed = 0;

	// Use this for initialization
	void Awake ()
    {
        LevelController.LevelStart += OnLevelStart;
        LevelController.LevelEnd += OnLevelEnd;
        UIController.UnitPercentChanged += OnUnitPercentChanged;
        UIController.LevelQuit += OnLevelQuit;
        TowerButtonBehavior.EnduranceUpgradeUsed += IncEnduranceCount;
        TowerButtonBehavior.ProductionUpgradeUsed += IncProductionCount;
        ConvergenceCountDefeatCondition.ConvergenceDefeat += OnConvergenceDefeat;
        UnitBehavior.UnitKilled += IncUnitDestroyed;
	}

    void Update()
    {
        unitPercentTimes[currentUnitPercentChoice] += Time.deltaTime;
    }

    void OnDestroy()
    {
        LevelController.LevelStart -= OnLevelStart;
        LevelController.LevelEnd -= OnLevelEnd;
        UIController.UnitPercentChanged -= OnUnitPercentChanged;
        TowerButtonBehavior.EnduranceUpgradeUsed -= IncEnduranceCount;
        TowerButtonBehavior.ProductionUpgradeUsed -= IncProductionCount;
    }

    private void IncEnduranceCount()
    {
        enduranceUpgradeCount++;
    }

    private void IncProductionCount()
    {
        productionUpgradeCount++;
    }

    private void IncUnitDestroyed(UnitBehavior unit)
    {
        unitsDestroyed++;
    }
    
    private void OnUnitPercentChanged(float unitPercent)
    {
        unitPercentChangeCount++;

        if (unitPercent == .25f)
        {
            currentUnitPercentChoice = 0;
        }
        else if (unitPercent == .5f)
        {
            currentUnitPercentChoice = 1;
        }
        else if (unitPercent == .75f)
        {
            currentUnitPercentChoice = 2;
        }
        else if (unitPercent == 1.0f)
        {
            currentUnitPercentChoice = 3;
        }
    }

    private void OnLevelStart()
    {
        if (Game.CurrentCampaign == null || Game.CurrentLevel == null)
        {
            return;
        }

        var lvlStartData = new CampaignLevelStartData();
        lvlStartData.campaignName = Game.CurrentCampaign.Name;
        lvlStartData.levelName = Game.CurrentLevel.Name;
        Analytics.SendCampaignLevelStart(lvlStartData);
    }

    private void OnConvergenceDefeat()
    {
        convergenceDefeat = true;
    }

    private void OnLevelQuit()
    {
        levelQuit = true;
    }
    
    private void OnLevelEnd(bool obj)
    {
        var playerChoiceData = new PlayerChoiceData();
        playerChoiceData.enduranceUpgradeCount = enduranceUpgradeCount;
        playerChoiceData.productionUpgradeCount = productionUpgradeCount;
        playerChoiceData.unitPercentChanged = unitPercentChangeCount;
        playerChoiceData.timeFor25Unit = unitPercentTimes[0];
        playerChoiceData.timeFor50Unit = unitPercentTimes[1];
        playerChoiceData.timeFor75Unit = unitPercentTimes[2];
        playerChoiceData.timeFor100Unit = unitPercentTimes[3];

        Analytics.SendPlayerChoiceData(playerChoiceData);

        var lvlEndData = new LevelEndData();

        lvlEndData.playTime = LevelController.PlayTime;

        int totalUnitProduced = 0;
        var towers = TowerController.GetAllTowers();
        foreach(var t in towers)
        {
            if (t.Faction == FactionController.PlayerFaction)
            {
                lvlEndData.playerControlledPlanets++;
            }
            else if (t.Faction == FactionController.NeutralFaction)
            {
                lvlEndData.neutralPlanets++;
            }
            else if (t.Faction == FactionController.OtherFaction1)
            {
                lvlEndData.opponentControlledPlanets++;
            }

            totalUnitProduced += t.Generator.TotalProduced;
        }

        if (obj)
        {
            lvlEndData.statusCode = LevelStatusCode.Victory;
        }
        else
        {
            lvlEndData.statusCode = LevelStatusCode.Defeat;
        }

        if (convergenceDefeat)
        {
            lvlEndData.statusCode = LevelStatusCode.Tie;
        }

        if (levelQuit)
        {
            lvlEndData.statusCode = LevelStatusCode.Surrender;
        }
        
        lvlEndData.unitsCreated = totalUnitProduced;
        lvlEndData.unitsDestroyed = unitsDestroyed;

        Analytics.SendLevelEnd(lvlEndData);
    }
}
