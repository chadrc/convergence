using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System;

public class AudioManager : MonoBehaviour
{
    private static AudioManager current;

    public AudioMixer MasterMixer;
    public SoundGroup NovaSndGrp;
    public SoundGroup PlanetCapturedSndGrp;
    public SoundGroup PlanetLostSndGrp;
    public SoundGroup UISndGrp;
    public SoundGroup UnitDeathSndGrp;
    public SoundGroup TowerSelectSndGrp;
    public SoundGroup LevelThemeSndGrp;
    public SoundGroup TowerAttackedSndGrp;

    void Awake()
    {
        if (current != null)
        {
            Debug.LogWarning("Multiple Audio Managers. Destroying: " + this);
            GameObject.Destroy(gameObject);
        }
        else
        {
            current = this;
            Preferences.PreferencesChanged += OnPreferencesChanged;
            DontDestroyOnLoad(this);
        }
    }

    void Start()
    {
        SetVolumes();
    }

    private void OnPreferencesChanged()
    {
        //Debug.Log("Volume Changed");
        SetVolumes();
    }

    private void SetVolumes()
    {
        //Debug.Log("Setting Volumes: " + Preferences.MusicVolume + " | " + Preferences.SFXVolume);
        MasterMixer.SetFloat("MasterVolume", Mathf.Lerp(-80, 0, Preferences.MasterVolume));
        MasterMixer.SetFloat("MusicVolume", Mathf.Lerp(-80, 0, Preferences.MusicVolume));
        MasterMixer.SetFloat("SFXVolume", Mathf.Lerp(-80, 0, Preferences.SFXVolume));
    }

    public static void PlayRandomUISound()
    {
        if (current == null) return;
        current.UISndGrp.PlayRandomOneShot();
    }

    public static void PlayRandomUnitDeathSound()
    {
        if (current == null) return;
        current.UnitDeathSndGrp.PlayRandomOneShot();
    }

    public static void PlayRandomPlanetLostSound()
    {
        if (current == null) return;
        current.PlanetLostSndGrp.PlayRandomOneShot();
    }

    public static void PlayRandomPlanetCapturedSound()
    {
        if (current == null) return;
        current.PlanetCapturedSndGrp.PlayRandomOneShot();
    }

    public static void PlayTowerSelected()
    {
        if (current == null) return;
        current.TowerSelectSndGrp.Play(0, true);
    }

    public static void PlayTowerDeselected()
    {
        if (current == null) return;
        current.TowerSelectSndGrp.StopAllSources();
    }

    public static void PlayTowerHovered()
    {
        if (current == null) return;
        current.TowerSelectSndGrp.Play(1, true);
    }

    public static void PlayTowerUnhovered()
    {
        if (current == null) return;
        current.TowerSelectSndGrp.Play(0, true);
    }

    public static void PlayStartingLevelTheme()
    {
        if (current == null) return;
        current.LevelThemeSndGrp.Play(0, true);
    }

    public static void CrossfadeToNextLevelTheme(float time)
    {
        current.LevelThemeSndGrp.CrossFadeToNextTrack(time, true);
    }

    public static void PlayNova(int num)
    {
        current.NovaSndGrp.Play(num);
    }

    public static void PlayRandomTowerAttackedSound()
    {
        current.TowerAttackedSndGrp.PlayRandomOneShot();
    }

    public static void StopAll()
    {
        current.NovaSndGrp.StopAllSources();
        current.PlanetCapturedSndGrp.StopAllSources();
        current.PlanetLostSndGrp.StopAllSources();
        current.UISndGrp.StopAllSources();
        current.UnitDeathSndGrp.StopAllSources();
        current.TowerSelectSndGrp.StopAllSources();
        current.LevelThemeSndGrp.StopAllSources();
        current.TowerAttackedSndGrp.StopAllSources();
    }
}