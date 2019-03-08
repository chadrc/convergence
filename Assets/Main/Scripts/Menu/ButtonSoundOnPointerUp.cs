using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class ButtonSoundOnPointerUp : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        //FMOD_StudioSystem.instance.PlayOneShot("event:/UI", Camera.main.transform.position, Preferences.SFXVolume);
        AudioManager.PlayRandomUISound();
    }
}
