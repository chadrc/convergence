using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour
{
    public float transitionTime = 1.0f;

    //FMOD.Studio.EventInstance engine;
    //FMOD.Studio.ParameterInstance texture;
    //FMOD.Studio.ParameterInstance type;

    //FMOD.Studio.EventInstance novaEngine;
    //FMOD.Studio.ParameterInstance novaTexture;

    ////FMOD.Studio.EventInstance unitImpactEngine;
    ////FMOD.Studio.ParameterInstance unitImpactTexture;

    //FMOD.Studio.EventInstance planetCapturedEngine;
    //FMOD.Studio.ParameterInstance planetCapturedTexture;

    //FMOD.Studio.EventInstance planetSelectedEngine;
    //FMOD.Studio.ParameterInstance planetSelectedTexture;

    private int convergenceOccurence;
    private bool novaPlaying = false;

    //private float textureParam;
    //private float typeParam;

    void Awake()
    {
        LevelController.LevelStart += OnLevelStart;
        LevelController.LevelEnd += OnLevelEnd;
        ConvergenceController.ConvergenceOccurred += OnConvergenceOccurred;
        UnitBehavior.UnitKilled += OnUnitDeath;
    }

    void Start()
    {
        //engine = FMOD_StudioSystem.instance.GetEvent("event:/music");
        //novaEngine = FMOD_StudioSystem.instance.GetEvent(NovaEvent.path);
        ////unitImpactEngine = FMOD_StudioSystem.instance.GetEvent(UnitImpactEvent.path);
        //planetCapturedEngine = FMOD_StudioSystem.instance.GetEvent(PlanetCapturedEvent.path);
        //planetSelectedEngine = FMOD_StudioSystem.instance.GetEvent(PlanetSelectedEvent.path);

        //engine.start();
        //engine.getParameter("texture", out texture);
        //engine.getParameter("type", out type);

        //novaEngine.getParameter("Texture", out novaTexture);

        ////unitImpactEngine.getParameter("Texture", out unitImpactTexture);

        //planetCapturedEngine.getParameter("Texture", out planetCapturedTexture);

        //planetSelectedEngine.getParameter("Texture", out planetSelectedTexture);

        UIPointerResponder.PlayerSelectedTower += OnPlayerSelectedTower;
        UIPointerResponder.PlayerDeselectedTower += OnPlayerDeselectedTower;
        UIPointerResponder.PlayerHoveredTower += OnPlayerHoveredTower;
        UIPointerResponder.PlayerUnhoveredTower += OnPlayerUnhoveredTower;

        var tower = TowerController.GetAllTowers();
        foreach (var t in tower)
        {
            //Debug.Log("Tower Attacked Registered");
            t.AttackedByUnit += OnTowerAttackedByUnit;
            t.ChangedFaction += OnTowerChangedFaction;
        }
    }

    void OnDestroy()
    {
        LevelController.LevelStart -= OnLevelStart;
        LevelController.LevelEnd -= OnLevelEnd;
        ConvergenceController.ConvergenceOccurred -= OnConvergenceOccurred;
        UnitBehavior.UnitKilled -= OnUnitDeath;
        try
        {
            AudioManager.StopAll();
        }
        catch (System.Exception)
        {

        }
    }

    private void OnLevelStart()
    {
        AudioManager.PlayStartingLevelTheme();
    }

    void OnLevelEnd(bool result)
    {
    }

    void Update()
    {
        if (ConvergenceController.Exists && ConvergenceController.TimeTillNextConvergence < 30.0f && !novaPlaying)
        {
            //novaEngine.start();
            AudioManager.PlayNova(convergenceOccurence);
            novaPlaying = true;
        }
    }

    void OnPlayerSelectedTower(TowerButtonBehavior tower)
    {
        //planetSelectedEngine.start();
        //planetSelectedTexture.setValue(0);
        AudioManager.PlayTowerSelected();
    }

    void OnPlayerDeselectedTower(TowerButtonBehavior tower)
    {
        //planetSelectedEngine.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        AudioManager.PlayTowerDeselected();
    }

    void OnPlayerHoveredTower(TowerButtonBehavior tower)
    {
        //planetSelectedTexture.setValue(1);
        AudioManager.PlayTowerHovered();
    }

    void OnPlayerUnhoveredTower(TowerButtonBehavior tower)
    {
        //planetSelectedTexture.setValue(0);
        AudioManager.PlayTowerUnhovered();
    }

    void OnUnitDeath(UnitBehavior unit)
    {
        if (unit.Faction == FactionController.PlayerFaction)
        {
            //FMOD_StudioSystem.instance.PlayOneShot(UnitKilledEvent.path, Camera.main.transform.position, Preferences.SFXVolume);
            AudioManager.PlayRandomUnitDeathSound();
        }
    }

    void OnTowerAttackedByUnit(UnitBehavior unit)
    {
        //Debug.Log("Tower Attacked");
        //FMOD_StudioSystem.instance.PlayOneShot(UnitImpactEvent.path, Camera.main.transform.position, Preferences.SFXVolume);
        AudioManager.PlayRandomTowerAttackedSound();
    }

    void OnTowerChangedFaction(int faction)
    {
        if (faction == FactionController.PlayerFaction)
        {
            //planetCapturedTexture.setValue(0);
            AudioManager.PlayRandomPlanetCapturedSound();
            //Debug.Log("Tower Changed to Player");
        }
        else if (faction == FactionController.OtherFaction1)
        {
            //planetCapturedTexture.setValue(1);
            AudioManager.PlayRandomPlanetLostSound();
            //Debug.Log("Tower Changed to AI");
        }

        //planetCapturedEngine.start();
        //FMOD_StudioSystem.instance.PlayOneShot(PlanetCapturedEvent, Camera.main.transform.position);
    }

    void OnDisable()
    {
        //engine.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        //engine.release();
    }

    void OnConvergenceOccurred()
    {
        //StartCoroutine("CrossFadeTrack");
        AudioManager.CrossfadeToNextLevelTheme(transitionTime);
    }

    //IEnumerator CrossFadeTrack()
    //{
    //    if (convergenceOccurence > 3)
    //        yield break;

    //    convergenceOccurence++;
    //    //float tempTexValue = textureParam;

    //    float t = 0.0f;
    //    while (t < transitionTime)
    //    {
    //        t += Time.deltaTime;
    //        //textureParam = Mathf.Lerp(tempTexValue, convergenceOccurence, t / transitionTime);
    //        //texture.setValue(textureParam);
    //        yield return null;
    //    }

    //    //        novaTexture.setValue(convergenceOccurence);
    //    //        novaPlaying = false;
    //    //        novaEngine.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    //    yield return null;
    //}


}
