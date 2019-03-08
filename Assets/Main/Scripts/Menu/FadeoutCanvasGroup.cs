using UnityEngine;
using System.Collections;

public class FadeoutCanvasGroup : MonoBehaviour
{
    public CanvasGroup FadeGroup;
    public float SecondsToFade;

    // Use this for initialization
    void Awake ()
    {
        LevelController.Pause();
	}

    void OnLevelWasLoaded(int level)
    {
        StartCoroutine(fadeOut());
        LevelController.Unpause();
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
