using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//Class that takes car of most AI Behavior
//Wrapper Class that has a reference to its curret tower
//this way we don't have to worry about having each instance of a tower having its own AI script
//and turning that script on and off
public class AIBehavior {

    //reference to tower
    public TowerBehavior myTower;
    //timer to attack
    private float Timer;
 
    //reference to current coroutine
    private IEnumerator myCoroutine;

    //constructor
    public AIBehavior(TowerBehavior tower, float timer)
    {
        myTower = tower;
        Timer = timer;
    }
    /// <summary>
    /// Decrements the timer for tower to attack
    /// Should be replaced with a coroutine
    /// </summary>
    public bool DecrementTimer()
    {
        Timer -= Time.deltaTime;
     
        if (Timer <= 0)
        {
            return true;
        }
        return false;
    }

    //variable to set timer
    public void SetMyTimer(float time)
    {
        Timer = time;
    }

    //allow other scripts to access the distance and unit priority
    //not to sure why I programm this, this way, should have just made the other functions public
    public List<List<TowerBehavior>> GetDistancePriority() { return UpdateDistancePrioritys(); }
    public List<List<TowerBehavior>> GetUnitPriority() { return UpdateUnitPrioritys(); }


    private List<List<TowerBehavior>> UpdateDistancePrioritys()
    {
        //list of towers we can attack
        List<TowerBehavior> towersToAttack = TowerController.GetTowersNotOfFaction(myTower.Faction);
        
        //dictonary to associate a list of towers with a distance
        Dictionary<int, List<TowerBehavior>> dictionary = new Dictionary<int, List<TowerBehavior>>();


        foreach (TowerBehavior tower in towersToAttack)
        {
            //get the distance between the AI tower and a tower we can attack
            float fDistance = Vector3.Distance(tower.gameObject.transform.position, myTower.gameObject.transform.position);
            //we want distance as an integer, distance as an int allows for the randomness in AI Attack Controller
            //see if we need to round up or down
            int distance = 0;
            if(fDistance - Mathf.Floor(fDistance) >=0.5f)
            {
                distance = Mathf.CeilToInt(fDistance);
            }
            else
            {
                distance = Mathf.FloorToInt(fDistance);
            }


            //if distance isn't a key in the dictionary already, make a new list, add the tower to the list and add them to the 
            //dictionary
            if (!dictionary.ContainsKey(distance))
            {
                List<TowerBehavior> newList = new List<TowerBehavior>();
                newList.Add(tower);
                dictionary.Add(distance, newList);
            }
            //if it is then add the tower to the distance's list
            else
            {
                dictionary[distance].Add(tower);
            }
        }


        return AISortingMethods.InsertionSortDistance(this, dictionary);
    }

    //pretty much the same as the function above
    private List<List<TowerBehavior>> UpdateUnitPrioritys()
    {
       
        List<TowerBehavior> towersToAttack = TowerController.GetTowersNotOfFaction(myTower.Faction);
        Dictionary<int, List<TowerBehavior>> dictionary = new Dictionary<int, List<TowerBehavior>>();
        foreach (TowerBehavior tower in towersToAttack)
        {
            if (!dictionary.ContainsKey(tower.StationedUnits))
            {
                List<TowerBehavior> newList = new List<TowerBehavior>();
                newList.Add(tower);
                dictionary.Add(tower.StationedUnits, newList);
               
            }
            else
            {
                dictionary[tower.StationedUnits].Add(tower);
            }
        }
        
        return AISortingMethods.InsertionSortUnits(this,dictionary);
    }

    //function that can call for the current AI to start its attack
    public bool StartAttack(TowerBehavior destination, float percentage,float time)
    {
        if (percentage <= 0)
            return false;

        UnitGroup currentGroup = UnitController.CreateUnitGroupForFaction(myTower.Faction, (int)(myTower.StationedUnits * percentage));
        currentGroup.PrepareUnits(myTower);
        myCoroutine = this.WaittoAttack(currentGroup,destination,time);
        AIController.CallCoroutine(myCoroutine);
       
        
        return true;
    }

    //function that is called for a tower that is going to attack, but is waiting on an AI to send units to it
    public IEnumerator StartOverloadAttack(TowerBehavior destination, float percent, float time)
    {
        yield return new WaitForSeconds(time);
        UnitGroup currentGroup = UnitController.CreateUnitGroupForFaction(myTower.Faction, (int)(myTower.StationedUnits * percent));
        currentGroup.PrepareUnits(myTower);
        myCoroutine = this.WaittoAttack(currentGroup, destination, 1f);
        AIController.CallCoroutine(myCoroutine);
    }

    //function called off the class itself to start a multi attack(two towers attacking the same tower at the same time)
    public static bool StartMultiAttack(TowerBehavior destination, List<MultiAttackInfo> Info)
    {
        //go through to see if any of the percents are zero, if they are, then don't have anyone attack
        foreach(MultiAttackInfo info in Info)
        {
            if (info.Percent <= 0)
            {
                return false;
            }
        }

        //start attacking for each one
        foreach(MultiAttackInfo info in Info)
        {
            info.AI.StartAttack(destination, info.Percent,1f);
        }

        return true;
    }
    
    //function called of the class to start a multi Attack
    //on a personal note, this was done quickly for PAX 
    //after pax i will be changing the syntax for this
    public static bool StartOverLoadAttack(TowerBehavior destination, List<MultiAttackInfo> Info)
    {
    
        //saftey Check, this shouldn't go off, but just incase
        foreach (MultiAttackInfo info in Info)
        {
            if (info.Percent <= 0)
            {
                
                return false;
            }
        }

        //if the original Attacking tower (1) is closer
        if (Vector3.Distance(Info[1].AI.myTower.transform.position, destination.transform.position) <=
            Vector3.Distance(Info[0].AI.myTower.transform.position, destination.transform.position))
        {
            //send the units from 0 to 1
            Info[0].AI.StartAttack(Info[1].AI.myTower, Info[0].Percent, 1f);

            //determine time for the attacking tower to wait for
            float distance = Vector3.Distance(Info[0].AI.myTower.transform.position, Info[1].AI.myTower.transform.position);
            float speed = FactionController.GetSpeedForFaction(FactionController.OtherFaction1);
            float time = (distance / speed) + 1.1f; //1.01f to wait for other towers timer

            //have 1 wait for 0
            IEnumerator coroutine = Info[1].AI.StartOverloadAttack(destination, Info[1].Percent, time);

            AIController.CallCoroutine(coroutine);
        }

        //if the second tower is closer
        else
        {
            //send the units from the original attacker(1) to the new one (0)
            Info[1].AI.StartAttack(Info[0].AI.myTower, Info[1].Percent, 1f);

            //determine time for the attacking tower to wait for
            float distance = Vector3.Distance(Info[1].AI.myTower.transform.position, Info[0].AI.myTower.transform.position);
            float speed = FactionController.GetSpeedForFaction(FactionController.OtherFaction1);
            float time = (distance / speed) + 1.1f; //1.01f to wait for other towers timer

            //Have 0 wait for 1
            IEnumerator coroutine = Info[0].AI.StartOverloadAttack(destination, Info[0].Percent, time);

            AIController.CallCoroutine(coroutine);
        }

        return true;
    }

    //enumator to have ai Behavior wait for the units to occur on scene
    private IEnumerator WaittoAttack(UnitGroup currentGroup,TowerBehavior destination, float time)
    {
        yield return new WaitForSeconds(time);
        TowerBehavior.MoveGroupFromTo(currentGroup, myTower, destination);

    }

}

