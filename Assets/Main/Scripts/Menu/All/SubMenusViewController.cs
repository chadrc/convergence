using UnityEngine;
using System.Collections;

public class SubMenusViewController : MonoBehaviour
{
    public CampaignListViewController CampaignMenu;
    public MultiplayerMenuViewController MultiplayerMenu;
    public SettingsViewController SettingsMenu;
    
    void Awake()
    {
        Reset();
    }

    void Reset()
    {
        CampaignMenu.Close();
        MultiplayerMenu.Close();
        SettingsMenu.Close();
    }

    public void CampaignBtnPressed()
    {
        Reset();
        CampaignMenu.Open();
    }

    public void MultiplayerBtnPressed()
    {
        Reset();
        MultiplayerMenu.Open();
    }

    public void SettingsBtnPressed()
    {
        Reset();
        SettingsMenu.Open();
    }
}
