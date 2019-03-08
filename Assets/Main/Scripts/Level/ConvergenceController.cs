using UnityEngine;
using System.Collections;
using System;

public class ConvergenceController
{
    public static event System.Action ConvergenceOccurred;
	public static event System.Action ConvergenceCanceled;

    private static ConvergenceController current;

    private int[] currentConvergenceCounts;
    private LevelData level;
    private float startTime;
    private float tillNextConvergence;
	private float currentInterval;
	private int convergenceWaveCount;
    private MonoBehaviour routineBehavior;
    private Coroutine updateRoutine;

    public static bool Exists { get { return current != null; } }
    public static float TimeTillNextConvergence { get { return current == null ? -1 : current.tillNextConvergence; } }
	public static int ConvergenceCount
	{
		get
		{
			int total = 0;
			foreach (var c in current.currentConvergenceCounts)
			{
				total += c;
			}
			return total;
		}
	}
	public static float CurrentInterval
	{
		get
		{
			return current == null ? -1 : current.currentInterval;
		}
		private set
		{
			current.currentInterval = value;
		}
	}
	public static int ConvergenceWaveCount
	{
		get
		{
			return current == null ? -1 : current.convergenceWaveCount;
		}
		private set
		{
			current.convergenceWaveCount = value;
		}
	}

    private ConvergenceController()
    {

    }

    private ConvergenceController(MonoBehaviour routineBehavior)
    {
        this.routineBehavior = routineBehavior;
        level = LevelController.GetCurrentLevel();
        currentConvergenceCounts = new int[level.convergences.Count];
        startTime = 0;

        CalcNextConvergence();

        updateRoutine = routineBehavior.StartCoroutine(Update());
    }

    private IEnumerator Update()
    {
        yield return new WaitForEndOfFrame();

        while(true)
        {
            CalcNextConvergence();
            yield return new WaitForEndOfFrame();
        }
    }

    private void CalcNextConvergence()
    {
        startTime += Time.deltaTime;
        int nextConvergence = -1;
        float nextConvergenceTime = float.MaxValue;
        for (int i = 0; i < level.convergences.Count; i++)
        {
            float nextOccurrence = level.convergences[i].IntervalTime * currentConvergenceCounts[i] + level.convergences[i].FirstTime;
            if (nextOccurrence < nextConvergenceTime)
            {
                nextConvergence = i;
                nextConvergenceTime = nextOccurrence;
            }
        }
        if (nextConvergence != -1)
        {
            tillNextConvergence = nextConvergenceTime - startTime;
            if (tillNextConvergence <= 0)
            {
                currentConvergenceCounts[nextConvergence]++;
				bool ignore = (ConvergenceCount == 1 && level.IgnoreFirstConvergence);
				if (ConvergenceOccurred != null && !ignore)
				{
					ConvergenceWaveCount++;
					Debug.Log(convergenceWaveCount);
					ConvergenceOccurred ();
				}
				else if (ConvergenceCanceled != null)
				{
					ConvergenceCanceled ();
				}
                tillNextConvergence = float.MaxValue;
            }
        }
    }

    public static void CreateConvergenceController(MonoBehaviour routineBehavior)
    {
        if (current == null)
        {
            current = new ConvergenceController(routineBehavior);
			CurrentInterval = TimeTillNextConvergence;
			ConvergenceWaveCount = 1;
        }
    }

    public static void DestoryConvergenceController()
    {
        if (current != null)
        {
            current.routineBehavior.StopCoroutine(current.updateRoutine);
        }
        current = null;
        ConvergenceOccurred = null;
	}
		
}
