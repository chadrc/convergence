using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//Class that Controls all AI in the current Level

public class AIController : MonoBehaviour {

    [SerializeField]
    private AIData startingData;

	private List<AIBehavior> AIBehaviors;

   
	private static AIController instance;
    private AIQueue myQueue; 
    private AIAttackDecisionController decisionController;
    private AIAttackController attackController;
    private AIDefendController defendController;
    private AIGameStateManager gameStateManager;

    //make constructor private so no other scripts can make an instance
    private AIController()
    {

    }

	// Use this for initialization
	void Start () 
	{
        if(instance != null)
        {
            Debug.Log("replace AI instance variable");
        }
        instance = this;
		

        gameStateManager = new AIGameStateManager(startingData);
        AIBehaviors = instance.SetupAIList ();
        myQueue = new AIQueue();
        decisionController = new AIAttackDecisionController();
        attackController = new AIAttackController();
        defendController = new AIDefendController();

        //assigned to events
        TowerController.TowerConverted += UpdateAIList;
        TowerController.DeletingInstance += GameStateUnsubscribe;

	}


	private void GenerateAIUnits(AIBehavior attackingAI)
    {
        //find a tower to attack
	    TowerBehavior destination = attackController.FindTowerToSendTroops (attackingAI);

        float percentage;
        AIConstants.ReasonFailed reason;

        //call attack simulation to see if an attack would succeed
        if(decisionController.ShouldAIAttack(attackingAI.myTower,destination, out percentage,out reason))
        {
            //if starting attack wasn't successful
            if(!attackingAI.StartAttack(destination,percentage,1f))
            {
                //enqueue tower
                myQueue.EnqueueTower(attackingAI);
            }
        }

        
        else
        {
            //random chance to not try a power attack
            int randNum = UnityEngine.Random.Range(0, 101);
            if(randNum <= AIGameStateManager.ChanceToQueue)
            {
                myQueue.EnqueueTower(attackingAI);
            }
            
            //see if a tower is waiting for a power attack, power attack can only be done if the reason for a failed attack
            //was because of the number of units
            else if(myQueue.IsTowerWaiting() == true && reason == AIConstants.ReasonFailed.Units)
            {
                //get our second tower
                AIBehavior secondTowerAttacking = myQueue.GetTowerWaiting();
                float queuePerc;
                //if power attack isn't possible, try overload attack
                if(decisionController.CanTwoAttack
                    (destination,attackingAI.myTower,secondTowerAttacking.myTower,percentage,out queuePerc)==false)
                {
                    List<MultiAttackInfo> info = new List<MultiAttackInfo>();
                    info.Add(new MultiAttackInfo(attackingAI, percentage));
                    info.Add(new MultiAttackInfo(secondTowerAttacking, queuePerc));

                    //if we can't overload, just enqueue the towers
                    if (decisionController.CanOverloadAttack(info) == false)
                    {
                        myQueue.EnqueueTower(secondTowerAttacking);
                        myQueue.EnqueueTower(attackingAI);
                    }

                    //if we can overload, try to start the overload attack
                    else
                    {
                        //if we can't start the overload attack for some reason, enqueue the towers
                        if(AIBehavior.StartOverLoadAttack(destination,info) == false)
                        {
                            myQueue.EnqueueTower(secondTowerAttacking);
                            myQueue.EnqueueTower(attackingAI);
                        }

                        //if Overload attack is successfully Happening, then set the timer for the second tower,
                        //the first timer is set after this function returns
                        else
                        {
                            secondTowerAttacking.SetMyTimer(gameStateManager.GetAIAttackTimer());
                        }
                    }


                }
                else
                {
                    List<MultiAttackInfo> info = new List<MultiAttackInfo>();
                    info.Add(new MultiAttackInfo(attackingAI, percentage));
                    info.Add(new MultiAttackInfo(secondTowerAttacking, queuePerc));
                    
                    if(!AIBehavior.StartMultiAttack(destination,info))
                    {
                        myQueue.EnqueueTower(secondTowerAttacking);
                        myQueue.EnqueueTower(attackingAI);
                    }

                    //we are successfully attacking, so set the timer of the second attacking tower
                    else
                    {
                        secondTowerAttacking.SetMyTimer(gameStateManager.GetAIAttackTimer());
                    }

                }
            }

            //if distance,both, or notEnoughToSend
            else 
            {
                myQueue.EnqueueTower(attackingAI);
            }

        }
      
	}

