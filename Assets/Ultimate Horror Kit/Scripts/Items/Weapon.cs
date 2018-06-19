using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AxlPlay;

[System.Serializable]
public class Effect
{
    public GameObject Go;
    public float OffsetY = 0.1f;
    public int LayerMask;
    public AudioClip Sound;
}
[System.Serializable]

public class TiltEffect
{
    public Vector3 Values;
    public Vector3 Speed;
}
namespace AxlPlay
{

    public class Weapon : MonoBehaviour
    {
        [Header("Shell")]
        public GameObject ShellPrefab;
        public Transform ShellPosBase;
        public float shellForce = 1.1f;
        public float shellRandomForce = 0.35f;
        public float shellTorqueX = 0.1f;
        public float shellTorqueY = 0.15f;
        public float shellRandomTorque = 1.0f;

        
        public bool InfiniteAmmo;
        public string AnimationType = "Rifle";

        public WeaponAmmoTypes WeaponType;

        public int WeaponShoots = 1;

        public float DistanceWithCamera = 0.25f;

        [Header("Sight 2D")]
        [HideInInspector]
        public UIEffects Sight2D;
        public bool UseSight2D;
        public bool SightBreath = true;
        //  public KeyCode HoldBreath = KeyCode.X;

        public float Sight2DLookSensitivity = 2f;
        public float Sight2DCameraFov = 15f;
        public float Sight2DMaxCameraFov = 20f;
        public float Sight2DMinDistanceToShow = 0.01f;
        public float MaxTimeSight2DToExplote = 3.5f;
        public float Sight2DZoomSpeed = 5f;

       
        #region recoil
        [Header("Recoil")]

        public float minKB;
        public float maxKB;
        public float minKBSide;
        public float maxKBSide;
        public float returnSpeed = 5f;


        public float minCameraKB;
        public float maxCameraKB;
        public float minCameraKBSide;
        public float maxCameraKBSide;
        public float returnCameraSpeed = 5f;

        public float MaxAimRecoilX = 0.1f;
        public float MaxAimRecoilY = 0.1f;

        public float maxRecoil_y = 5f;
        public float maxRecoil_x = -20;

        public float recoilSpeed = 10;
        public float RecoilForce = 1f;


        #endregion

        public bool CameraShakeOnShoot = false;

        public GameObject FPSArms;
        // [HideInInspector]
        public PlayerController Owner;
        [HideInInspector]
        public AIPlayer AIOwner;

        /*[Header("Sway")]

        public bool SwayWeapon = true;
        public float Sway_Amount = 0.02f;
        public float Sway_MaxAmount = 0.06f;
        public float Sway_SmoothAmount = 6f;
        public float TiltX = 8f;
        public float TiltY = 4f;
        */
        public bool Automatic = true;

        [Header("Crosshair")]
        public float CrosshairCrouchPrecision = 50f;

        public float CrosshairIdlePrecision = 60f;
        public float CrosshairWalkPrecision = 70f;
        public float CrosshairRunPrecision = 90f;

        public float CrosshairShootingPrecision = 100f;

        public List<Effect> EffectsInHit = new List<Effect>();
        public GameObject MuzzleEffect;
        public AudioClip ShootSound;
        public AudioSource AudioSource;

        public Transform ShootBase;

        public float DamageBody = 10f;
        public float DamageArms = 10f;
        public float DamageLegs = 10f;
        public float DamageTorso = 10f;
        public float DamageHips = 10f;
        public float DamageHeadShot = 20f;
        public float DistanceShoot = 30f;

        public int CartridgeAmmo = 30;
        public int StartAmmunition = 90;
        [HideInInspector]
        public int ammunition;
        [HideInInspector]
        private int _cartridgeAmmo1;

        public float ShootInterval = 0.25f;

        [Header("Reload")]
        public AudioClip MagazineOffSound;
        public AudioClip MagazineOnSound;
        public AudioClip PullSpringSound;
        public AudioClip NoAmmoSound;

        [HideInInspector]
        public bool hasReloaded;

        [HideInInspector]
        public float recoil;

        [HideInInspector]
        public Vector3 initialPosition;


