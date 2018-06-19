using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AxlPlay;

namespace AxlPlay
{
    public class Door : MonoBehaviour
    {

        public Lock.LockTypes LockType = Lock.LockTypes.A;
        public Table[] Tables;

        public bool OpenRandomly;
        public float MinTimeToOpenRandomly = 20f;
        public float MaxTimeToOpenRandomly = 40f;

        public string OpenDoorState;
        public string CloseDoorState;
        public float OpenSpeed = 1f;
        public float CloseSpeed = 1f;
        public AudioClip OpenSound;
        public AudioClip CloseSound;
        public AudioClip LockedSound;

        public AudioSource audioSource;

        private bool open;
        private bool close;

        private bool isOpen;
        private KeySystem key;

        private Lock.LockTypes startLockType;
        private Animator animator;
        private float timer;
        private bool flag;
        private float timeToOpenDoorRandomly;
        
        void Awake()
        {

            animator = GetComponent<Animator>();
        }

        void Start()
        {

            startLockType = LockType;

        }
        void Update()
        {
            if (OpenRandomly)
            {
                timer += Time.deltaTime;
                if (!flag)
                {
                    timeToOpenDoorRandomly = Random.Range(MinTimeToOpenRandomly, MaxTimeToOpenRandomly);
                    flag = true;
                }
                if (timer >= timeToOpenDoorRandomly)
                {
                    timer = 0f;
                    flag = false;


                    if (LockType == Lock.LockTypes.Free)
                    {
                            open = false;
                            close = true;
                    }
                    else
                    {
                        open = true;
                        close = false;
                    }

                    if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
                    {

                        if (open)
                        {
                            OpenDoor(OpenSpeed);

                            LockType = Lock.LockTypes.Free;
                            isOpen = true;
                            open = false;
                        }
                    }

                    if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
                    {
                        if (close)
                        {
                            CloseDoor(CloseSpeed);
                            isOpen = false;
                            LockType = startLockType;
                            close = false;

                        }
                    }

                }
            }
        }

        public void OpenOrCloseDoorByEvent()
        {
            timer = 0f;
            flag = false;


            if (LockType == Lock.LockTypes.Free)
            {
                open = false;
                close = true;
            }
            else
            {
                open = true;
                close = false;
            }

            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
            {

                if (open)
                {
                    OpenDoor(OpenSpeed);

                    LockType = Lock.LockTypes.Free;
                    isOpen = true;
                    open = false;
                }
            }

            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
            {
                if (close)
                {
                    CloseDoor(CloseSpeed);
                    isOpen = false;
                    LockType = startLockType;
                    close = false;

                }
            }
        }
        bool JammedDoor()
        {
            foreach(var table in Tables)
            {
                if (table.OnDoor)
                    return true;
            }
            return false;
        }
        public IEnumerator Interaction()
        {
            if (JammedDoor())
                yield break;
    

            if (GameManager.Instance.Player.pickupSystem.CurrentItem != null && GameManager.Instance.Player.pickupSystem.CurrentItem.item != null)
                key = GameManager.Instance.Player.pickupSystem.CurrentItem.item.GetComponent<KeySystem>();

            bool doorWasOpen = false;

            if (key != null)
            {

                if (key.LockType == LockType)
                {
                    doorWasOpen = true;
                    open = true;
                    close = false;

                }
                else if (LockType != Lock.LockTypes.Free)
                {

                    if (audioSource != null && LockedSound != null)
                    {
                        audioSource.PlayOneShot(LockedSound);
                    }
                    GameManager.Instance.SendMessage("KeyDontMatch");

                    // NotificationCenter.DefaultCenter.PostNotification (this, "KeyDontMatch");
                }
            }


            if (LockType == Lock.LockTypes.Free)
            {
                if (isOpen)
                {
                    open = false;
                    close = true;
                }
                else
                {
                    open = true;
                    close = false;
                }

            }
            else if (!doorWasOpen)
            {
                //NotificationCenter.DefaultCenter.PostNotification (this, "NeedAKey");
                GameManager.Instance.SendMessage("NeedAKey");
            }
            
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
            {

                if (open)
                {
                    OpenDoor(OpenSpeed);

                    LockType = Lock.LockTypes.Free;
                    isOpen = true;
                    open = false;
                }
            }

            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
            {
                if (close)
                {
                    CloseDoor(CloseSpeed);
                    isOpen = false;
                    LockType = startLockType;
                    close = false;

                }
            }
        }

        public void OpenDoor(float animationSpeed)
        {
            if (OpenDoorState != "")
            {
                if (key != null)
                {
                    key.audioSource.PlayOneShot(key.OpenDoor);

                }
                if (animator != null)
                {
                    animator.speed = animationSpeed;
                    animator.Play(OpenDoorState);

                }

            }
            if (audioSource != null && OpenSound != null)
            {
                audioSource.PlayOneShot(OpenSound);

            }
            if (transform.parent)
                transform.parent.SendMessage("OpenDoor", SendMessageOptions.DontRequireReceiver);
        }
        public void CloseDoor(float animationSpeed)
        {

            if (CloseDoorState != "")
            {
                if (animator != null)
                {
                    animator.speed = animationSpeed;
                    animator.Play(CloseDoorState);


                }
            }
            if (audioSource != null && CloseSound != null)
            {
                audioSource.PlayOneShot(CloseSound);
            }
        }


    }



}