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
            vehicleControlReader.BrakeTriggered.Add(OnBrake);
        }

        private void OnDisable()
        {
            vehicleControlReader.BrakeTriggered.Remove(OnBrake);
        }

        private void OnBrake(BrakeEvent e)
        {
            transform.GetChild(0).gameObject.SetActive(e.brakeOn);
        }
    }
}