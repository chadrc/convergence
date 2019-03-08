using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//Class to hold info for different factions
//was made in preparation for more Factions, but the team decided against that
public class FactionGameStateInfo
{
    public readonly int factionNumber;
    public int numTowers;
    public int numUpgrades;

    public FactionGameStateInfo(int _factionNumber, int _numTowers)
    {
        factionNumber = _factionNumber;
        numTowers = 0;
        numUpgrades = 0;
    }
}

public class AIGameStateManager
{ 
    // track number of convergences
    private static int numConvergences;

    // faction state info for both AI and player
    private static FactionGameStateInfo AIGameStateInfo;
    private static FactionGameStateInfo playerGameStateInfo;

    //bool to let Ai controller know if the game State manager has 
    //this was used to fix and error, 
    //Both AI controller and the game's Tower Controller use Unity's On Destroy called back as a Destructor,
    //but on some levels the order is different, thus causing an error
    private bool hasUnsubsribed = false;
   
    public bool HasUnsubscribed
    {
        get
        {
            return hasUnsubsribed;
        }
    }   

    //file with all our our variables to change game state
    private static AIData currentData;

    #region Public Fields/Properties.

    public static float MaxNumberAICanPowerAttack
    {
        get
        {
            return currentData.maxNumberAICanPowerAttack;
        }
    }


    public static AIConstants.Difficulty Difficulty
    {
        get
        {
            return currentData.difficulty;
        }
    }

    //change this so that the number changes
    public static float maxNumberAICanDefend
    {
        get
        { 
            return currentData.maxNumberAICanDefend;
        }
    }

    public static float ChanceToQueue
    {
        get
        {
            float chance = currentData.aiPercentContainer.chancetoQueue;
            //chance to queue a tower, not trying a power attack after failure
            //increasing chance increases chance that AI won't power attack
            chance += GameStatePercentDifference * (AIGameStateInfo.numTowers - playerGameStateInfo.numTowers);
            chance += GameStatePercentDifference * (UnitController.GetUnitCountForFaction(FactionController.OtherFaction1) - UnitController.GetUnitCountForFaction(FactionController.PlayerFaction));

            chance -= GameStatePercentDifference * numConvergences;
            chance -= GameStatePercentDifference * (int)Difficulty;

   
            chance += ((UnityEngine.Random.Range(0, 2) == 1) ? 1 : -1) * UnityEngine.Random.Range(MinimumRandPercent, MaximumRandPercent);


            chance = Mathf.Clamp(chance, 0, 100);
            return chance;
        }
    }

    public static float PercentToAttackAnyway
    {
        get
        {
            float chance = currentData.aiPercentContainer.PercentToAttackAnyway; 

            //chance to attack anyway

            chance += GameStatePercentDifference * (AIGameStateInfo.numTowers - playerGameStateInfo.numTowers);
            chance += GameStatePercentDifference * (UnitController.GetUnitCountForFaction(FactionController.OtherFaction1) - UnitController.GetUnitCountForFaction(FactionController.PlayerFaction));

            chance -= GameStatePercentDifference * numConvergences;
            chance -= GameStatePercentDifference * (int)Difficulty;

            chance += ((UnityEngine.Random.Range(0, 2) == 1) ? 1 : -1) * UnityEngine.Random.Range(MinimumRandPercent, MaximumRandPercent);

            chance = Mathf.Clamp(chance, 0, 100);

           
            return chance;

        }
    }

    public static float PercentChanceAIWontDefend
    {
        get
        {
            float chance = currentData.aiPercentContainer.percentChanceAIWontDefend;

            chance += GameStatePercentDifference * (AIGameStateInfo.numTowers - playerGameStateInfo.numTowers);
            chance += GameStatePercentDifference * (UnitController.GetUnitCountForFaction(FactionController.OtherFaction1) - UnitController.GetUnitCountForFaction(FactionController.PlayerFaction));

            chance -= GameStatePercentDifference * numConvergences;
            chance -= GameStatePercentDifference * (int)Difficulty;

            chance += ((UnityEngine.Random.Range(0, 2) == 1) ? 1 : -1) * UnityEngine.Random.Range(MinimumRandPercent, MaximumRandPercent);

            chance = Mathf.Clamp(chance, 0, 100);

            return chance;
        }
    }
    #endregion
    //all fields here are used for readability
    #region Private Fields/Properties
    private static float DifficultyTimeDeduction
    {
        get
        {
            return currentData.aiTimeContainer.difficultyTimeDeduction;
        }
    }

    private static float ConvergenceTimeDeduction
    {
        get
        {
            return currentData.aiTimeContainer.convergenceTimeDeduction;
        }
    }

    private static float UnitDifferenceTime
    {
        get
        {
            return currentData.aiTimeContainer.unitDifferenceTime;
        }
    }

    private static float TowerDifferencTime
    {
        get
        {
            return currentData.aiTimeContainer.towerDifferenceTime;
        }
    }

