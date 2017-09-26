using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Gamelogic.Vehicle
{
    [RequireComponent(typeof(Collider))]
    public class Sensor : MonoBehaviour
    {
        public List<GameObject> NearbyObjects = new List<GameObject>();
        
        private void OnEnable()
        {
            NearbyObjects = new List<GameObject>();
        }

        private void OnDisable()
        {
            NearbyObjects = new List<GameObject>();
        }

        private void FixedUpdate()
        {
            var thisId = gameObject.EntityId().Id;
            var collider = GetComponent<BoxCollider>();
            
            NearbyObjects = Physics.OverlapBox(collider.transform.position + collider.center, collider.size / 2f, transform.rotation, -1, QueryTriggerInteraction.Ignore)
                .Select(x => x.gameObject)
                .Where(x => x.tag != "Sensor")
                .Where(x => x.EntityId().Id != thisId)
                .Where(x => x.gameObject.activeSelf)
                .ToList();
        }
    }
}