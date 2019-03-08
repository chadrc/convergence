using UnityEngine;
using System.Collections;
using System;

public class UnitMovementMotor : IMotor
{
    private UnitBehavior unit;
    //private Vector3 origin;
    private TowerBehavior destination;

    private float currentSpeed;
    private float acceleration;
    private float maxSpeed;

    public bool AtDestination
    {
        get
        {
            return Vector3.Distance(unit.transform.position, destination.transform.position) <= .5f;
        }
    }

    public Transform DrivenTransform
    {
        get
        {
            return unit.transform;
        }

        set
        {
            var u = value.GetComponent<UnitBehavior>();
            if (u != null)
            {
                unit = u;
            }
        }
    }

    /// <summary>
    /// Creates a UnitMovementMotor with given UnitBehavior.
    /// </summary>
    /// <param name="unit">Unit to be driven by this motor.</param>
    public UnitMovementMotor(UnitBehavior unit)
    {
        this.unit = unit;
        currentSpeed = 3.0f;
        maxSpeed = 5.0f;
        acceleration = 5f;
    }

    /// <summary>
    /// Sets the destination of this motor.
    /// </summary>
    /// <param name="destination">Tower for this motor to move toward.</param>
    public void SetDestination(TowerBehavior destination)
    {
        //origin = unit.transform.position;
        this.destination = destination;
    }

    public void Drive()
    {
        if (AtDestination)
        {
            destination.UnitEntered(unit);
        }
    }

    public void FixedDrive()
    {
        if (!AtDestination)
        {
            if (currentSpeed < maxSpeed)
            {
                currentSpeed += (acceleration * Time.deltaTime);
            }
            var direction = (destination.transform.position - unit.transform.position);
            unit.Rigidbody.velocity = (unit.Rigidbody.velocity + direction).normalized * currentSpeed;
        }
    }
}
