using UnityEngine;
using System.Collections;

public static class Preferences
{
    public static event System.Action PreferencesChanged;
    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

    private static float masterVolume;
    private static float musicVolume;
    private static float sfxVolume;

    public static float MasterVolume
    {
        get
        {
            return masterVolume;
        }

        set
        {
            if (masterVolume != value)
            {
                masterVolume = value;
                if (PreferencesChanged != null)
                {
                    PreferencesChanged();
                    Save();
                }
            }
        }
    }

    public static float MusicVolume
    {
        get
        {
            return musicVolume;
        }

        set
        {
            if (musicVolume != value)
            {
                musicVolume = value;
                if (PreferencesChanged != null)
                {
                    PreferencesChanged();
                    Save();
                }
            }
        }
    }

    public static float SFXVolume
    {
        get
        {
            return sfxVolume;
        }

        set
        {
            if (sfxVolume != value)
            {
                sfxVolume = value;
                if (PreferencesChanged != null)
                {
                    PreferencesChanged();
                    Save();
                }
            }
        }
    }

    public static void Initialize()
    {
        if (PlayerPrefs.HasKey(MasterVolumeKey))
        {
            MasterVolume = PlayerPrefs.GetFloat(MasterVolumeKey);
        }
        else
        {
            MasterVolume = 0;
        }

        if (PlayerPrefs.HasKey(MusicVolumeKey))
        {
            MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey);
        }
        else
        {
            MusicVolume = 0;
        }

        if (PlayerPrefs.HasKey(SFXVolumeKey))
        {
            SFXVolume = PlayerPrefs.GetFloat(SFXVolumeKey);
        }
        else
        {
            SFXVolume = 0;
        }
    }

    public static void Save()
    {
        PlayerPrefs.SetFloat(MasterVolumeKey, MasterVolume);
        PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolume);
        PlayerPrefs.SetFloat(SFXVolumeKey, SFXVolume);
    }
}
