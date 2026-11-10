using System.Text;
using UnityEngine;

namespace BapakSimulator
{
    public sealed class CommuteHud : MonoBehaviour
    {
        [SerializeField] private CommuteLeg[] trips;
        [SerializeField] private UnityEngine.UI.Text status;

        private CommuteLeg.Progress[] shown;
        private bool initialized;

        private void Awake()
        {
            if (status == null || trips == null || trips.Length == 0 ||
                System.Array.Exists(trips, trip => trip == null || !trip.enabled))
            {
                Debug.LogError($"{nameof(CommuteHud)} on '{name}' has incomplete trip references.", this);
                enabled = false;
                return;
            }

            shown = new CommuteLeg.Progress[trips.Length];
        }

        private void LateUpdate()
        {
            bool changed = !initialized;
            for (int i = 0; i < trips.Length; i++)
            {
                var state = trips[i].State;
                if (shown[i] != state) changed = true;
                shown[i] = state;
            }

            if (!changed) return;

            initialized = true;
            var text = new StringBuilder("Perjalanan keluarga");
            for (int i = 0; i < trips.Length; i++)
            {
                string state = shown[i] switch
                {
                    CommuteLeg.Progress.Underway => "Dalam perjalanan",
                    CommuteLeg.Progress.Complete => "Selesai",
                    _ => "Belum"
                };
                text.Append('\n').Append(trips[i].Label).Append(" • ").Append(state);
            }

            status.text = text.ToString();
        }
    }
}
