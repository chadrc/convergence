using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class TextPulse : MonoBehaviour, IPulseable
{
    public Text text;
    public OpacityPulse pulse;

    public Color color
    {
        get
        {
            return text.color;
        }

        set
        {
            text.color = value;
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
