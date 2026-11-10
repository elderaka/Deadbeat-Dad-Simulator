using UnityEngine;

namespace BapakSimulator
{
    public sealed class VillageDayNight : MonoBehaviour
    {
        [SerializeField] private PickupDay household;
        [SerializeField] private Light sun;
        [SerializeField] private Light moon;
        [SerializeField] private Transform moonDisk;
        [SerializeField] private Transform skyOrigin;
        [SerializeField] private Material skyProfile;
        [SerializeField, Range(0, 1439)] private int sunrise;
        [SerializeField, Range(0, 360)] private float azimuth;
        [SerializeField, Min(1)] private float moonDistance;
        [SerializeField, Min(0)] private float daylightIntensity;
        [SerializeField, Min(0)] private float moonlightIntensity;
        [SerializeField] private Color daylight;
        [SerializeField] private Color dusk;
        [SerializeField] private Color night;
        [SerializeField] private Color dayAmbient;
        [SerializeField] private Color nightAmbient;

        private Material sky;
        private Material previousSky;
        private Light previousSun;

        private void Awake()
        {
            if (household == null || sun == null || moon == null || moonDisk == null || skyOrigin == null || skyProfile == null)
            {
                Debug.LogError("Village Day Night needs the day clock, two lights, a moon disk and sky material.", this);
                enabled = false;
                return;
            }

            previousSky = RenderSettings.skybox;
            previousSun = RenderSettings.sun;
            sky = new Material(skyProfile);
            RenderSettings.skybox = sky;
            RenderSettings.sun = sun;
        }

        private void LateUpdate()
        {
            if (!household.IsConfigured) return;
            float phase = (household.ClockMinute - sunrise) * (2f * Mathf.PI / (24 * 60));
            float height = Mathf.Sin(phase);
            float horizontal = Mathf.Cos(phase);
            var compass = Quaternion.AngleAxis(azimuth, Vector3.up);
            var sunDirection = compass * new Vector3(horizontal, height, .23f).normalized;
            var moonDirection = compass * new Vector3(-horizontal, -height, -.23f).normalized;
            sun.transform.rotation = Quaternion.LookRotation(-sunDirection);
            moon.transform.rotation = Quaternion.LookRotation(-moonDirection);
            moonDisk.position = skyOrigin.position + moonDirection * moonDistance;

            float dayBlend = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(-.17f, .27f, height));
            float twilight = Mathf.Clamp01(1f - Mathf.Abs(height) / .32f);
            sun.intensity = daylightIntensity * Mathf.SmoothStep(0, 1, Mathf.InverseLerp(-.04f, .36f, height));
            moon.intensity = moonlightIntensity * (1f - dayBlend);
            sun.color = Color.Lerp(daylight, dusk, twilight);
            moonDisk.gameObject.SetActive(moonDirection.y > 0f);
            RenderSettings.ambientLight = Color.Lerp(nightAmbient, dayAmbient, dayBlend);
            RenderSettings.fogColor = Color.Lerp(night, Color.Lerp(daylight, dusk, twilight * .6f), dayBlend);
            sky.SetColor("_SkyTint", Color.Lerp(Color.Lerp(night, daylight, dayBlend), dusk, twilight * .65f));
            sky.SetFloat("_Exposure", Mathf.Lerp(.3f, 1.15f, dayBlend));
        }

        private void OnDestroy()
        {
            if (sky == null) return;
            if (RenderSettings.skybox == sky)
            {
                RenderSettings.skybox = previousSky;
                RenderSettings.sun = previousSun;
            }
            Destroy(sky);
        }
    }
}
