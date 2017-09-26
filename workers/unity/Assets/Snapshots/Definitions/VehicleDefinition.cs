using UnityEngine;

namespace Assets.Snapshots.Definitions
{
    public class VehicleDefinition
    {
        public int? Id { get; set; }
        public float? MaxAcceleration { get; set; }
        public float? MaxSpeed { get; set; }
        public float? PanicDistance { get; set; }
        public int? ReactionTime { get; set; }
        public float? ResponseScaling { get; set; }
        public float? SensorRange { get; set; }
        public Color? Colour { get; set; }
    }
}