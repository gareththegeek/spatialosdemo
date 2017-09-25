using Assets.Gamelogic.Core;
using Improbable;
using Improbable.Core;
using Improbable.Unity.Visualizer;
using Improbable.Vehicle;
using System.Linq;
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
        [Require]
        private VehicleControl.Writer vehicleControlWriter;

        private Rigidbody rigidBody;

        [SerializeField]
        private Sensor sensor;

        private bool brakeDebounce;

        private void OnEnable()
        {
            rigidBody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (Time.time < 10f) return;
            if (vehicleControlWriter == null) return;

            var desired = UpdateDesiredSpeed();

            var speed = UpdateSpeed(desired);

            vehicleControlWriter.Send(
                new VehicleControl.Update()
                    .SetDesiredSpeed(desired)
                    .SetSpeed(speed));

            UpdatePosition(speed);
        }

        private float UpdateDesiredSpeed()
        {
            var maxSpeed = vehicleControlWriter.Data.maxSpeed;

            sensor.NearbyObjects = sensor.NearbyObjects
                .Where(x => x != null)
                .ToList();

            if (!sensor.NearbyObjects.Any())
            {
                return maxSpeed;
            }

            var closest = GetClosest(sensor.NearbyObjects);
            var distance = (closest.transform.position - transform.position).magnitude;
            
            var desiredSpeed = distance;

            desiredSpeed = Mathf.Clamp(desiredSpeed, 0f, maxSpeed);

            return desiredSpeed;
        }

        private Collider GetClosest(System.Collections.Generic.List<Collider> colliders)
        {
            Collider closest = null;
            float closestDistance = float.MaxValue;
            foreach (var collider in colliders)
            {
                if (collider == null) continue;
                if (collider.transform == null || collider.transform.position == null) continue;

                var distance = (collider.transform.position - transform.position).magnitude;

                if (distance < closestDistance)
                {
                    closest = collider;
                    closestDistance = distance;
                }
            }
            return closest;
        }

        private float UpdateSpeed(float desired)
        {
            var data = vehicleControlWriter.Data;
            var speed = data.speed;
            var acceleration = data.maxAcceleration;
            var maxSpeed = data.maxSpeed;

            var delta = desired - speed;

            if (speed != 0f && delta >= -0.2f)
            {
                if (brakeDebounce)
                {
                    brakeDebounce = false;
                    vehicleControlWriter.Send(new VehicleControl.Update().AddBrake(new BrakeEvent(false)));
                }
            }
            else
            {
                if (!brakeDebounce)
                {
                    brakeDebounce = true;
                    vehicleControlWriter.Send(new VehicleControl.Update().AddBrake(new BrakeEvent(true)));
                }
            }

            if (delta >= 0f)
            {
                speed += Mathf.Min(acceleration, delta);
            }
            else if (delta < 0f)
            {
                speed += Mathf.Min(-acceleration, delta);
            }
            speed = Mathf.Clamp(speed, 0f, maxSpeed);

            return speed;
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