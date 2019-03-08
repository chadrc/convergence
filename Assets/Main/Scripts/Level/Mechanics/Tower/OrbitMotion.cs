using UnityEngine;
using System.Collections;

public class OrbitMotion : MonoBehaviour
{
    [Range(0.01f, 100.0f)]
    public float StartingSecondsForFullOrbit = 10.0f;
    [Range(0f, 1f)]
    public float StartPositionInOrbit;
    public float HorizontalRadius;
    public float VerticalRadius;
    [Range(0f, 360f)]
    public float OrbitRotation;
    public bool Clockwise;
    public Transform OrbitCenter;

    private float upTime;
    private Matrix4x4 rotMatrix;
    private float secondsForFullOrbit;

    const float TwoPI = Mathf.PI * 2;

    public float UpTime
    {
        get { return upTime; }
        //set
        //{
        //    upTime = StartPositionInOrbit * SecondsForFullOrbit;
        //}

    }

    public float SecondsForFullOrbit
    {
        get { return secondsForFullOrbit; }
        set
        {
            float posInOrbit = upTime / SecondsForFullOrbit;
            secondsForFullOrbit = value;
            upTime = posInOrbit * secondsForFullOrbit;
        }
    }
    // Use this for initialization
    void Start()
    {
        SecondsForFullOrbit = StartingSecondsForFullOrbit;
        upTime = StartPositionInOrbit * SecondsForFullOrbit;
        rotMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(new Vector3(0, OrbitRotation)), Vector3.one);

        var tower = GetComponent<TowerBehavior>();
        if (tower != null)
        {
            UIController.MarkTowerAsDynamic(tower);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SecondsForFullOrbit <= 0) return;
        //float x, y, z, angle;

        // Increase upTime, wrap around if greater than SecondsForFullOrbit
        upTime += Time.deltaTime;
        if (upTime > SecondsForFullOrbit)
        {
            upTime -= SecondsForFullOrbit;
        }

        CalculatePosition();
    }

    public void CalculatePosition()
	{
		if (OrbitCenter == null)
		{
			return;
		}

        float x, y, z, angle;

        angle = (UpTime / SecondsForFullOrbit) * TwoPI;
        if (Clockwise)
        {
            angle = -angle;
        }

        // Trig to calculate position on ellipse around (0,0,0)
        x = HorizontalRadius * Mathf.Cos(angle);
        y = transform.position.y;
        z = VerticalRadius * Mathf.Sin(angle);

        // If in editor constantly recalculate rotation matrix for tweaking in inspector
        #if UNITY_EDITOR
        rotMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(new Vector3(0, OrbitRotation)), Vector3.one);
        #endif

       this.transform.position = rotMatrix.MultiplyPoint3x4(new Vector3(x, y, z)) + OrbitCenter.position;

    }

    public void CalculatePositionEditor()
    {
		if (OrbitCenter == null)
		{
			return;
		}

        float x, y, z, angle;

        angle = ((StartPositionInOrbit * StartingSecondsForFullOrbit) / StartingSecondsForFullOrbit) * TwoPI;
        if (Clockwise)
        {
            angle = -angle;
        }

        // Trig to calculate position on ellipse around (0,0,0)
        x = HorizontalRadius * Mathf.Cos(angle);
        y = transform.position.y;
        z = VerticalRadius * Mathf.Sin(angle);

        // If in editor constantly recalculate rotation matrix for tweaking in inspector
        rotMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(new Vector3(0, OrbitRotation)), Vector3.one);

        this.transform.position = rotMatrix.MultiplyPoint3x4(new Vector3(x, y, z)) + OrbitCenter.position;
    }


    void OnDrawGizmosSelected()
    {
        
        #region Drawing ellipse/circle orbit gizmo

        Gizmos.color = Color.green;

        //Grabbing our two radii for shapping our gizmo.
        float hRad = HorizontalRadius;
        float vRad = VerticalRadius;
        float numberOfSteps = 50;

        float TwoPI = Mathf.PI * 2; //Full circle in Radians
        var lastPoint = Vector3.zero; //Used in determining our last point on the circle/ellipse.

        //Set an initial point to start with
        lastPoint = new Vector3((this.transform.position.x + (hRad * Mathf.Cos(0))),this.transform.position.y, (this.transform.position.z + (vRad * Mathf.Sin(0))));

        for(int i = 0; i < 360; i++)
        {
            float angle = Mathf.Lerp(0, TwoPI, i / numberOfSteps);
            var curPoint = new Vector3((OrbitCenter.transform.position.x + (hRad * Mathf.Cos(angle))), OrbitCenter.transform.position.y + 0.1f, (OrbitCenter.transform.position.z + (vRad * Mathf.Sin(angle))));
            Gizmos.DrawLine(lastPoint, curPoint);
            lastPoint = curPoint;
        }

        #endregion

    }

    #region Matts Addition 

    public Vector3 CalculatePositionWithMoreUpTime(float moreUp)
    {

        float x, y, z, angle;

        float tempUpTime = UpTime + moreUp;

        if (tempUpTime > SecondsForFullOrbit)
        {
            tempUpTime -= SecondsForFullOrbit;
        }

        angle = (tempUpTime / SecondsForFullOrbit) * TwoPI;
        if (Clockwise)
        {
            angle = -angle;
        }

        // Trig to calculate position on ellipse around (0,0,0)
        x = HorizontalRadius * Mathf.Cos(angle);
        y = transform.position.y;
        z = VerticalRadius * Mathf.Sin(angle);

       
        Matrix4x4 tempMat = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(new Vector3(0, OrbitRotation)), Vector3.one);
        

        return tempMat.MultiplyPoint3x4(new Vector3(x, y, z)) + OrbitCenter.position;

    }

    #endregion


}
