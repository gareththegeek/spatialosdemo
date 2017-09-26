using Improbable.Light;
using Improbable.Unity.Visualizer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    private enum TrafficLightStates
    {
        Stop = 0,
        ReadyGo,
        Go,
        ReadyStop,
        Count
    }

    [Require]
    private TrafficLight.Writer trafficLightWriter;

    private float lastChange = -100f;

    private float GetStateDuration(int state)
    {
        switch ((TrafficLightStates)state)
        {
            case TrafficLightStates.Stop:
                return trafficLightWriter.Data.stopDuration;
            case TrafficLightStates.ReadyGo:
                return trafficLightWriter.Data.readyGoDuration;
            case TrafficLightStates.Go:
                return trafficLightWriter.Data.goDuration;
            case TrafficLightStates.ReadyStop:
                return trafficLightWriter.Data.readyStopDuration;
            default:
                return 0;
        }
    }

    private void FixedUpdate()
    {
        var state = trafficLightWriter.Data.state;
        if (Time.time - lastChange > GetStateDuration(state))
        {
            lastChange = Time.time;

            state += 1;
            if (state >= (int)TrafficLightStates.Count) state = 0;

            var update = new TrafficLight.Update();
            update.SetState(state);
            //Debug.LogWarning("New State " + state);

            switch ((TrafficLightStates)state)
            {
                case TrafficLightStates.Stop:
                    update.AddAmberOff(new LightEvent())
                          .AddRedOn(new LightEvent());
                    break;
                case TrafficLightStates.ReadyGo:
                    update.AddAmberOn(new LightEvent());
                    break;
                case TrafficLightStates.Go:
                    update.AddRedOff(new LightEvent())
                          .AddAmberOff(new LightEvent())
                          .AddGreenOn(new LightEvent());
                    break;
                case TrafficLightStates.ReadyStop:
                    update.AddGreenOff(new LightEvent())
                          .AddAmberOn(new LightEvent());
                    break;
            }

            trafficLightWriter.Send(update);
        }
    }
}
