using System.Collections.Generic;
using UnityEngine;

namespace BapakSimulator
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class PondWater : MonoBehaviour
    {
        private readonly List<FishingFish> fish = new List<FishingFish>();
        private BoxCollider volume;

        public Bounds Bounds => Volume.bounds;
        public float SurfaceY => Bounds.max.y;
        private BoxCollider Volume => volume != null ? volume : volume = GetComponent<BoxCollider>();

        public bool Raycast(Ray ray, out RaycastHit hit, float range) => Volume.Raycast(ray, out hit, range);

        internal void Register(FishingFish swimmer) => fish.Add(swimmer);
        internal void Unregister(FishingFish swimmer) => fish.Remove(swimmer);

        internal FishingFish FindFish(Vector3 bait, float range)
        {
            FishingFish nearest = null;
            float best = range * range;
            foreach (var swimmer in fish)
            {
                if (!swimmer.IsAvailable) continue;

                float distance = (swimmer.Position - bait).sqrMagnitude;
                if (distance > best) continue;

                best = distance;
                nearest = swimmer;
            }

            return nearest;
        }

        public float SubmergedFraction(Vector3 center, float halfHeight)
        {
            var bounds = Bounds;
            if (center.x < bounds.min.x || center.x > bounds.max.x ||
                center.z < bounds.min.z || center.z > bounds.max.z) return 0;

            float submerged = Mathf.Min(center.y + halfHeight, bounds.max.y) -
                Mathf.Max(center.y - halfHeight, bounds.min.y);
            return Mathf.Clamp01(submerged / (2f * halfHeight));
        }

        public Vector3 KeepInWater(Vector3 target, float margin)
        {
            var bounds = Bounds;
            return new Vector3(
                Mathf.Clamp(target.x, bounds.min.x + margin, bounds.max.x - margin),
                Mathf.Clamp(target.y, bounds.min.y + margin, bounds.max.y - margin),
                Mathf.Clamp(target.z, bounds.min.z + margin, bounds.max.z - margin));
        }
    }
}
