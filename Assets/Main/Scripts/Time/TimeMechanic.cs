using UnityEngine;
using System.Collections;

public abstract class TimeMechanic
{
	public static MonoBehaviour RoutineBehavior;
	
	private float timeScale = 1.0f;
	public float TimeScale
	{
		get
		{
			float totalScale = timeScale;
			if (parent != null)
			{
				totalScale *= parent.TimeScale;
			}
			return totalScale;
		}
		
		set
		{
			if (value > 0)
			{
				timeScale = value;
			}
			else
			{
				timeScale = 0;
			}
		}
	}

	protected TimeMechanic parent { get; set; }
	public float ElapsedTime { get; private set; }
	public bool Paused { get; private set; }
	public string Label;

	private Coroutine routine;

	public virtual void Start()
	{
		if (RoutineBehavior != null)
		{
			Paused = false;
			routine = RoutineBehavior.StartCoroutine(Run());
		}
	}
	
	public virtual void Stop()
	{
		if (RoutineBehavior != null && routine != null)
		{
			RoutineBehavior.StopCoroutine(routine);
			routine = null;
		}
	}

	public virtual void Reset()
	{
		ElapsedTime = 0.0f;
	}

	public virtual void Pause()
	{
		Paused = true;
	}

	public virtual void Unpause()
	{
		Paused = false;
	}

	protected abstract void PostCalculateTime();

	private IEnumerator Run()
	{
		while(true)
		{
			if (!Paused)
			{
				ElapsedTime += Time.deltaTime * TimeScale;
				PostCalculateTime();
			}
			yield return new WaitForEndOfFrame();
		}
	}
}