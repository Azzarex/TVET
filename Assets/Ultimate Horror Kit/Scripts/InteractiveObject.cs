using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AxlPlay;

namespace AxlPlay
{
    [System.Serializable]
    public class InteractiveObject : MonoBehaviour
    {
        
        [HideInInspector]
        public bool switchedOut;

        public bool Sway = true;
        public float Sway_Amount = 0.02f;
        public float Sway_MaxAmount = 0.06f;
        public float Sway_SmoothAmount = 6f;
        public float TiltX = 8f;
        public float TiltY = 4f;

        [Header("Animations")]


        public AnimationClip Idle;
        public AnimationClip SightIdle;
        public AnimationClip Walk;
        public AnimationClip SightWalk;
        public AnimationClip Run;
        public AnimationClip SightRun;
        public AnimationClip Shoot;
        public AnimationClip SightShoot;
        public AnimationClip SwitchInClip;
        public AnimationClip SwitchOutClip;

        public AnimationClip ReloadClip;
        public AnimationClip JumpClip;
        public AnimationClip LandClip;

        public GameObject FPSArms;
        public Vector3 ItemBasePos = new Vector3(0.11f, -0.1f, 0.25f);
        public Vector3 ItemBaseRot;
        [Header("Running")]
        public Vector3 runPosition;
        public Vector3 runRotation;

        public float runPosePosSpeed = 10f;
        public float runPoseRotSpeed = 300f;


        public AudioSource ItemAudioSource;

        public AudioClip StartUsingSound;
        public AudioClip StopUsingSound;
        public AudioClip StartExaminingSound;
        public AudioClip StopExaminingSound;
        public AudioClip PickupedSound;
        public AudioClip ConsumedSound;

        public float HeightIcon = 100f;
        public float WidthIcon = 100f;

        public UIEffects UsingUI;
        public UIEffects ExaminingUI;
        public UIEffects PickupedUI;
        public UIEffects ConsumedUI;


        public string ItemName;
        public string Description;

        public Types Type;
        public bool ShowItemSettings;

        public enum Types
        {
            OneClickInteraction,
            Examinable,
            Usable,
            Consumable
        }


        public Sprite Icon;
        public string InputToInteract = "Interact";
        //public string InputToUse = KeyCode.U;
        public bool Examinable;

        public string InputToExamine = "Examine";

        public bool canRun;
        public Animation animationC;
        public bool canTilt;
        public Vector3 initialPosition;
        public PlayerController Owner;

        private bool playerFlag;


        private bool startRun;


 
        void Start()
        {

            if (ItemAudioSource)
            {
                ItemAudioSource.loop = false;
                ItemAudioSource.playOnAwake = false;
            }
        }

        public void Pickuped(PlayerController owner)
        {

            if (PickupedUI)
                PickupedUI.DoFadeIn();

            Owner = owner;
            if (ItemAudioSource && PickupedSound)
                ItemAudioSource.PlayOneShot(PickupedSound);

        }

        public IEnumerator StartUsing()
        {

            switchedOut = false;

            if (UsingUI)
                UsingUI.DoFadeIn();

            if (ItemAudioSource && StartUsingSound)
                ItemAudioSource.PlayOneShot(StartUsingSound);

            // add animations
            if (Idle)
                animationC.AddClip(Idle, "Idle");
            if (SightIdle)

                animationC.AddClip(SightIdle, "SightIdle");
            if (Walk)

                animationC.AddClip(Walk, "Walk");
            if (SightWalk)

                animationC.AddClip(SightWalk, "SightWalk");
            if (Run)

                animationC.AddClip(Run, "Run");
            if (SightRun)

                animationC.AddClip(SightRun, "SightRun");

            if (Shoot)

                animationC.AddClip(Shoot, "Shoot");
            if (SightShoot)
                animationC.AddClip(SightShoot, "SightShoot");

            if (ReloadClip)

                animationC.AddClip(ReloadClip, "Reload");
            if (SwitchInClip)
                animationC.AddClip(SwitchInClip, "SwitchIn");
            if (SwitchOutClip)
                animationC.AddClip(SwitchOutClip, "SwitchOut");
            if (JumpClip)
                animationC.AddClip(JumpClip, "Jump");
            if (LandClip)
                animationC.AddClip(LandClip, "Land");
            transform.parent.localPosition = ItemBasePos; // new Vector3(transform.parent.localPosition.x, transform.parent.localPosition.y, DistanceWithCamera);
            transform.parent.localEulerAngles = ItemBaseRot;
            transform.localPosition = Vector3.zero;
            transform.localRotation = new Quaternion(0, 0, 0, 0);
            if (FPSArms)
                FPSArms.gameObject.SetActive(true);
            // play switch in animation before start using
            if (SwitchInClip)
            {

                animationC.Play("SwitchIn");
             

                yield return new WaitWhile(() => animationC.IsPlaying("SwitchIn"));

            }
            transform.parent.localPosition = ItemBasePos; //new Vector3(transform.parent.localPosition.x, transform.parent.localPosition.y, DistanceWithCamera);
            transform.parent.localEulerAngles = ItemBaseRot;
            var weaponBaseData = transform.parent.GetComponent<WeaponBaseData>();
            if(weaponBaseData)
            weaponBaseData.PickupedWeapon(ItemBasePos,transform.parent.localRotation,ItemBaseRot);


        }
        public IEnumerator StopUsing()
        {
            if (UsingUI)
                UsingUI.DoFadeOut();

            if (ItemAudioSource && StopUsingSound)
                ItemAudioSource.PlayOneShot(StopUsingSound);


            if (animationC)
            {
                // play switch out animation before stop using completely
                if (SwitchOutClip)
                {

                    animationC.Play("SwitchOut");
                    yield return new WaitWhile(() => animationC.IsPlaying("SwitchOut"));
                    switchedOut = true;

                }


            }
            transform.localScale = Vector3.zero;

            if (FPSArms)
                FPSArms.gameObject.SetActive(false);

        }
        public void StartExamining()
        {

            if (ExaminingUI)
                ExaminingUI.DoFadeIn();

            if (ItemAudioSource && StartExaminingSound)
                ItemAudioSource.PlayOneShot(StartExaminingSound);
        }
        // play basic actions sounds
        public void StopExamining()
        {
            if (ExaminingUI)
                ExaminingUI.DoFadeOut();

            if (ItemAudioSource && StopExaminingSound)
                ItemAudioSource.PlayOneShot(StopExaminingSound);
        }
        public void Consumed()
        {

            if (ConsumedUI)
                ConsumedUI.DoFadeIn();

            if (ItemAudioSource && ConsumedSound)
                ItemAudioSource.PlayOneShot(ConsumedSound);
        }

