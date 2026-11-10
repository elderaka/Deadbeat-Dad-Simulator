using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BapakSimulator
{
    [DisallowMultipleComponent, RequireComponent(typeof(BoxCollider))]
    public sealed class ParkingMarker : MonoBehaviour
    {
        [SerializeField] private GameObject routeVisual;
        [SerializeField] private Transform[] destinations = Array.Empty<Transform>();
        [SerializeField, Min(0.01f)] private float parkingSpeedKph = .5f;
        [SerializeField] private UnityEvent<bool> occupiedChanged = new();

        private readonly HashSet<VehicleDrive> vehicles = new();
        private BoxCollider bay;

        public bool IsOccupied => vehicles.Count > 0;
        internal IEnumerable<VehicleDrive> Vehicles => vehicles;

        private void Awake()
        {
            bay = GetComponent<BoxCollider>();
            if (bay.isTrigger && parkingSpeedKph > 0) return;
            Debug.LogError($"Parking marker '{name}' needs a trigger and a positive parking speed.", this);
            enabled = false;
        }

        internal void SetRouteVisible(bool visible)
        {
            if (routeVisual != null) routeVisual.SetActive(visible);
        }

        internal bool Accepts(Transform destination) =>
            destination != null && Array.IndexOf(destinations, destination) >= 0;

        internal bool ContainsStoppedVehicle(VehicleDrive vehicle)
        {
            if (!isActiveAndEnabled || vehicle.SpeedKph > parkingSpeedKph) return false;
            Vector3 offset = bay.transform.InverseTransformPoint(vehicle.transform.position) - bay.center;
            Vector3 half = bay.size * .5f;
            return Mathf.Abs(offset.x) <= half.x && Mathf.Abs(offset.y) <= half.y && Mathf.Abs(offset.z) <= half.z;
        }

        internal void SetParked(VehicleDrive vehicle, bool parked)
        {
            bool wasOccupied = IsOccupied;
            if (parked) vehicles.Add(vehicle);
            else vehicles.Remove(vehicle);
            if (wasOccupied != IsOccupied) occupiedChanged.Invoke(IsOccupied);
        }

        private void OnDisable()
        {
            if (!IsOccupied) return;
            vehicles.Clear();
            occupiedChanged.Invoke(false);
        }
    }
}
