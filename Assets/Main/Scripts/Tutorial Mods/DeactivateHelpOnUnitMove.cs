using UnityEngine;
using System.Collections;

public class DeactivateHelpOnUnitMove : MonoBehaviour
{

	// Use this for initialization
	void Awake ()
    {
        TowerBehavior.UnitsMoved += OnUnitsMoved;
	}

	void OnDestroy()
	{
		TowerBehavior.UnitsMoved -= OnUnitsMoved;
	}

    void OnUnitsMoved(MovedUnitsInfo info)
    {
        UIController.DisableHelpUI();
    }
}
