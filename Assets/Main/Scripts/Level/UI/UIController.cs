using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using System;

public class UIController : MonoBehaviour, IPointerResponder
{
	private const int LeftClickID = -1;
	private const int RightClickID = -2;
	private const int MiddleClickID = -3;

	public static event System.Action<float> UnitPercentChanged;
	public static event System.Action LevelQuit;
	public static event System.Action<CameraControl> CamControlInitialized;

	private static UIController current;

	public static UIPointerResponder DefaultResponder
	{
		get
		{
			return current.defaultResponder;
		}
	}

	public static CanvasScaler Scaler { get { return current == null ? null : current.GetComponent<CanvasScaler>(); } }

	[Header("Options")]
	public GameObject TowerButtonFab;
	public float TowerButtonRadius;

	[Header("References")]
	public GameObject ConvergencePanel;
	public GameObject PausedPanelObj;
	public GameObject SettingsPanelObj;
	public GameObject TowerButtonParent;
	public GameObject UnitFollowerPrefab;
	public Text ConvergenceCountdownText;
    public LevelEndPanelController LevelEndPanel;
    public GameObject NextLevelBtn;
	public Text OutcomeText;
	public GameObject PercentTogglesGroup;
	public Toggle PercentToggle25;
	public Toggle PercentToggle50;
	public Toggle PercentToggle75;
	public Toggle PercentToggle100;
	public RadialUnitPercentController UnitPercentController;
	public HelpAndTipsController helpAndTips;
	public CanvasGroup LoadPanel;
	public GameObject convergenceIndicator;
	public Image timerOverlay;
	public Text convergenceWaveText;

	// Privates
	private List<TowerButtonBehavior> buttons = new List<TowerButtonBehavior>();
	private List<TowerButtonBehavior> dynamicBtns = new List<TowerButtonBehavior>();

	private UIPointerResponder defaultResponder;
	private IPointerResponder currentResponder;

	private float unitPercent = 1.0f;
	private CameraControl camCtrl = null;

	private bool leftClickPanning = false;
	private float twoTouchDist = 0.0f;

	private float curConvergenceInterval;
	private float elapsedTime;

	public float UnitPercent { get { return unitPercent; } }

	public UIController Controller
	{
		get
		{
			throw new InvalidOperationException();
		}

		set
		{
			throw new InvalidOperationException();
		}
	}

	#region UnityCallbacks
	// Raise warning if more than one UIController exists.
	void Awake ()
	{
		if (current != null)
		{
			Debug.LogWarning("Multiple UIControllers created. Replacing current.");
		}
		current = this;
	}

	void Start()
	{
		TowerBehavior.UnitsMoved += TowerBehavior_UnitsMoved;
		LevelController.LevelEnd += OnLevelEnd;
		Preferences.PreferencesChanged += UpdatePreferences;
		// For updating the UI during gameplay.
		ConvergenceController.ConvergenceOccurred += UpdateConvergenceCountText;
		// The initial UI update which is set in after the convergence controller is created on leve start.
		LevelController.LevelStart += UpdateConvergenceCountText;
		LevelController.LevelStart += DisableConvergenceUI;
		defaultResponder = new UIPointerResponder();
		defaultResponder.Controller = this;
		currentResponder = defaultResponder;

		UpdatePreferences();
		UpdateConvergenceCountText();

    }

    // null static current in preparation for next UnitController
    void OnDestroy()
    {
        Time.timeScale = 1.0f;
        LevelController.LevelEnd -= OnLevelEnd;
        TowerBehavior.UnitsMoved -= TowerBehavior_UnitsMoved;
        Preferences.PreferencesChanged -= UpdatePreferences;
        ConvergenceController.ConvergenceOccurred -= UpdateConvergenceCountText;
        LevelController.LevelStart -= UpdateConvergenceCountText;
        LevelController.LevelStart -= DisableConvergenceUI;
        current = null;
    }

