using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HostNameViewController : MonoBehaviour
{
    public Text HostNameText;
    public InputField ChangeNamePanel;
	// Use this for initialization
	void Start ()
    {
        string name = PlayerInfo.PlayerName; ;
        if (name == "")
        {
            name = "(Not Set)";
        }
        HostNameText.text = name;
	}

    void Update()
    {
        if (ChangeNamePanel.gameObject.activeSelf == true && Input.GetKeyUp(KeyCode.Return))
        {
            PlayerInfo.PlayerName = ChangeNamePanel.text;
            HostNameText.text = ChangeNamePanel.text;
            PlayerInfo.SavePlayerData();
            ChangeNamePanel.gameObject.SetActive(false);
        }
    }

    public void ChangeNameBtnPressed()
    {
        ChangeNamePanel.gameObject.SetActive(true);
    }
}
