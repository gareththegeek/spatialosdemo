using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Vehicle
{
    [RequireComponent(typeof(SphereCollider))]
    public class ScalePanicDistance : MonoBehaviour
    {
        [Require]
        private Improbable.Vehicle.VehicleControl.Reader vehicleControlReader;

        public SphereCollider SphereCollider;

        private void OnEnable()
        {
            SphereCollider.radius = vehicleControlReader.Data.panicDistance;
        }
    }
}