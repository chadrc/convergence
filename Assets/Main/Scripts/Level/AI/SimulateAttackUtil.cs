using UnityEngine;
using System.Collections;

//A static class so it an instance of the class can't be made
//this script is used to simulate attacks
//Does Distance Checks
//Sees in the current Attack will succeed
//and how many units are need to take down a tower

public static class SimulateAttackUtil {

    /// <summary>
    /// Returns true if the from's units can reach the to tower
    /// </summary>
	public static bool DistanceCheck(TowerBehavior from, TowerBehavior to)
    {
        float timeAlive = Game.TowerInfo.DefaultUnitKillTime;
        Vector3 destinationFinalPos;

        if (to.GetComponent<OrbitMotion>() != null)
        {
            destinationFinalPos = to.Orbit.CalculatePositionWithMoreUpTime(timeAlive);
        }

        else
        {
            destinationFinalPos = to.transform.position;
        }

        float distance = (to.transform.position - from.transform.position).magnitude;

        float maxDistanceTravel = timeAlive * FactionController.GetSpeedForFaction(from.Faction);

        if (maxDistanceTravel < distance)
            return false;

        return true;
    }

    /// <summary>
    /// Returns True if the attacking Units will take over a tower
    /// </summary>
    public static bool SimulateAttack(int numUnitsAttacking, TowerBehavior destination, int AttackingFaction, out int UnitsLeft)
    { 
        
        UnitsLeft = numUnitsAttacking;
        //how many stationUnits does the destination have 
        int defendingUnits = destination.StationedUnits;
        //what is the current endurance of the destination tower
        int currentEndurance = destination.StationedGroup.Endurance;
        //strength of soldiers
        int strength = FactionController.GetAttackStrengthForFaction(AttackingFaction);

        //Simulate attacking
        for (int i = 0; i < numUnitsAttacking; i++)
        {
            currentEndurance -= strength;
            UnitsLeft--;
            if (currentEndurance <= 0)
            {
                currentEndurance += destination.CurrentStats.Endurance;
                defendingUnits--;
            }

            //if the player is out of units stop the loop
            if (defendingUnits <= 0)
                break;
        }

        
        //if the there are defending Units leftover, we have failed the attack
        if (defendingUnits > 0)
        {
            return false;
        }

        //we have succeeded the attack in defending Units is 0 or less
        return true;
    }

    /// <summary>
    /// Returns the Number of Units Needed to successfully defend a tower
    /// </summary>
    public static int NumUnitsNeededToDefend(TowerBehavior defendingTower, int attackingFaction, int numAttackingUnits)
    {
        int strength = FactionController.GetAttackStrengthForFaction(attackingFaction);
        int unitsNeeded = 0;
        int endurance = defendingTower.StationedGroup.Endurance;

        for(int i = 0; i < numAttackingUnits; i++)
        {
            endurance -= strength;
            if(endurance <=0)
            {
                unitsNeeded++;
                endurance += defendingTower.CurrentStats.Endurance;
            }
        }

        return unitsNeeded + 1;
    }

}
