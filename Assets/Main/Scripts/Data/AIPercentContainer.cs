using UnityEngine;
using System.Collections;

[System.Serializable]
public class AIPercentContainer {


    [Header(" If a tower fails to attack because of lack of units, it will try to do a combo attack with another tower that failed.What are the chances that AI will be added to the queue instead of checking for a double attack")]
    [Range(0, 100)]
    public float chancetoQueue;

    [Header("Chance to tell the AI just to attack, don't do any checks what so ever. Will Send as many Units as possible")]
    [Range(0, 100)]
    public float PercentToAttackAnyway;

    [Header("What are the chances that the AI just won't defend and let the threatened tower be attacked")]
    [Range(0, 100)]
    public float percentChanceAIWontDefend;

    [Header("What percent is added/subtracted from the pecentages above based off game state")]
    [Range(0, 100)]
    public float gameStatePercentDifference;

    [Header("A minimum value add to the percentage. Makes AI a little more random")]
    [Range(0, 100)]
    public float minimumRandomPercent;

    [Header("A maximum value add to the percentage. Makes AI a little more random")]
    [Range(0, 100)]
    public float maximumRandomPercent;
}
