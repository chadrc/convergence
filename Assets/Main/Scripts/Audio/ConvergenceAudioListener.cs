using UnityEngine;
using System.Collections;

public class ConvergenceAudioListener : MonoBehaviour
{
    void Awake()
    {
        LevelController.LevelStart += OnLevelStart;
    }

    private void OnLevelStart()
    {
    }
}
