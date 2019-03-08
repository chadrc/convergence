using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// This script is used to decide if an AI should attack its destination after a tower has been chosen
/// </summary>
public class AIAttackDecisionController  {

    //This is the function the AI calls to see if it should attack the chosen tower
    public bool ShouldAIAttack(TowerBehavior AI, TowerBehavior destination, out float percentage, 
        out AIConstants.ReasonFailed reason)
    {
        float chanceToAttackAnyway = AIGameStateManager.PercentToAttackAnyway;

        //setting our out variables(must be garunteed set before the function returns)
        reason = AIConstants.ReasonFailed.None;
        percentage = 0;

        if(AI.StationedUnits <= 0)
        {
            //percentage is already set to zero
            reason = AIConstants.ReasonFailed.NotEnoughtoSend;
            return false;
        }

        //random chance to attack anyway
        int randomNum = Random.Range(0, 101);
        if (randomNum <= chanceToAttackAnyway)
        {
            percentage = AIConstants.RoundPercentToClosestOption(((float)AI.StationedUnits / (float)AI.StationedUnits));
            return true;
        }

        else
        {

            bool cantravel = SimulateAttackUtil.DistanceCheck(AI, destination);
            bool enoughtUnits = HaveEnoughSoldiers(AI, destination, ref percentage);
            //if distance is ever a factor, than the we don't need to adjust the percentage 
            if (enoughtUnits == false && cantravel == false)
            {
                reason = AIConstants.ReasonFailed.Both;
                return false;
            }
            else if (enoughtUnits == false)
            {
                reason = AIConstants.ReasonFailed.Units;
                return false;
            }

            else if (cantravel == false)
            {
                reason = AIConstants.ReasonFailed.Distance;
                return false;
            }

        }

       
        return true;
    }

    private bool HaveEnoughSoldiers(TowerBehavior AI, TowerBehavior destination,ref float percent)
    {
        
        //if the Ai doesn't have more units than the destination in general
        //then we can't take over the tower so there is no need to do more calculations
        if(AI.StationedUnits <= destination.StationedUnits)
        {
            percent = AIConstants.RoundPercentToClosestOption(((float)AI.StationedUnits / (float)AI.StationedUnits));
            return false;
        }

        int AIunits = AI.StationedUnits;

        //how many Units are left after the attack
        int AIunitsLeft;

        //did the "attack" succeed?
        if(SimulateAttackUtil.SimulateAttack(AIunits,destination,AI.Faction,out AIunitsLeft) == false)
        {
            percent = AIConstants.RoundPercentToClosestOption((float)AIunits / (float)AI.StationedUnits);
            return false;
        }

    
        AIunits -= AIunitsLeft;
        AIunits += 1;
        percent = AIConstants.RoundPercentToClosestOption((float)AIunits / (float)AI.StationedUnits);
        return true;
    }

   
    ///two towers attack stuff
    public bool CanTwoAttack(TowerBehavior destination, TowerBehavior attackingTower, TowerBehavior queueTower,
        float attackPercentage, out float queuePercent)
    {
        queuePercent = 0;
        if (queueTower.StationedUnits <= 0)
        {
            return false;
        }


        int unitsLeft;

        int numOriginalAttacking = Mathf.CeilToInt(attackingTower.StationedUnits * attackPercentage);
        int maxAdditionAttackers = queueTower.StationedUnits;
        if (SimulateAttackUtil.SimulateAttack((numOriginalAttacking + maxAdditionAttackers),
            destination, attackingTower.Faction, out unitsLeft) == false)
        {
            //incase for overload attack
            maxAdditionAttackers -= unitsLeft - 1;
            queuePercent = AIConstants.RoundPercentToClosestOption((float)maxAdditionAttackers / (float)queueTower.StationedUnits);
            return false;
        }

        maxAdditionAttackers -= unitsLeft - 1;
        queuePercent = AIConstants.RoundPercentToClosestOption((float)maxAdditionAttackers / (float)queueTower.StationedUnits);

        if (SimulateAttackUtil.DistanceCheck(queueTower, destination) == false)
        {
            
            return false;
        }

        
        return true;
    }

    public bool CanOverloadAttack(List<MultiAttackInfo> info)
    {
        //at this point it only equals 2
       
        //can the first tower reach the second tower
        if (SimulateAttackUtil.DistanceCheck(info[0].AI.myTower,info[1].AI.myTower) == false)
        {
           return false;
        }

        return true;
    }
}
