using UnityEngine;

namespace BapakSimulator
{
    [CreateAssetMenu(menuName = "Bapak Simulator/Vehicle Handling")]
    public sealed class VehicleHandling : ScriptableObject
    {
        [SerializeField, Min(0.1f)] private float forwardSpeed;
        [SerializeField, Min(0)] private float reverseSpeed;
        [SerializeField, Min(0.1f)] private float acceleration;
        [SerializeField, Min(0.1f)] private float braking;
        [SerializeField, Min(0.1f)] private float steeringRate;
        [SerializeField, Min(0)] private float leanDegrees;
        [SerializeField, Min(0.1f)] private float fuelCapacity;
        [SerializeField, Min(0)] private float litersPerKilometer;
        [SerializeField, Min(0)] private int idleRpm;
        [SerializeField, Min(1)] private int maximumRpm;

        public float ForwardSpeed => forwardSpeed;
        public float ReverseSpeed => reverseSpeed;
        public float Acceleration => acceleration;
        public float Braking => braking;
        public float SteeringRate => steeringRate;
        public float LeanDegrees => leanDegrees;
        public float FuelCapacity => fuelCapacity;
        public float LitersPerKilometer => litersPerKilometer;
        public int IdleRpm => idleRpm;
        public int MaximumRpm => maximumRpm;

        public bool IsValid => forwardSpeed > 0 && reverseSpeed >= 0 && acceleration > 0 &&
            braking > 0 && steeringRate > 0 && fuelCapacity > 0 && litersPerKilometer >= 0 &&
            idleRpm >= 0 && maximumRpm > idleRpm;
    }
}