        [Header("Aiming Down")]
        public Vector3 aimPosition;
        public float adsSpeed = 6f;

        public float FovAds = 40;
        public float FovAdsSpeed = 5f;
        [HideInInspector]
        public bool isAimingDown;


        private Vector3 positionBeforePickUp;
        private Quaternion rotationBeforePickUp;
        private Animator animator;

        private bool tiltFlag;
        [HideInInspector]
        public Animation animationC;

        private float horizontal;
        private float waveslice;
        private float vertical;
        private float timer;
        private float translateChange;
        private float totalAxes;

        public int cartridgeAmmo
        {
            get
            {
                return _cartridgeAmmo1;
            }

            set
            {
                _cartridgeAmmo1 = value;
                if (GameManager.Instance)
                    GameManager.Instance.UpdateAmmoUI();

            }
        }

        private bool testFlag;


        private bool called;

        private AnimationSounds animationSounds;

        [HideInInspector]
        public Rigidbody rigidBody;

        [HideInInspector]
        public List<Weapon> tookAmmoFrom;

        // and not a scene object
        private bool wasSpawned;
        private InteractiveObject interactiveObj;

        public void Reset()
        {

            transform.SetParent(null);
            if (FPSArms)
                FPSArms.gameObject.SetActive(false);



                if (wasSpawned)
                {
                   Destroy(this.gameObject);
                }
         
            Owner = null;

            hasReloaded = true;
            ammunition = StartAmmunition;
            cartridgeAmmo = CartridgeAmmo;

            GameManager.Instance.UpdateAmmoUI();

            transform.position = positionBeforePickUp;
            transform.rotation = rotationBeforePickUp;
            recoil = 0f;

        }

        private void Awake()
        {
            wasSpawned = true;

            name = name.Replace("(Clone)", "");

            positionBeforePickUp = transform.position;
            rotationBeforePickUp = transform.rotation;

            rigidBody = GetComponent<Rigidbody>();
            interactiveObj = GetComponent<InteractiveObject>();
        }
        void Start()
        {

            hasReloaded = true;
            ammunition = StartAmmunition;
            cartridgeAmmo = CartridgeAmmo;

            if (UseSight2D)
                Sight2D = GameManager.Instance.Sight2DSniper;

        }

