using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Vehicle
{
    [RequireComponent(typeof(BoxCollider))]
    public class SensorScale : MonoBehaviour
    {
        public BoxCollider BoxCollider;

        [Require]
        private Improbable.Vehicle.Sensor.Reader sensorReader;

        private void OnEnable()
        {
            var range = sensorReader.Data.sensorRange;
            BoxCollider.size = new Vector3(range, 5f, range);

            var offset = -3f;
            offset -= range / 2f;

            transform.localPosition = new Vector3(0f, 0f, offset);
        }
    }
}