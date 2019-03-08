using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Handles setting up a list item for a level list
/// </summary>
public class ListItemViewController : MonoBehaviour 
{
    public int index;
	public Text itemName;
	public Button selectBtn;

	public void SetData(int i, LevelData level)
	{
        index = i;
		itemName.text = level.Name;
		selectBtn.onClick.AddListener (() => {
			Game.SetSelectedLevel (index);
		});
	}
}
