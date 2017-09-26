using Improbable.Unity.Visualizer;
using Improbable.Vehicle;
using UnityEngine;

namespace Assets.Gamelogic.Vehicle
{
    public class BrakeController : MonoBehaviour
    {
        [Require]
        private VehicleControl.Reader vehicleControlReader;

        private void OnEnable()
        {
            if (vehicleControlReader == null) return;

            vehicleControlReader.BrakeTriggered.Add(OnBrake);
        }

        private void OnDisable()
        {
            if (vehicleControlReader == null) return;

            vehicleControlReader.BrakeTriggered.Remove(OnBrake);
        }

        private void OnBrake(BrakeEvent e)
        {
            transform.GetChild(0).gameObject.SetActive(e.brakeOn);
        }
    }
}