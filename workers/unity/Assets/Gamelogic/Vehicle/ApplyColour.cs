using Improbable.Unity.Visualizer;
using Improbable.Vehicle;
using UnityEngine;

namespace Assets.Gamelogic.Vehicle
{
    [RequireComponent(typeof(Renderer))]
    public class ApplyColour : MonoBehaviour
    {

        [Require]
        private VehicleControl.Reader vehicleControlReader;

        public Renderer Renderer;

        private void OnEnable()
        {
            if (vehicleControlReader == null) return;

            var colour = new Color(
                vehicleControlReader.Data.colourRed,
                vehicleControlReader.Data.colourGreen,
                vehicleControlReader.Data.colourBlue);
            Renderer.material.color = colour;
        }
    }
}