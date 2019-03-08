using UnityEngine;
using System.Collections;

public static class MenuAnimations
{
    public static IEnumerator FadeMoveCanvasGroup(CanvasGroup c, Vector2 start, Vector2 dest, float t, float dir)
    {
        var rect = c.transform as RectTransform;
        float timer = 0;
        float startA = c.alpha;
        float destA = 1.0f;
        if (dir == -1)
        {
            destA = 0;
        }
        yield return new WaitForEndOfFrame();
        while (true)
        {
            timer += Time.deltaTime;
            float frac = timer / t;
            c.alpha = Mathf.Lerp(startA, destA, frac);
            var newPos = Vector2.Lerp(start, dest, frac);
            rect.anchoredPosition = newPos;
            if (timer >= t)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }
}
