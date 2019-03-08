using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MultiplayerMenuViewController : MonoBehaviour
{
    public NamePassViewController CreateNamePass;
    public NamePassViewController JoinNamePass;

    Vector2 initialPos;
    Vector2 destPos;

    Vector2 cnpInitPos;
    Vector2 cnpDestPos;

    Vector2 jnpInitPos;
    Vector2 jnpDestPos;

	// Use this for initialization
	void Awake ()
    {
        var offset = new Vector2(500, 0);
        initialPos = (transform as RectTransform).anchoredPosition;
        destPos = initialPos - offset;

        cnpInitPos = (CreateNamePass.transform as RectTransform).anchoredPosition;
        jnpInitPos = (JoinNamePass.transform as RectTransform).anchoredPosition;

        cnpDestPos = cnpInitPos - offset;
        jnpDestPos = jnpInitPos - offset;

        CreateNamePass.SubmitPressed += OnCreateSubmit;
        JoinNamePass.SubmitPressed += OnJoinSubmit;
	}

    public void QuickMatchBtnPressed()
    {
        Reset();
        Game.SetSelectedLevel(-1);
        NetworkLevelController.Option = MatchOption.QuickMatch;
        SceneManager.LoadScene("Multiplayer");
    }

    public void JoinBtnPressed()
    {
        StartCoroutine(MenuAnimations.FadeMoveCanvasGroup(CreateNamePass.GetComponent<CanvasGroup>(), initialPos, destPos, .2f, -1.0f));
        StartCoroutine(MenuAnimations.FadeMoveCanvasGroup(JoinNamePass.GetComponent<CanvasGroup>(), jnpDestPos, jnpInitPos, .2f, 1.0f));
    }

    public void CreateBtnPressed()
    {
        StartCoroutine(MenuAnimations.FadeMoveCanvasGroup(JoinNamePass.GetComponent<CanvasGroup>(), initialPos, destPos, .2f, -1.0f));
        StartCoroutine(MenuAnimations.FadeMoveCanvasGroup(CreateNamePass.GetComponent<CanvasGroup>(), cnpDestPos, cnpInitPos, .2f, 1.0f));
    }

    public void Open()
    {
        StartCoroutine(MenuAnimations.FadeMoveCanvasGroup(GetComponent<CanvasGroup>(), destPos, initialPos, .2f, 1.0f));
    }

    public void Close()
    {
        StartCoroutine(MenuAnimations.FadeMoveCanvasGroup(GetComponent<CanvasGroup>(), initialPos, destPos, .2f, -1.0f));
        Reset();
    }

    void Reset()
    {
        StartCoroutine(MenuAnimations.FadeMoveCanvasGroup(CreateNamePass.GetComponent<CanvasGroup>(), initialPos, destPos, .2f, -1.0f));
        StartCoroutine(MenuAnimations.FadeMoveCanvasGroup(JoinNamePass.GetComponent<CanvasGroup>(), initialPos, destPos, .2f, -1.0f));
    }

    void OnCreateSubmit(string name, string pass)
    {
        Game.SetSelectedLevel(-1);
        NetworkLevelController.Option = MatchOption.Create;
        NetworkLevelController.RoomName = name;
        NetworkLevelController.RoomPass = pass;
        SceneManager.LoadScene("Multiplayer");
    }

    void OnJoinSubmit(string name, string pass)
    {
        Game.SetSelectedLevel(-1);
        NetworkLevelController.Option = MatchOption.Join;
        NetworkLevelController.RoomName = name;
        NetworkLevelController.RoomPass = pass;
        SceneManager.LoadScene("Multiplayer");
    }
}
