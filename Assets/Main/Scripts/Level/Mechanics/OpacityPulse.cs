using UnityEngine;
using System.Collections;

[System.Serializable]
public class OpacityPulse
{
    public IPulseable pulseObj { get; set; }
    public float PulseTime;
    [Range(0,1)]
    public float MinOpacity;
    [Range(0, 1)]
    public float MaxOpacity;

    private float uptime = 0.0f;

    public OpacityPulse(IPulseable obj)
    {
        pulseObj = obj;
    }

	// Update is called once per frame
	public void Pulse ()
    {
        uptime += Time.deltaTime;
        if (uptime > PulseTime)
        {
            uptime = PulseTime - uptime;
        }
        float frac = uptime / PulseTime;
        var clr = pulseObj.color;
        float x = (Mathf.PI * 2) * frac;
        float y = .5f * Mathf.Sin(x - (Mathf.PI / 2)) + .5f;
        float result = (MaxOpacity - MinOpacity) * y + MinOpacity;
        clr.a = result;
        pulseObj.color = clr;
    }
}

public interface IPulseable
{
    Color color { get; set; }
}