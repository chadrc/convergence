using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

public class ConverganceCalc : MonoBehaviour {

    [MenuItem("Convergence/Calculate")]
    public static void CalculateConvergences()
    {
        // Find level controller to obtain the data and prefab
        var levelCtrl = GameObject.FindObjectOfType<LevelController>();
        //Debug.Log("Level Ctrl: " + levelCtrl);
        var levelData = levelCtrl.CurrentLevel;
        //Debug.Log("Level Data: " + levelData);
        var levelObj = levelData.Prefab;
        //Debug.Log("Level Object: " + levelObj);

        var data = SimulateForConvergences(levelObj);

        // Display convergences
        //Debug.Log("Convergences found: " + data.Convergences.Count);
        //foreach(var c in data.Convergences)
        //{
        //    string convergenceStr = "";
        //    foreach(var o in c)
        //    {
        //        var t = o.gameObject.GetComponent<TowerBehavior>();
        //        convergenceStr += t.Index + ", ";
        //    }
        //    string str = convergenceStr.Substring(0, convergenceStr.Length - 2);
        //    Debug.Log("Convergence at " + c.TimeOccurred + ": " + str);
        //}

        // Go through all convergences to combine the convergences that have the same towers involved
        // and get the first occurrence time and interval time
        var uniqueConvergences = new Dictionary<float, ConvergenceInfo>();
        foreach(var c in data.Convergences)
        {
            // Calculate unique key for convergence based on the towers involved
            float key = 0.0f;
            int index = 0;
            foreach(var o in c)
            {
                var tower = o.GetComponent<TowerBehavior>();
                key += (index + 1) * tower.Index;
                index++;
            }
            
            // If convergence is already accounted for, set interval time if not already set
            if (uniqueConvergences.ContainsKey(key))
            {
                var info = uniqueConvergences[key];
                if (info.IntervalTime == -1)
                {
                    info.IntervalTime = c.TimeOccurred - info.FirstTime;
                    Debug.Log("Interval found for " + key + ": " + info.IntervalTime);
                }
            }
            else
            {
                // Create new info and add to unique dictionary
                var info = new ConvergenceInfo(c);
                uniqueConvergences.Add(key, info);
                Debug.Log("Unique Convergence found: " + key);
            }
        }

        // Maybe?
        levelData.convergences.Clear();

        // Copy all convergences to to current level data
        foreach(var c in uniqueConvergences.Keys)
        {
            levelData.convergences.Add(uniqueConvergences[c]);
        }

        // Force Unity to save changes to level data
        EditorUtility.SetDirty(levelData);
    }

    public static SimData SimulateForConvergences(GameObject levelObj)
    {
        var data = new SimData();
        var simFrames = new List<SimFrame>();

        // Loop through all transforms in scene to find all OrbitMotion components
        // Organize them according by the object they are orbiting around
        var orbitCenters = new Dictionary<GameObject, List<OrbitMotion>>();
        foreach (Transform t in levelObj.transform)
        {
            var orbit = t.gameObject.GetComponent<OrbitMotion>();
            if (orbit != null)
            {
                if (!orbitCenters.ContainsKey(orbit.OrbitCenter.gameObject))
                {
                    orbitCenters.Add(orbit.OrbitCenter.gameObject, new List<OrbitMotion>());
                    //Debug.Log("Orbit Center found: " + orbit.OrbitCenter.gameObject);
                }
                //Debug.Log("Orbit found: " + orbit);
                orbitCenters[orbit.OrbitCenter.gameObject].Add(orbit);
            }
        }

        var convergences = new List<Convergence>();

        // Calculate convergences for each center
        foreach (var k in orbitCenters.Keys)
        {
            var orbits = orbitCenters[k];

            var distances = new Dictionary<OrbitMotion, float>();
            var rates = new Dictionary<OrbitMotion, float>();

            // Calculate Rates and create distance dictionary
            for (int i = 0; i < orbits.Count; i++)
            {
                rates.Add(orbits[i], (360.0f / orbits[i].StartingSecondsForFullOrbit));
                distances.Add(orbits[i], orbits[i].StartPositionInOrbit * 360);
            }

            // Organize the orbits by their horizontal radius because ... words ...
            float firstRing = float.MaxValue;
            var orbitRings = new Dictionary<float, List<OrbitMotion>>();
            foreach (var o in orbits)
            {
                if (!orbitRings.ContainsKey(o.HorizontalRadius))
                {
                    orbitRings.Add(o.HorizontalRadius, new List<OrbitMotion>());
                    if (firstRing == float.MaxValue)
                    {
                        firstRing = o.HorizontalRadius;
                    }
                }

                orbitRings[o.HorizontalRadius].Add(o);
            }
            
            //foreach (var r in orbitRings.Keys)
            //{
            //    Debug.Log("Ring at " + r + ": " + orbitRings[r].Count);
            //}

            // Setup simulation variables
            float t = 0.0f;
            float step = 0.1f;
            float precision = 1.0f;
            while (t < 10000)
            {
                t += step;
                // Update Distances
                for (int i = 0; i < orbits.Count; i++)
                {
                    distances[orbits[i]] = (orbits[i].StartPositionInOrbit * 360) + rates[orbits[i]] * t;
                }

                simFrames.Add(new SimFrame(t, new Dictionary<OrbitMotion, float>(distances)));

                var first = orbitRings[firstRing];
                foreach (var o in first)
                {
                    var convergenceList = new List<OrbitMotion>();
                    convergenceList.Add(o);
                    float d1 = distances[o] % 360;
                    foreach (var ring in orbitRings.Keys)
                    {
                        if (firstRing == ring) continue;

                        foreach (var obj in orbitRings[ring])
                        {
                            float d2 = distances[obj] % 360;
                            float dif = Mathf.Abs(d1 - d2);
                            if (dif > 180)
                            {
                                dif = 360 - dif;
                            }
                            if (dif <= precision)
                            {
                                convergenceList.Add(obj);
                                break;
                            }
                        }
                    }

                    // If there is a tower in convergence list for each ring a convergence occurred
                    if (convergenceList.Count == orbitRings.Count)
                    {
                        convergences.Add(new Convergence(t, convergenceList));
                    }
                }
            }
        }

        data.Convergences = convergences;
        data.Frames = simFrames;
        return data;
    }
}

public class SimData
{
    public List<SimFrame> Frames;
    public List<Convergence> Convergences;
}

public class SimFrame : IEnumerable<KeyValuePair<OrbitMotion, float>>
{
    private float time;
    private Dictionary<OrbitMotion, float> distances;

    public float Time { get { return time; } }

    public SimFrame(float time, Dictionary<OrbitMotion, float> distances)
    {
        this.time = time;
        this.distances = distances;
    }

    public IEnumerator<KeyValuePair<OrbitMotion, float>> GetEnumerator()
    {
        return distances.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return distances.GetEnumerator();
    }
}