        void Update()
        {

            if (animationSounds)
                animationSounds.weapon = this;

            /*    if (!photonView.isMine || MultiplayerGameManager.Instance.finished)
                    return;*/


            if (!hasReloaded && ammunition > 0)
            {
                if (GameManager.Instance.Player.pickupSystem.IsUsing(this.gameObject))
                {
                    if (!animationC.IsPlaying("Reload"))
                    {

                        Reload();
                    }
                }

                else if (!called)
                {
                    called = true;
                    StartCoroutine(WaitToReload());

                }

            }
            /*
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
            */
            if (GameManager.Instance.Player.pickupSystem.IsUsing(this.gameObject))
            {
                if (animator)
                {
                    if (Owner.GetIsIdle())
                    {
                        animator.SetBool("Idle", true);
                        animator.SetBool("Walk", false);
                        animator.SetBool("Run", false);

                    }
                    if (Owner.GetIsWalk())
                    {
                        animator.SetBool("Walk", true);
                        animator.SetBool("Idle", false);
                        animator.SetBool("Run", false);

                    }
                    if (Owner.GetIsRun())
                    {
                        animator.SetBool("Run", true);
                        animator.SetBool("Walk", false);
                        animator.SetBool("Idle", false);
                    }
                    animator.SetBool("Shoot", Owner.isShooting);

                }


                if (Owner.IsGrounded() && animationC && hasReloaded && !interactiveObj.switchedOut && !animationC.IsPlaying("SwitchOut") && !animationC.IsPlaying("SwitchIn") && !animationC.IsPlaying("Land"))
                {
                    if (!isAimingDown)
                    {
                        if (animationC.GetClip("Idle") != null)
                        {
                            if (Owner.GetIsIdle())
                            {
                                animationC.CrossFade("Idle");
                            }
                        }
                        if (animationC.GetClip("Walk") != null)
                        {
                            if (Owner.GetIsWalk())
                            {
                                animationC.CrossFade("Walk");


                            }
                        }
                        if (animationC.GetClip("Run") != null)
                        {
                            if (Owner.GetIsRun())
                            {

                                animationC.CrossFade("Run");

                            }
                        }
                    }
                    else
                    {

                        if (animationC.GetClip("SightIdle") != null)
                        {
                            if (Owner.GetIsIdle())
                            {
                                animationC.CrossFade("SightIdle");
                            }
                            else
                            {
                                if (animationC.IsPlaying("SightIdle"))
                                {


                                }
                            }
                        }
                        if (animationC.GetClip("SightWalk") != null)
                        {
                            if (Owner.GetIsWalk())
                            {
                                animationC.CrossFade("SightWalk");


                            }

                        }
                        if (animationC.GetClip("SightRun") != null)
                        {
                            if (Owner.GetIsRun())
                            {

                                animationC.CrossFade("SightRun");

                            }

                        }
                    }
                    if (animationC.GetClip("Shoot") != null)
                    {
                        if (Owner.isShooting)
                        {

                            if (isAimingDown)
                            {
                                // before play
                                animationC.Play("SightShoot", PlayMode.StopAll);

                            }
                            else
                            {
                                animationC.Play("Shoot", PlayMode.StopAll);
                            }

                        }
                    }

                }

                if (cartridgeAmmo == 0 && ammunition > 0)
                {
                    ReloadEffects();
                }
                if (cartridgeAmmo == 0 && ammunition <= 0)
                {
                    hasReloaded = false;

                }
                if (cartridgeAmmo < CartridgeAmmo && ammunition > 0 && !Owner.onTriggerWithWeapon && Input.GetButtonDown(InputManager.inputManager.Reload))
                {
                    if (Owner.GrabIcon && Owner.GrabIcon.canvasGroup.alpha < 0.9f || !Owner.GrabIcon)
                    {

                        if (GameManager.Instance.InteractIcon && GameManager.Instance.InteractIcon.canvasGroup.alpha < 0.9f || !GameManager.Instance.InteractIcon)
                        {

                            ReloadEffects();

                        }
                    }
                }
            }

        }


        IEnumerator WaitToReload()
        {
            yield return new WaitForSeconds(3.5f);
            Reload();
            called = false;
        }
        public void DesactivateModel()
        {
            FPSArms.SetActive(false);
            var meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
                meshRenderer.enabled = false;

            var meshRendererChildren = GetComponentsInChildren<MeshRenderer>();

            foreach (MeshRenderer meshRender in meshRendererChildren)
            {
                meshRender.enabled = false;
            }
        }

        public void ActivateModel()
        {

            FPSArms.SetActive(true);
            var meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
                meshRenderer.enabled = true;
            var meshRendererChildren = GetComponentsInChildren(typeof(MeshRenderer), true);

            foreach (MeshRenderer meshRender in meshRendererChildren)
            {
                meshRender.enabled = true;
            }
        }
        public void WeaponKick()
        {
      //      if (!Owner.photonView.isMine)
      //          return;
            if (CameraShakeOnShoot)
                StartCoroutine(Kick3(Owner.camKickBack, new Vector3(-Random.Range(minCameraKB, maxCameraKB), Random.Range(minCameraKBSide, maxCameraKBSide), 0) * (1f + recoil), 0.1f));
            StartCoroutine(Kick3(Owner.weaponKickBack, new Vector3(-Random.Range(minKB, maxKB), Random.Range(minKBSide, maxKBSide), 0) * (1f + recoil), 0.1f));

        }
        
        IEnumerator Kick3(Transform goTransform, Vector3 kbDirection, float time)
        {
            Quaternion startRotation = goTransform.localRotation;
            Quaternion endRotation = goTransform.localRotation * Quaternion.Euler(kbDirection);
            float rate = 1.0f / time;
            var t = 0.0f;
            while (t < 1.0f)
            {
                t += Time.deltaTime * rate;
                goTransform.localRotation = Quaternion.Slerp(startRotation, endRotation, t);
                yield return null;
            }
        }

