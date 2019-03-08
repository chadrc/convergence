using UnityEngine;
using System.Collections.Generic;

public class AstroidBelt : MonoBehaviour
{
    public TriggerProxy InnerTriggerProxy;
    public TriggerProxy OuterTriggerProxy;
    public float AstroidDensity = 1.0f;
    public float ExplosionForce = 200.0f;
    public int BeltDamage;
    public float SecondsForFullRotation;
    [Range(0, 1)]
    public float StartRotationPercent;
    public bool ClockWise;
    
    public int ActiveOpenings;
    public AstroidOpening[] Openings;

    public Sprite[] sprites;

    private float upTime;
    private Dictionary<UnitBehavior, bool> unitInBelt = new Dictionary<UnitBehavior, bool>();

    private static Vector3 startRot;
    private static Vector3 endRot;

    private GameObject astroidParent;

    // Debugging
    //private List<Vector3> entrancePoints = new List<Vector3>();
    //private List<Vector3> exitPoints = new List<Vector3>();

    static AstroidBelt()
    {
        startRot = new Vector3(0, 0, 0);
        endRot = new Vector3(0, 360, 0);
    }

    void Awake()
    {

    }

	// Use this for initialization
	void Start ()
    {
        CreateAstroidGraphics();

        if (ActiveOpenings > Openings.Length)
        {
            ActiveOpenings = Openings.Length;
        }
        
        foreach (Transform t in astroidParent.transform)
        {
            if (PointIsInSafeZones(t.position.x, t.position.z))
            {
                t.gameObject.SetActive(false);
            }
        }

        Init();
        InnerTriggerProxy.EnterAction += OnInnerTriggerEnter;
        InnerTriggerProxy.ExitAction += OnInnerTriggerExit;
        OuterTriggerProxy.EnterAction += OnOuterTriggerEnter;
        OuterTriggerProxy.ExitAction += OnOuterTriggerExit;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (SecondsForFullRotation > 0)
        {
            CalcPos();
        }
	}

    /// <summary>
    /// Initializes nessesary information for AstroidBelt's calculation to work
    /// </summary>
    public void Init()
    {
        // Set uptime to the a percentage of total rotation time
        upTime = StartRotationPercent * SecondsForFullRotation;
    }

    /// <summary>
    /// Creates graphics within astroid belt volume
    /// </summary>
    public void CreateAstroidGraphics()
    {
        if (sprites.Length == 0)
        {
            Debug.LogWarning("No sprites in astroid belt: " + gameObject);
            return;
        }
        //Debug.Log("Generating belt");
        astroidParent = new GameObject();
        astroidParent.name = "AstroidParent";
        astroidParent.transform.SetParent(transform);
        float baseRot = transform.rotation.eulerAngles.y;

        float astroidCount = 0;
        if (AstroidDensity > 0)
        {
            astroidCount = 360 / AstroidDensity;
        }

        for (float i = 0; i < astroidCount; i++)
        {
            float angle = (i - baseRot) * Mathf.Deg2Rad;
            var obj = new GameObject();
            obj.name = "Astroid " + i;
            obj.transform.SetParent(astroidParent.transform);
            obj.transform.localScale = Vector3.one;
            obj.transform.localEulerAngles = new Vector3(-90, 0, 0);
            float radius = Random.Range(InnerTriggerProxy.Collider.radius, OuterTriggerProxy.Collider.radius);
            float x = Mathf.Cos(angle) * radius;
            float y = 0.0f;
            float z = Mathf.Sin(angle) * radius;
            obj.transform.position = new Vector3(x, y, z) + transform.position;

            var sprite = sprites[Random.Range(0, sprites.Length)];
            var renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;

            var rotation = obj.AddComponent<TowerRangeRotate>();
            rotation.RotationSpeed = Random.Range(1.0f, 2.0f);

            var body = obj.AddComponent<Rigidbody>();
            body.useGravity = false;
        }
    }

    /// <summary>
    /// Calculates the astroid belts rotation based on how long it has been active and rotation speed.
    /// </summary>
    public void CalcPos()
    {
        upTime += Time.deltaTime;
        if (upTime > SecondsForFullRotation)
        {
            upTime -= SecondsForFullRotation;
        }
        var rotVec = Vector3.Lerp(startRot, endRot, (upTime / SecondsForFullRotation));
        if (!ClockWise)
        {
            rotVec.y *= -1;
        }
        transform.rotation = Quaternion.Euler(rotVec);
    }

