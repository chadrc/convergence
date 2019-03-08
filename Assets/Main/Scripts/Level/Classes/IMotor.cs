using UnityEngine;

public interface IMotor
{
    /// <summary>
    /// Wheather or not DrivenTransform is at its destination.
    /// </summary>
    bool AtDestination { get; }
    /// <summary>
    /// Transform to be moved by this motor.
    /// </summary>
    Transform DrivenTransform { get; set; }
    /// <summary>
    /// For use of moving DrivenTransform in Unity's Update callback.
    /// </summary>
    void Drive();
    /// <summary>
    /// For use of moving DrivenTransform in Unity's FixedUpdate callback.
    /// </summary>
    void FixedDrive();
}
