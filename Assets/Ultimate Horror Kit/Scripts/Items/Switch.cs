using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AxlPlay
{
    public class Switch : MonoBehaviour
    {
        public AudioSource AudioSource;
        public AudioClip SwitchSound;

        public Transform SwitchRotate;
        public float RotToAngle = 90f;
        public float SpeedRot = 35f;
        public LampSystem[] Lamps;
        private bool isOn;
        private float startRot;

        private void Awake()
        {
            startRot = transform.localEulerAngles.x;

        }
        public void Update()
        {
            Vector3 tempLocalEulers = transform.localEulerAngles;
            if (isOn)
                tempLocalEulers.x = Mathf.Lerp(tempLocalEulers.x, RotToAngle, Time.deltaTime * SpeedRot);
            else
                tempLocalEulers.x = Mathf.Lerp(tempLocalEulers.x, startRot, Time.deltaTime * SpeedRot);

            transform.localEulerAngles = tempLocalEulers;
        }
        public void Interaction()
        {
            foreach(LampSystem lamp in Lamps)
            {
                lamp.SwitchLight();
                isOn = lamp.IsOn;
                if (AudioSource && SwitchSound)
                    AudioSource.PlayOneShot(SwitchSound);

            }
        }

    }
}
