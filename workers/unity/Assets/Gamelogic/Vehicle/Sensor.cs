using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Gamelogic.Vehicle
{
    [RequireComponent(typeof(Collider))]
    public class Sensor : MonoBehaviour
    {
        private new Collider collider;

        public List<Collider> NearbyObjects = new List<Collider>();

        private void OnEnable()
        {
            collider = GetComponent<Collider>();

            NearbyObjects.AddRange(
                Physics
                    .OverlapBox(collider.bounds.center, collider.bounds.extents / 2f)
                    .Where(x => x.tag != "Sensor"));
        }

        private void OnDisable()
        {
            NearbyObjects = new List<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (NearbyObjects.Contains(other)) return;
            if (other.tag == "Sensor") return;
            if (!collider.bounds.Intersects(other.bounds)) return;

            NearbyObjects.Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!NearbyObjects.Contains(other)) return;

            NearbyObjects.Remove(other);
        }
    }
}