        void Update()
        {
            if (GameManager.Instance.Player != null && !playerFlag)
            {
                playerFlag = true;
                // it was pickuped in last game
                // load items saved in last game
                if (PlayerPrefs.HasKey(ItemName))
                {
                    GameManager.Instance.Player.pickupSystem.PickupItem(this);
                    if (PlayerPrefs.GetInt(ItemName) == 1)
                        GameManager.Instance.Player.pickupSystem.UseItem(this);

                    //   transform.localScale = Vector3.zero;

                }
            }
            
            // play animations and run pose
            if (GameManager.Instance.Player.pickupSystem.IsUsing(this.gameObject))
            {
                if (Owner.GetIsRun() && transform.parent != null)
                {

                    startRun = true;

                    Vector3 target = runRotation;
                    target.x = (target.x > 180) ? target.x - 360 : target.x;
                    target.y = (target.y > 180) ? target.y - 360 : target.y;
                   target.z = (target.z > 180) ? target.z - 360 : target.z;

                    Vector3 current = transform.parent.localEulerAngles;
                    current.x = (current.x > 180) ? current.x - 360 : current.x;
                    current.y = (current.y > 180) ? current.y - 360 : current.y;
                    current.z = (current.z > 180) ? current.z - 360 : current.z;


                    if (Vector3.Distance(current, target) > 0.01f)
                    {
                        transform.parent.localEulerAngles = Vector3.MoveTowards(current, target, Time.deltaTime * runPoseRotSpeed);

                    }
                    if (Vector3.Distance(transform.parent.localPosition, runPosition) > 0.01f)
                    {
                        transform.parent.localPosition = Vector3.MoveTowards(transform.parent.localPosition, runPosition, Time.deltaTime * runPosePosSpeed);

                    }


                }
                else
                {
                    if (startRun)
                    {
                        transform.parent.localPosition = Vector3.MoveTowards(transform.parent.localPosition, transform.parent.GetComponent<WeaponBaseData>().weaponBaseInitialPosition, Time.deltaTime * runPosePosSpeed);

                        Vector3 target = transform.parent.GetComponent<WeaponBaseData>().weaponBaseInitialLocalEulerAngles;
                        target.x = (target.x > 180) ? target.x - 360 : target.x;
                        target.y = (target.y > 180) ? target.y - 360 : target.y;
                        target.z = (target.z > 180) ? target.z - 360 : target.z;


                        Vector3 current = transform.parent.localEulerAngles;
                        current.x = (current.x > 180) ? current.x - 360 : current.x;
                        current.y = (current.y > 180) ? current.y - 360 : current.y;
                        current.z = (current.z > 180) ? current.z - 360 : current.z;

                        transform.parent.localEulerAngles = Vector3.MoveTowards(current, target, Time.deltaTime * runPoseRotSpeed);

                        if (Vector3.Distance(current, target) < 0.01f)
                        {
                            startRun = false;
                        }
                    }
                }
            }

            if (Owner && Owner.pickupSystem.IsUsing(this.gameObject) && Sway   && !GameManager.Instance.Player.pickupSystem.IsExamining)
            {

                // sway weapon

                float movementX = -InputManager.inputManager.GetAxis(InputManager.inputManager.TurnAroundX) * Sway_Amount;
                float movementY = -InputManager.inputManager.GetAxis(InputManager.inputManager.TurnAroundY) * Sway_Amount;

                movementX = Mathf.Clamp(movementX, -Sway_MaxAmount, Sway_MaxAmount);
                movementY = Mathf.Clamp(movementY, -Sway_MaxAmount, Sway_MaxAmount);

                Vector3 finalPosition = new Vector3(movementX, movementY, 0);
                // tilt weapon when moving into that direction
                // if (!CurrentWeapon.isAimingDown)

                if (canTilt)
                {
                    var tiltAroundZ = -InputManager.inputManager.GetAxis(InputManager.inputManager.MoveHorizontal) * TiltX;
                    var tiltAroundX = -InputManager.inputManager.GetAxis(InputManager.inputManager.MoveVertical) * TiltY;
                    var target = Quaternion.Euler(tiltAroundX, 0, tiltAroundZ);
                    transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * 2f);
                }
                else
                {
                    transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, Time.deltaTime * 2f);

                }

                Vector3 newPos = Vector3.zero;
                Vector3 newRot = Vector3.zero;

                transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * Sway_SmoothAmount);


            }

        }



    }

}