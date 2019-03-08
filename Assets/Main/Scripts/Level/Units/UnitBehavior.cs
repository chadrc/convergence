using UnityEngine;
using System.Collections;

/// <summary>
/// A Unit behavior. Controls a units movement, pathing and initialization.
/// </summary>
[RequireComponent (typeof(Rigidbody))]
public class UnitBehavior : MonoBehaviour 
{
    public static System.Action<UnitBehavior> UnitPrepared;
    public static System.Action<UnitBehavior> UnitKilled;

    public GameObject GraphicObject;
    public ParticleSystem DeathSystem;
    public ParticleSystem OrbitSystem;

    private int faction;
    private TowerBehavior origin;
    private TowerBehavior destination;
    private Rigidbody body;

	private int orbitCounter;
    private float deathTimer;
    private bool attacking;

    //private UnitMovementMotor moveMotor;
    //private UnitPrepMotor prepMotor;
    //private IMotor curMotor;

    public UnitGroup Group { get; private set; }
	public int Faction { get { return faction; } }
    public Rigidbody Rigidbody { get { return body; } }

    private SphereCollider collider;
    private Coroutine movementRoutine;

	// Use this for initialization
	void Awake () 
	{
		if (GraphicObject == null)
		{
			GraphicObject = gameObject;
		}
		faction = -1;
		body = GetComponent<Rigidbody>();
        collider = GetComponent<SphereCollider>();
  //      prepMotor = new UnitPrepMotor(this);
		//moveMotor = new UnitMovementMotor(this);
		//curMotor = prepMotor;
	}
	
	// Moves unit toward destination tower. When close enough unit quickly closes in on tower position.
	void Update () 
	{
		if (faction == -1) return;

        //curMotor.Drive();

        if (attacking)
        {
            deathTimer += Time.deltaTime;
            if (deathTimer >= Game.TowerInfo.DefaultUnitKillTime)
            {
                Kill();
            }
        }
        float scale = 1.0f - Mathf.Lerp(0, 1.0f, deathTimer / Game.TowerInfo.DefaultUnitKillTime);
        GraphicObject.transform.localScale = new Vector3(scale, scale, scale);
    }

    void FixedUpdate()
    {
        //curMotor.FixedDrive();
    }

    /// <summary>
    /// Sets a unit to move from origin to destination. Places unit within a radius around origin before begining movement.
    /// </summary>
    /// <param name="origin">Origin tower.</param>
    /// <param name="destination">Destination tower.</param>
    public void MoveFromTo(TowerBehavior origin, TowerBehavior destination)
    {
        //faction = this.origin.Faction;
        //curMotor = moveMotor;
    }

    /// <summary>
    /// Prepares unit for moving to a destination tower.
    /// </summary>
    /// <param name="origin"></param>
    public void Prepare(TowerBehavior origin)
    {
        this.origin = origin;
        attacking = false;
        gameObject.SetActive(true);
        GraphicObject.SetActive(true);
        GraphicObject.transform.localScale = Vector3.one;
        collider.enabled = true;
        orbitCounter = 1;
        //prepMotor.SetOrigin(origin);
        //curMotor = prepMotor;
        StartRoutine(Prep());
    }

    /// <summary>
    /// Undoes what the Prepare method did.
    /// </summary>
    public void Unprepare()
    {
        //prepMotor.Reverse();
        StartRoutine(Unprep());
    }

    /// <summary>
    /// Starts process of unit moving toward a destination.
    /// </summary>
    /// <param name="destination">Destination tower to move toward.</param>
    public void MoveTo(TowerBehavior destination)
    {
        faction = origin.Faction;
        this.destination = destination;
        orbitCounter = 0;
        //prepMotor.Reset();
        //moveMotor.SetDestination(destination);
        //curMotor = moveMotor;
        attacking = true;
        StartRoutine(Attack());
    }

    /// <summary>
    /// Sets the group of this unit as well as its faction.
    /// </summary>
    /// <param name="unitGroup">Group for unit to join.</param>
    public void SetGroup(UnitGroup unitGroup)
    {
        Group = unitGroup;
        faction = unitGroup.Faction;
    }

    /// <summary>
    /// Instantly destroys this unit.
    /// </summary>
    public void Kill ()
	{
        if (movementRoutine != null)
        {
            StopCoroutine(movementRoutine);
        }
        Group.SubtractUnit();
        body.velocity = Vector3.zero;
        DeathSystem.Emit(50);
        GraphicObject.SetActive(false);
        collider.enabled = false;
        if (UnitKilled != null)
        {
            UnitKilled(this);
		}
		faction = -1;
	}

