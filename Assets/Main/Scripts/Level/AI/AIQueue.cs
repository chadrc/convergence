using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this list handles all towers that tried to attack but have failed
//this keeps a queue of towers waiting to attack

//This script is very simple right now.
public class AIQueue {

    private Queue<AIBehavior> AIOnStandby;

    public AIQueue ()
    {
        AIOnStandby = new Queue<AIBehavior>();
    }

    public void EnqueueTower(AIBehavior towerToEnqueue)
    {
        AIOnStandby.Enqueue(towerToEnqueue);
    }

    public bool IsTowerWaiting()
    {
        return (AIOnStandby.Count > 0);
    }

    public AIBehavior GetTowerWaiting()
    {
        return AIOnStandby.Dequeue();
    }
    
    //if an AI is waiting in the queue and its time is up, take him away from the queue
    public void RemoveAIFromQueue(AIBehavior AIToRemove)
    {
      
        if(AIOnStandby.Contains(AIToRemove))
        {
           
            if (AIOnStandby.Peek() != AIToRemove)
            {
                Queue<AIBehavior> newQueue = new Queue<AIBehavior>();
                foreach (AIBehavior AI in AIOnStandby)
                {
                    if (AI != AIToRemove)
                    {
                        newQueue.Enqueue(AI);
                    }
                }
                AIOnStandby.Clear();
                AIOnStandby = newQueue;
            }
            else
            {
                AIOnStandby.Dequeue();
            }
        }
    }

    public void ClearQueue()
    {
        AIOnStandby.Clear();
    }
}
