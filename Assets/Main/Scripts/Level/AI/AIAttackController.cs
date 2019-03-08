
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// A class only to be used by the AI controller.
/// This class finds a tower to attack when the AI wants to attack. 
/// </summary>
public class AIAttackController{

    /// <summary>
	///This function is called to find a tower to send the troops
	/// </summary>
	public TowerBehavior FindTowerToSendTroops(AIBehavior attackingAI)
    {
        //gives me a dictionary with numUnits associated with their towers lowest num being at the 0 element
        List<List<TowerBehavior>> UnitList = attackingAI.GetUnitPriority();
        //gives me a dictionary with distance associated with the tower from the origin. smallest distance is the zero element
        List<List<TowerBehavior>> DistanceList = attackingAI.GetDistancePriority();

        //create a final list that holds a tower and it's total score from both dictionaries
        Dictionary<TowerBehavior, int> finalDict = new Dictionary<TowerBehavior, int>();


         //this variable is used for the value that we will give each tower
         //we put it outside the foreach loop so that towers in the same 
         //list are given the same value
         int counter = 0;
        //go through each list in the unit dictionary
         foreach (List<TowerBehavior> list in UnitList)
         {
            //go through each tower through each list
             foreach (TowerBehavior tower in list)
             {
                //add that tower the the final dictionary with its value
                finalDict.Add(tower, counter);
             }
             //increment counter after going through the list
             counter++;
         }

         //reset the counter
         counter = 0;
        //go through each list in the distance dictionary
         foreach (List<TowerBehavior> list in DistanceList)
         {
            //go through each tower in each list
             foreach (TowerBehavior tower in list)
             {
                //if the final dictionary already contains the tower
                 if (finalDict.ContainsKey(tower))
                 {
                    //add the new counter to the final value
                     finalDict[tower] += (counter);
                 }

                 //if not, then something is wrong
                 else
                 {
                     #if Unity_Editor
                     Debug.LogError("DICTIONARY DOESN'T CONTAIN TOWER ALREADY,RETURNING NULL");
                     #endif  
                     return null;
                 }
             }
             //increment the counter
             counter++;
         }

         //send the dictionary to the function so that the tower can actually be chosen
        return FindTowerToAttack(finalDict);
    }

    /// <summary>
    /// This function is used by the function above in order to find the actual tower the AI should attack.
    /// </summary>
    private TowerBehavior FindTowerToAttack(Dictionary<TowerBehavior, int> towerDict)
    {
        //the list of possible towers to attack
        List<TowerBehavior> towersCouldAttack = new List<TowerBehavior>();
        //the value associated with the tower/s that are in the current list
        int finalNum = int.MaxValue;

        //go through each tower in the dictionary
        foreach (KeyValuePair<TowerBehavior, int> pair in towerDict)
        {
            //if that towers final value is lower than the current lowest value
            if (pair.Value < finalNum)
            {
                //set the new final number
                finalNum = pair.Value;
                //clear the list of possible tower
                towersCouldAttack.Clear();
                //add the current tower to the list
                towersCouldAttack.Add(pair.Key);
            }
            
            //if the value of the current tower is equal to the currentLowest value
            else if (pair.Value == finalNum)
            {
                //add the current tower to the list(without clearing it)
                towersCouldAttack.Add(pair.Key);
            }
        }

        //if the list only has 1 tower in it, then
        //just return that tower
        if(towersCouldAttack.Count == 1)
            return towersCouldAttack[0];

        // if there is more than one, choose a random tower in the list
        int rand = Random.Range(0, towersCouldAttack.Count);
        return towersCouldAttack[rand];
    }
}

/* Unused Code 
  //if priority is distance 
                if (AIGameStateManager.Priority == AIConstants.Priority.Distance)
                {
                    //dist1 is for the tower already set to be attacked
                    float dist1 = Vector3.Distance(origin.gameObject.transform.position, towerToAttack.gameObject.transform.position);
                    //dist2 is for the tower we are currently checking
                    float dist2 = Vector3.Distance(origin.gameObject.transform.position, pair.Key.gameObject.transform.position);
                    //if the already set tower is the closer one, go through next iteration
                    if (dist1 <= dist2)
                        continue;
                    //otherwise if the tower we are currently checking is the closer one
                    else
                        towerToAttack = pair.Key;
                }
                //if the priority is numUnits
                else if (AIGameStateManager.Priority == AIConstants.Priority.NumUnits)
                {
                    if (towerToAttack.StationedUnits <= pair.Key.StationedUnits)
                        continue;
                    else
                        towerToAttack = pair.Key;
                }
*/