    //UnSubscribe all Ai events
	void OnDestroy()
	{
        TowerController.TowerConverted -= UpdateAIList;
        defendController.Unsubscribe();
        myQueue.ClearQueue();
        if (instance.gameStateManager.HasUnsubscribed == false)
        {
            instance.gameStateManager.Unsubscribe();
            TowerController.DeletingInstance -= GameStateUnsubscribe;
        }
    }
	 
	// Update is called once per frame
    //go through timers on AI
    //might change this to coroutines on AI behavior 
	void Update () 
	{
		foreach(AIBehavior AI in AIBehaviors) 
		{
			//decrements the timer and see if we are able to attack
			bool Attack = AI.DecrementTimer(); 
			if(Attack)
			{
                myQueue.RemoveAIFromQueue(AI);
				instance.GenerateAIUnits(AI);
				AI.SetMyTimer(gameStateManager.GetAIAttackTimer());
			}
		}
	}

	/// <summary>
	/// Setups the AI dictionary at the beginning of a level.
	/// </summary>
	private List<AIBehavior> SetupAIList()
	{
		List<AIBehavior> list = new List<AIBehavior> ();
		List<TowerBehavior> towers = TowerController.GetTowersForFaction (FactionController.OtherFaction1);
  
		foreach (TowerBehavior tower in towers) 
		{
            AIBehavior newAI = new AIBehavior(tower,gameStateManager.GetAIAttackTimer());
			list.Add(newAI);
		}

		return list;
	}  

	/// <summary>
	/// This Function is subscribed to an event. This function will be called when a tower is converted to a new faction.
    /// The function will Check to see we need to make adjustments to the current AI list
	/// </summary>
	private void UpdateAIList(TowerBehavior towerChanged, int oldFaction ,int newFaction)
	{
        if (instance == null)
            return;

        //tower coverted to AI
        if(newFaction == FactionController.OtherFaction1 && oldFaction != FactionController.OtherFaction1)
        {
            AIBehavior newAI = new AIBehavior(towerChanged, gameStateManager.GetAIAttackTimer());
            instance.AIBehaviors.Add(newAI);
        }
        
        //AI tower converted to something else
        else if(oldFaction == FactionController.OtherFaction1 && newFaction != FactionController.OtherFaction1)
        {
            //thanks to C# i have to set removed AI to something or else later in program it will throw an error because
            //it is possible that removedAI won't be set to anything while trying to use it
            AIBehavior removedAI = AIBehaviors[0];
            //bool to make sure that we don't accidentally remove what removedAI is initially set to
            bool found = false;
            foreach (AIBehavior AI in instance.AIBehaviors)
            {
                //make sure the AI we are removing is in the List
                if (AI.myTower == towerChanged)
                {
                    found = true;
                    removedAI = AI;
                    break;
                }
            }


            if(found)
            {
                // we remove AI ouside of the foreach loop because removing something in a list that a foreach loop is currently using can 
                //cause some issues
                instance.AIBehaviors.Remove(removedAI);
                myQueue.RemoveAIFromQueue(removedAI);
            }
            //try to remove AI from the queue
            
        }
	}

    /// <summary>
    /// Returns the current AI towers
    /// </summary>
    public static List<AIBehavior> GetAITowers()
    {
        return instance.AIBehaviors;
    }

    /// <summary>
    /// Gets an AI timer for other scripts to use
    /// </summary>
    public static float GetTimer()
    {
        return instance.gameStateManager.GetAIAttackTimer();
    }

    /// <summary>
    /// Runs the coroutine that is passed to it
    /// </summary>
    public static void CallCoroutine(IEnumerator coroutine)
    {
        //Coroutines can only be started by a monobehavior
        instance.StartCoroutine(instance.PerformCoroutine(coroutine));
    }

    //we call this function to start the coroutine because coroutines and static functions don't play nice apparently
    private IEnumerator PerformCoroutine(IEnumerator coroutine)
    {
        yield return StartCoroutine(coroutine);
    }

    private static void GameStateUnsubscribe()
    {
        if(instance.gameStateManager.HasUnsubscribed == false)
        {
            instance.gameStateManager.Unsubscribe();
            TowerController.DeletingInstance -= GameStateUnsubscribe;
        }
    }
}
/**/