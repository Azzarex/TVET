using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AxlPlay;

namespace AxlPlay
{
    public class JumpScare : MonoBehaviour
    {

        public GameObject Jumpscare;
        public Animator JumpscareAnimator;
        public float LifeTime = 5f;
        public AudioSource audioSource;
        public AudioClip SpawnSound;
        private float timer;

        // activate and play sound
        public void SpawnJumpscare()
        {
            Jumpscare.SetActive(true);
            if (audioSource && SpawnSound)
                audioSource.PlayOneShot(SpawnSound);
        }

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
            // check to desactivate after seconds
            if (Jumpscare.activeInHierarchy)
            {
                timer += Time.deltaTime;


                if (timer >= LifeTime)
                {

                    timer = 0f;
                    Jumpscare.SetActive(false);

                }
            }
        }

        public void CreateJumpscare()
        {
            if (!Jumpscare.GetComponent<Animator>())
                Jumpscare.AddComponent<Animator>();

            JumpscareAnimator = Jumpscare.GetComponent<Animator>();
            if (JumpscareAnimator)
            {
                JumpscareAnimator.applyRootMotion = true;
                JumpscareAnimator.runtimeAnimatorController = Resources.Load("Jumpscare") as RuntimeAnimatorController;
            }
            if (audioSource && SpawnSound)
            {
                audioSource.clip = SpawnSound;
                audioSource.playOnAwake = false;
                audioSource.loop = false;
            }

        }


    }

}