    private static float GameStatePercentDifference
    {
        get
        {
            return currentData.aiPercentContainer.gameStatePercentDifference;
        }
    }

    private static float AbsoluteMinimumTimer
    {
        get
        {
            return currentData.aiTimeContainer.absoluteMinimumTimer;
        }
    }

    private static float AbsoluteMaximumTimer
    {
        get
        {
            return currentData.aiTimeContainer.absoluteMaximumTimer;
        }
    }

    private static float StartingMinimumTimer
    {
        get
        {
            return currentData.aiTimeContainer.startingMaximumTimer;
        }
    }

    private static float StartingMaximmTimer
    {
        get
        {
            return currentData.aiTimeContainer.startingMaximumTimer;
        }
    }

    private static float MinimumRandPercent
    {
        get
        {
            return currentData.aiPercentContainer.minimumRandomPercent;
        }
    }


    private static float MaximumRandPercent
    {
        get
        {
            return currentData.aiPercentContainer.maximumRandomPercent;
        }
    }

    private static float UpgradeDifferenceTime
    {
        get
        {
            return currentData.aiTimeContainer.upgradeDifferenceTime;
        }
    }

    #endregion
    //constructor
    public AIGameStateManager( AIData startingData)
    {
        //make the Game State Info Contrainers
        AIGameStateInfo = new FactionGameStateInfo(FactionController.OtherFaction1, TowerController.GetTowerCountForFaction(FactionController.OtherFaction1));
        playerGameStateInfo = new FactionGameStateInfo(FactionController.PlayerFaction, TowerController.GetTowerCountForFaction(FactionController.PlayerFaction));
        //Subscribed to necessary events
        TowerController.TowerConverted += TowerHasChangedFaction;
        ConvergenceController.ConvergenceOccurred += OnConvergence;
        List<TowerBehavior> towers = TowerController.GetAllTowers();
        //each individual tower has an upgrade event(not static so we need to subscribe to all of them)
        foreach (TowerBehavior tower in towers)
        {
            tower.Upgraded += TowerUpgraded;
        }
        numConvergences = 0;
        currentData = startingData;
    }


    private void TowerHasChangedFaction(TowerBehavior tower, int oldFaction, int newFaction)
    {
        //Add 1 to the faction that gained a tower
        if (newFaction == FactionController.PlayerFaction)
        {
            playerGameStateInfo.numTowers++;
        }
        else if (newFaction == FactionController.OtherFaction1)
        {
            AIGameStateInfo.numTowers++;
        }

        //subtract 1 from the faction that lost a tower
        if (oldFaction == FactionController.PlayerFaction)
        {
            playerGameStateInfo.numTowers--;
            if(tower.IsUpgraded)
            {
                playerGameStateInfo.numUpgrades--;
                AIGameStateInfo.numUpgrades++;
            }
        }
        else if (oldFaction == FactionController.OtherFaction1)
        {
            AIGameStateInfo.numTowers--;
            if(tower.IsUpgraded)
            {
                AIGameStateInfo.numUpgrades--;
                playerGameStateInfo.numUpgrades++;
            }
        } 
    }

    private void TowerUpgraded(TowerBehavior tower, TowerUpgrade upgrade)
    {
        //Add 1 to the faction that gained a tower
        if (tower.Faction == FactionController.PlayerFaction) { playerGameStateInfo.numUpgrades++; }
        else if (tower.Faction == FactionController.OtherFaction1) { AIGameStateInfo.numUpgrades++; }
    }

    public void Unsubscribe()
    {
        TowerController.TowerConverted -= TowerHasChangedFaction;
        ConvergenceController.ConvergenceOccurred -= OnConvergence;
        //setup for upgrading towers
        List<TowerBehavior> towers = TowerController.GetAllTowers();
        foreach (TowerBehavior tower in towers)
        {
            tower.Upgraded -= TowerUpgraded;
        }

        hasUnsubsribed = true;
    }

    private void OnConvergence()
    {
        numConvergences += 1;
    }

    public float GetAIAttackTimer()
    {

        float timer = UnityEngine.Random.Range(StartingMinimumTimer, StartingMaximmTimer);

        timer -= (int)Difficulty * DifficultyTimeDeduction;

        timer -= numConvergences * ConvergenceTimeDeduction;

        timer += (UnitController.GetUnitCountForFaction(FactionController.OtherFaction1) - UnitController.GetUnitCountForFaction(FactionController.PlayerFaction)) * UnitDifferenceTime;

        timer += (AIGameStateInfo.numTowers - playerGameStateInfo.numTowers) * TowerDifferencTime;

        timer += (AIGameStateInfo.numUpgrades - playerGameStateInfo.numUpgrades) * UpgradeDifferenceTime;

        timer = Mathf.Clamp(timer, AbsoluteMinimumTimer, AbsoluteMaximumTimer);

        return timer;
    }


}
