using UnityEngine;

namespace BapakSimulator
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class FishingFloat : MonoBehaviour
    {
        [SerializeField, Min(0.01f)] private float halfHeight = 0.09f;
        [SerializeField, Min(1)] private float buoyancy = 2.2f;
        [SerializeField, Min(0)] private float waterDrag = 7f;
        [SerializeField, Min(1)] private float tetherStrength = 25f;
        [SerializeField, Min(0)] private float tetherDamping = 7f;

        private Rigidbody body;
        private PondWater water;
        private Vector3? target;

        public Vector3 Position => Body.position;
        private Rigidbody Body => body != null ? body : body = GetComponent<Rigidbody>();
        public bool IsInWater => water != null && water.SubmergedFraction(Position, halfHeight) > 0;

        public void Launch(PondWater pond, Vector3 origin, Vector3 velocity)
        {
            water = pond;
            target = null;
            Body.position = origin;
            Body.linearVelocity = velocity;
            Body.angularVelocity = Vector3.zero;
        }

        public void SetTether(Vector3? point) => target = point;

        private void FixedUpdate()
        {
            float submerged = water != null ? water.SubmergedFraction(Body.position, halfHeight) : 0;
            if (submerged > 0)
                Body.AddForce((-Physics.gravity * buoyancy - Body.linearVelocity * waterDrag) * submerged,
                    ForceMode.Acceleration);

            if (target is Vector3 point)
                Body.AddForce((point - Body.position) * tetherStrength - Body.linearVelocity * tetherDamping,
                    ForceMode.Acceleration);
        }
    }
}
