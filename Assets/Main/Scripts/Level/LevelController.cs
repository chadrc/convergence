using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class LevelController : MonoBehaviour 
{
    public static event System.Action LevelStart;
    public static event System.Action<bool> LevelEnd;
	public static event System.Action<bool> PlayPaused;

	private static LevelController current;
    
    public LevelList DefaultList;
	public LevelData CurrentLevel;

    public bool StartLevelWithSceneLoad = true;

	private GameObject rootLevelObj;
    private float playTime = 0;
    private bool levelPlaying = false;

    public static float PlayTime { get { return current.playTime; } }
    public static bool Paused { get { return Time.timeScale == 0.0f; } }
    public static bool LevelOver { get { return !current.levelPlaying; } }

    void Awake ()
    {
        // Redundant Reset
        ConvergenceController.DestoryConvergenceController();
        Time.timeScale = 1.0f;
        FactionController.SetDefaultFactions();

        // Raise warning if more than one LevelController exists.
        if (current != null)
		{
			Debug.LogWarning("Multiple LevelControllers created. Replacing current.");
		}
		current = this;

        // Check Game for level list
        if (!Game.HasLevelList())
        {
            Game.SetLevelList(DefaultList);
        }

        // Check Game for a selected level
        var selectedLevel = Game.CurrentLevel;

        // If Game has one, else use one assigned in editor 
        if (selectedLevel != null)
		{
			CurrentLevel = selectedLevel;
		}

        // If there isn't one and CurrentLevel is still null (i.e. nothing assigned in editor) error is raised.
        if (CurrentLevel == null)
		{
			Debug.LogError("Attempting to start game without any level data.");
			return;
		}
	}

	// Attempts to load CurrentLevel by instantiating the GameObject
	void Start()
    {
        if (StartLevelWithSceneLoad)
        {
            LoadLevel(); 
        }
	}

    void Update()
    {
        if (levelPlaying)
        {
            playTime += Time.deltaTime;
        }

        //if (Input.GetKeyUp(KeyCode.S))
        //{
        //    Time.timeScale += .5f;
        //}

        if (Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene("LevelSelect");
        }
    }

	void OnDestroy()
	{
        ConvergenceController.DestoryConvergenceController();
		current = null;
	}

    private void LoadLevel()
    {
        if (CurrentLevel == null || CurrentLevel.Prefab == null)
        {
            Debug.LogError("Attempting to start level with invalid level data.");
            return;
        }

        var obj = GameObject.Instantiate(CurrentLevel.Prefab);
        if (obj == null)
        {
            Debug.LogError("Failed to create prefab of level: " + CurrentLevel.Name);
            return;
        }

		rootLevelObj = obj;

        levelPlaying = true;
        if (CurrentLevel.convergences.Count > 0)
        {
            ConvergenceController.CreateConvergenceController(this);
        }

		if (LevelStart != null)
		{
			//Debug.Log("Level Starting");
			LevelStart();
		}
    }

	public static void DestroyLevel()
	{
		Debug.Log("Destroying Levelfab");
		GameObject.Destroy(current.rootLevelObj);
	}

	public static void StartCreateLevel()
    {
        current.LoadLevel();
    }

	public static LevelData GetCurrentLevel()
	{
		return current.CurrentLevel;
	}

    public static void LoadNextLevelInList()
    {
        if (Game.HasNextLevelInList())
        {
            Game.SetCurrentLevelToNextInList();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            SceneManager.LoadScene("LevelSelect");
        }
    }

    public static void EndLevel(bool victory = true)
    {
        //Pause();
        current.levelPlaying = false;
        //Debug.Log("PT: " + current.playTime);
        PlayerInfo.EditLevel(current.CurrentLevel.name, victory, current.playTime);
        PlayerInfo.SavePlayerData();
        if (LevelEnd != null)
        {
            LevelEnd(victory);
        }
    }

    public static void Unpause()
    {
        if (Time.timeScale == 0.0f)
        {
			Time.timeScale = 1.0f;
			if (PlayPaused != null)
			{
				PlayPaused (false);
			}
        }
    }

    public static void Pause()
    {
        if (Time.timeScale == 1.0f)
        {
            Time.timeScale = 0.0f;
			if (PlayPaused != null)
			{
				PlayPaused (true);
			}
        }
    }
}
