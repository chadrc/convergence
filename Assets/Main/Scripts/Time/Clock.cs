using UnityEngine;
using System.Collections;

public class Clock : TimeMechanic
{
	public Clock(Clock parent = null)
	{
		this.parent = parent;
	}

	protected override void PostCalculateTime ()
	{

	}

	public void Attach(Timer timer)
	{

	}

	public void Attach(Clock clock)
	{

	}

	public void Cancel()
	{

	}
}
