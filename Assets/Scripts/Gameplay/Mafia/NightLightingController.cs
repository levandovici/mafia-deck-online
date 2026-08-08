using System.Collections;
using UnityEngine;

namespace Mafia
{
    public class NightLightingController : MonoBehaviour
    {
        public static NightLightingController Instance { get; private set; }

        [SerializeField]
        private Light _sunLight;

        [SerializeField]
        private Light _lampLight;

        private float _originalSunIntensity = 1.0f;
        private float _originalLampIntensity = 6.26f;
        private Color _originalAmbientColor;

        private Color _nightAmbientColor = new Color(0.05f, 0.07f, 0.15f, 1f);

        private Coroutine _transitionCoroutine;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            if (_sunLight == null)
            {
                GameObject sunObj = GameObject.Find("Sun");
                if (sunObj != null) _sunLight = sunObj.GetComponent<Light>();
            }

            if (_lampLight == null)
            {
                GameObject lampObj = GameObject.Find("Lamp");
                if (lampObj != null) _lampLight = lampObj.GetComponent<Light>();
            }

            if (_sunLight != null) _originalSunIntensity = _sunLight.intensity;
            if (_lampLight != null) _originalLampIntensity = _lampLight.intensity;
            _originalAmbientColor = RenderSettings.ambientLight;
        }

        public void SetNight(bool isNight, float duration = 1.5f)
        {
            if (_transitionCoroutine != null) StopCoroutine(_transitionCoroutine);
            _transitionCoroutine = StartCoroutine(TransitionLighting(isNight, duration));
        }

        private IEnumerator TransitionLighting(bool isNight, float duration)
        {
            float elapsed = 0f;
            float startSun = _sunLight != null ? _sunLight.intensity : 0f;
            float targetSun = isNight ? _originalSunIntensity * 0.15f : _originalSunIntensity;

            float startLamp = _lampLight != null ? _lampLight.intensity : 0f;
            float targetLamp = isNight ? _originalLampIntensity * 0.4f : _originalLampIntensity;

            Color startAmbient = RenderSettings.ambientLight;
            Color targetAmbient = isNight ? _nightAmbientColor : _originalAmbientColor;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

                if (_sunLight != null) _sunLight.intensity = Mathf.Lerp(startSun, targetSun, t);
                if (_lampLight != null) _lampLight.intensity = Mathf.Lerp(startLamp, targetLamp, t);
                RenderSettings.ambientLight = Color.Lerp(startAmbient, targetAmbient, t);

                yield return null;
            }

            if (_sunLight != null) _sunLight.intensity = targetSun;
            if (_lampLight != null) _lampLight.intensity = targetLamp;
            RenderSettings.ambientLight = targetAmbient;
        }
    }
}
