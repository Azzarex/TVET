using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AxlPlay
{
    public class LampSystem : MonoBehaviour
    {

        public Light lightC;


        public Renderer LampRenderer;
        public Material OnMaterial;
        public Material OffMaterial;


        public Color distortColor = Color.white;
        public bool IsOn;

        private Color baseColor;

        public float blinkFrequency = 0.05f;

        private float blinkIterator = 0f;

        public bool FlickeringLight;

        public AudioSource AudioSource_Switch;
        public AudioClip SwitchOn;
        public AudioClip SwitchOff;


        public AudioSource AudioSource_Noise;
        public AudioClip Noise;


        void Start()
        {
            baseColor = lightC.color;
            if (FlickeringLight)
            {
                if (AudioSource_Noise && Noise)
                {
                    AudioSource_Noise.clip = Noise;
                    AudioSource_Noise.loop = true;
                    AudioSource_Noise.playOnAwake = true;
                    AudioSource_Noise.enabled = false;
                }

            }

            if (!IsOn && lightC)
            {
                lightC.enabled = false;
                if (OnMaterial && OffMaterial && LampRenderer)
                {
                    LampRenderer.material = OffMaterial;
                }
            }
            if (IsOn && lightC)
            {
                lightC.enabled = true;
                if (OnMaterial && OffMaterial && LampRenderer)
                {
                    LampRenderer.material = OnMaterial;
                }
            }
        }

        void Update()
        {
            if (IsOn && FlickeringLight)
            {
                blinkIterator += 1f * Time.deltaTime;

                if (blinkIterator >= blinkFrequency)
                {
                    blinkIterator = Random.Range(0f, blinkFrequency) * 0.5f;

                    if (lightC.color != distortColor)
                        lightC.color = distortColor;
                    else
                        lightC.color = baseColor;
                }

            }
        }

        public void Interaction()
        {
            IsOn = !IsOn;

            if (IsOn)
            {
                lightC.enabled = true;
                if (OnMaterial && OffMaterial && LampRenderer)
                {
                    LampRenderer.material = OnMaterial;
                }
                if (AudioSource_Switch && SwitchOn)
                {
                    AudioSource_Switch.PlayOneShot(SwitchOn);
                }
                if (FlickeringLight && AudioSource_Noise)
                    AudioSource_Noise.enabled = true;
            }
            else
            {
                lightC.enabled = false;
                if (OnMaterial && OffMaterial && LampRenderer)
                {
                    LampRenderer.material = OffMaterial;
                }
                if (AudioSource_Switch && SwitchOff)
                {
                    AudioSource_Switch.PlayOneShot(SwitchOff);
                }
                if (FlickeringLight && AudioSource_Noise)
                    AudioSource_Noise.enabled = false;

            }

        }
        // the same that Interaction func but without play switch sounds because it is called from switch 
        public void SwitchLight()
        {
            IsOn = !IsOn;

            lightC.enabled = IsOn;

            if (FlickeringLight && AudioSource_Noise)
                AudioSource_Noise.enabled = IsOn;

        }
    }
}