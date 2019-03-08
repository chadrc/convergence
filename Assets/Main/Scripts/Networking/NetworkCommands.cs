using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class NetworkCommands : NetworkBehaviour 
{
    private static NetworkCommands current;

	public static event System.Action<string> OpponentReady;
	public static event System.Action<string> OpponentNameRecieved;
	public static event System.Action LevelCreated;

    private Coroutine syncRoutine;
    private List<TowerBehavior> towers;
    private List<UnitGroup> curFactionGroups;

    private List<TowerBehavior> Towers
    {
        get
        {
            var allTowers = TowerController.GetAllTowers();
            if (towers == null || towers.Count != allTowers.Count)
            {
                towers = new List<TowerBehavior>(TowerController.GetAllTowers());
                towers.Sort((t1, t2) => {
                    return t1.Index < t2.Index ? -1 : 1;
                });

				foreach (var t in towers)
				{
					if (!NetworkServer.active)
					{
						t.DisableUnitGeneration();
					}
					else
					{
						t.ChangedFaction += OnTowerConverted;
					}
				}

                //Debug.Log("Tower Count: " + towers.Count);
            }

            return towers;
        }
    }

	#region Unity Callbacks

	// Use this for initialization
	void Awake () 
    {
	    current = this;
        LevelController.LevelStart += OnLevelStart;
	}

    void Start()
    {
        // Holds most recent unit group created by each faction
        // Subract one when accessing becuase 0 is neutral faction, 1 is server faction, 2 is client faction
        curFactionGroups = new List<UnitGroup>();
        curFactionGroups.Add(null); // Server Faction
        curFactionGroups.Add(null); // Client Faction
        //Debug.Log("Faction Group Count: " + curFactionGroup.Count);
    }
	
	// Update is called once per frame
	void Update ()
    {

	}

    void OnDestory()
    {
        current = null;
    }

	#endregion

	#region Event Callbacks

	private void OnTowerConverted(int toFaction)
	{
		int opponentTowerCount = 0;
		int myTowerCount = 0;
		foreach(var t in Towers)
		{
			if (t.Faction == FactionController.OtherFaction1)
			{
				opponentTowerCount++;
			}
			else if (t.Faction == FactionController.PlayerFaction)
			{
				myTowerCount++;
			}
		}

		if (myTowerCount == 0)
		{
			RpcEndLevel(FactionController.OtherFaction1);
		}
		else if (opponentTowerCount == 0)
		{
			RpcEndLevel(FactionController.PlayerFaction);
		}
	}

    private void OnLevelStart()
    {
        //towers = new List<TowerBehavior>(TowerController.GetAllTowers());
        //towers.Sort((t1, t2) => {
        //    return t1.Index < t2.Index ? -1 : 1;
        //});
        //Debug.Log("Tower Count: " + towers.Count);
    }

	#endregion

	#region Commands
    
    [Command]
    private void CmdSendUnits(int faction, int fromTowerID, int toTowerID)
    {
//        Debug.Log("Request to send units from tower " + fromTowerID + " to tower " + toTowerID);

        // Sync routine checked and called here to give client and server proper initialization time
        // Look for potentially better location
        if (current.syncRoutine == null)
        {
//            Debug.Log("Starting Sync Routine");
            current.syncRoutine = current.StartCoroutine(current.TowerSyncCoroutine());
        }

        // Validation needed
        RpcSendUnits(faction, fromTowerID, toTowerID);
    }

    [Command]
	private void CmdReadySig(string name)
    {
        OpponentReady(name);
        //Debug.Log("Cmd Ready Sig");
    }

	[Command]
	private void CmdSendName(string name)
	{
		Debug.Log("Opponent Name Received");
		OpponentNameRecieved(name);
		CreateLevel();
		RpcLevelCreationComplete();
	}

    [Command]
    private void CmdTowerSelected(int faction, int towerID, float percent)
    {
        // Validation needed
        RpcTowerSelected(faction, towerID, percent);
    }

    [Command]
    private void CmdDeselectTower(int faction)
    {
        // Validation needed
        RpcDeselectTower(faction);
    }

	#endregion

	#region Client Rpc

    [ClientRpc]
    private void RpcSendUnits(int faction, int fromTowerID, int toTowerID)
    {
        var tower = Towers[fromTowerID];
        var toTower = Towers[toTowerID];
        if (curFactionGroups[faction-1] == null)
        {
            Debug.LogError("Failed to send Units: No unit group exists for faction " + faction);
            return;
        }
        TowerBehavior.MoveGroupFromTo(curFactionGroups[faction-1], tower, toTower);
//		tower.HideAtmosphere();
//		toTower.HideAtmosphere();
    }

    [ClientRpc]
	private void RpcReadySig(string name)
    {
		// Ignore if on host
		if (!NetworkServer.active)
		{
			OpponentReady(name);
		}
    }

	[ClientRpc]
	private void RpcSendName(string name)
	{
		Debug.Log("Rpc Send Name");
		// Ignore if on host
		if (!NetworkServer.active)
		{
			Debug.Log("Opponent Name Recieved");
			OpponentNameRecieved(name);
		}
	}

    [ClientRpc]
    private void RpcTowerSelected(int faction, int towerID, float percent)
    {
        var tower = Towers[towerID];
        var group = UnitController.CreateUnitGroupForFaction(faction, Mathf.FloorToInt(tower.StationedUnits * percent));
        group.PrepareUnits(tower);
        curFactionGroups[faction - 1] = group;
//		tower.ShowAtmosphere();
    }

    [ClientRpc]
	private void RpcDeselectTower(int faction)
    {
//        Debug.Log("Faction Deselecting: " + faction);
        if (curFactionGroups[faction - 1] == null)
        {
            Debug.LogError("Failed to deselect Tower: No unit group exists for faction " + faction);
            return;
		}
//		var tower = Towers[towerID];
//		tower.HideAtmosphere();
        curFactionGroups[faction - 1].UnprepareUnits();
    }

    [ClientRpc]
    private void RpcSyncTowerCounts(int[] counts)
    {   
        if (counts.Length != Towers.Count)
        {
            Debug.LogError("Count mismatch between count list recived from server and tower count in scene.");
            return;
        }

        foreach(var t in Towers)
        {
//            Debug.Log("New Count " + t.Index + ": " + counts[t.Index]);
            t.SetUnits(counts[t.Index]);
        }
    }

	[ClientRpc]
	private void RpcEndLevel(int victorFaction)
	{
		if (victorFaction == FactionController.PlayerFaction)
		{
			LevelController.EndLevel(true);
			PlayerInfo.MultiplayerWins++;
		}
		else
		{
			LevelController.EndLevel(false);
			PlayerInfo.MultiplayerDefeats++;
		}
	}

	[ClientRpc]
	private void RpcLevelCreationComplete()
	{
		LevelCreated();
	}

	#endregion

	#region Static

	public static void SendReadySig(string name)
    {
        if (NetworkServer.active)
        {
            current.RpcReadySig(name);
        }
        else
        {
            current.CmdReadySig(name);
        }
    }

	public static void SendName(string name)
	{
		if (NetworkServer.active)
		{
			Debug.Log("Server sending name to client");
			current.RpcSendName(name);
		}
		else
		{
			Debug.Log("Client sending name to server");
			current.CmdSendName(name);
		}
	}

    public static void SelectTower(int faction, int towerID, float percent)
    {
        current.CmdTowerSelected(faction, towerID, percent);
    }

    public static void DeselectTower(int faction)
    {
        current.CmdDeselectTower(faction);
    }
    
    public static void SendUnits(int faction, int fromTowerID, int toTowerID)
    {
        //Debug.Log("Sending Units");
        current.CmdSendUnits(faction, fromTowerID, toTowerID);
    }

	#endregion

	private void CreateLevel()
	{
		LevelController.StartCreateLevel();
	}

	// Coroutine to be only started on the server to sync the tower counts for units
    private IEnumerator TowerSyncCoroutine()
    {
        while(true)
        {
            int[] counts = new int[Towers.Count];
            int i = 0;
            foreach(var t in Towers)
            {
                counts[i] = t.StationedUnits;
                i++;
//                Debug.Log("Tower " + i + ": " + t.StationedUnits);
            }
            RpcSyncTowerCounts(counts);
            yield return new WaitForSeconds(1.0f);
        }
    }
}
