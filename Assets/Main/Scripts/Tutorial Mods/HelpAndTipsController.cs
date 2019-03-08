using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HelpAndTipsController : MonoBehaviour
{
    public Text InfoText;
    public RectTransform Panel;
    public Button ActivateBtn;
    public float AnimTime = 1.0f;
    public float ActiveWidth = 600.0f;

    private float InitialWidth;

    public IHelpAndTipsResponder Responder { get; set; }
    
    void Awake()
    {
        InitialWidth = Panel.rect.size.x;
    }

    public void SetText(string text)
    {
        InfoText.text = text;
        StartCoroutine(OpenHelp());
    }

    public void Reset()
    {
        InfoText.text = "!";
        StartCoroutine(CloseHelp());
        ActivateBtn.gameObject.SetActive(true);
        InfoText.gameObject.SetActive(false);
    }

    public void Activate()
    {
        if (Responder != null)
        {
            Responder.HelpAndTipsActivated();
        }
        ActivateBtn.gameObject.SetActive(false);
    }

    public void Deactivate()
    {
        if (Responder != null)
        {
            Responder.HelpAndTipsDeactivated();
        }
        Reset();
    }

    IEnumerator OpenHelp()
    {
        while(Panel.rect.size.x < ActiveWidth)
        {
            float frac = Time.unscaledDeltaTime / AnimTime;
            float increase = frac * (ActiveWidth - InitialWidth);
            float width = Panel.rect.size.x + increase;
            Panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            Panel.anchoredPosition = new Vector2(width/2, Panel.anchoredPosition.y);
            yield return new WaitForEndOfFrame();
        }

        Panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ActiveWidth);
        Panel.anchoredPosition = new Vector2(ActiveWidth / 2, Panel.anchoredPosition.y);
        InfoText.gameObject.SetActive(true);
    }

    IEnumerator CloseHelp()
    {
        while (Panel.rect.size.x > 200.0f)
        {
            float frac = Time.unscaledDeltaTime / AnimTime;
            float change = frac * (ActiveWidth - InitialWidth);
            float width = Panel.rect.size.x - change;
            Panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            Panel.anchoredPosition = new Vector2(width / 2, Panel.anchoredPosition.y);
            yield return new WaitForEndOfFrame();
        }
        Panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, InitialWidth);
        Panel.anchoredPosition = new Vector2(InitialWidth / 2, Panel.anchoredPosition.y);
        InfoText.gameObject.SetActive(false);
    }
}

public interface IHelpAndTipsResponder
{
    void HelpAndTipsActivated();
    void HelpAndTipsDeactivated();
}