    // Update is called once per frame
    void Update ()
	{
		float till = 0;
		if (ConvergenceController.Exists)
		{
			till = ConvergenceController.TimeTillNextConvergence;
		}
		string txt = "";

		int min = (int)till / 60;
		string minStr = min.ToString();
		if (min < 10)
		{
			minStr = "0" + minStr;
		}

		float sec = till % 60.0f;
		int secInt = (int)sec;
		string secStr = secInt.ToString();
		if (secInt < 10)
		{
			secStr = "0" + secStr;
		}

		float ms = (sec - secInt) * 100.0f;
		string msStr = ms.ToString("0");
		if (ms < 10)
		{
			msStr = "0" + msStr;
		}
		txt = minStr + ":" + secStr + "." + msStr;

		if (till <= 0.01f)
		{
			ConvergenceCountdownText.text = "00:00.00";
		}
		else
		{
			ConvergenceCountdownText.text = txt;
		}

		foreach (var b in buttons) 
		{
			b.transform.position = Camera.main.WorldToScreenPoint (b.Tower.transform.position);
		}

		UnitPercentController.Reposition();

		// Section for controlling the convergence timer overlay's fill.
        if (ConvergenceController.Exists)
        {
            elapsedTime = ConvergenceController.CurrentInterval - till;
            timerOverlay.fillAmount = elapsedTime / ConvergenceController.CurrentInterval;
        }

		// Inputs
		if (Input.GetKeyDown(KeyCode.P)) // Pause
		{
			if (LevelController.Paused)
			{
				OnResumeBtnPressed();
			}
			else
			{
				OnPauseBtnPressed();
			}
		}

		// Ignore rest of input if Paused
		if (LevelController.Paused)
		{
			return;
		}

		if (Input.GetKeyDown(KeyCode.Alpha1)) // Set Percentage to 25%
		{
			unitPercent = 0.25f;
			PercentToggle25.isOn = true;
			RaiseUnitChangedEvent();
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2)) // Set Percentage to 50%
		{
			unitPercent = 0.5f;
			PercentToggle50.isOn = true;
			RaiseUnitChangedEvent();
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3)) // Set Percentage to 75% 
		{
			unitPercent = 0.75f;
			PercentToggle75.isOn = true;
			RaiseUnitChangedEvent();
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4)) // Set Percentage to 100%
		{
			unitPercent = 1.0f;
			PercentToggle100.isOn = true;
			RaiseUnitChangedEvent();
		}
	}


	#region Private Utility

	private void RaiseUnitChangedEvent()
	{
		if (UnitPercentChanged != null)
		{
			UnitPercentChanged(UnitPercent);
		}
	}

	private void RaiseLevelQuit()
	{
		if (LevelQuit != null)
		{
			LevelQuit();
		}
	}

	private void UpdateConvergenceCountText()
	{
        if (ConvergenceController.Exists)
        {
            convergenceWaveText.text = "Wave: " + ConvergenceController.ConvergenceWaveCount.ToString();
        }
	}

	#endregion

	#endregion

	#region PointerEvents

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (!leftClickPanning && eventData.pointerId == LeftClickID)
		{
			currentResponder.OnBeginDrag(eventData);
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (Input.touchCount == 2 && eventData.pointerId == 1)
		{
			float distance = Vector2.Distance (Input.GetTouch (0).position, Input.GetTouch (1).position);
			float dif = twoTouchDist - distance;
			float fracOfScreen = dif / Screen.height;
			float orthoDif = fracOfScreen * (camCtrl.YLimits.y - camCtrl.YLimits.x);
			if (Mathf.Abs(orthoDif) > .5f)
			{
				camCtrl.Zoom(new Vector2(0, orthoDif));
				twoTouchDist = distance;
			}
			else
			{
				camCtrl.Pan (eventData.delta);
			}
			return;
		}

		if (leftClickPanning)
		{
			camCtrl.Pan (eventData.delta);
		}
		else if (eventData.pointerId == RightClickID)
		{
			if (camCtrl != null)
			{
				camCtrl.Pan(eventData.delta);   
			}
		}
		else if (eventData.pointerId == LeftClickID)
		{
			currentResponder.OnDrag(eventData);
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (!leftClickPanning && eventData.pointerId == LeftClickID)
		{
			currentResponder.OnEndDrag(eventData);
		}

		leftClickPanning = false;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
        if (eventData.pointerId == LeftClickID)
        {
            currentResponder.OnPointerClick(eventData);
        }
	}

	/// <summary>
	/// Implements interface for IPointerDownHandler. Should only be called by Unity. Checks to see if the down event is over a tower button.
	/// </summary>
	/// <param name="data">Data.</param>
	public void OnPointerDown(PointerEventData eventData)
	{
		if (Input.touchCount == 2 && eventData.pointerId == 1)
		{
			twoTouchDist = Vector2.Distance (Input.GetTouch (0).position, Input.GetTouch (1).position);
			return;
		}

		if (eventData.pointerId == LeftClickID && Input.GetKey (KeyCode.LeftAlt))
		{
			leftClickPanning = true;
		}
		else if (eventData.pointerId == LeftClickID)
		{
			currentResponder.OnPointerDown(eventData);
		}
	}

	/// <summary>
	/// Implements interface for IPointerUpHandler. Should only be called by Unity. Checks to see if the up event is over a tower button.
	/// </summary>
	/// <param name="data">Data.</param>
	public void OnPointerUp(PointerEventData data)
	{
		if (!leftClickPanning && data.pointerId == LeftClickID)
		{
			currentResponder.OnPointerUp(data);
		}

		leftClickPanning = false;
	}

	public void OnScroll(PointerEventData eventData)
	{
		if (camCtrl != null)
		{
			camCtrl.Zoom(-eventData.scrollDelta);
		}
	}

	#endregion

	#region UnityInspectorConnections
	/// <summary>
	/// Changes the percentage amount that will be used when sending units. To be used only by linking to canvas elements in Unity inspector. 
	/// </summary>
	/// <param name="percent">Percent.</param>
	public void ChangeUnitPercent(float percent)
	{
		unitPercent = percent;
		RaiseUnitChangedEvent();
	}

	public void OnPauseBtnPressed()
	{
		PausedPanelObj.SetActive (true);
		SettingsPanelObj.SetActive(true);
		LevelController.Pause();
	}

	public void OnResumeBtnPressed()
	{
		PausedPanelObj.SetActive (false);
		SettingsPanelObj.SetActive(false);
		LevelController.Unpause();
	}

	public void OnMenuBtnPressed()
	{
		if (!LevelController.LevelOver)
		{
			RaiseLevelQuit();
		}
		LoadPanel.gameObject.SetActive(true);
		LoadPanel.alpha = 1.0f;
		SceneManager.LoadScene("LevelSelect");
	}

	public void OnNextLevelButtonPressed()
	{
		LoadPanel.gameObject.SetActive(true);
		LoadPanel.alpha = 1.0f;
		LevelController.LoadNextLevelInList();
	}

	public void OnRestartBtnPressed()
	{
		if (!LevelController.LevelOver)
		{
			RaiseLevelQuit();
		}
		LoadPanel.gameObject.SetActive(true);
		LoadPanel.alpha = 1.0f;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	#endregion

	#region Public
	// Checks for a tower button at position on screen. Uses TowerButtonRadius to determine range of the buttons.
	public TowerButtonBehavior GetTowerButton(Vector2 position)
	{
        // Scale down radius to account for multiple screen sizes
        //Debug.Log("Ratio: " + ratio);
        float minDist = 300f;
        float closestDist = float.MaxValue;
        TowerButtonBehavior closest = null;
		foreach (var btn in buttons)
		{
            float dist = Vector2.Distance(position, new Vector2(btn.transform.position.x, btn.transform.position.y));
            if (dist < minDist && dist < closestDist)
			{
                closestDist = dist;
                closest = btn;
			}
		}
		//Debug.Log("No button found");
		return closest;
	}
	#endregion

	#region PublicStatic

	public static HelpAndTipsController EnableHelpUI(IHelpAndTipsResponder responder)
	{
		current.helpAndTips.gameObject.SetActive(true);
		current.helpAndTips.Responder = responder;
		return current.helpAndTips;
	}

	public static void DisableHelpUI()
	{
		current.helpAndTips.Deactivate();
	}

	public static void EnableConvergenceCountUI(int count)
	{
		//current.ConvergenceCounter.gameObject.SetActive (true);
		//current.ConvergenceCounter.Initialize (count);
	}

	public static void DisableConvergenceUI()
	{		
		if (!ConvergenceController.Exists)
		{
			current.ConvergencePanel.SetActive(false);
		}
	}

	public static void LinkResponder(IPointerResponder responder)
	{
		responder.Controller = current;
	}

	public static void SetResponder(IPointerResponder responder)
	{
		current.currentResponder = responder;
	}

	public static void ResetResponder()
	{
		current.currentResponder = current.defaultResponder;
	}

	/// <summary>
	/// Initialize the camera control.
	/// </summary>
	/// <param name="tower">Tower to mark.</param>
	public static void InitializeCamControl(CamControlMod mod)
	{
		current.camCtrl = new CameraControl(mod);
		if (CamControlInitialized != null && mod.AllowPanZoom)
        {
            CamControlInitialized(current.camCtrl);
        }
		if (!mod.AllowPanZoom)
        {
            current.camCtrl = null;
        }
	}

	/// <summary>
	/// Tell the UIController that this tower will be moving and to update the position of the button accordingly.
	/// </summary>
	/// <param name="tower">Tower to mark.</param>
	public static void MarkTowerAsDynamic(TowerBehavior tower)
	{
		TowerButtonBehavior tbtn = null;
		foreach (var b in current.buttons)
		{
			if (b.Tower == tower)
			{
				current.dynamicBtns.Add (b);
				tbtn = b;
			}
		}

		if (tbtn == null) 
		{
			Debug.LogError ("No TowerButton for Tower found: " + tower);
		}
	}

	/// <summary>
	/// Creates a virtual button to represent pressing on the tower. Button is linked to tower.
	/// </summary>
	/// <param name="tower">Tower to create button for.</param>
	public static void CreateTowerButtonForTower(TowerBehavior tower)
	{
		Vector3 screenPoint = Camera.main.WorldToScreenPoint(tower.transform.position);
		var towerBtn = GameObject.Instantiate(current.TowerButtonFab) as GameObject;
		towerBtn.transform.position = screenPoint;
		towerBtn.transform.SetParent(current.TowerButtonParent.transform);
		var behavior = towerBtn.GetComponent<TowerButtonBehavior>();
		if (behavior == null)
		{
			Debug.LogError("TowerButtonFab of UIController does not have TowerButtonBehavior attached.");
			return;
		}
		behavior.Tower = tower;
		float camHeight = Camera.main.orthographicSize*2;
        //Debug.Log("Cam Height: " + camHeight);
        //Debug.Log("Screen Height: " + Screen.height);
		float towerGraphicSize = tower.GraphicObj.GetComponent<SpriteRenderer>().bounds.size.x;
        //Debug.Log("GraphicSize: " + towerGraphicSize);
		float buttonSizeRatio = towerGraphicSize / camHeight;
        //Debug.Log("Scale Factor: " + Scaler.scaleFactor);
		float buttonPixelSize = Screen.height * buttonSizeRatio * (Scaler.referenceResolution.y/Screen.height);
		behavior.Size = buttonPixelSize;
		current.buttons.Add(behavior);

		// Visual representation; mainly for testing and debugging
		// Resize to 2 times TowerButtonRadius because the sprite scale ends up half size when using raw radius.
		var rectTrans = behavior.GetComponent<RectTransform>();
		rectTrans.localScale = new Vector3(1,1,1);
		rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, behavior.Size);
		rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, behavior.Size);
	}

	#endregion

	#region EventCallbacks

	void UpdatePreferences()
	{
		PercentTogglesGroup.gameObject.SetActive(true);
	}

	void OnLevelEnd(bool victory)
	{
        //Debug.Log("Level Ending");
        LevelEndPanel.StartEndSequence(victory);

		foreach(var b in buttons)
		{
			GameObject.Destroy(b.gameObject);
		}

		dynamicBtns.Clear();
		buttons.Clear();
		
	}

	private void TowerBehavior_UnitsMoved(MovedUnitsInfo obj)
	{
		//Debug.Log("Units Moved From " + obj.From.Index + " to " + obj.To.Index);
	}
	#endregion

}