    /// <summary>
    /// Activates next opening by increasing the active openings counter and hiding all astroids in that opening.
    /// Returns true if an opening was activated, false otherwise.
    /// </summary>
    public bool ActivateNextOpening()
    {
        if (ActiveOpenings >= Openings.Length)
        {
            return false;
        }

        List<Transform> toDeparent = new List<Transform>();
        // Hide all objects in next opening range
        foreach(Transform t in astroidParent.transform)
        {
            if (PointIsInOpening(Openings[ActiveOpenings], t.position.x, t.position.z))
            {
                //t.gameObject.SetActive(false);
                var body = t.GetComponent<Rigidbody>();
                var force = (t.position - transform.position).normalized * ExplosionForce;
                body.AddForce(force);
                toDeparent.Add(t);
            }
        }

        foreach(var t in toDeparent)
        {
            t.SetParent(null);
        }

        ActiveOpenings++;
        return true;
    }

    public void ThinOutAstroids(float percentToRemove)
    {
        percentToRemove = Mathf.Clamp01(percentToRemove);
        int iPercent = Mathf.CeilToInt(percentToRemove * 100.0f);
        if (iPercent == 100)
        {
            RemoveRemainingAstroids();
            return;
        }

        int i = 0;
        List<Transform> toDeparent = new List<Transform>();

        foreach (Transform t in astroidParent.transform)
        {
            i += iPercent;
            if (i >= 100)
            {
                var body = t.GetComponent<Rigidbody>();
                var force = (t.position - transform.position).normalized * ExplosionForce;
                body.AddForce(force);
                toDeparent.Add(t);
                i -= 100;
            }
        }
        
        foreach (var t in toDeparent)
        {
            t.SetParent(null);
        }
    }

    public void RemoveRemainingAstroids()
    {
        List<Transform> toDeparent = new List<Transform>();

        foreach (Transform t in astroidParent.transform)
        {
            var body = t.GetComponent<Rigidbody>();
            var force = (t.position - transform.position).normalized * ExplosionForce;
            body.AddForce(force);
            toDeparent.Add(t);
        }

        foreach (var t in toDeparent)
        {
            t.SetParent(null);
        }
    }

    // Unit exits astroid belt
    void OnInnerTriggerEnter(Collider col)
    {
        var unit = col.gameObject.GetComponent<UnitBehavior>();
        if (unit != null)
        {
            UnitExited(unit);
        }
    }

    // Unit enters astroid belt if its distance is greater than the inner radius
    void OnOuterTriggerEnter(Collider col)
    {
        var unit = col.gameObject.GetComponent<UnitBehavior>();
        if (unit != null && Vector3.Distance(unit.transform.position, transform.position) >= InnerTriggerProxy.Collider.radius)
        {
            UnitEntered(unit);
        }
    }

    // Unit enters astroid belt
    void OnInnerTriggerExit(Collider col)
    {
        var unit = col.gameObject.GetComponent<UnitBehavior>();
        if (unit != null)
        {
            UnitEntered(unit);
        }
    }

    // Unit exits astroid belt
    void OnOuterTriggerExit(Collider col)
    {
        var unit = col.gameObject.GetComponent<UnitBehavior>();
        if (unit != null)
        {
            UnitExited(unit);
        }
    }

    // Removes unit from dictionary
    void UnitExited(UnitBehavior unit)
    {
        if (unitInBelt.ContainsKey(unit))
        {
            unitInBelt[unit] = false;
        }
        else
        {
            unitInBelt.Add(unit, false);
        }
        //exitPoints.Add(unit.transform.position);
    }

    // Adds unit to dictionary. If unit is not in safe zone the units group is damaged
    void UnitEntered(UnitBehavior unit)
    {
        if (unitInBelt.ContainsKey(unit))
        {
            unitInBelt[unit] = true;
        }
        else
        {
            unitInBelt.Add(unit, true);
        }

        if (!PointIsInSafeZones(unit.transform.position.x, unit.transform.position.z))
        {
            unit.DamageGroup(BeltDamage);
        }
        //else
        //{
        //    Debug.Log("Unit passed safely");
        //}
        //entrancePoints.Add(unit.transform.position);
    }

    // Determines if a point with coordinantes x and z is in any of the active openings
    bool PointIsInSafeZones(float x, float z)
    {
        for (int i=0; i<ActiveOpenings; i++)
        {
            //Debug.Log("Checking Opening: " + i);
            if (PointIsInOpening(Openings[i], x, z))
            {
                return true;
            }
        }

        return false;
    }