    /// <summary>
    /// Kill method for when unit kill by tower.
    /// </summary>
    public void ImpactKill()
    {
        Group.SubtractUnit();
        if (movementRoutine != null)
        {
            StopCoroutine(movementRoutine);
        }
        body.velocity = Vector3.zero;
        GraphicObject.SetActive(false);
        collider.enabled = false;
        faction = -1;
    }

    /// <summary>
    /// Damages this units group. Kills unit if group subtracts a unit.
    /// </summary>
    /// <param name="amount">Amount of endurance to subtract from group.</param>
    public void DamageGroup(int amount)
    {
        if (Group.Damage(amount))
        {
            Kill();
        }
    }

    /// <summary>
    /// Removes this unit from its current group and adds it the the specified group.
    /// </summary>
    /// <param name="group">Group to add this unit to.</param>
    public void TransferGroup(UnitGroup group)
    {
        //Debug.Log("transferint unit from " + Group.ID + " to " + group.ID);
        Group.SubtractUnit();
        Group = group;
        Group.AddUnit();
    }

    // If entered a tower orbit increase orbit counter.
	void OnTriggerEnter(Collider col)
    {
        var tower = col.GetComponent<TowerBehavior> ();
        //Debug.Log("Entering: " + tower);
        if (tower != null)
		{
			orbitCounter++;
            deathTimer = 0;
            if (tower != origin)
            {
                OrbitSystem.Emit(10);
            }
		}
	}

    void OnTriggerStay(Collider col)
    {
        var tower = col.GetComponent<TowerBehavior>();
        //Debug.Log("Entering: " + tower);
        if (tower != null)
        {
            deathTimer = 0;
        }
    }

    // If exited a tower's orbit decrease orbit counter.
    // Ensure that orbitCounter does not go below 0.
	void OnTriggerExit(Collider col)
	{
		var tower = col.GetComponent<TowerBehavior> ();
        //Debug.Log("Exiting: " + tower);
        if (tower != null)
		{
			//orbitCounter--;
   //         if (orbitCounter < 0)
   //         {
   //             orbitCounter = 0;
   //         }

   //         if (orbitCounter == 0)
   //         {
   //             deathTimer = 0;
   //         }
            OrbitSystem.Emit(10);
        }
	}

    void StartRoutine(IEnumerator routine)
    {
        if (movementRoutine != null)
        {
            StopCoroutine(movementRoutine);
            movementRoutine = null;
        }
        if (gameObject.activeSelf)
        {
            movementRoutine = StartCoroutine(routine);
        }
    }

    IEnumerator Prep()
    {
        var norm = Random.insideUnitCircle;
        var pos = new Vector3(norm.x, 0, norm.y);
        transform.position = origin.transform.position + pos*.1f;
        var targetPos = pos * .5f;
        float speed = 2.0f;
        float acceleration = 1.0f;
        float finalSpeed = Random.Range(2.5f, 3.5f);
        var gravity = (origin.transform.position - transform.position).normalized;
        float gravityForce = .2f;
        var dir = new Vector3(-gravity.z, 0, gravity.x);

        body.velocity = dir * speed;
        yield return new WaitForFixedUpdate();
        while (true)
        {
            speed += Time.fixedDeltaTime;
            if (speed > finalSpeed)
            {
                speed = finalSpeed;
            }
            var target = targetPos + origin.transform.position;
            var dif = target - transform.position;
            float dist = dif.magnitude;
            float boost = 1.0f;
            if (dist > 1.0f)
            {
                boost = dist * 10.0f;
            }
            gravity = (dif).normalized * gravityForce * boost;
            body.velocity = (body.velocity + gravity).normalized * speed;
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator Unprep()
    {
        float speed = body.velocity.magnitude;
        var gravity = (origin.transform.position - transform.position).normalized;
        while (true)
        {
            speed -= Time.fixedDeltaTime * .5f;
            if (speed < 0)
            {
                speed = 0;
            }
            gravity = origin.transform.position - transform.position;
            //body.velocity = (body.velocity*speed + gravity).normalized;
            body.velocity += gravity;
            if (Vector3.Distance(origin.transform.position, transform.position) <= .5f)
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        gameObject.SetActive(false);
    }

    IEnumerator Attack()
    {
        float speed = body.velocity.magnitude;
        float maxSpeed = Game.TowerInfo.DefaultMovementSpeed;
        var gravity = (destination.transform.position - transform.position).normalized;
        while (true)
        {
            speed += Time.fixedDeltaTime * 5.0f;
            if (speed > maxSpeed)
            {
                speed = maxSpeed;
            }

            gravity = (destination.transform.position - transform.position).normalized;
            body.velocity = (body.velocity + gravity).normalized * speed;
            if (Vector3.Distance(destination.transform.position, transform.position) <= .5f)
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        gameObject.SetActive(false);
        destination.UnitEntered(this);
    }
}
