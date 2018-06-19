using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AxlPlay
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]

    public class Table : MonoBehaviour
    {
          [HideInInspector]
        public bool OnDoor = true;
        public AudioSource BreakAudioSource;
        public AudioClip BreakSound;

        private Rigidbody rigidBody;
        private void Awake()
        {
            // rigidbody needs to be kinematic, otherwise it will fall from the door
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.isKinematic = true;
        }
        // break and come off table by shooting on it
        public void Break()
        {
            rigidBody.isKinematic = false;
            OnDoor = false;
            if (BreakAudioSource && BreakSound)
                BreakAudioSource.PlayOneShot(BreakSound);
        }
    }
}