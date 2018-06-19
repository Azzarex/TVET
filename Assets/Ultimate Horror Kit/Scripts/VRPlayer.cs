using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AxlPlay
{
    public enum MovementTypes
    {
        Feet,
        BluetoothController
    }
    public class VRPlayer : MonoBehaviour
    {


        public MovementTypes MovementType = MovementTypes.Feet;


        public Transform HeadBase;
        public string GroundTag = "Ground";
        public AudioSource AudioSource;
        public AudioClip[] FootstepsSounds;
        public AudioClip JumpSound;
        public AudioClip TouchGroundSound;


        public float LowerStepLimit = -1.3f;
        public float UpperStepLimit = -0.9f;
        public float SpeedFactor = 0.011f;


        public float WalkMinTime = 0.6f;
        public float DeltaDerivative = 0.045f;
        public float DerivativeThreshold = 17;


        public float Dacc;
        public bool jumping;
        public float JumpSpeed = 250f;
        public float timeJumping = 1f;

        private float timeElapsed;
        private float acc_j_1;
        private float acc_j;


        private float accelerationY;

        private float timeDown;
        private float timeUp;
        private CharacterController characterController;

        private int footstepSoundIndex;

        private PlayerController playerController;

        void Awake()
        {
            characterController = GetComponent<CharacterController>();
            playerController = GetComponent<PlayerController>();
        }
        void Start()
        {

            timeUp = 1000;
            timeDown = 500;

            timeElapsed = 0;


        }


        void Update()
        {
            if (!playerController.canMove)
            {
                // move with real feet movement

                accelerationY = Input.acceleration.y;



                timeElapsed += Time.fixedDeltaTime;


                if (timeElapsed > DeltaDerivative)
                {

                    acc_j = accelerationY;

                    Dacc = Mathf.Abs((acc_j - acc_j_1) / timeElapsed);

                    acc_j_1 = accelerationY;

                    timeElapsed = 0;
                }

                //check downstep
                if (accelerationY > UpperStepLimit)
                {

                    timeUp = Time.fixedTime;
                }
                else if (Time.fixedTime - timeUp > WalkMinTime)
                {

                    timeUp = 1000;
                }

                // player is in  upstep?
                if (accelerationY < LowerStepLimit)
                {

                    timeDown = Time.fixedTime;
                }
                else if (Time.fixedTime - timeUp > WalkMinTime)
                {
                    timeUp = 500;
                }

                // player can jump so call function jump
                if (Dacc > DerivativeThreshold && !jumping)
                {
                    if (AudioSource && !AudioSource.isPlaying && FootstepsSounds.Length > 0)
                    {

                        AudioSource.clip = FootstepsSounds[footstepSoundIndex];
                        AudioSource.Play();

                        if (footstepSoundIndex < FootstepsSounds.Length)
                        {

                            footstepSoundIndex++;

                        }
                        if (footstepSoundIndex > FootstepsSounds.Length - 1)
                        {



                            footstepSoundIndex = 0;

                        }
                    }
                    Jump();

                }

                Move(1 / Mathf.Abs(timeUp - timeDown));
            }
        }
        // perform movement 
        public void Move(float input)
        {
            characterController.SimpleMove(transform.position + HeadBase.transform.forward * input * SpeedFactor + transform.up * 0.01f);
        }
        // jump with character controller
        public void Jump()
        {
            if (AudioSource && JumpSound)
                AudioSource.PlayOneShot(JumpSound);

            Invoke("StopJumping", timeJumping);
            characterController.Move(new Vector3(0, JumpSpeed * 80f, 0));

            jumping = true;

        }
        private void OnCollisionEnter(Collision collision)
        {
            // touch ground sounds
            if (collision.transform.gameObject.CompareTag(GroundTag))
            {
                if (AudioSource && TouchGroundSound)
                {
                    AudioSource.PlayOneShot(TouchGroundSound);

                }

            }

        }

        public void StopJumping()
        {
            jumping = false;
        }

    }
}