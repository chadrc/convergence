using UnityEngine;
using System.Collections;

/// <summary>
/// Creates a level list under this gameObject with prefab set in inspector
/// </summary>
public class ListViewController : MonoBehaviour 
{
	public LevelList Levels;
	public GameObject ListItemFab;

	// Use this for initialization
	void Start () 
	{
        Game.SetLevelList(Levels);
        int i = 0;
		foreach (var level in Levels.Levels) 
		{
			var obj = GameObject.Instantiate (ListItemFab) as GameObject;
			obj.transform.SetParent (transform);
			obj.transform.localScale = new Vector3 (1, 1, 1);
			var item = obj.GetComponent<ListItemViewController> ();
			item.SetData (i++, level);
		}
	}
}
