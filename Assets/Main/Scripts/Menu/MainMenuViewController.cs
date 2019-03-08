using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuViewController : MonoBehaviour
{
    public Text VersionText;
    public CanvasGroup FadeGroup;
    public float SecondsToFade;

    void Start()
    {
        VersionText.text = "v." + Application.version + "\nuv." + Application.unityVersion;
    }

    public void OnSinglePlayerBtnPressed()
    {
        StartCoroutine(fadeOut());
    }

    public void OnMultiplayerBtnPressed()
    {
        SceneManager.LoadScene("Multiplayer");
    }

    public void Show()
    {
        FadeGroup.gameObject.SetActive(true);
        FadeGroup.alpha = 1.0f;
    }

    IEnumerator fadeOut()
    {
        while (FadeGroup.alpha > 0)
        {
            float reduce = Time.unscaledDeltaTime / SecondsToFade;
            FadeGroup.alpha -= reduce;
            yield return new WaitForEndOfFrame();
        }
        FadeGroup.gameObject.SetActive(false);
    }
}
