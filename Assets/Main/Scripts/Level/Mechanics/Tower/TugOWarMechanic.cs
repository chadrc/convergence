using UnityEngine;
using System.Collections;

/// <summary>
/// Makes attached gameObject move to a specified point based on which faction controls the attached tower
/// </summary>
[RequireComponent (typeof(TowerBehavior))]
[RequireComponent (typeof(Rigidbody))]
public class TugOWarMechanic : MonoBehaviour 
{
	public float MovementSpeed;
	public Transform[] FactionDestPoints;

	private TowerBehavior tower;
	new private Rigidbody rigidbody;

	// Use this for initialization
	void Start () 
	{
		tower = GetComponent<TowerBehavior> ();
		rigidbody = GetComponent<Rigidbody> ();

		UIController.MarkTowerAsDynamic (tower);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (tower.Faction == -1)
			return;
		
		int index = tower.Faction;
		if (tower.StationedUnits == 0)
		{
			index = 0;
		}

		var t = FactionDestPoints [index];
		var direction = t.position - transform.position;
		if (direction.magnitude <= 0.001f) 
		{
			rigidbody.velocity = Vector3.zero;
		}
		else
		{
			rigidbody.velocity = direction.normalized * MovementSpeed;
		}
	}
}
