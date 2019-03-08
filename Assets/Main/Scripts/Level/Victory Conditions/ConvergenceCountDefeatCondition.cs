using UnityEngine;
using System.Collections;

public class ConvergenceCountDefeatCondition : MonoBehaviour 
{
    public static event System.Action ConvergenceDefeat;

	public int NumConvergencesTillDefeat;
	private int count;
	// Use this for initialization
	void Awake () 
	{
		if (NumConvergencesTillDefeat <= 0)
		{
			gameObject.SetActive (false);
			Debug.LogWarning ("Convergence Count Defeat Condition: Number of convergences till defeat is zero or less. Disabling condition.");
			return;
		}
		ConvergenceController.ConvergenceOccurred += OnConvergence;
		UIController.EnableConvergenceCountUI (NumConvergencesTillDefeat);
	}

	void OnDestroy()
	{
		ConvergenceController.ConvergenceOccurred -= OnConvergence;
	}

	void OnConvergence()
	{
		count++;
		if (count >= NumConvergencesTillDefeat)
		{
            if (ConvergenceDefeat != null)
            {
                ConvergenceDefeat();
            }
			LevelController.EndLevel (false);
		}
	}
}
