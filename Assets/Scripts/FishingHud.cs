using UnityEngine;

namespace BapakSimulator
{
    public sealed class FishingHud : MonoBehaviour
    {
        [SerializeField] private FishingRod rod;
        [SerializeField] private GameObject panel;
        [SerializeField] private UnityEngine.UI.Text readout;
        [SerializeField] private GameObject tensionTrack;
        [SerializeField] private UnityEngine.UI.Image tensionFill;
        [SerializeField] private Color safeColor;
        [SerializeField] private Color warningColor;
        [SerializeField] private Color dangerColor;
        [SerializeField, Range(0, 1)] private float warningAt;
        [SerializeField, Range(0, 1)] private float dangerAt;

        private void Awake()
        {
            if (rod != null && panel != null && readout != null && tensionTrack != null &&
                tensionFill != null && dangerAt > warningAt) return;

            Debug.LogError($"{nameof(FishingHud)} on '{name}' has incomplete HUD setup.", this);
            enabled = false;
        }

        private void LateUpdate()
        {
            var phase = rod.CurrentPhase;
            bool visible = phase != FishingRod.Phase.Idle;
            if (panel.activeSelf != visible) panel.SetActive(visible);
            if (!visible) return;

            bool fighting = phase == FishingRod.Phase.Hooked;
            if (tensionTrack.activeSelf != fighting) tensionTrack.SetActive(fighting);
            float tension = rod.LineTension;
            tensionFill.fillAmount = tension;
            tensionFill.color = tension >= dangerAt ? dangerColor : tension >= warningAt ? warningColor : safeColor;

            string cue = phase switch
            {
                FishingRod.Phase.Ready => $"Arahkan ke air • {rod.ReelBinding}: lempar",
                FishingRod.Phase.Casting => "Pelampung meluncur",
                FishingRod.Phase.Waiting => "Perhatikan pelampung",
                FishingRod.Phase.Biting => $"Pelampung tenggelam • tahan {rod.ReelBinding}",
                FishingRod.Phase.Retrieving => $"Tahan {rod.ReelBinding} untuk menarik tali",
                _ when tension >= dangerAt => "Tali hampir putus • lepas gulungan",
                _ when rod.FishIsRunning => "Ikan lari • lepas gulungan",
                _ => $"Ikan melambat • tahan {rod.ReelBinding}"
            };

            string detail = fighting ? $"Tegangan {tension:P0} | Tali {rod.LineLength:0.0} m" :
                $"Tangkapan: {rod.CatchCount} | {rod.ExitBinding}: simpan pancing";
            string text = $"<color=#F6CD87><b>MANCING</b></color>\n<b>{cue}</b>\n{detail}";
            if (readout.text != text) readout.text = text;
        }
    }
}
