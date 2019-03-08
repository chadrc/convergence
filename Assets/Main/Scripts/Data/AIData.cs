using UnityEngine;
using System.Collections;

//The tools tips are the summaries
[System.Serializable]
public class AIData : ScriptableObject {

    [Header("How difficult do you want the Ai to be at the start of the level.")]
    public AIConstants.Difficulty difficulty;

    [Header("When an AI is in danger of being taken over by the player, these are the total number of towers that can help save that tower")]
    public float maxNumberAICanDefend;

    [Header("How many AI can perform a power attack")]
    public float maxNumberAICanPowerAttack;

    [Header("This affects time variables based on what level the player is playing in the campaign")]
    public float timeLevelDeduction;

    [Header("This affects percent variables based on what level the player is playing in the campaign")]
    [Range(0, 100)]
    public float percentLevelDeduction;

    [Header("A container that holds all time related variables for the AI.")]
    public AITimeContainer aiTimeContainer;

    [Header("A container that holds all percent related variables for the AI.")]
    public AIPercentContainer aiPercentContainer;
}