        // effects
        void ReloadEffects()
        {

            if (hasReloaded)
            {

                if (animationC != null)
                {
                    if (animationC.GetClip("Reload") != null && hasReloaded)
                    {
                        animationC.Play("Reload");



                    }
                }
                if (animator != null)

                    animator.SetTrigger("Reload");
      
            }

            hasReloaded = false;

        }
        void Reload()
        {
            if (InfiniteAmmo)
            {
                hasReloaded = true;
                return;
            }
            // 30 - 14 = 16
            if (ammunition - cartridgeAmmo >= 0)
            {
                ammunition -= (CartridgeAmmo - cartridgeAmmo);
                cartridgeAmmo = CartridgeAmmo;


            }
            else
            {
                cartridgeAmmo = ammunition;
                ammunition = 0;
            }
            hasReloaded = true;
            GameManager.Instance.UpdateAmmoUI();

        }

        public void Pickuped(PlayerController _picker)
        {
            Owner = _picker;
            // desactivate box collider because if it is activated it makes collision force with another colliders
            if (GetComponent<BoxCollider>() && !GetComponent<BoxCollider>().isTrigger)
            {
                GetComponent<BoxCollider>().enabled = false;

            }
            transform.localScale = Vector3.zero;

            // and set kinematic
            if (rigidBody)
                rigidBody.isKinematic = true;
        }
        public void StartUsing()
        {


           Use();
            /*
            if (rigidBody)
            {
                rigidBody.isKinematic = true;
            }

            transform.parent = Owner.WeaponBase;
            transform.localPosition = Vector3.zero;
            transform.localRotation = new Quaternion(0, 0, 0, 0);
            transform.localScale = Vector3.one;
            */
        }
        /*
        public void StopUsing()
        {
            //    photonView.RPC("StopUsingDrop", PhotonTargets.All);
            StopUsingDrop();
            transform.localScale = Vector3.zero;

        }
        */


        public void Use()
        {
            transform.localScale = Vector3.one;

            //     playerController.CurrentWeapon = this;


    //        if (Owner.photonView.isMine)
   //         {
              
                transform.SetParent(Owner.WeaponBase.transform);

                animationC = transform.parent.parent.GetComponent<Animation>();
                GetComponent<InteractiveObject>().animationC = animationC;
             

         

                if (animator)
                    animator.enabled = true;

                GameManager.Instance.PlayerWeapon = this;
                GameManager.Instance.UsingWeapon();

           //     transform.parent.GetComponent<WeaponBaseData>().PickupedWeapon(DistanceWithCamera);

                initialPosition = Vector3.zero;//transform.localPosition;
                animationC = transform.parent.parent.GetComponent<Animation>();
                animator = transform.parent.parent.GetComponent<Animator>();
    //        }
            /*
            else
            {
                transform.SetParent(Owner.ModelWeaponBase.transform);
                transform.localPosition = Vector3.zero;
                transform.localRotation = new Quaternion(0, 0, 0, 0);

            }
            */
            animationSounds = transform.parent.parent.GetComponent<AnimationSounds>();

            if (animator)
                animator.enabled = false;

            Owner.finishPicking = true;

        }

        // hacer que sepa si esta usando este arma if hecho por hacer
        /*
        [PunRPC]

        public void StopUsingDrop()
        {
            if (animator)
                animator.enabled = false;
            if (Owner.photonView.isMine)
            {
                Owner.ModelAnimator.SetBool(AnimationType, false);
                GameManager.Instance.StopUsingWeapon();

            }
            switchedOut = true;

            transform.SetParent(null);
            FPSArms.gameObject.SetActive(false);




            Pickup pickScript = GetComponent<Pickup>();
            pickScript.enabled = true;
            pickScript.TriggerPickup.enabled = true;
            pickScript.PickupByTrigger = true;


            Owner.Items.RemoveAll(x => x.item == GetComponent<Item>());

            Owner.index = 0;

        }

    */

        public void StopUsing()
        {


            if (animator)
                animator.enabled = false;
          //  if (Owner.photonView.isMine)
          //  {
                GameManager.Instance.StopUsingWeapon();

          //  }


            gameObject.SetActive(false);
        }
    }
}