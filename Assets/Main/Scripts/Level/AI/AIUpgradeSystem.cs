using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIUpgradeSystem{

    private List<AIBehavior> currentAI;

    private void Initialize()
    {
        currentAI = AIController.GetAITowers();

    }
    
    private void OnTowerChange(AIBehavior towerChanged, bool isAdding = true)
    {
        if(isAdding == true)
        {
            currentAI.Add(towerChanged);
        }

        else if(isAdding == false)
        {
            currentAI.Remove(towerChanged);
        }
    }
}
