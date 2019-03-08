using UnityEngine;

public class UnitPrepMotor : IMotor
{
    private UnitBehavior unit;
    private TowerBehavior originTower;
    private Vector3 destination;
    private Vector3 origin;
    private float animTime;
    private float upTime;
	private bool deactivate;

    public Transform DrivenTransform
    {
        get
        {
            return unit.transform;
        }

        set
        {
            var u = value.GetComponent<UnitBehavior>();
            if (u != null)
            {
                unit = u;
            }
        }
    }

    public bool AtDestination
    {
        get
        {
            return Vector3.Distance(unit.transform.position, destination) <= .1f;
        }
    }

    /// <summary>
    /// Creates UnitPrepMotor with given UnitBehavior.
    /// </summary>
    /// <param name="unit"></param>
    public UnitPrepMotor(UnitBehavior unit)
    {
        this.unit = unit;
        originTower = null;
        animTime = Random.Range(.5f, 1.0f);
		deactivate = false;
    }

    /// <summary>
    /// Makes this motor's origin its destination and vice versa.
    /// </summary>
    public void Reverse()
    {
        var temp = origin;
        origin = unit.transform.position - originTower.transform.position;
        destination = temp;
        upTime = 0;
		deactivate = true;
    }

    public void Reset()
    {
        originTower = null;
    }

    /// <summary>
    /// Sets the starting point of this motor.
    /// </summary>
    /// <param name="origin">Point this motor should start from.</param>
    public void SetOrigin(TowerBehavior origin)
    {
        if (originTower != null)
        {
            return;
        }
        originTower = origin;
        this.origin = Vector3.zero;
        var spriteRenderer = origin.GraphicObj.GetComponent<SpriteRenderer>();
        float limit = spriteRenderer.bounds.extents.x;
        float radius = Random.Range(limit, limit + 0.5f);
        var point = Random.insideUnitCircle.normalized * radius;
        var pos = new Vector3(point.x, 0, point.y);
        destination = pos;
		unit.transform.position = origin.transform.position;
		upTime = 0;
		deactivate = false;
    }

    public void Drive()
    {
        upTime += Time.deltaTime;
        float frac = Mathf.Clamp01(upTime / animTime);
        float x = frac * (Mathf.PI/2);
        unit.transform.position = Vector3.Lerp(origin, destination, Mathf.Sin(x)) + originTower.transform.position;
		if (Vector3.Distance (unit.transform.position, originTower.transform.position) <= .1f && deactivate)
		{
			unit.ImpactKill ();
			originTower = null;
		}
    }

    public void FixedDrive()
    {

    }
}
