using UnityEngine;

namespace BapakSimulator
{
    public sealed class FuelStation : WorldInteractable
    {
        [SerializeField] private PickupDay household;
        [SerializeField] private ParkingMarker serviceBay;
        [SerializeField, Min(1)] private int pricePerLiter;
        [SerializeField, Min(0.1f)] private float litersPerPurchase;

        private void Awake()
        {
            if (household != null &&
                serviceBay != null && pricePerLiter > 0 && litersPerPurchase > 0)
                return;

            Debug.LogError($"{nameof(FuelStation)} on '{name}' has incomplete setup.", this);
            enabled = false;
        }

        public override bool TryInteract()
        {
            if (!isActiveAndEnabled || !household.IsConfigured) return false;

            VehicleDrive nearest = null;
            float closestDistance = float.PositiveInfinity;
            foreach (var vehicle in serviceBay.Vehicles)
            {
                if (vehicle == null || !vehicle.CanRefuel || !serviceBay.ContainsStoppedVehicle(vehicle)) continue;

                float distance = (vehicle.transform.position - transform.position).sqrMagnitude;
                if (distance > closestDistance) continue;

                nearest = vehicle;
                closestDistance = distance;
            }

            if (nearest == null)
                return Refuse("Parkir kendaraan di marka pompa.");

            float missing = nearest.FuelCapacity - nearest.FuelLiters;
            if (missing < 0.001f)
                return Refuse("Tangki sudah penuh.");

            float affordable = (float)household.AvailableCash / pricePerLiter;
            float liters = Mathf.Min(missing, Mathf.Min(litersPerPurchase, affordable));
            if (liters < 0.001f)
                return Refuse("Saldo tidak cukup untuk membeli bensin.");

            int cost = Mathf.Min(household.AvailableCash, Mathf.CeilToInt(liters * pricePerLiter));
            if (!household.TrySpendCash(cost))
                return Refuse("Saldo tidak cukup untuk membeli bensin.");

            nearest.RecordFuelPurchase(liters, cost);
            household.ReportActivity($"{nearest.name}: isi {liters:0.###} L • Rp{cost}.");
            return true;
        }

        private bool Refuse(string message)
        {
            household.ReportActivity(message);
            return false;
        }
    }
}
