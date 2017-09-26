using System.Collections.Generic;
using Improbable;
using Improbable.Worker;
using Assets.Gamelogic.Core;
using UnityEngine;
using Improbable.Vehicle;
using Assets.Gamelogic.EntityTemplates;
using Improbable.Light;
using System.Linq;
using Assets.Snapshots.Definitions;

namespace Assets.Snapshots
{
    public class SnapshotBuilder
    {
        public IDictionary<EntityId, Entity> Build(ISnapshot snapshot)
        {
            Debug.LogFormat("Building..");

            var entities = new Dictionary<EntityId, Entity>();
            var currentEntityId = 1;

            entities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreatePlayerCreatorTemplate());

            AddVehicles(ref currentEntityId, entities, snapshot);

            if (snapshot.HasTrafficLight)
            {
                var position = new Vector3(SimulationSettings.TrackRadius + 2f, 0f, 0f);
                var data = new TrafficLightData(0, 8f, 2f, 20f, 2f);
                entities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateTrafficLightTemplate(position, Quaternion.Euler(0f, 180f, 0f), data));
            }

            return entities;
        }

        private static void AddVehicles(ref int currentEntityId, Dictionary<EntityId, Entity> snapshotEntities, ISnapshot snapshot)
        {
            var index = 0;
            for (var i = 0; i < snapshot.VehicleCount; i++)
            {
                var angle = 2 * Mathf.PI * i / snapshot.VehicleCount;

                var x = SimulationSettings.TrackRadius * Mathf.Cos(angle);
                var y = SimulationSettings.TrackRadius * Mathf.Sin(angle);
                var position = new Vector3(x, 0f, y);

                var quaternion = Quaternion.AngleAxis(-angle * 180 / Mathf.PI, Vector3.up);

                var data = new VehicleControlData();
                data.maxSpeed = RandomRange(snapshot.MaxSpeed);
                data.maxAcceleration = RandomRange(snapshot.MaxAcceleration);
                data.responseScaling = RandomRange(snapshot.ResponseScaling);
                data.panicDistance = RandomRange(snapshot.PanicDistance);
                data.reactionTime = RandomRange(snapshot.ReactionTime);

                var sensorData = new SensorData();
                sensorData.sensorRange = RandomRange(snapshot.SensorRange);

                data.colourRed = 1f;
                data.colourGreen = 1f;
                data.colourBlue = 1f;

                data.speed = 0f;
                data.desiredSpeed = data.maxSpeed;
                data.reactionBuffer = Bytes.FromBackingArray(new byte[data.reactionTime * sizeof(float)]);

                var id = currentEntityId;

                if (snapshot.SpecialVehicles.Any(v => v.Id == id))
                {
                    PopulateSpecial(ref data, ref sensorData, snapshot.SpecialVehicles.Single(v => v.Id == id));
                    index += 1;
                }
                else if (index < snapshot.SpecialVehicles.Count)
                {
                    if (snapshot.SpecialVehicles[index].Id == null)
                    {
                        PopulateSpecial(ref data, ref sensorData, snapshot.SpecialVehicles[index]);
                    }
                    index += 1;
                }

                snapshotEntities.Add(
                    new EntityId(currentEntityId++), 
                    EntityTemplateFactory.CreateVehicleTemplate(position, quaternion, data, sensorData));
            }
        }

        private static void PopulateSpecial(ref VehicleControlData data, ref SensorData sensorData, VehicleDefinition vehicleDefinition)
        {
            if (vehicleDefinition.MaxAcceleration != null) data.maxAcceleration = (float)vehicleDefinition.MaxAcceleration;
            if (vehicleDefinition.MaxSpeed != null) data.maxSpeed = (float)vehicleDefinition.MaxSpeed;
            if (vehicleDefinition.PanicDistance != null) data.panicDistance = (float)vehicleDefinition.PanicDistance;
            if (vehicleDefinition.ReactionTime != null) data.reactionTime = (int)vehicleDefinition.ReactionTime;
            if (vehicleDefinition.ResponseScaling != null) data.responseScaling = (float)vehicleDefinition.ResponseScaling;
            if (vehicleDefinition.SensorRange != null) sensorData.sensorRange = (float)vehicleDefinition.SensorRange;
            if (vehicleDefinition.Colour != null)
            {
                data.colourRed = vehicleDefinition.Colour.Value.r;
                data.colourGreen = vehicleDefinition.Colour.Value.g;
                data.colourBlue = vehicleDefinition.Colour.Value.b;
            }
        }

        private static float RandomRange(FloatRange range)
        {
            return UnityEngine.Random.Range(range.Minimum, range.Maximum);
        }

        private static int RandomRange(IntRange range)
        {
            return UnityEngine.Random.Range(range.Minimum, range.Maximum);
        }
    }
}
