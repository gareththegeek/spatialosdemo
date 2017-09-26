using System.Collections.Generic;

namespace Assets.Snapshots.Definitions
{
    public class Heavy : ISnapshot
    {
        public string Name { get { return "heavy"; } }
        public int VehicleCount { get { return 50; } }
        public FloatRange MaxAcceleration { get { return new FloatRange(2f / 20f, 5f / 20f); } }
        public FloatRange MaxSpeed { get { return new FloatRange(50f, 80f); } }
        public FloatRange PanicDistance { get { return new FloatRange(7f, 14f); } }
        public IntRange ReactionTime { get { return new IntRange(1, 5); } }
        public FloatRange ResponseScaling { get { return new FloatRange(0.8f, 1.2f); } }
        public FloatRange SensorRange { get { return new FloatRange(70f, 130f); } }
        public bool HasTrafficLight { get { return false; } }
        public List<VehicleDefinition> SpecialVehicles { get { return new List<VehicleDefinition>(); } }
    }
}
