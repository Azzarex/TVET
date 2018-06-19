using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AxlPlay
{

    public class KeySystem : MonoBehaviour
    {

        public Lock.LockTypes LockType = Lock.LockTypes.A;
        public AudioSource audioSource;
        public AudioClip OpenDoor;

        void Start()
        {
            if (audioSource)
            {
                audioSource.playOnAwake = false;
                audioSource.loop = false;
            }
        }

        void Update()
        {

        }

        public void Pickuped()
        {
            transform.localScale = Vector3.zero;
        }

        public void StartUsing()
        {
   
        }

        public void StopUsing()
        {
            //	transform.parent = InventoryItems.transform;
            //transform.localScale = Vector3.zero;


        }

    }

}