using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// UI display for a TowerBehavior. Also acts as a virtual button controlled by UnitController.
public class TowerButtonBehavior : MonoBehaviour
{
    public static event System.Action ProductionUpgradeUsed;
    public static event System.Action EnduranceUpgradeUsed;

	public Text countText;
	public GameObject UpgradePanel;
    public Button EnduranceUpgradeBtn;
    public Button UnitProductionUpgradeBtn;

    public float Size { get; set; }
	private TowerBehavior tower;
	public TowerBehavior Tower
	{
		set
		{
			if (tower == null)
			{
				tower = value;
			}
		}

		get 
		{
			return tower;
		}
	}

	// Use this for initialization
	void Start () 
	{
		UpgradePanel.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		countText.text = tower.StationedUnits.ToString();
	}

    /// <summary>
    /// Shows tower upgrade panel if tower has not been upgraded. Disables upgrade buttons if tower doesn't have enough units to use upgrade.
    /// </summary>
	public void Select()
	{
        if (!Tower.IsUpgraded)
        {
            UpgradePanel.gameObject.SetActive(true);
            EnduranceUpgradeBtn.interactable = Tower.StationedUnits >= Game.TowerInfo.EnduranceUpgrade.Cost;
            UnitProductionUpgradeBtn.interactable = Tower.StationedUnits >= Game.TowerInfo.UnitProductionUpgrade.Cost;
        }
	}

    /// <summary>
    /// Hides tower upgrade panel.
    /// </summary>
	public void Deselect()
	{
		UpgradePanel.gameObject.SetActive (false);
	}

    // Functions for linking in Unity inspector to UI buttons

	public void UpgradeTowerUnitProduction()
	{
        if (Tower.UpgradeBy(Game.TowerInfo.UnitProductionUpgrade))
        {
            Deselect();
            if (ProductionUpgradeUsed != null)
            {
                ProductionUpgradeUsed();
            }
        }
    }

	public void UpgradeTowerDefense()
	{
        if (Tower.UpgradeBy(Game.TowerInfo.EnduranceUpgrade))
        {
            Deselect();
            if (EnduranceUpgradeUsed != null)
            {
                EnduranceUpgradeUsed();
            }
        }
    }
}
