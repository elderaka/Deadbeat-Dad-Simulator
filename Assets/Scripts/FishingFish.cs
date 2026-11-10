using UnityEngine;

namespace BapakSimulator
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class FishingFish : MonoBehaviour
    {
        private enum State { Swimming, Approaching, Hooked, Landed }

        [SerializeField] private PondWater pond;
        [SerializeField] private ItemDefinition catchItem;
        [SerializeField] private FishingFightTuning fightTuning;
        [SerializeField, Min(0.1f)] private float swimSpeed;
        [SerializeField, Min(0.1f)] private float swimAcceleration;
        [SerializeField, Min(0.01f)] private float halfHeight;
        [SerializeField, Min(0)] private float buoyancy;
        [SerializeField, Min(0)] private float waterDrag;
        [SerializeField, Min(0)] private float roamRadius;
        [SerializeField, Min(0.01f)] private float swimDepth;
        [SerializeField, Min(0.01f)] private float patrolRate;
        [SerializeField] private float patrolPhase;

        private Rigidbody body;
        private State state;
        private Vector3 origin;
        private Vector3 swimTarget;
        private Vector3 lineAnchor;
        private float lineLength;
        private bool running;

        public ItemDefinition CatchItem => catchItem;
        public FishingFightTuning FightTuning => fightTuning;
        public Vector3 Position => body.position;
        public bool IsAvailable => isActiveAndEnabled && state == State.Swimming &&
            pond.SubmergedFraction(body.position, halfHeight) > 0;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            origin = body.position;
            if (pond != null && catchItem != null && catchItem.IsValid && fightTuning != null && fightTuning.IsValid &&
                halfHeight > 0 && swimSpeed > 0 && swimAcceleration > 0 && body.useGravity && !body.isKinematic)
                return;

            Debug.LogError($"{nameof(FishingFish)} on '{name}' needs a pond, catch item, fight tuning, and a dynamic gravity-enabled body.", this);
            enabled = false;
        }

        private void OnEnable()
        {
            if (pond != null) pond.Register(this);
        }

        private void OnDisable()
        {
            if (pond != null) pond.Unregister(this);
        }

        internal void Approach(Vector3 target)
        {
            swimTarget = target;
            state = State.Approaching;
        }

        internal void Hook(Vector3 anchor, float length)
        {
            state = State.Hooked;
            running = false;
            ConstrainLine(anchor, length);
        }

        internal void ConstrainLine(Vector3 anchor, float length)
        {
            lineAnchor = anchor;
            lineLength = length;
        }

        internal void SetRun(bool active, int turn)
        {
            running = active;
            if (!active) return;

            var away = Vector3.ProjectOnPlane(Position - lineAnchor, Vector3.up).normalized;
            var side = Vector3.Cross(Vector3.up, away) * turn;
            swimTarget = pond.KeepInWater(Position + (away + side * 0.65f).normalized * fightTuning.RunSpeed,
                halfHeight);
        }

        internal void Release()
        {
            if (state != State.Landed) state = State.Swimming;
        }

        internal void Land()
        {
            state = State.Landed;
            body.linearVelocity = Vector3.zero;
        }

        private void FixedUpdate()
        {
            float submerged = pond.SubmergedFraction(body.position, halfHeight);
            if (submerged > 0)
            {
                body.AddForce((-Physics.gravity * buoyancy - body.linearVelocity * waterDrag) * submerged,
                    ForceMode.Acceleration);
                if (state == State.Swimming)
                {
                    float angle = Time.fixedTime * patrolRate + patrolPhase;
                    var target = origin + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * roamRadius;
                    target.y = pond.SurfaceY - swimDepth;
                    Swim(pond.KeepInWater(target, halfHeight), swimSpeed);
                }
                else if (state == State.Approaching || (state == State.Hooked && running))
                    Swim(swimTarget, running && state == State.Hooked ? fightTuning.RunSpeed : swimSpeed);
            }

            if (state != State.Hooked) return;
            var offset = body.position - lineAnchor;
            float stretch = offset.magnitude - lineLength;
            if (stretch <= 0) return;

            var radial = offset.normalized;
            float outwardSpeed = Mathf.Max(0, Vector3.Dot(body.linearVelocity, radial));
            body.AddForce(-radial * Mathf.Min(70f, stretch * 95f + outwardSpeed * 12f), ForceMode.Acceleration);
        }

        private void Swim(Vector3 target, float speed)
        {
            var desired = Vector3.ClampMagnitude((target - body.position) * 4f, speed);
            var acceleration = Vector3.ClampMagnitude((desired - body.linearVelocity) * 8f, swimAcceleration);
            body.AddForce(acceleration, ForceMode.Acceleration);
            var heading = Vector3.ProjectOnPlane(body.linearVelocity, Vector3.up);
            if (heading.sqrMagnitude > 0.04f)
                body.MoveRotation(Quaternion.LookRotation(heading));
        }
    }
}
