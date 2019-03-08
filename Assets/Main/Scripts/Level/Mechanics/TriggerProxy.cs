using UnityEngine;
using System;
using System.Collections;

public interface ITriggerProxyReciever
{

}

[RequireComponent (typeof(Collider))]
public class TriggerProxy : MonoBehaviour
{
    public Action<Collider> EnterAction;
    public Action<Collider> StayAction;
    public Action<Collider> ExitAction;

    public SphereCollider Collider { get; private set; }

    void Awake()
    {
        Collider = GetComponent<SphereCollider>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (EnterAction != null)
        {
            EnterAction(col);
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (StayAction != null)
        {
            StayAction(col);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (ExitAction != null)
        {
            ExitAction(col);
        }
    }
}
