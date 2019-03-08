using UnityEngine;
using System.Collections;

public class MapControl : MonoBehaviour
{
	/// <summary>
	/// The middle of the gradient being used represents white/fully visible/full color.
	/// This is the neutral area where the map indicates that no particular faction has control.
	/// </summary>
	public const float GRADIENT_MIDDLE_TIME = 0.5f;
	public const float COLOR_TRANSITION_TIME = 1.0f;

	public delegate void MapControlDelegate();

	public static event MapControlDelegate TowerControlChangeEvent;

	private SpriteRenderer renderer;
	private	Gradient grad;
	private GradientAlphaKey[] gradAlphaKey;
	private GradientColorKey[] gradColorKey;

	private Color maxEnemyColor
	{ 
		get
		{ 
			return new Color(0.5f, 0.85f, 1.0f, 1.0f); 
		} 
	}
	private Color maxPlayerColor
	{ 
		get
		{ 
			return new Color(1.0f, 0.5f, 0.5f, 1.0f); 
		} 
	}


	void Awake()
	{
		renderer = GetComponent<SpriteRenderer>();

		// Gradient used for changing the background color based on what faction has control.
		grad = new Gradient();

		// Three elements for red, white and blue.
		// The time sets the position of said colors in the associated color key.
		gradColorKey = new GradientColorKey[3];
		gradColorKey[0].color = maxEnemyColor;
		gradColorKey[0].time = 0.0f;
		gradColorKey[1].color = Color.white;
		gradColorKey[1].time = 0.5f;
		gradColorKey[2].color = maxPlayerColor;
		gradColorKey[2].time = 1.0f;

		//Only need one alpha key because the background has no transparency.
		gradAlphaKey = new GradientAlphaKey[1];
		gradAlphaKey[0].alpha = 1.0f;
		gradAlphaKey[0].time = 0.0f;

		// Create the gradient with the devised color key and alpha key.
		grad.SetKeys(gradColorKey, gradAlphaKey);
	}


	void Start()
	{
		TowerControlChangeEvent += AdjustMapColorWrapper;
	}


	/// <summary>
	/// Wrapper function for the AdjustMapColor coroutine.
	/// </summary>
	private void AdjustMapColorWrapper()
	{
		// In the event that the AdjustMapColor coroutine is already running, it will get cut off.
		// The most recent version will take precendence, in essence.
		StopCoroutine("AdjustMapColor");
		StartCoroutine(AdjustMapColor((float)TowerController.GetTowersForFaction(1).Count, 
			(float)TowerController.GetTowersForFaction(2).Count, 
			(float)TowerController.GetAllTowers().Count));
	}


	/// <summary>
	/// This method is used to adjust the background color based on what faction has control of the map, or doesn't.
	/// </summary>
	private IEnumerator AdjustMapColor(float playerTowersControlled, float enemyTowersControlled, float totalTowers)
	{
		// How far along the lerp is.
		float progress = 0;
		// Time step that will be added to the progress each frame.
		float timeStep = COLOR_TRANSITION_TIME * Time.deltaTime;
		float totalControlledTowers = playerTowersControlled + enemyTowersControlled;
		// A negative value represents the enemies being in control. Positive means the player is in control.
		float controlValue = (((playerTowersControlled - enemyTowersControlled) / totalTowers) 
			* (totalControlledTowers / totalTowers));
		// The gradient will decrease or increase based on what faction is in control.
		// If enemies are in control, the below will begin to move towards the blue end of the gradient. (decrease)
		// If the player is in control, it will move towards the red gradient (increase)
		Color colorToLerpTo = grad.Evaluate(GRADIENT_MIDDLE_TIME + controlValue);
		Color curColor = renderer.color;

		while (progress < 1)
		{
			renderer.color = Color.Lerp(curColor, colorToLerpTo, progress);
			progress += timeStep;
			yield return null;
		}

		yield break;
	}


	/// <summary>
	/// Ensures that the event being raised has listeners.
	/// </summary>
	public static void TowerControlChangeEventTrigger()
	{
		if (TowerControlChangeEvent != null)
		{
			//0 == neutral, 1 == player faction, 2 == enemy faction
			TowerControlChangeEvent();
		}
	}


	/// <summary>
	/// Unsubcribes methods.
	/// </summary>
	void OnDisable()
	{
		TowerControlChangeEvent -= AdjustMapColorWrapper;
	}
}
