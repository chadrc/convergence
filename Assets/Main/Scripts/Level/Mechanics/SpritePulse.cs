using UnityEngine;
using System.Collections;
using System;

public class SpritePulse : MonoBehaviour, IPulseable
{
    public SpriteRenderer sRenderer;
    public OpacityPulse pulse;

    public Color color
    {
        get
        {
            return sRenderer.color;
        }

        set
        {
            sRenderer.color = value;
        }
    }

    // Use this for initialization
    void Start ()
    {
        pulse.pulseObj = this;
	}
	
	// Update is called once per frame
	void Update ()
    {
        pulse.Pulse();
	}
}
