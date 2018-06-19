using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AxlPlay;

namespace AxlPlay
{
    public class Flashlight : MonoBehaviour
    {
        public float Intensity = 5f;
        //	public GameObject Arms;
        public Collider colliderC;
        public AudioSource audioSource;
        public AudioClip SwitchOnSound;
        public AudioClip SwitchOffSound;
        public Transform SpawnIn;
        public Light Lamp;
        public KeyCode Switch = KeyCode.F;

        private Rechargable rechargable;

        private void Awake()
        {
            rechargable = GetComponent<Rechargable>();
        }
        void Start()
        {
            if (audioSource)
            {
                audioSource.loop = false;
                audioSource.playOnAwake = false;
            }
        }

        void Update()
        {
            if (GameManager.Instance.Player.pickupSystem.IsUsing(this.gameObject))
            {
           
                if (Input.GetKeyDown(Switch))
                {
                    if (Lamp.enabled)
                    {
                        Lamp.enabled = false;

                    }
                    else
                    {
                        if (rechargable.batteries >= 1)
                        {

                            if (audioSource && SwitchOnSound)
                                audioSource.PlayOneShot(SwitchOnSound);
                            Lamp.enabled = true;
                            //    Lamp.intensity *= rechargable.batteries;
                            Lamp.intensity = (rechargable.GetRemainingBattery() * Intensity) / rechargable.totalDuration / (rechargable.MaxBatteries / rechargable.batteries);

                        }

                    }
                }


                if (Lamp.enabled)
                {


                    rechargable.ConsumeBattery();

                   // Debug.Log(rechargable.GetRemainingBattery() +  " total duration " + rechargable.totalDuration + " max batteries " + rechargable.MaxBatteries + " batteries " + rechargable.batteries);

                    Lamp.intensity = (rechargable.GetRemainingBattery() * Intensity) / rechargable.totalDuration / (rechargable.MaxBatteries / rechargable.batteries);


                }

            }
        }
        public void NoBattery()
        {
            Lamp.enabled = false;

        }


        public void Pickuped()
        {

            if (colliderC)
                colliderC.enabled = false;

            //	transform.parent = PlayerData.InventoryItems.transform;
            transform.localScale = Vector3.zero;
            if (Lamp)
                Lamp.enabled = false;
        }

        public void StartUsing()
        {

                transform.parent = SpawnIn;
                if(transform.parent && transform.parent.parent)
                GetComponent<InteractiveObject>().animationC = transform.parent.parent.GetComponent<Animation>();

                transform.localPosition = Vector3.zero;
                transform.localRotation = new Quaternion(0, 0, 0, 0);
                transform.localScale = Vector3.one;
                if (Lamp)
                    Lamp.enabled = false;
     
        }


        public void StopUsing()
        {
          
            transform.localScale = Vector3.zero;
           
            if (Lamp)

                Lamp.enabled = false;
        }
    }

}