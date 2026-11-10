using UnityEngine;

namespace BapakSimulator
{
    public sealed class TitleBackdropCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField, Min(0)] private float arcDegrees;
        [SerializeField, Min(1)] private float cycleSeconds;

        private Vector3 offset;
        private float started;

        private void OnEnable()
        {
            if (target == null || cycleSeconds <= 0)
            {
                enabled = false;
                return;
            }

            offset = transform.position - target.position;
            started = Time.unscaledTime;
        }

        private void LateUpdate()
        {
            float progress = (Time.unscaledTime - started) / cycleSeconds;
            float angle = Mathf.Sin(progress * Mathf.PI * 2f) * arcDegrees;
            Vector3 position = target.position + Quaternion.AngleAxis(angle, Vector3.up) * offset;
            transform.SetPositionAndRotation(position, Quaternion.LookRotation(target.position - position));
        }
    }
}
