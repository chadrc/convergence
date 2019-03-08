using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class SoundGroup : MonoBehaviour
{
    public List<AudioClip> Clips;

    private List<AudioSource> Sources;
    private int currentSourceNo = 0;
    private int currentTrackNo = 0;

    public AudioSource CurrentSource
    {
        get
        {
            return Sources[currentSourceNo];
        }
    }

    public AudioClip CurrentTrack
    {
        get
        {
            return Clips[currentTrackNo];
        }
    }

    public void Awake()
    {
        Sources = new List<AudioSource>(gameObject.GetComponents<AudioSource>());
        if (Sources.Count < 1)
        {
            Debug.LogWarning("No Audio Sources on Sound Group: " + this);
        }

        if (Clips.Count < 1)
        {
            Debug.LogWarning("No Audio Clips on Sound Group: " + this);
        }
    }

    public void PlayRandomOneShot()
    {
        int trackNo = Random.Range(0, Clips.Count);
        CurrentSource.PlayOneShot(Clips[trackNo]);
        NextSource();
    }

    public void StopAllSources()
    {
        //Debug.Log("Source Count: " + Sources.Count);
        foreach(var s in Sources)
        {
            s.Stop();
        }
    }

    public void Play(int trackNo = 0, bool loop = false)
    {
        if (trackNo >= Clips.Count) return;
        CurrentSource.clip = Clips[trackNo];
        CurrentSource.loop = loop;
        CurrentSource.Play();
        currentTrackNo = trackNo;
    }

    public void PlaySecondary(int trackNo = 0, bool loop = false)
    {
        NextSource();
        Play(trackNo, loop);
    }

    public void CrossFadeToNextTrack(float time, bool loop = false)
    {
        if (Sources.Count < 2)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = CurrentSource.outputAudioMixerGroup;
            Sources.Add(source);
        }
        StartCoroutine(Crossfade(time, loop));
    }

    // Increments currentSource so a different source is used next time a sound is played.
    // Wraps around if becomes greater than amount of sources
    private void NextSource()
    {
        currentSourceNo++;
        if (currentSourceNo >= Sources.Count)
        {
            currentSourceNo = 0;
        }
    }

    private void NextTrack()
    {
        currentTrackNo++;
        if (currentTrackNo >= Clips.Count)
        {
            currentTrackNo = 0;
        }
    }

    private IEnumerator Crossfade(float time, bool loop)
    {
        float t = 0;

        var sourceOne = CurrentSource;
        NextSource();
        var sourceTwo = CurrentSource;
        NextTrack();
        sourceTwo.clip = CurrentTrack;
        sourceTwo.volume = 0;
        sourceTwo.Play();

        while (t < time)
        {
            t += Time.deltaTime;
            float frac = t / time;

            sourceOne.volume = 1.0f - frac;
            sourceTwo.volume = frac;

            yield return new WaitForEndOfFrame();
        }

        sourceOne.Stop();
        sourceTwo.volume = 1.0f;
    }
}
