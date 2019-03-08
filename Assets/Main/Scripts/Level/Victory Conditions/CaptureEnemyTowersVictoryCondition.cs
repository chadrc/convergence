using UnityEngine;
using System.Collections;

public class CaptureEnemyTowersVictoryCondition : MonoBehaviour
{
    Coroutine routine;
	// Use this for initialization
	void Start ()
    {
        TowerController.TowerConverted += OnTowerConverted;
        UnitBehavior.UnitKilled += OnUnitKilled;
        ConvergenceController.ConvergenceOccurred += OnConvergenceOccurred;
        foreach(var t in TowerController.GetAllTowers())
        {
            t.AttackedByUnit += OnUnitKilled;
        }
	}

    void OnDestroy()
    {
        ConvergenceController.ConvergenceOccurred -= OnConvergenceOccurred;
        TowerController.TowerConverted -= OnTowerConverted;
        UnitBehavior.UnitKilled -= OnUnitKilled;
    }

    void OnConvergenceOccurred()
    {
        CheckResult();
    }

    void OnUnitKilled(UnitBehavior unit)
    {
        CheckResult();
    }

    void OnTowerConverted(TowerBehavior tower, int fromFraction, int toFaction)
    {
        CheckResult();
    }

    void CheckResult()
    {
        // Victory condition to be move to separate class.
        int playerTowerCount = TowerController.GetTowerCountForFaction(FactionController.PlayerFaction);
        int playerUnitCount = UnitController.GetFieldUnitCountForFaction(FactionController.PlayerFaction);
        int enemyUnitCount = UnitController.GetFieldUnitCountForFaction(FactionController.OtherFaction1);
        int enemyTowerCount = TowerController.GetTowerCountForFaction(FactionController.OtherFaction1);

        //Debug.Log("Player (Towers:" + playerTowerCount + " | Units:" + playerUnitCount);
        //Debug.Log("Enemy (Towers:" + enemyTowerCount + " | Units:" + enemyUnitCount);

        if (enemyTowerCount == 0 && enemyUnitCount == 0)
        {
            LevelController.EndLevel();
            if (routine != null)
            {
                StopCoroutine(routine);
            }
        }
        else if (playerTowerCount == 0 && playerUnitCount == 0)
        {
            LevelController.EndLevel(false);
            if (routine != null)
            {
                StopCoroutine(routine);
            }
        }

        if (routine == null)
        {
            routine = StartCoroutine(RedundantCheckResult());
        }
    }

    IEnumerator RedundantCheckResult()
    {
        while(true)
        {
            yield return new WaitForSeconds(1.0f);
            CheckResult();
        }
    }
}
