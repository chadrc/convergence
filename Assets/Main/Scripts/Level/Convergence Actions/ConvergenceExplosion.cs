using UnityEngine;
using System.Collections;

public class ConvergenceExplosion : MonoBehaviour
{
    public GameObject Center;
    public AstroidBelt Belt;
    public float QTStartTime;
    public float MaxPowerUpParticles;
    public ParticleSystem PowerUpSystem;
    public ParticleSystem ExplosionSystem;
	public GameObject ExplosionSystemPrefab;
    public ShieldThresholdData[] ShieldThresholds;
    public ExplosionDamageData[] ExplosionThresholds;
    [Tooltip("Number to set the AstroidBelt's damage to after each convergence.")]
    public int[] AstroidBeltDamagesPerConvergence;
    [Tooltip("Percent of astroids to remove from AstoidBelt each convergence.")]
    public float[] AstroidBeltThinOutPercents;

    private int convergenceCount;
    private bool swapped = false;
    private ExplosionQuickTimePointerResponder responder;
	// Use this for initialization
	void Start ()
    {
        responder = new ExplosionQuickTimePointerResponder(UIController.DefaultResponder);
        ConvergenceController.ConvergenceOccurred += ConvergenceController_ConvergenceOccurred;
        UIController.LinkResponder(responder);
        PowerUpSystem.Stop();
        //ExplosionSystem.Stop();
	}

    void Update()
    {
        float t = ConvergenceController.TimeTillNextConvergence;

        if (swapped)
        {
            float time = Mathf.Clamp(t, 0, QTStartTime);
            float frac = 0.0f;
            if (time <= PowerUpSystem.startLifetime*2)
            {
                time -= PowerUpSystem.startLifetime;
                if (time < 0 )
                {
                    time = 0;
                }
                frac = time / PowerUpSystem.startLifetime;
            }
            else
            {
                frac = 1.0f - (time / QTStartTime);
            }

            // New way not working
            //var emision = PowerUpSystem.emission.rate;
            //emision.constantMin = frac * MaxPowerUpParticles;
            //emision.constantMax = frac * MaxPowerUpParticles;

            // Old way working
            PowerUpSystem.emissionRate = frac * MaxPowerUpParticles;
        }
        else if (t <= QTStartTime)
        {
            // Swap out UI pointer responder
            //Debug.Log("Quick Time Begin");
            swapped = true;
            responder.Reset();
            PowerUpSystem.Play();
            //ExplosionSystem.Clear();
            UIController.SetResponder(responder);
        }
    }

    void OnDestroy()
    {
        ConvergenceController.ConvergenceOccurred -= ConvergenceController_ConvergenceOccurred;
    }

    private void ConvergenceController_ConvergenceOccurred()
    {
        //Debug.Log("Convergence Occurred: Damaging Planets");
        if (ExplosionThresholds.Length == 0)
        {
            return;
        }

        var towers = TowerController.GetAllTowers();
        if (ExplosionThresholds.Length == 1)
        {
            foreach (var t in towers)
            {
                DamageTower(ExplosionThresholds[0], t);
            }
        }
        else
        {
            // need to sort list
            float min = 0;
            foreach (var e in ExplosionThresholds)
            {
                foreach (var t in towers)
                {
                    float dist = Vector3.Distance(t.transform.position, Center.transform.position);
                    if (dist >= min && dist < e.DistanceThreshold)
                    {
                        DamageTower(e, t);
                    }
                }
                min = e.DistanceThreshold;
            }
        }

        UnitController.DestoryAllUnitGroups();

		if (Belt != null)
        {
            //Belt.ActivateNextOpening ();
            Belt.ThinOutAstroids(AstroidBeltThinOutPercents[convergenceCount]);
            Belt.BeltDamage = AstroidBeltDamagesPerConvergence[convergenceCount];
        }

        PowerUpSystem.Stop();
//        ExplosionSystem.Play();

		var exObj = GameObject.Instantiate(ExplosionSystemPrefab);
		exObj.transform.position = transform.position;
		var exParticles = exObj.GetComponent<ParticleSystem>();
        //Debug.Log("Explosion Created");
		//GameObject.Destroy(exObj, exParticles.startLifetime);

        // Revert UIController to default
        swapped = false;
        UIController.ResetResponder();
        convergenceCount++;
        //Debug.Log("Quick Time End");
    }

    private void DamageTower(ExplosionDamageData e, TowerBehavior t)
    {
        //Debug.Log("Tower " + t.Index);
        float reduction = GetDamageReductionForIndex(t.Index);
        //Debug.Log("Reduction Percent: " + reduction);
        float netPercent = Mathf.Clamp01(e.PercentOfUnitsDestroyed - reduction);
        //Debug.Log("Net Damage: " + netPercent);
        int numUnitsKilled = (int)(t.StationedUnits * netPercent);
        t.StationedGroup.SubtractUnits(numUnitsKilled);
        //Debug.Log("Lost " + numUnitsKilled + " units.");
    }

    private float GetDamageReductionForIndex(int i)
    {
        var clicks = responder.TowerClicks[i];
        float reduction = 0;
        foreach(var s in ShieldThresholds)
        {
            if (clicks >= s.ClickCount)
            {
                reduction = s.PercentReduction;
            }
        }
        //Debug.Log("Num Clicks: " + clicks[i]);
        return reduction;
    }
}

[System.Serializable]
public class ExplosionDamageData
{
    public float DistanceThreshold;
    [Range(0,1)]
    public float PercentOfUnitsDestroyed;
}

[System.Serializable]
public class ShieldThresholdData
{
    public int ClickCount;
    [Range(0, 1)]
    public float PercentReduction;
}
