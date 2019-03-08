using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A class only to be used by the AI Controller
/// This class takes care of defending the AI whenever the enemy attacks
/// </summary>
public class AIDefendController
{
    // max number of Ai that can defend
    //this number might change 
    private const int maxNumberTowersCanDefend = 2;

    //constructor to subscribe to an event
    public AIDefendController()
    {
        TowerBehavior.UnitsMoved += TowerHasAttack;
    }

    //function is called when the UnitsMoved event is fired
    private void TowerHasAttack(MovedUnitsInfo attackInfo)
    {
        //check to see if the tower being attacked is the AI and the attacking tower is the player
        if (attackInfo.FromFaction != FactionController.OtherFaction1 &&
            attackInfo.ToFaction == FactionController.OtherFaction1)
        {

            //Check if the enemy units can even reach this tower, 
            if (SimulateAttackUtil.DistanceCheck(attackInfo.From, attackInfo.To) == false)
                return;

            //Check if the enemy units can even take over the tower
            int EnemyUnitsLeft;
            if (SimulateAttackUtil.SimulateAttack(attackInfo.NumberOfUnits, attackInfo.To, attackInfo.FromFaction, out EnemyUnitsLeft) == false)
                return;

            //random check to see if the Ai will defend it 
            float rand = Random.Range(0f, 100f);
            if (rand <= AIGameStateManager.PercentChanceAIWontDefend)
                return;
            //if we make it through the check, then Defend
            Defend(attackInfo, EnemyUnitsLeft);

        }

    }

    private void Defend(MovedUnitsInfo info, int enemyUnitsLeft)
    {

        
        //if none of the towers are able to make the distance to the AI. 
        List<AIBehavior> friendlyTowers = closestFriendlyTowers(info.To);
        //Check if we even have towers to use to defend
        if (friendlyTowers == null || friendlyTowers.Count == 0)
            return;

        Dictionary<AIBehavior, int> helpDefending = new Dictionary<AIBehavior, int>();
        int totalDefendingUnits = info.To.StationedUnits;
        int numUnitsNeededToDefend = SimulateAttackUtil.NumUnitsNeededToDefend(info.To, info.FromFaction, info.NumberOfUnits);

        for(int i = 0; i < (maxNumberTowersCanDefend <= friendlyTowers.Count ? maxNumberTowersCanDefend : friendlyTowers.Count); i++)
        {
            int additionalUnits = friendlyTowers[0].myTower.StationedUnits;
            //even though this has some extra steps for if condition is equal
            //but figured it would be better than writing out another if statement
            if(totalDefendingUnits + additionalUnits >= numUnitsNeededToDefend )
            {
                additionalUnits -= ((totalDefendingUnits + additionalUnits) - numUnitsNeededToDefend);
                helpDefending.Add(friendlyTowers[i], additionalUnits);
                totalDefendingUnits += additionalUnits;
                break;
            }
            else
            {
                helpDefending.Add(friendlyTowers[i], additionalUnits);
                totalDefendingUnits += additionalUnits;      
            }
        }

        if(totalDefendingUnits >= numUnitsNeededToDefend && helpDefending.Count > 0)
        {  
            foreach(KeyValuePair<AIBehavior,int> pair in helpDefending)
            {
                pair.Key.SetMyTimer(AIController.GetTimer());
                //find the percent of Soldiers we are attacking 
                float percent = AIConstants.RoundPercentToClosestOption((float)pair.Value / (float)pair.Key.myTower.StationedUnits);
               
                //we don't really need to do anything with the bool from this, it will defend as intended or nothing will happen
                //we don't need to do anything if the start attack fails
                pair.Key.StartAttack(info.To, percent,1f);
            }
        }
    }

    private List<AIBehavior> closestFriendlyTowers(TowerBehavior defendingAI)
    {
        //Get the list of current AI Towers
        List<AIBehavior> AI = AIController.GetAITowers();

        //Hashtable for all of the towers
        SortedDictionary<float, List<AIBehavior>> closestTowers = new SortedDictionary<float, List<AIBehavior>>();
        foreach (AIBehavior ai in AI)
        {
            if (ai.myTower == defendingAI)
                continue;

            float distance = Vector3.Distance(defendingAI.transform.position, ai.myTower.transform.position);
            if (SimulateAttackUtil.DistanceCheck(defendingAI, ai.myTower) == true)
            {
                if (!closestTowers.ContainsKey(distance))
                {
                    List<AIBehavior> newList = new List<AIBehavior>();
                    newList.Add(ai);
                    closestTowers.Add(distance, newList);
                }
                else
                {
                    closestTowers[distance].Add(ai);
                }
            }
        }


        List<AIBehavior> towersToSend = new List<AIBehavior>();
        foreach (KeyValuePair<float, List<AIBehavior>> pair in closestTowers)
        {
            foreach (AIBehavior ai in pair.Value)
            {
                towersToSend.Add(ai);
            }
        }

        return towersToSend;
    }

    public void Unsubscribe()
    {
        TowerBehavior.UnitsMoved -= TowerHasAttack;
    }
}
