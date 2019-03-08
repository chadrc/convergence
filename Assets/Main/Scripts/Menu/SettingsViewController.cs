using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsViewController : MonoBehaviour
{
    public Slider MasterVolumeSlider;
    public Slider MusicVolumeSlider;
    public Slider SFXVolumeSlider;

    public Toggle OnDemandToggleOn;
    public Toggle OnDemandToggleOff;

    private Vector2 initPos;
    private Vector2 destPos;

    // Stop script from changing values accidently
    private bool canEdit = false;

    void Start()
    {
        initPos = (transform as RectTransform).anchoredPosition;
        destPos = initPos - new Vector2(500, 0);

        MasterVolumeSlider.value = Preferences.MasterVolume;
        MusicVolumeSlider.value = Preferences.MusicVolume;
        SFXVolumeSlider.value = Preferences.SFXVolume;
        //Debug.Log("On Demand Select: " + Preferences.OnDemandUnitPercent);
        canEdit = true;
    }

    public void Open()
    {
        GetComponent<CanvasGroup>().interactable = true;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        StartCoroutine(MenuAnimations.FadeMoveCanvasGroup(GetComponent<CanvasGroup>(), destPos, initPos, .2f, 1.0f));
    }

    public void Close()
    {
        StartCoroutine(MenuAnimations.FadeMoveCanvasGroup(GetComponent<CanvasGroup>(), initPos, destPos, .2f, -1.0f));
        GetComponent<CanvasGroup>().interactable = false;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnMasterVolumeChange(float volume)
    {
        Preferences.MasterVolume = volume;
    }

    public void OnMusicVolumeChange(float volume)
    {
        Preferences.MusicVolume = volume;
    }

    public void OnEffectsVolumeChange(float volume)
    {
        Preferences.SFXVolume = volume;
    }

    public void OnSaveBtnPressed()
    {
        Preferences.Save();
    }

    public void OnSaveAndExitBtnPressed()
    {
        gameObject.SetActive(false);
        Preferences.Save();
    }
}
