using Assets.Snapshots.Definitions;
using System.Collections.Generic;

namespace Assets.Snapshots
{
    public interface ISnapshot
    {
        string Name { get; }
        int VehicleCount { get; }
        FloatRange MaxSpeed { get; }
        FloatRange MaxAcceleration { get; }
        FloatRange ResponseScaling { get; }
        FloatRange PanicDistance { get; }
        IntRange ReactionTime { get; }
        FloatRange SensorRange { get; }

        bool HasTrafficLight { get; }

        List<VehicleDefinition> SpecialVehicles { get; }
    }
}
