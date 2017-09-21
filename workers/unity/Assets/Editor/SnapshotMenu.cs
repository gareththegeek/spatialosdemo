using Assets.Gamelogic.Core;
using Assets.Gamelogic.EntityTemplates;
using Improbable;
using Improbable.Worker;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor 
{
	public class SnapshotMenu : MonoBehaviour
	{
		[MenuItem("Improbable/Snapshots/Generate Default Snapshot")]
		private static void GenerateDefaultSnapshot()
		{
			var snapshotEntities = new Dictionary<EntityId, Entity>();
			var currentEntityId = 1;

			snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreatePlayerCreatorTemplate());
            
            AddVehicles(ref currentEntityId, snapshotEntities);

            SaveSnapshot(snapshotEntities);
		}

        private static void AddVehicles(ref int currentEntityId, Dictionary<EntityId, Entity> snapshotEntities)
        {
            for (var i = 0; i < SimulationSettings.VehicleCount; i++)
            {
                var angle = 2 * Mathf.PI * i / SimulationSettings.VehicleCount;

                var x = SimulationSettings.TrackRadius * Mathf.Cos(angle);
                var y = SimulationSettings.TrackRadius * Mathf.Sin(angle);
                var position = new Vector3(x, 0f, y);

                var quaternion = Quaternion.AngleAxis(-angle * 180 / Mathf.PI, Vector3.up);
                
                snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateVehicleTemplate(position, quaternion));
            }
        }

        private static void SaveSnapshot(IDictionary<EntityId, Entity> snapshotEntities)
		{
			File.Delete(SimulationSettings.DefaultSnapshotPath);
			var maybeError = Snapshot.Save(SimulationSettings.DefaultSnapshotPath, snapshotEntities);

			if (maybeError.HasValue)
			{
				Debug.LogErrorFormat("Failed to generate initial world snapshot: {0}", maybeError.Value);
			}
			else
			{
				Debug.LogFormat("Successfully generated initial world snapshot at {0}", SimulationSettings.DefaultSnapshotPath);
			}
		}
	}
}
