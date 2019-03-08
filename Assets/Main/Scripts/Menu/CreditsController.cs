using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CreditsController : MonoBehaviour 
{
	public CanvasGroup FadeGroup;
	public float SecondsToFade;

	private Coroutine currentRoutine;

	public void Show()
	{
		CheckRoutine ();
		currentRoutine = StartCoroutine (fadeIn ());
	}

	public void Hide()
	{
		CheckRoutine ();
		currentRoutine = StartCoroutine (fadeOut ());
	}

	private void CheckRoutine()
	{
		if (currentRoutine != null)
		{
			StopCoroutine (currentRoutine);
		}
	}
		
	IEnumerator fadeIn()
	{
		FadeGroup.gameObject.SetActive(true);
		while (FadeGroup.alpha < 1.0f)
		{
			float change = Time.unscaledDeltaTime / SecondsToFade;
			FadeGroup.alpha += change;
			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator fadeOut()
	{
		while (FadeGroup.alpha > 0)
		{
			float change = Time.unscaledDeltaTime / SecondsToFade;
			FadeGroup.alpha -= change;
			yield return new WaitForEndOfFrame();
		}
		FadeGroup.gameObject.SetActive(false);
	}
}
