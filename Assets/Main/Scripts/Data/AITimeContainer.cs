using UnityEngine;
using System.Collections;

[System.Serializable]
public class AITimeContainer{

    [Header("Minimum Time for AI to attack at the beginning of the Level. The timer for the AI is random between this and the maximum Timer.")]
    public float startingMinimumTimer;

    [Header("Maximum Time for AI to attack at the beginning of the Level. The timer for the AI is random between this and the minimum timer.")]
    public float startingMaximumTimer;

    [Header("This is the absolute Minimum Time that an AI could Attack")]
    public float absoluteMinimumTimer;

    [Header("This is the absolute Maximum Time that an AI could Attack")]
    public float absoluteMaximumTimer;

    [Header("Timer Subtraction for difficulty, the more difficulty the ai, the more multiples of this timer are subtracted from the attack timer.This number should probably be the biggest of all the timedeductions")]
    public float difficultyTimeDeduction;

    [Header("Timer Subtraction based off the number of convergences that have happened Thus Far in the game")]
    public float convergenceTimeDeduction;

    [Header("Time Subtraction/Addition for the difference in units between the player and the AI, number should be very small.")]
    public float unitDifferenceTime;

    [Header("Time subtraction/addition for the difference in towers between the player and the AI, number should be pretty small.")]
    public float towerDifferenceTime;

    [Header("Time chance to add/subtract to all percentages based off Tower Upgrades")]
    public float upgradeDifferenceTime;

    public float minRandomTime;

    public float maxRandomTime;

}
