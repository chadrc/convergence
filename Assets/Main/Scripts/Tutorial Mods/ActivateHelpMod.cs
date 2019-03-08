using UnityEngine;
using System.Collections;
using System;

public class ActivateHelpMod : MonoBehaviour, IHelpAndTipsResponder
{
    [TextArea(2, 30)]
    public string helpText;

    HelpAndTipsController helpAndTips;

    // Use this for initialization
    void Start()
    {
        helpAndTips = UIController.EnableHelpUI(this);
        StartCoroutine(DelayOpen());
    }

    public void HelpAndTipsActivated()
    {
        helpAndTips.SetText(helpText);
    }

    public void HelpAndTipsDeactivated()
    {

    }

    IEnumerator DelayOpen()
    {
        yield return new WaitForSeconds(1.0f);
        helpAndTips.Activate();
    }
}
