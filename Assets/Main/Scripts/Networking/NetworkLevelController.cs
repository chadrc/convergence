using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;
using System.Collections;

public enum MatchOption
{
    Join = 0,
    Create,
    QuickMatch,
}

public class NetworkLevelController : NetworkManager
{
    public static bool Networked { get; private set; }
    public static MatchOption Option;
    public static string RoomName;
    public static string RoomPass;

    public GameObject NetworkPanel;
	public GameObject NameInputPanel;
	public GameObject EndLevelPanel;
    public Text NetworkDisplayMsg;
    public Button ReadyBtn;
	public Button MainMenuBtn;
	public Button CancelBtn;
    public LevelData NetworkLevel;
	public Text NameText;
	public Text OpponentNameText;
	public NetworkVersusPanelViewController VsCtrl;
	public Text WaitingText;

    private bool ready = false;
    private bool opponentReady = false;
    private bool setupComplete = false;
	private bool findingMatch = false;
	private bool levelEnded = false;
	private bool nameRecieved = false;

	private CreateMatchResponse createdMatch;

    static NetworkLevelController()
    {
        Networked = false;
    }

	#region Unity Callbacks

	// Use this for initialization
	void Start () 
	{
		NetworkCommands.OpponentNameRecieved += OnOpponentNameRecieved;;
        NetworkCommands.OpponentReady += OnOpponentReady;
		NetworkCommands.LevelCreated += OnLevelCreated;
		LevelController.LevelEnd += OnLevelEnd;

        switch(Option)
        {
            case MatchOption.QuickMatch:
                OnFindGameBtnPressed();
                break;

            case MatchOption.Join:
                JoinMatch(RoomName, RoomPass);
                break;

            case MatchOption.Create:
                CreateMatch(RoomName, RoomPass);
                break;

            default: 
                NetworkDisplayMsg.text = "Network Error";
                ResetScene();
                break;
        }
        CancelBtn.gameObject.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () 
    {
		
	}

    void OnDestroy()
    {
        NetworkCommands.OpponentReady -= OnOpponentReady;
		NetworkCommands.OpponentNameRecieved -= OnOpponentNameRecieved;
        NetworkCommands.LevelCreated -= OnLevelCreated;
        LevelController.LevelEnd -= OnLevelEnd;
    }

	#endregion

	#region Event Callbacks

	void OnLevelCreated ()
	{
		NetworkDisplayMsg.text = "Level Created";
		ReadyBtn.gameObject.SetActive(true);

		// To be changed (possible removed) when procedural creation is implemented
		// Performed only on client
		if (!NetworkServer.active)
		{
			LevelController.StartCreateLevel();
		}
	}

	private void OnOpponentNameRecieved (string name)
	{
		if (nameRecieved)
		{
			return;
		}

		nameRecieved = true;
		NetworkDisplayMsg.text = "Opponent Info Recieved";
		OpponentNameText.text = name;
		VsCtrl.Show(PlayerInfo.PlayerName, name);
		if (!NetworkServer.active)
		{
			// Send our name back to the server
			NetworkCommands.SendName(PlayerInfo.PlayerName);
		}
	}

	private void OnOpponentReady(string opponentName)
	{
		NetworkDisplayMsg.text = "Opponent Ready";
		opponentReady = true;
		if (ready)
		{
			SetupGame();
		}
	}

	private void OnLevelEnd(bool victory)
	{
		levelEnded = true;
        BreakDown();
	}

	#endregion

	#region Button Actions

    public void OnReadyBtnPressed()
    {
		ready = true;
		NetworkCommands.SendReadySig(PlayerInfo.PlayerName);
        if (opponentReady)
        {
            SetupGame();
        }
		else
		{
			WaitingText.gameObject.SetActive(true);
			ReadyBtn.gameObject.SetActive(false);
		}
    }
    
    public void OnFindGameBtnPressed()
	{
		if (createdMatch != null)
		{
			Debug.LogWarning("Created match still exists.");
			return;
		}

		StartMatchMaker();
		findingMatch = true;
        NetworkDisplayMsg.text = "Finding Match";
        matchMaker.ListMatches(0, 20, "", OnRecievedListMatches);
        
		MainMenuBtn.gameObject.SetActive(false);
		CancelBtn.gameObject.SetActive(true);
    }

	public void OnCancelBtnPressed()
	{
		findingMatch = false;
		BreakDown();
		ResetScene("Matchmaking canceled.");
	}

	public void OnNewMatchBtnPressed()
	{
		ResetScene();
		BreakDown();
	}

	public void OnConcedBtnPressed()
	{
		BreakDown();
		ResetScene("Match conceded.");
	}

	public void OnMainMenuBtnPressed()
	{
		BreakDown();
		SceneManager.LoadScene("LevelSelect");
	}

	#endregion

	#region Utility

	private void BreakDown()
	{
//		Debug.Log("Breaking Down");
		if (NetworkServer.active)
		{
            if (matchMaker != null && createdMatch != null)
            {
                matchMaker.DestroyMatch(createdMatch.networkId, OnMatchDestroy);
            }
            StopServer();
            try
            {
                StopClient();
            }
            catch (System.Exception)
            {
                Debug.LogWarning("Could not stop client.");
            }
		}
		else
		{
			StopClient();
		}

		Networked = false;
		LevelController.DestroyLevel();
        RoomName = "";
        RoomPass = "";
	}

	private void ResetScene(string networkMsg = "")
    {
        FactionController.SetDefaultFactions();
        LevelController.Unpause();

        SceneManager.LoadScene("LevelSelect");

		//EndLevelPanel.SetActive(false);
		//VsCtrl.Hide();
		//FindMatchBtn.gameObject.SetActive(true);
		//MainMenuBtn.gameObject.SetActive(true);
		//ReadyBtn.gameObject.SetActive(false);
		//CancelBtn.gameObject.SetActive(false);
		//ChangeNamePanel.SetActive(true);
		//NetworkPanel.SetActive(true);
		//NetworkDisplayMsg.text = networkMsg;
		//ready = false;
		//opponentReady = false;
		//setupComplete = false;
		//nameRecieved = false;
		//WaitingText.gameObject.SetActive(false);
	}

    private void SetupGame()
    {
        if (setupComplete)
        {
            return;
        }

        NetworkPanel.SetActive(false);
        setupComplete = true;
	}

	#endregion

	#region Matchmaker Callbacks

	private void OnMatchDestroy(BasicResponse resp)
	{
//		Debug.Log("Match Destroyed");
		createdMatch = null;
		StopHost();
	}

    private void CreateMatch(string name, string pass)
    {
        if (name == "")
        {
            SceneManager.LoadScene("LevelSelect");
        }

        StartMatchMaker();
        matchMaker.CreateMatch(name, 2, true, pass, OnCreateMatch);
        NetworkDisplayMsg.text = "Creating Room: " + name + " | Password: " + pass;
    }

    private void JoinMatch(string name, string pass)
    {
        if (name == "")
        {
            SceneManager.LoadScene("LevelSelect");
        }

        StartMatchMaker();
        var req = new ListMatchRequest();
        req.pageSize = 1;
        req.pageNum = 0;
        req.nameFilter = name;
        req.includePasswordMatches = pass != "";
        matchMaker.ListMatches(req, OnRecievedListMatchesJoinOnly);
        NetworkDisplayMsg.text = "Searching for room: " + name;
    }

    private void OnRecievedListMatchesJoinOnly(ListMatchResponse resp)
    {
        OnMatchList(resp);
        if (matches == null || matches.Count == 0)
        {
            NetworkDisplayMsg.text = "Could not find game named: " + RoomName;
            SceneManager.LoadScene("LevelSelect");
        }
        else
        {
            NetworkDisplayMsg.text = "Joining room: " + RoomName;
            matchMaker.JoinMatch(resp.matches[0].networkId, RoomPass, OnJoinMatch);
        }
    }

    private void OnRecievedListMatches(ListMatchResponse resp)
    {
        // Call base functionality
        OnMatchList(resp);

		if (!findingMatch)
		{
			return;
		}

        if (matches == null || matches.Count == 0)
        {
            // Create match if no matches exist
            matchMaker.CreateMatch("Generic Convergence Match - " + System.DateTime.Now, 2, true, "", OnCreateMatch);
        }
        else
        {
            // Attempt to join first match
            matchMaker.JoinMatch(resp.matches[0].networkId, "", OnJoinMatch);
        }
    }

	private void OnJoinMatch(JoinMatchResponse resp)
    {
        // Call base functionality
		try
		{
			OnMatchJoined(resp);
		}
		catch (System.Exception)
		{
			// Attempt to Join next match in list
			NetworkDisplayMsg.text = "Failed to join room.";
			ResetScene();
			return;
		}

		if (!findingMatch)
		{
			return;
		}
        //Debug.Log("On Join Match");
        if (resp.success)
        {
            NetworkDisplayMsg.text = "Match Found";
			StartCoroutine(TimeoutRoutine());
        }
        else
		{
			NetworkDisplayMsg.text = "Failed to join room.";
            ResetScene();
            // Attempt to join next match in list
        }
    }

    private void OnCreateMatch(CreateMatchResponse resp)
    {
        // Call base functionality
		OnMatchCreate(resp);

		if (!findingMatch)
		{
			return;
		}

        //Debug.Log("On Create Match");
        if (resp.success)
        {
			createdMatch = resp;
        }
        else
        {
            NetworkDisplayMsg.text = "Couldn't create room.";
            BreakDown();
            ResetScene();
        }
    }

	#endregion

	#region Server and Client Callbacks

    // Called on server when client connects
    public override void OnServerConnect(NetworkConnection conn)
    {
		//Debug.Log("Client Connected: " + conn.connectionId);
		base.OnServerConnect(conn);

		// Ingore if on host, Initialization handled elsewhere
        if (conn.connectionId == 0)
        {
            return;
        }

		// Send name to client when they connect
		StartCoroutine(SendNameRoutine());
        NetworkDisplayMsg.text = "Match Found - Connected";
        //Debug.Log("Server Opponent: " + conn.address);
    }

	public override void OnServerDisconnect(NetworkConnection conn)
	{
		base.OnServerDisconnect(conn);

		if (!levelEnded)
		{
			BreakDown();
			ResetScene("Disconnected from server.");
		}
	}

    // Called on a client when connected to server
	public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        Networked = true;
		findingMatch = false;

		// Ready button shouldn't show for host until a client has connected
		if (!NetworkServer.active)
		{
			FactionController.SetNetworkClientFactions();
			NetworkDisplayMsg.text = "Connected";
		}
    }

	public override void OnClientDisconnect(NetworkConnection conn)
	{
		base.OnClientDisconnect(conn);
		this.StopClient();

		if (!levelEnded)
		{
			BreakDown();
			ResetScene("Disconnected from server.");
		}
	}

	#endregion

	private IEnumerator SendNameRoutine()
	{
		yield return new WaitForSeconds(.5f);

		int attemptLimit = 5;
		int attempts = 0;

		while (!nameRecieved)
		{
			NetworkCommands.SendName(PlayerInfo.PlayerName);
			yield return new WaitForSeconds(1.0f);
			attempts++;
			if (attempts >= attemptLimit)
			{
				break;
			}
		}
	}

	private IEnumerator TimeoutRoutine()
	{
		yield return new WaitForSeconds(30.0f);

		if (!Networked)
		{
			Debug.Log("Join Match Timeout");
			findingMatch = false;
			BreakDown();
			ResetScene("Connection timeout - Restarting search");
			OnFindGameBtnPressed();
		}
	}
}
