using UnityEngine;

namespace BapakSimulator
{
    public sealed class VehicleDashboard : MonoBehaviour
    {
        [SerializeField] private PlayerController player;
        [SerializeField] private GameObject panel;
        [SerializeField] private UnityEngine.UI.Text readout;

        private void LateUpdate()
        {
            var vehicle = player.ActiveVehicle;
            panel.SetActive(vehicle != null);
            if (vehicle == null) return;

            string camera = player.VehicleCameraHint;
            readout.text = $"{vehicle.name}   {vehicle.SpeedKph:0} km/j\n" +
                $"RPM ~{vehicle.EstimatedRpm:0}   |   Bensin {vehicle.FuelLiters:0.00}/{vehicle.FuelCapacity:0.0} L\n" +
                vehicle.Status + (string.IsNullOrEmpty(camera) ? string.Empty : $"\n{camera}");
        }
    }
}
