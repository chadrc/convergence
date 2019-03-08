using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;

public static class LevelStatusCode
{
    public const string Error = "Error";
    public const string Victory = "Victory";
    public const string Defeat = "Defeat";
    public const string Tie = "Tie";
    public const string Surrender = "Surrender";
    public const string Disconnect = "Disconnect";
}

public static class Analytics
{
    public static void SendLevelEnd(LevelEndData data)
    {
        CheckResult(UnityEngine.Analytics.Analytics.CustomEvent("Level End", data.ToDictionary()));
    }

    public static void SendCampaignLevelStart(CampaignLevelStartData data)
    {
        CheckResult(UnityEngine.Analytics.Analytics.CustomEvent("Campaign Level Start", data.ToDictionary()));
    }

    public static void SendMultiplayerLevelStart(MultiplayerLevelStartData data)
    {
        CheckResult(UnityEngine.Analytics.Analytics.CustomEvent("Multiplayer Level Start", data.ToDictionary()));
    }

    public static void SendPlayerChoiceData(PlayerChoiceData data)
    {
        CheckResult(UnityEngine.Analytics.Analytics.CustomEvent("Player Choices", data.ToDictionary()));
    }

    private static void CheckResult(UnityEngine.Analytics.AnalyticsResult result)
    {
        Debug.Log("Analytics Result: " + result.ToString());
        switch(result)
        {
            case UnityEngine.Analytics.AnalyticsResult.Ok:

                break;
            case UnityEngine.Analytics.AnalyticsResult.NotInitialized:

                break;
            case UnityEngine.Analytics.AnalyticsResult.InvalidData:

                break;
            case UnityEngine.Analytics.AnalyticsResult.AnalyticsDisabled:

                break;
            case UnityEngine.Analytics.AnalyticsResult.SizeLimitReached:

                break;
            case UnityEngine.Analytics.AnalyticsResult.TooManyItems:

                break;
            case UnityEngine.Analytics.AnalyticsResult.TooManyRequests:

                break;
            case UnityEngine.Analytics.AnalyticsResult.UnsupportedPlatform:

                break;
            default:

                break;
        }
    }
}

public struct CampaignLevelStartData
{
    public string campaignName;
    public string levelName;

    public Dictionary<string, object> ToDictionary()
    {
        var d = new Dictionary<string, object>()
        {
            { "Campaign ID", campaignName },
            { "Level ID", levelName },
        };

        return d;
    }
}

public struct MultiplayerLevelStartData
{
    public float waitTime;

    public Dictionary<string, object> ToDictionary()
    {
        var d = new Dictionary<string, object>()
        {
            { "Wait Time", waitTime },
        };

        return d;
    }
}

public struct LevelEndData
{
    public string statusCode;
    public float playTime;
    public int unitsCreated;
    public int unitsDestroyed;
    public int playerControlledPlanets;
    public int neutralPlanets;
    public int opponentControlledPlanets;

    public Dictionary<string, object> ToDictionary()
    {
        var d = new Dictionary<string, object>()
        {
            { "Status", statusCode },
            { "Play Time", playTime },
            { "Units Created", unitsCreated },
            { "Units Destroyed", unitsDestroyed },
            { "Player Controlled Planets", playerControlledPlanets },
            { "Neutral Planets", neutralPlanets },
            { "Opponent Controlled Planets", opponentControlledPlanets },
        };

        return d;
    }
}

public struct PlayerChoiceData
{
    public int enduranceUpgradeCount;
    public int productionUpgradeCount;
    public int unitPercentChanged;
    public float timeFor25Unit;
    public float timeFor50Unit;
    public float timeFor75Unit;
    public float timeFor100Unit;

    public Dictionary<string, object> ToDictionary()
    {
        var d = new Dictionary<string, object>()
        {
            { "Endurance Upgrade Count", enduranceUpgradeCount },
            { "Production UpgradeCount", productionUpgradeCount },
            { "Unit Percent Changed", unitPercentChanged },
            { "Time with 25%", timeFor25Unit },
            { "Time with 50%", timeFor50Unit },
            { "Time with 75%", timeFor75Unit },
            { "Time with 100%", timeFor100Unit },
        };

        return d;
    }
}