using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RadialUnitPercentController : MonoBehaviour
{
    public Image Graphic;
    public float TimeTill100Percent;

    private int numSections = 4;
    private bool timing;
    private float timer;
    private float unitPercent;

    public event System.Action<float> UnitPercentChanged;
    public float UnitPercent
    {
        get { return unitPercent; }

        private set
        {
            if (value != unitPercent && UnitPercentChanged != null)
            {
                UnitPercentChanged(value);
            }
            unitPercent = value;
        }
    }

    public TowerButtonBehavior AttachedTowerBtn { get; set; }

    void Awake()
    {
        Reset();
        //gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if (timing)
        {
            timer += Time.deltaTime;
            float percent = Mathf.Clamp01(timer / TimeTill100Percent);
            int round = Mathf.CeilToInt(percent * numSections);
            UnitPercent = round / (float)numSections;
            Graphic.fillAmount = percent;
            //Debug.Log("Percent: " + percent);
        }
	}

    public void Reset()
    {
        Graphic.fillAmount = 0.0f;
        UnitPercent = 1.0f / numSections;
        timer = 0.0f;
    }

    public void StartTiming()
    {
        Reset();
        gameObject.SetActive(true);
        timing = true;
    }

    public void StopTiming()
    {
        Reset();
        gameObject.SetActive(false);
        timing = false;
    }

    public void Reposition()
    {
        if (AttachedTowerBtn != null)
        {
            transform.position = AttachedTowerBtn.transform.position;
        }
    }
}
