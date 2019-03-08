using UnityEngine;
using System.Collections;

public static class AIConstants{

    /// <summary>
    /// What Priority should the AI take when attacking other towers
    /// </summary>
    public enum Priority { None = 0, Distance, NumUnits };
    /// <summary>
    /// How difficult should the AI be
    /// </summary>
    public enum Difficulty { Easy = 0, Medium, Hard, Expert, };
    /// <summary>
    /// Why did trying to attack fail
    /// </summary>
    public enum ReasonFailed { None, Distance, Units, Both, NotEnoughtoSend };
    /// <summary>
    /// 
    /// </summary>
    public enum UpgradeType {None,UnitProduction,Defense,Atmosphere};

    private const float noPercent = 0f;
    private const float smallPercent = .25f;
    private const float mediumPercent = .50f;
    private const float largePercent = .75f;
    private const float allPercent = 1f;


    //player only has option
    public static float RoundPercentToClosestOption(float percent)
    {
       if(percent <= 0)
       {
            
            return noPercent;
       }
       else if(percent > 0 && percent <= smallPercent)
       {
            
            return smallPercent;
       }
       else if(percent > smallPercent && percent <= mediumPercent)
       {
           
            return mediumPercent;
       }
       else if (percent > mediumPercent && percent <= largePercent)
       {
            
            return largePercent;
       }

    
        return allPercent;
    }

}

//class to hold multi attack info with
public class MultiAttackInfo
{
    private float percent;
    private AIBehavior Ai;

    public float Percent
    {
        get
        {
            return percent;
        }
    }

    public AIBehavior AI
    {
        get
        {
            return Ai;
        }
    }

    public MultiAttackInfo(AIBehavior Ai, float percent)
    {
        this.percent = percent;
        this.Ai = Ai;
    }
}
