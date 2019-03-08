using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelData : ScriptableObject
{
	public string Name;
	public string Discription;
	public GameObject Prefab;
	public bool IgnoreFirstConvergence;
    public List<ConvergenceInfo> convergences = new List<ConvergenceInfo>();
}

[System.Serializable]
public class ConvergenceInfo
{
    public float FirstTime = -1;
    public float IntervalTime = -1;
    public int[] TowersInvolved;

    public ConvergenceInfo(Convergence con)
    {
        FirstTime = con.TimeOccurred;
        TowersInvolved = new int[con.OrbitersCount];
        int i = 0;
        foreach (var o in con)
        {
            var tower = o.gameObject.GetComponent<TowerBehavior>();
            if (tower != null)
            {
                TowersInvolved[i] = tower.Index;
            }
            i++;
        }
    }
}

public class Convergence : IEnumerable<OrbitMotion>
{
    private float timeOccurred;
    private List<OrbitMotion> orbiters = new List<OrbitMotion>();

    public float TimeOccurred { get { return timeOccurred; } }
    public int OrbitersCount { get { return orbiters.Count; } }

    public Convergence(float timeOccurred, List<OrbitMotion> orbiters)
    {
        this.timeOccurred = timeOccurred;
        this.orbiters = orbiters;
    }

    public IEnumerator<OrbitMotion> GetEnumerator()
    {
        return orbiters.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return orbiters.GetEnumerator();
    }
}