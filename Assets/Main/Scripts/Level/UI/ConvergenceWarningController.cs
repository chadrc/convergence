using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConvergenceWarningController : MonoBehaviour
{
    public Image WarningSymbol;
    public Text WarningText;

    public float StartAtSecondsLeft = 30.0f;
    public float PulseTime = 1.0f;
    public float DefaultAlpha = .2f;

    private float timer = 0.0f;

	// Use this for initialization
	void Start ()
    {
        ConvergenceController.ConvergenceOccurred += OnConvergence;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!ConvergenceController.Exists)
        {
            return;
        }
        
	    if (ConvergenceController.TimeTillNextConvergence < StartAtSecondsLeft)
        {
            timer += Time.deltaTime;
            float num = Mathf.PingPong(timer/PulseTime, 1.0f);

            var clr = WarningSymbol.color;
            clr.a = num;
            WarningSymbol.color = clr;

            clr = WarningText.color;
            clr.a = num;
            WarningText.color = clr;
        }
	}

    void OnDestroy()
    {
        ConvergenceController.ConvergenceOccurred -= OnConvergence;
    }

    void OnConvergence()
    {
        var clr = WarningSymbol.color;
        clr.a = DefaultAlpha;
        WarningSymbol.color = clr;

        clr = WarningText.color;
        clr.a = DefaultAlpha;
        WarningText.color = clr;
    }
}
