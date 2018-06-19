using UnityEngine;

namespace AxlPlay
{
    // light flickering for flame torch or fx
    public class LightFlicker : MonoBehaviour
    {


        public bool flicker = true;

        public float flickerIntensity = 0.5f;
        private Light lightC;

        private float startIntensity;


        void Start()
        {
            lightC = gameObject.GetComponent<Light>();

            startIntensity = lightC.intensity;

        }


        void FixedUpdate()
        {
            if (flicker == true)
            {
                float value = Mathf.PerlinNoise(Random.Range(0.0f, 1000.0f), Time.time);
                lightC.intensity = Mathf.Lerp(startIntensity - flickerIntensity, startIntensity, value);
            }
        }
    }
}