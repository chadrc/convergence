using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class NetworkVersusPanelViewController : MonoBehaviour 
{
	public Text PlayerNameText;
	public Text OpponentNameText;
	public Image PlayerFactionImage;
	public Image OpponentFactionImage;

	public Sprite ServerFactionSprite;
	public Sprite ClientFactionSprite;

	public void Show(string playerName, string opponentName)
	{
		gameObject.SetActive(true);
		PlayerNameText.text = playerName;
		OpponentNameText.text = opponentName;

		if (NetworkServer.active)
		{
			PlayerFactionImage.sprite = ServerFactionSprite;
			OpponentFactionImage.sprite = ClientFactionSprite;
		}
		else
		{
			PlayerFactionImage.sprite = ClientFactionSprite;
			OpponentFactionImage.sprite = ServerFactionSprite;
		}
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}
}
