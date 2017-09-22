using Assets.Gamelogic.Core;
using Improbable;
using Improbable.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Vehicle
{
    [RequireComponent(typeof(Rigidbody))]
    public class VehicleController : MonoBehaviour
    {
        [Require]
        private Position.Writer positionWriter;
        [Require]
        private Rotation.Writer rotationWriter;
        
        private Rigidbody rigidBody;

        private void OnEnable()
        {
            rigidBody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            const float speed = 10f;
            UpdatePosition(speed);
        }

        private void UpdatePosition(float speed)
        {
            var current = positionWriter.Data.coords.ToUnityVector();

            var distanceFromCentre = transform.position.magnitude;

            var av = speed / distanceFromCentre;

            var angle = Mathf.Atan2(current.z, current.x);
            var x = Mathf.Cos(angle - av) * SimulationSettings.TrackRadius;
            var y = Mathf.Sin(angle - av) * SimulationSettings.TrackRadius;

            var v = new Vector3(x, 0, y) - transform.position;
            v.Normalize();

            rigidBody.velocity = v * speed;
            rigidBody.rotation = UnityEngine.Quaternion.AngleAxis(-angle * 180 / Mathf.PI, Vector3.up);

            positionWriter.Send(new Position.Update().SetCoords(VectorToCoordinates(rigidBody.position)));
            rotationWriter.Send(new Rotation.Update().SetRotation(rigidBody.rotation.ToNativeQuaternion()));
        }

        private Coordinates VectorToCoordinates(Vector3 vector)
        {
            return new Coordinates(vector.x, vector.y, vector.z);
        }
    }
}