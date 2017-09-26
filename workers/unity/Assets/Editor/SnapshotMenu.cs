using Assets.Gamelogic.Core;
using Assets.Snapshots;
using Assets.Snapshots.Definitions;
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
        private static List<ISnapshot> snapshots = new List<ISnapshot>
        {
            new Default(),
            new Heavy(),
            new TrafficLight()
        };

        [MenuItem("Improbable/Snapshots/Generate Snapshots")]
        private static void GenerateSnapshots()
        {
            var builder = new SnapshotBuilder();
            foreach (var snapshot in snapshots)
            {
                var entities = builder.Build(snapshot);
                SaveSnapshot(entities, snapshot.Name);
            }
        }

        private static void SaveSnapshot(IDictionary<EntityId, Entity> snapshotEntities, string snapshotName)
        {
            var path = Path.Combine(SimulationSettings.SnapshotPath, snapshotName + ".snapshot");
            Debug.LogWarning("Snapshot path: " + path);
            File.Delete(path);
            var maybeError = Improbable.Worker.Snapshot.Save(path, snapshotEntities);

            if (maybeError.HasValue)
            {
                Debug.LogErrorFormat("Failed to generate initial world snapshot: {0}", maybeError.Value);
            }
            else
            {
                Debug.LogFormat("Successfully generated snapshot {0} at {1}", snapshotName, path);
            }
        }
    }
}
