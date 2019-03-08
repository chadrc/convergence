using UnityEngine;
using System.Collections;

public delegate void TimerCallback();

public class Timer : TimeMechanic
{
	public float Duration;
	public float Elapsed;
	public float Time;
	protected bool _valid;

	TimerCallback callback;

	public Timer()
	{

	}

	public Timer(float duration, TimerCallback callback, Clock parent = null)
	{
		Duration = duration;
		this.callback = callback;
		this.parent = parent;
	}

	public void Restart()
	{
		Stop ();
		Reset ();
		Start ();
	}

	protected override void PostCalculateTime ()
	{
		if (ElapsedTime > Duration)
		{
			callback();
			Stop();
		}
	}

	protected virtual void CalculateTime(TimeInfo info)
	{

	}

	public void Cancel()
	{

	}
}


public class TimeInfo
{
	public float deltaTime;
}