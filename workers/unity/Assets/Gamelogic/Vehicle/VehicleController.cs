using Assets.Gamelogic.Core;
using Improbable;
using Improbable.Core;
using Improbable.Unity.Visualizer;
using Improbable.Vehicle;
using System;
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

        private float[] desiredSpeedBuffer;
        private int desiredSpeedBufferIndex;

        private bool brakeDebounce;

        private void OnEnable()
        {
            rigidBody = GetComponent<Rigidbody>();
            desiredSpeedBuffer = Deserialise(vehicleControlWriter.Data.reactionBuffer.BackingArray);
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

        private float[] Deserialise(byte[] bytes)
        {
            var result = new float[bytes.Length / sizeof(float)];
            for (var i = 0; i < bytes.Length / sizeof(float); i++)
            {
                result[i] = BitConverter.ToSingle(bytes, i * sizeof(float));
            }
            return result;
        }

        private void Serialise(float[] floats, byte[] bytes)
        {
            for (var i = 0; i < floats.Length; i++)
            {
                var b = BitConverter.GetBytes(floats[i]);
                Buffer.BlockCopy(b, 0, bytes, i * b.Length, b.Length);
            }
        }

        private float SetDesiredSpeed(float newDesired)
        {
            var currentDesired = desiredSpeedBuffer[desiredSpeedBufferIndex];

            desiredSpeedBuffer[desiredSpeedBufferIndex++] = newDesired;
            desiredSpeedBufferIndex %= desiredSpeedBuffer.Length;

            Serialise(desiredSpeedBuffer, vehicleControlWriter.Data.reactionBuffer.BackingArray);

            vehicleControlWriter.Send(new VehicleControl.Update()
                .SetSpeed(currentDesired)
                .SetReactionBufferIndex(desiredSpeedBufferIndex)
                .SetReactionBuffer(vehicleControlWriter.Data.reactionBuffer));

            return currentDesired;

            //vehicleControlWriter.Send(new VehicleControl.Update().SetSpeed(newDesired));
            //return newDesired;
        }

        private float UpdateDesiredSpeed()
        {
            var maxSpeed = vehicleControlWriter.Data.maxSpeed;
            
            if (!sensor.NearbyObjects.Any())
            {
                return SetDesiredSpeed(maxSpeed);
            }

            var closest = GetClosest(sensor.NearbyObjects);
            
            var distance = (closest.transform.position - transform.position).magnitude;

            var desiredSpeed = distance / vehicleControlWriter.Data.responseScaling;

            if (distance < vehicleControlWriter.Data.panicDistance)
            {
                return SetDesiredSpeed(0f);
            }

            desiredSpeed = Mathf.Clamp(desiredSpeed, 0f, maxSpeed);

            return SetDesiredSpeed(desiredSpeed);
        }

        private GameObject GetClosest(System.Collections.Generic.List<GameObject> objects)
        {
            GameObject closest = null;
            float closestDistance = float.MaxValue;
            foreach (var o in objects)
            {
                if (o == null) continue;
                if (o.transform == null || o.transform.position == null) continue;

                var distance = (o.transform.position - transform.position).magnitude;

                if (distance < closestDistance)
                {
                    closest = o;
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