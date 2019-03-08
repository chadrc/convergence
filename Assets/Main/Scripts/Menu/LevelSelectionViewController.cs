using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Contains functions for Menu unit controls to use. Updates current level information when selected level changes.
/// </summary>
public class LevelSelectionViewController : MonoBehaviour
{
	public Text SelectedLevelName;
	public Text SelectedLevelDiscription;
    public Button PlayBtn;
    public GameObject LoadPanel;
    public MainMenuViewController MainMenuCtrl;

	void Awake()
	{
		Game.SelectedLevelChanged += OnSelectedLevelChanged;
        PlayBtn.interactable = false;	
	}

    void Start()
    {

    }

	void OnDestroy()
	{
		Game.SelectedLevelChanged -= OnSelectedLevelChanged;
	}

	public void OnPlayBtnPressed()
	{
        LoadPanel.SetActive(true);
		if (Game.HasSelectedLevel ())
		{
            SceneManager.LoadScene("Level");
		}
	}

    public void OnMainMenuBtnPressed()
    {
        MainMenuCtrl.Show();
    }

	private void OnSelectedLevelChanged(LevelData data)
	{
        if (data == null)
        {
            PlayBtn.interactable = false;
            SelectedLevelDiscription.text = "";
            SelectedLevelName.text = "";
        }
        else
        {
            PlayBtn.interactable = true;
            SelectedLevelName.text = data.Name;
            SelectedLevelDiscription.text = data.Discription;
        }
	}
}
