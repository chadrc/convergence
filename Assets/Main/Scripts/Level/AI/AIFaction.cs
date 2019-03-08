using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Not in use 
//was made in anticipation for more factions
public class AIFaction {

        public List<AIBehavior> towersOfSameFaction;
        private int myFactionNumber;
        public int MyFactionNumber
        {
            get
            {
                return myFactionNumber;
            }
        }

        public AIFaction()
        {

        }
        public void AddToMyTowers(TowerBehavior tower)
        {
           /* AIBehavior newAI = new AIBehavior(tower, AIController.SetTimer());
			towersOfSameFaction.Add(newAI);*/
        }
    
}
