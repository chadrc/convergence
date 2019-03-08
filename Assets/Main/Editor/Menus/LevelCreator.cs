using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public class LevelCreator : EditorWindow
{
	public const string SAVE = "Save";
	/// <summary>
	/// Used as the prompt during the level selection
	/// menu while attempting to create a level.
	/// </summary>
	public const string NEW_LEVEL_PROMPT = "Select a Home List for the New Level";
	/// <summary>
	/// Used as the prompt during the level list selection
	/// menu while attempting to edit a level.
	/// </summary>
	public const string EDIT_LEVEL_PROMPT = "Select the Level List Containing Desired Level";
	/// <summary>
	/// Used as the prompt during the level selection menu
	/// while attempting to edit a level.
	/// </summary>
	public const string SELECT_LEVEL_PROMPT = "Select a Level To Edit";
	/// <summary>
	/// Path to the default level prefab.
	/// </summary>
	public const string DEFAULT_LEVEL_PREFAB_PATH = "Assets/Main/Fabs/DefaultLevelFab.prefab";
	/// <summary>
	/// Path to the Level Controller prefab.
	/// </summary>
	public const string LEVEL_CONTROLLER_PREFAB_PATH = "Assets/Main/Fabs/LevelController.prefab";
	/// <summary>
	/// Directory used to access the contents of the "Lists" directory
	/// </summary>
	public const string LEVEL_LIST_FOLDER_DIRECTORY_INSIDE = "Assets/Main/Levels/Lists/";
	/// <summary>
	/// Directory used for storing newly created level lists.
	/// </summary>
	public const string LEVEL_LIST_FOLDER_DIRECTORY_OUTSIDE = "Assets/Main/Levels/Lists";
	/// <summary>
	/// Directory used to store newly created folders specifically 
	/// corresponding to the Level List creation process.
	/// </summary>
	public const string LEVELS_FOLDER_DIRECTORY = "Assets/Main/Levels";
	public const string ASSET_EXTENSION = ".asset";
	public const string PREFAB_EXTENSION = ".prefab";
	public const string EMPTY_STRING = "";
	public const string FORWARD_SLASH = "/";

	#region Tag Consts

	public const string TOWER_TAG = "Tower";
	public const string LEVEL_FAB_TAG = "LevelFab";
	public const string GAME_CONTROLLER_TAG = "GameController";

	#endregion

	/// <summary>
	/// Used to determine which menu we are in. 
	/// </summary>
	private enum MenuPositions
	{
		start,

		#region Creating A New Level List

		creatingNewLevelList,

		#endregion

		#region Creating A New Level

		selectingListForNewLevel,
		creatingNewLevel,

		#endregion

		#region Editing A Level

		selectingListToEdit,
		selectingLevelToEdit,
		editingLevel

		#endregion

	}

	private MenuPositions currentMenuPos = MenuPositions.start;
	private Stack<MenuPositions> backButtonStorage = new Stack<MenuPositions>();
	private GameObject controller;

	private string newLevelListName = "";
	private string newLevelName = "";
	private string newLevelDescription = "";

	private string pathOfSelectedLevelList = "";
	private string nameOfSelectedList = "";

	private GameObject levelPrefabInstance = null;
	private LevelData currentLevelBeingEdited = null;
	private Object currentPrefabBeingEdited = null;

	private bool displayConvergences = false;

	private bool isTesting = false;

	/// <summary>
	/// Allows us to have a window pop for editing.
	/// </summary>
	[MenuItem("Convergence/Create Stuff %g")]
	public static void ShowWindow()
	{
		var levelEditorWin = EditorWindow.GetWindow(typeof(LevelCreator));
		levelEditorWin.minSize = new Vector2(700, 400);
		levelEditorWin.Show();
	}


	/// <summary>
	/// Validates the window. 
	/// Checks if the "Level" scene is active.
	/// If it is not, the window option will be grayed out.
	/// </summary>
	[MenuItem("Convergence/Create Stuff %g", true)]
	public static bool ValidateWindow()
	{
		return EditorSceneManager.GetActiveScene().name == "Level";
	}


	/// <summary>
	/// Searches the scene for the "Level Controller" gameobject which controls
	/// what prefab is loaded into the scene at runtime. If there isn't one, it
	/// will be instantiated.
	/// </summary>
	void OnEnable()
	{
		controller = GameObject.FindGameObjectWithTag(GAME_CONTROLLER_TAG);
		CheckForLevelController();
	}


	void OnGUI()
	{
		if (!isTesting)
		{
			switch (currentMenuPos)
			{
				case MenuPositions.start:
					{
						ShowStartMenuOptions();
					}
					break;

				case MenuPositions.creatingNewLevelList:
					{
						ShowCreateNewLevelListOptions();
					}
					break;

				case MenuPositions.selectingListForNewLevel:
					{
						ShowLevelLists(NEW_LEVEL_PROMPT);
					}
					break;

				case MenuPositions.creatingNewLevel:
					{
						ShowCreateNewLevelOptions();
					}
					break;

				case MenuPositions.selectingListToEdit:
					{
						ShowLevelLists(EDIT_LEVEL_PROMPT);
					}
					break;

				case MenuPositions.selectingLevelToEdit:
					{
						ShowLevels(SELECT_LEVEL_PROMPT);
					}
					break;

				case MenuPositions.editingLevel:
					{
						ShowLevelSpecs();
					}
					break;

				default:
					break;
			}

			if (backButtonStorage.Count > 0)
			{
				ShowBackButton();
			}
			else if (currentMenuPos != MenuPositions.start && backButtonStorage.Count == 0)
			{
				ShowHomeButton();
			}
		}

		ShowTestButton();
	}


	#region Menu Option Viewing Methods

	/// <summary>
	/// Shows the start menu options.
	/// </summary>
	void ShowStartMenuOptions()
	{
		if (GUILayout.Button("Create New Level List"))
		{
			backButtonStorage.Push(currentMenuPos);
			currentMenuPos = MenuPositions.creatingNewLevelList;
		}

		if (GUILayout.Button("Create New Level"))
		{
			backButtonStorage.Push(currentMenuPos);
			currentMenuPos = MenuPositions.selectingListForNewLevel;
		}

		if (GUILayout.Button("Edit a Pre-existing Level"))
		{
			backButtonStorage.Push(currentMenuPos);
			currentMenuPos = MenuPositions.selectingListToEdit;
		}
	}


	//Creates a new level (prefab) in a pre-existing level list.
	//It needs to grab the directory of the desired level list. (foreach item in the level_list_folder_directory, load them buttons. One will need to be selected in order to continue)
	//Grab the proper levelist scriptable object
	//Incrememnt the size of the array.
	//Store the new level in the newly added index.
	void ShowLevelLists(string labelMessage)
	{
		var levelListPathArray = Directory.GetFiles(LEVEL_LIST_FOLDER_DIRECTORY_OUTSIDE, "*.asset");
		GUIStyle myStyle = new GUIStyle();
		myStyle.alignment = TextAnchor.MiddleCenter;

		EditorGUILayout.LabelField(labelMessage, myStyle);

		// "item" refers to the path of the object grabbed from the levelListPathArray.
		foreach (var item in levelListPathArray)
		{
			// This section is used to parse the path stored in "item" for thr actual level list name.
			string listName = item;
			listName = ParseString(listName);

			// After parsing the path for the level list name, buttons 
			// are then created for each level list.
			if (GUILayout.Button(listName))
			{
				// Store the path of the selected level list.
				pathOfSelectedLevelList = item;
				// Store the actual name of the selected list.
				nameOfSelectedList = listName;
				backButtonStorage.Push(currentMenuPos);

				if (currentMenuPos == MenuPositions.selectingListForNewLevel)
				{
					// If the user is creating a new level list, then the next step from
					// there is to actually create the level. Move to thatmenu position.
					currentMenuPos = MenuPositions.creatingNewLevel;
				}
				else
				{
					// The only other route that the user can take that requires
					// showing the level lists is "Edit a Pre-existing Level".
					// If the user isn't creating a new level, then they must be tring to
					// edit one, so move to that menu position.
					currentMenuPos = MenuPositions.selectingLevelToEdit;
				}
			}
		}
	}


	//Creates a new level (prefab) in a pre-existing level list.
	//It needs to grab the directory of the desired level list. (foreach item in the level_list_folder_directory, load them buttons. One will need to be selected in order to continue)
	//Grab the proper levelist scriptable object
	//Incrememnt the size of the array.
	//Store the new level in the newly added index.
	void ShowLevels(string labelMessage)
	{
		// Reference to the new level list.
		// Used to store a reference to the List we will be adding to.
		LevelList selectedList = AssetDatabase.LoadAssetAtPath (pathOfSelectedLevelList, typeof(LevelList)) as LevelList; 
		// Grab a reference the the list of levels stored in the selected level list.
		List<LevelData> levelList = selectedList.Levels;

		GUIStyle myStyle = new GUIStyle ();
		myStyle.alignment = TextAnchor.MiddleCenter;

		EditorGUILayout.LabelField (labelMessage, myStyle);
		foreach (LevelData level in levelList) {
			if (GUILayout.Button (level.name)) {
				CheckForLevelController ();

				currentLevelBeingEdited = level;
				currentPrefabBeingEdited = currentLevelBeingEdited.Prefab;
				controller.GetComponent<LevelController> ().CurrentLevel = currentLevelBeingEdited;

				if (EditorUtility.DisplayDialog ("Current Level Swapped", string.Format ("The Level Controller's Current Level has been changed to \"{0}\"!", level.name), "Ok")) {
					var previousLevelFabs = GameObject.FindGameObjectsWithTag (LEVEL_FAB_TAG);
					foreach (GameObject fab in previousLevelFabs) {
						DestroyImmediate (fab, false);
					}

					// Allows for the creation of an instance of the curent l prefab assigned to the level.
					levelPrefabInstance = PrefabUtility.InstantiatePrefab (level.Prefab) as GameObject; 
					currentMenuPos = MenuPositions.editingLevel;

					break;
				}
			}
		}
	}


	/// <summary>
	/// Method that ensures that a name is provided for the new LevelList
	/// which then calls CreateNewLevelListWithFolder to complete the creation process.
	/// </summary>
	void ShowCreateNewLevelListOptions()
	{
		GUIStyle myStyle = new GUIStyle();
		myStyle.alignment = TextAnchor.MiddleCenter;

		EditorGUILayout.LabelField("New Level List Name (Will be used to create a new folder too)", myStyle);
		newLevelListName = EditorGUILayout.TextField(EMPTY_STRING, newLevelListName);

		if (GUILayout.Button("Generate New Level List and Folder"))
		{
			if (newLevelListName == EMPTY_STRING)
			{
				if (EditorUtility.DisplayDialog("Naming Error", "It looks like you still need to enter a name!", "Ok"))
				{
					return;
				}
			}

			CreateNewLevelListWthFolder(newLevelListName);
		}
	}


	/// <summary>
	/// This method is used to ensure that the text fields for the expected new LevelData's name 
	/// and description are filled in. The user can then press the "Generate New Level" button which
	/// calls the CreateNewLevel method to wrap up the creation process.
	/// </summary>
	void ShowCreateNewLevelOptions()
	{
		GUIStyle myStyle = new GUIStyle();
		myStyle.alignment = TextAnchor.MiddleCenter;

		EditorGUILayout.LabelField("Please enter the name of your new level)", myStyle);
		newLevelName = EditorGUILayout.TextField(EMPTY_STRING, newLevelName);

		EditorGUILayout.LabelField("Please enter the description of your new level)", myStyle);
		newLevelDescription = EditorGUILayout.TextField(EMPTY_STRING, newLevelDescription);

		// Generate new level list and folder button
		if (GUILayout.Button("Generate New Level"))
		{
			// Empty string check which creates a popup if it is indeed empty
			if (string.IsNullOrEmpty(newLevelName) || string.IsNullOrEmpty(newLevelDescription))
			{
				if (EditorUtility.DisplayDialog("Naming Error", "It looks like you still need to enter a name!", "Ok"))
				{
					return;
				}
			}

			CreateNewLevel(newLevelName, newLevelDescription);
		}
	}


	/// <summary>
	/// Shows specs of the level currently being edited.
	/// Allows for particular variables to be edited such 
	/// as the name, description and convergences (on the LevelData)
	/// as well as the actual level prefab.
	/// </summary>
	void ShowLevelSpecs()
	{
		Rect leftSide = new Rect(0, 0, Screen.width / 2, Screen.height);
		Rect rightSide = new Rect(Screen.width / 2, 0, Screen.width / 2, Screen.height);

		GUIStyle leftStyle = new GUIStyle();
		GUIStyle rightStyle = new GUIStyle();

		leftStyle.alignment = TextAnchor.MiddleCenter;
		leftStyle.fontStyle = FontStyle.Bold;

		rightStyle.alignment = TextAnchor.MiddleCenter;
		rightStyle.fontStyle = FontStyle.Italic;

		if (levelPrefabInstance != null)
		{
			GUILayout.BeginArea(leftSide);

			EditorGUILayout.LabelField("Level Prefab Details", leftStyle);
			EditorGUILayout.LabelField("Name: " + currentLevelBeingEdited.Name);
			EditorGUILayout.LabelField("Description: " + currentLevelBeingEdited.Discription, EditorStyles.wordWrappedLabel);

			currentPrefabBeingEdited = EditorGUILayout.ObjectField("Current Level Prefab:", currentPrefabBeingEdited, typeof(Object), false);
			currentLevelBeingEdited.Prefab = (GameObject)currentPrefabBeingEdited;

			if (currentLevelBeingEdited.convergences.Count > 0)
			{
				displayConvergences = EditorGUILayout.Foldout(displayConvergences, "Convergences:");
				if (displayConvergences)
				{
					int count = 1;
					foreach (var c in currentLevelBeingEdited.convergences)
					{
						EditorGUI.indentLevel = 1;
						EditorGUILayout.LabelField("Occurrence " + count);
						EditorGUI.indentLevel = 2;
						EditorGUILayout.LabelField("First Occurence: " + c.FirstTime);
						EditorGUILayout.LabelField("IntervalTime: " + c.IntervalTime);

						string towersInvolved = "[";
						foreach (var t in c.TowersInvolved)
						{
							towersInvolved += t + ", ";
						}
						towersInvolved = towersInvolved.Substring(0, towersInvolved.Length - 2) + "]";
						EditorGUILayout.LabelField("Towers Involved: " + towersInvolved);

						count++;
					}
					EditorGUI.indentLevel = 0;

					if (GUILayout.Button("Reset Convergences?"))
					{
						if (EditorUtility.DisplayDialog("Resetting Convergences", "Are you sure? This will reset all convergences for this level.", "Yes", "No"))
						{
							currentLevelBeingEdited.convergences.Clear();
						}
					}
				}
			}
			else
			{
				EditorGUILayout.LabelField("No convergences currently exist for this level");
				ShowCalculateConvergenceButton();
			}

			if (GUILayout.Button("UpdateFab"))
			{
				CheckForExistingPrefab();
			}

			GUILayout.EndArea();

			GUILayout.BeginArea(rightSide);

			EditorGUILayout.LabelField("Level Data Details", leftStyle);
			EditorGUILayout.LabelField("Change Level Name", rightStyle);
			currentLevelBeingEdited.Name = EditorGUILayout.TextField(EMPTY_STRING, currentLevelBeingEdited.Name);
			EditorGUILayout.LabelField("Change Level Description", rightStyle);
			currentLevelBeingEdited.Discription = EditorGUILayout.TextField(EMPTY_STRING, currentLevelBeingEdited.Discription);

			GUILayout.EndArea();
		}
		else
		{
			if (EditorUtility.DisplayDialog("Missing Prefab Instance", "The prefab instance has gone missing! \nReturning to the start menu", "OK"))
			{ 
				ActivateHomeButton();
			}
		}
	}


	/// <summary>
	/// Shows the calculate convergence button.
	/// This also takes care of calling the CalculateConvergences method.
	/// In order to call this properly it requires at least one tower with
	/// the OrbitMotion script attached as well as a center point which
	/// is a public field on the aforementioned script.
	/// </summary>
	void ShowCalculateConvergenceButton()
	{
		if (GUILayout.Button("Calculate Convergences"))
		{
			ConverganceCalc.CalculateConvergences();
		}
	}

	#endregion


	#region Menu Option Utility Methods

	/// <summary>
	/// Creates the new level.
	/// </summary>
	void CreateNewLevel(string levelName, string description)
	{
		// Reference to the new level list.
		// Used to store a reference to the List we will be adding to.
		LevelList levelList = AssetDatabase.LoadAssetAtPath(pathOfSelectedLevelList, typeof(LevelList)) as LevelList;
		// Concatanating a bunch of necessary strings to reduce line length.
		string directoryToAddNewLevelAndFab = LEVELS_FOLDER_DIRECTORY + FORWARD_SLASH + levelList.name + FORWARD_SLASH;
		// New level Scriptable object asset as well as a reference to it.
		// Created in the folder specific to the chosen level list.
		LevelData newLevel = TWEditorUtil.CreateScriptableAsset<LevelData>(directoryToAddNewLevelAndFab + levelName + ASSET_EXTENSION, true, true);
		// Grabbing a reference to the default level prefab.
		var defaultLevelFab = AssetDatabase.LoadAssetAtPath(DEFAULT_LEVEL_PREFAB_PATH, typeof(GameObject));
		// Need to make an instance of the prefab in order to create a new one in another directory.
		GameObject tempObject = GameObject.Instantiate(defaultLevelFab) as GameObject; 
		// Creation of a unique prefab to be used for the newly created level.
		// This also takes care of setting the new prefabs name to the name of the level.
		var newLevelFab = PrefabUtility.CreatePrefab(directoryToAddNewLevelAndFab + levelName + PREFAB_EXTENSION, tempObject as GameObject);

		// Store the newly created prefab in the new Level's public prefab slot.
		newLevel.Name = levelName;
		newLevel.Discription = description;
		newLevel.Prefab = newLevelFab;

		DestroyImmediate(tempObject); //Destroy the instance that we had originally created off of "DefaultLevelFab", which is in the scene.

		// Add the new level to the referenced LevelList above.
		if (newLevel != null)
		{
			levelList.Levels.Add(newLevel);
			EditorUtility.SetDirty (levelList);


			AssetDatabase.SaveAssets();
		}
		else
		{
			Debug.LogError("The level you are trying to add to the level list is null.");
		}

		if (EditorUtility.DisplayDialog("Level Created", "A new level has been created!", "Ok"))
		{
			ActivateHomeButton();
		}
	}



	/// <summary>
	/// Creates a new folder for the new LevelList, used for storing any levels that will be a part of that list.
	/// Creates a new LevelList scriptable object in order to store new levels pertaining to the involved list.
	/// Returns the user to the start menu upon sucessful completion.
	/// </summary>
	void CreateNewLevelListWthFolder(string nameForListAndFolder)
	{
		AssetDatabase.CreateFolder(LEVELS_FOLDER_DIRECTORY, nameForListAndFolder);

		var newList = TWEditorUtil.CreateScriptableAsset<LevelList>(LEVEL_LIST_FOLDER_DIRECTORY_INSIDE + nameForListAndFolder + ASSET_EXTENSION, true, true);
		if (EditorUtility.DisplayDialog("Complete", "The new level list and its respective folder have been created", "Ok"))
		{
			ActivateHomeButton();
		}
	}


	/// <summary>
	/// This is used to parse a path for a level name.
	/// An example: Assets/Main/Levels/Lists/Tutorials.asset will return "Tutorials"
	/// </summary>
	string ParseString(string itemName)
	{
		bool hasReachedActualName = false;
		bool isExtension = false;
		string newName = EMPTY_STRING;

		foreach (char c in itemName)
		{
			// Once the forward slash is hit, that is the point where the real name begins
			if (c == '\\')
			{
				hasReachedActualName = true;
			}
			// Once it reaches this, we will be at the extension. We need nothing from here down.
			else if (c == '.')
			{
				isExtension = true;
			}

			// if we are passed the 3 levels in the file path, then the string starts 
			// storing each char, until we reach the extension portion
			if (hasReachedActualName == true && isExtension == false)
			{
				if (c != '\\')
				{//ensures that we don't grab that first '/'
					newName += c;
				}
			}
		}

		return newName;
	}


	/// <summary>
	/// Checks for existing prefab and asks the user if they'd like to replace it.
	/// This is used when updating a prefab.
	/// </summary>
	void CheckForExistingPrefab()
	{
		string prefabPath = LEVELS_FOLDER_DIRECTORY + FORWARD_SLASH + nameOfSelectedList + FORWARD_SLASH + currentPrefabBeingEdited.name + PREFAB_EXTENSION; 
		if (AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)))
		{
			if (EditorUtility.DisplayDialog("Are you sure?", "There is an instance of this prefab already. Do you wish to overwrite it?", "Yes", "No"))
			{
				UpdatePrefab(prefabPath);
			}
		}
		else
		{
			UpdatePrefab(prefabPath);
		}
	}


	/// <summary>
	/// Takes in the path of the prefab to replace and replaces it with a more
	/// up to date version, which is in the scene. This is similar to the "Apply" 
	/// operation found in the inspector, when focused on a prefab in the scene.
	/// </summary>
	void UpdatePrefab(string fabToReplacePath)
	{
		var oldFab = AssetDatabase.LoadAssetAtPath(fabToReplacePath, typeof(GameObject));
		PrefabUtility.ReplacePrefab((GameObject)levelPrefabInstance, oldFab, ReplacePrefabOptions.ConnectToPrefab);

		var replacedFab = AssetDatabase.LoadAssetAtPath(fabToReplacePath, typeof(GameObject));
		currentLevelBeingEdited.Prefab = (GameObject)replacedFab;
		currentPrefabBeingEdited = currentLevelBeingEdited.Prefab;
		AssignTowerIndices();
	}


	/// <summary>.
	/// This function finds all objects with TOWER_TAG taG
	/// and assigns them a unique index for tracking purposes.
	/// </summary>
	void AssignTowerIndices()
	{
		var towerList = GameObject.FindGameObjectsWithTag(TOWER_TAG);
		var index = 0;
		foreach (GameObject tower in towerList)
		{
			towerList[index].GetComponent<TowerBehavior>().Index = index;
			index++;
		}
	}


	/// <summary>
	/// Ensures that there is a level controller in the scene.
	/// If there is not, one is instantiated.
	/// </summary>
	void CheckForLevelController()
	{
		if (controller == null)
		{
			var newController = AssetDatabase.LoadAssetAtPath(LEVEL_CONTROLLER_PREFAB_PATH, typeof(GameObject));
			controller = Instantiate(newController, Vector3.zero, Quaternion.identity) as GameObject;

			if (EditorUtility.DisplayDialog("Missing Level Controller", "The level controller was missing. \nA new one has been instantiated!", "Ok"))
			{
				return;
			}
		}
	}

	#endregion


	#region Test Button Methods

	/// <summary>
	/// Shows the test button.
	/// </summary>
	void ShowTestButton()
	{
		Rect testButtonSpace = new Rect(Screen.width * 0.375f, Screen.height * 0.75f, Screen.width / 3.5f, 30.0f);
		string testingText = EditorApplication.isPlaying ? "Stop Testing" : "Test";

		GUILayout.BeginArea(testButtonSpace);

		if (GUILayout.Button(testingText))
		{
			if (!EditorApplication.isPlaying)
			{
				// If the game is NOT playing and this button was pressed, then we are ENTERING play mode
				// thus it is necessary to deactivate the current prefab in the scene.
				HideCurrentLevelPrefabInScene();
			}

			EditorApplication.isPlaying = !EditorApplication.isPlaying;
			isTesting = !isTesting;
		}

		GUILayout.EndArea();
	}


	/// <summary>
	/// Hides the current level prefab in scene when the user presses the "Test" button.
	/// This is to avoid duplicate level prefabs.
	/// </summary>
	void HideCurrentLevelPrefabInScene()
	{
		var levelFab = GameObject.FindGameObjectWithTag(LEVEL_FAB_TAG);
		if (levelFab != null)
		{
			levelFab.SetActive(false);	
		}
	}

	#endregion


	#region Back/Home Button Methods

	/// <summary>
	/// Activates the back button bringing the user back to the menu position
	/// prior to their current one. 
	/// Removes the new position from the stack as well.
	/// </summary>
	void ActivateBackButton()
	{
		var tempLastMenuPos = backButtonStorage.Peek();
		backButtonStorage.Pop();
		currentMenuPos = (MenuPositions)tempLastMenuPos;
	}


	/// <summary>
	/// Shows the back button.
	/// </summary>
	void ShowBackButton()
	{
		Rect backButtonSpace = new Rect(Screen.width * 0.375f, Screen.height * 0.82f, Screen.width / 3.5f, 30.0f);

		GUILayout.BeginArea(backButtonSpace);

		if (GUILayout.Button("Back"))
		{
			ActivateBackButton();
		}

		GUILayout.EndArea();
	}


	/// <summary>
	/// Returns the user to the start menu.
	/// </summary>
	void ActivateHomeButton()
	{
		currentMenuPos = MenuPositions.start;
		backButtonStorage.Clear();
	}


	/// <summary>
	/// Shows the home button.
	/// </summary>
	void ShowHomeButton()
	{
		Rect homeButtonSpace = new Rect(Screen.width * 0.375f, Screen.height * 0.82f, Screen.width / 3.5f, 30.0f);

		GUILayout.BeginArea(homeButtonSpace);

		if (GUILayout.Button("Return To Start"))
		{
			ActivateHomeButton();
		}

		GUILayout.EndArea();
	}

	#endregion

}
