using UnityEngine;
using System.Collections;

/// <summary>
/// Used to initialize any data from editor to Game script.
/// </summary>
public class GameInitializationBehavior : MonoBehaviour
{
	public GlobalTowerInfo TowerInfo;
    public Texture2D CursorTexture;

	void Awake()
	{
        //Debug.Log("Game Init");
		Game.EternalBehaviour = this;
		TimeMechanic.RoutineBehavior = this;
		if (!Game.Initialized)
		{
			Game.Initialize(this);
			Preferences.Initialize();
			PlayerInfo.LoadPlayerDataFromPlayerPrefs();
            Cursor.SetCursor(CursorTexture, new Vector2(CursorTexture.width/2, CursorTexture.height/2), CursorMode.Auto);
		}
	}
}
