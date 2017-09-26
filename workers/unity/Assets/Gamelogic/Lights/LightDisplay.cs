using System;
using Improbable.Light;
using Improbable.Unity.Visualizer;
using UnityEngine;

public class LightDisplay : MonoBehaviour
{
    public Transform RedLight;
    public Transform AmberLight;
    public Transform GreenLight;

    [Require]
    private TrafficLight.Reader lightControlReader;

    private void OnEnable()
    {
        lightControlReader.RedOnTriggered.Add(OnRedOn);
        lightControlReader.RedOffTriggered.Add(OnRedOff);
        lightControlReader.AmberOnTriggered.Add(OnAmberOn);
        lightControlReader.AmberOffTriggered.Add(OnAmberOff);
        lightControlReader.GreenOnTriggered.Add(OnGreenOn);
        lightControlReader.GreenOffTriggered.Add(OnGreenOff);
    }

    private void OnDisable()
    {
        lightControlReader.RedOnTriggered.Remove(OnRedOn);
        lightControlReader.RedOffTriggered.Remove(OnRedOff);
        lightControlReader.AmberOnTriggered.Remove(OnAmberOn);
        lightControlReader.AmberOffTriggered.Remove(OnAmberOff);
        lightControlReader.GreenOnTriggered.Remove(OnGreenOn);
        lightControlReader.GreenOffTriggered.Remove(OnGreenOff);
    }

    private void OnRedOn(LightEvent obj)
    {
        RedLight.gameObject.SetActive(true);
    }

    private void OnRedOff(LightEvent obj)
    {
        RedLight.gameObject.SetActive(false);
    }

    private void OnAmberOn(LightEvent obj)
    {
        AmberLight.gameObject.SetActive(true);
    }

    private void OnAmberOff(LightEvent obj)
    {
        AmberLight.gameObject.SetActive(false);
    }

    private void OnGreenOn(LightEvent obj)
    {
        GreenLight.gameObject.SetActive(true);
    }

    private void OnGreenOff(LightEvent obj)
    {
        GreenLight.gameObject.SetActive(false);
    }
}
