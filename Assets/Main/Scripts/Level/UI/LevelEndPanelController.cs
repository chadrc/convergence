using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class LevelEndPanelController : MonoBehaviour
{
    public Text ResultText;
    public Text LevelCompleteText;
    public Button NextLevelButton;
    public Image BackgroundImage;
    public CanvasGroup FlashPanel;
    public Image FlashImage;
    public CanvasGroup[] ButtonGroups;

    public Color PlayerColor;
    public Color EnemyColor;
    [Range(0,1)]
    public float BackgroundAlpha;
    [Range(0, 1)]
    public float FlashAlpha;

    public float flashTime;
    public float fadeInDelay;
    public float fadeInTime;
    public float buttonFadeDelay;

	// Use this for initialization
	void Start () {
	
	}

    public void StartEndSequence(bool result)
    {
        Color playerColor = PlayerColor;
        Color enemyColor = EnemyColor;
        if (NetworkLevelController.Networked && !NetworkServer.active)
        {
            playerColor = EnemyColor;
            enemyColor = PlayerColor;
        }
        Color temp;
        if (result)
        {
            ResultText.text = "Victory";
            LevelCompleteText.text = "\"" + LevelController.GetCurrentLevel().Name + "\" Complete";
            if (NextLevelButton != null)
            {
                NextLevelButton.gameObject.SetActive(true);
            }
            temp = playerColor;
        }
        else
        {
            ResultText.text = "Defeat";
            LevelCompleteText.text = LevelController.GetCurrentLevel().Name + " Failed";
            if (NextLevelButton != null)
            {
                NextLevelButton.gameObject.SetActive(true);
            }
            temp = enemyColor;
        }

        temp.a = FlashAlpha;
        FlashImage.color = temp;
        temp.a = BackgroundAlpha;
        BackgroundImage.color = temp;
        FlashPanel.alpha = 0;
        gameObject.SetActive(true);
        foreach(var group in ButtonGroups)
        {
            group.alpha = 0;
        }
        StartCoroutine(EndSequence());
    }

    public void NetworkEnd(bool result)
    {

    }

    IEnumerator EndSequence()
    {
        float timer = 0;
        float halfTime = flashTime / 4f;
        Time.timeScale = 0;
        
        while (timer < halfTime)
        {
            timer += Time.unscaledDeltaTime;
            FlashPanel.alpha = Mathf.Lerp(0, 1.0f, timer / halfTime);
            yield return new WaitForEndOfFrame();
        }

        Time.timeScale = .25f;
        timer = 0;
        float remainingTime = flashTime - halfTime;
        while(timer < halfTime)
        {
            timer += Time.unscaledDeltaTime;
            FlashPanel.alpha = Mathf.Lerp(1.0f, 0, timer / halfTime);
            yield return new WaitForEndOfFrame();
        }
        FlashPanel.interactable = false;
        FlashPanel.blocksRaycasts = false;

        timer = 0;
        float halfDelay = fadeInDelay / 2f;
        while (timer < fadeInDelay)
        {
            Time.timeScale = Mathf.Lerp(.25f, .1f, timer / halfDelay);
            timer += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }

        float timePerGroup = fadeInTime / ButtonGroups.Length;
        foreach (var group in ButtonGroups)
        {
            StartCoroutine(FadeInCanvasGroup(timePerGroup, group));
            timer = 0;
            while (timer < buttonFadeDelay)
            {
                timer += Time.unscaledDeltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        Time.timeScale = 0;
    }

    IEnumerator FadeInCanvasGroup(float time, CanvasGroup group)
    {
        float timer = 0;
        while (timer < time)
        {
            timer += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(0, 1.0f, timer / time);
            yield return new WaitForEndOfFrame();
        }
    }
}