    // Determine is a point with coordinantes x, z are within the given opening
    // Takes into account the current rotation of the astroid belt for calculations
    bool PointIsInOpening(AstroidOpening opening, float x, float z)
    {
        float angle = GetAngle(x, z);
        // Reverse y rotation because +z points into screen in level setup
        float baseRot = 360.0f - Mathf.Abs(transform.rotation.eulerAngles.y % 360);
        float lowerAngle = baseRot + opening.OpeningAngle;
        float upperAngle = lowerAngle + opening.OpeningSize;
        float rangeDif = Mathf.DeltaAngle(lowerAngle, upperAngle);
        float angleDif = Mathf.DeltaAngle(lowerAngle, angle);
        //Debug.Log("Angle: " + angle + " | Lower Angle: " + lowerAngle + " | Upper Angle: " + upperAngle + " | Dif: " + rangeDif + " | A Dif: " + angleDif);

        if (angleDif > 0 && angleDif <= rangeDif)
        {
            //Debug.Log("Point in opening");
            return true;
        }

        return false;
    }

    // Returns angle between Vector3.right and point with x, and z in range [0, 360]
    float GetAngle(float x, float z)
    {
        var uPos = new Vector3(x, 0, z) - transform.position;

        float angle = Vector3.Angle(Vector3.right, uPos);
        if (z < 0)
        {
            angle = 180 + (180 - angle);
        }
        return angle;
    }

    // Calculates point around gameObject's position with given angle and radius
    // Mainly used for OnDrawGizmos and Inspector functionality
    public Vector3 CalcPoint(float angle, float radius)
    {
        return new Vector3(Mathf.Cos(angle) * radius, transform.position.y, Mathf.Sin(angle) * radius) + transform.position;
    }

    void OnDrawGizmos()
    {
        float outerRadius = OuterTriggerProxy.gameObject.GetComponent<SphereCollider>().radius;
        float innerRadius = InnerTriggerProxy.gameObject.GetComponent<SphereCollider>().radius;
        float baseRot = transform.rotation.eulerAngles.y;
        
        var lastOuter = Vector3.zero;
        var lastInner = Vector3.zero;

        // Draw Danger Area
        Gizmos.color = Color.red;
        float TwoPI = Mathf.PI * 2;
        lastOuter = CalcPoint(0, outerRadius);
        lastInner = CalcPoint(0, innerRadius);
        for (int i = 1; i < 360; i++)
        {
            float angle = Mathf.Lerp(0, TwoPI, i / 50f);
            var outerPt = CalcPoint(angle, outerRadius);
            var innerPt = CalcPoint(angle, innerRadius);
            Gizmos.DrawLine(lastOuter, outerPt);
            Gizmos.DrawLine(lastInner, innerPt);
            lastOuter = outerPt;
            lastInner = innerPt;
        }

        // Draw Safe Area
        Gizmos.color = Color.blue;
        
        foreach(var opening in Openings)
        {
            float startAngle = (opening.OpeningAngle - baseRot) * Mathf.Deg2Rad;
            float endAngle = (opening.OpeningAngle + opening.OpeningSize - baseRot) * Mathf.Deg2Rad;
            //float halfAngle = opening.OpeningSize / 2;
            //var eular = transform.rotation.eulerAngles;

            var lowerOuterPoint = CalcPoint(startAngle, outerRadius);
            var upperOuterPoint = CalcPoint(endAngle, outerRadius);
            var lowerInnerPoint = CalcPoint(startAngle, innerRadius);
            var upperInnerPoint = CalcPoint(endAngle, innerRadius);

            lastOuter = lowerOuterPoint;
            lastInner = lowerInnerPoint;
            for (int i = 0; i < 50; i++)
            {
                float angle = Mathf.Lerp(startAngle, endAngle, i / 50f);
                var outerPt = CalcPoint(angle, outerRadius);
                var innerPt = CalcPoint(angle, innerRadius);
                Gizmos.DrawLine(lastOuter, outerPt);
                Gizmos.DrawLine(lastInner, innerPt);
                lastOuter = outerPt;
                lastInner = innerPt;
            }

            Gizmos.DrawLine(lastOuter, upperOuterPoint);
            Gizmos.DrawLine(lastInner, upperInnerPoint);

            Gizmos.DrawLine(lowerInnerPoint, lowerOuterPoint);
            Gizmos.DrawLine(upperInnerPoint, upperOuterPoint);
        }

        // Debugging
        //Gizmos.color = Color.magenta;
        //foreach(var p in exitPoints)
        //{
        //    Gizmos.DrawSphere(p, .1f);
        //}
        //Gizmos.color = Color.cyan;
        //foreach(var p in entrancePoints)
        //{
        //    Gizmos.DrawSphere(p, .1f);
        //}
    }
}

[System.Serializable]
public class AstroidOpening
{
    [Range(0, 360)]
    public float OpeningSize;
    [Range(0, 360)]
    public float OpeningAngle;
}