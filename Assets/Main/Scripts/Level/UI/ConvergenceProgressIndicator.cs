using UnityEngine;
using System.Collections;

public class ConvergenceProgressIndicator : MonoBehaviour 
{
	public Transform centerObj;

	// Update is called once per frame
	void Update () 
	{
		transform.RotateAround(centerObj.position, -Vector3.forward,  (360 / ConvergenceController.CurrentInterval) * Time.deltaTime);
	}
}
