using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AxlPlay;
using UnityEngine.UI;



namespace AxlPlay
{
    public class ItemInInventory
    {
        public Item item;
    }
    [System.Serializable]
    public class InteractableSound
    {
        public int LayerMask;
        public AudioClip[] Sounds;
    }

    public class PlayerController : MonoBehaviour
    {

        public float MaxTimeRunning = 5f;
        public float CameraZoomFov = 40;
        public float ZoomCameraSpeed = 5f;

        public PickupSystem pickupSystem;
        public float ClimbSpeed = 7f;

        public float TiltRightAngle = -0.1f;
        public float TiltLeftAngle = 0.1f;
        [Header("Grab")]
        public UIEffects GrabIcon;
        public Transform GrabOffset;
        public bool MoveToOffset = true;
        public float GrabFromDistance = 5f;

        [HideInInspector]
        public bool CanMove = true;

        [HideInInspector]
        public Item grabbedItem;
        [HideInInspector]

        public Transform itemLastParent;


        [HideInInspector]
        public int Kills;

        [HideInInspector]
        public int Deaths;

        [Header("Camera")]
        public Camera DollDieCamera;

        public string GroundTag = "Ground";

        public AudioSource AudioSource;
        public InteractableSound[] FootstepsSounds;
        public InteractableSound[] LandSounds;
        public AudioClip JumpSound;
        private int footstepSoundIndex;

        [HideInInspector]
        public float startFov;
        [Header("References")]
        public Animator CameraAnim;
        public Transform FPSView;
        public Transform weaponRecoil;
        public Transform camKickBack;
        public Transform weaponKickBack;

        [Header("Movement")]
        public float CrouchSpeed = 8f;

        public float CrouchHeight = 1f;
        public float JumpForce = 7f;
        public float WalkSpeed = 3.5f;
        public float AimingDown_WalkSpeed = 3f;

        public float RunSpeed = 6f;

        public float CrouchWalkSpeed = 2f;

        public float LookSensitivity = 3f;
        public float AimingDown_LookSensitivity = 2f;

        /*
        [Header("Model Animations")]
        public Animator ModelAnimator;
        public Transform Model;
        public float ModelRightAngle = 0f;
        public float ModelLeftAngle = 0f;
        public float ModelDefaultAngle = 0f;
        */

        [HideInInspector]
        public bool Team1;

        public Transform WeaponBase;
        public Transform ModelWeaponBase;

        private PlayerMotor motor;
        [HideInInspector]
        public Rigidbody rigidBody;
        private CapsuleCollider capsuleCollider;



        private float timer;
        private float distToGround;

        private Health health;
        public int index;




        private bool jumpFlag;
        [HideInInspector]
        public bool isCrouched;
        private float startHeight;
        private CharacterController characterController;
        private Vector3 moveDirection;
        private bool crouching;

        public float Gravity = -15f;
        private float vol;
        [HideInInspector]
        public bool isShooting;
        private bool tiltFlag;
        private bool tiltFlag2;


        private bool inRunPose;
        private bool returningToNormalPose;
        private bool startedToRunPose;


        private bool inJumpPose;
        private bool startedToJumpPose;

        private bool shootRecoilReturn;


        [HideInInspector]
        public float _yRot;
        [HideInInspector]

        public float _cameraRotationX;

        [Header("HeadBob")]
        public bool HeadBob = true;
        public float headbobSpeed = 1f;
        public float headbobStepCounter;
        public float headbobAmountX = 1f;
        public float headbobAmountY = 1f;
        public float eyeHeightRacio = 0.9f;

        public float WalkDistanceToPlayStep = 0.18f;
        public float StickToGroundForce = 10f;
        public float GravityMultiplier = 2f;

        Vector3 parentLastPos;


        private bool isSighting2D;
        private float timeSighting2D;

        private Vector3 lastPos;

        private bool previousGrounded;
        private bool isGrounded;



        [HideInInspector]
        public string userName;

        private bool statsFlag;

        private bool OnLadder;

        private Vector3 climbDirection;
        private Vector3 lateralMove;
        private Vector3 ladderMovement;
        private float downThreshold;

        private float ladderExitTimer;
        private bool ladderExit;

        RaycastHit hit;
        private bool runKeyDown;
        [HideInInspector]
        public bool finishPicking = true;
        [HideInInspector]
        public bool onTriggerWithWeapon;
        [HideInInspector]
        public bool Hidden;

        #region ManagedByMasterClient
        [HideInInspector]
        public float afkTime;
        [HideInInspector]
        public float afkLastShootTime;
        [HideInInspector]
        public Vector3 afklastPos;
        [HideInInspector]
        public Quaternion afklastRot;

        #endregion
        [HideInInspector]
        public bool canLookAround = true;

        private float timerRunning;
        private bool canRun = true;
        private bool movementVR = false;
        private VRPlayer vrPlayer;

        [HideInInspector]
        public bool canMove = true;
        private bool flag;
        public void Reset()
        {
            canLookAround = true;

            // delete the player items
            List<ItemInInventoryOld> deleteThings = new List<ItemInInventoryOld>();
            foreach (var item in pickupSystem.Items)
            {
                if (!pickupSystem.StartItemsSpawned.Contains(item.script))
                {
                    deleteThings.Add(item);
                }

            }

            foreach (var itemToDelete in deleteThings)
            {
                //    photonView.RPC("DropItem", PhotonTargets.All, itemToDelete);
                pickupSystem.DropItem(itemToDelete);
            }


            // use start item
            pickupSystem.UseItem(pickupSystem.StartItemsSpawned[0]);

            Team1 = false;
            index = 0;
            Kills = 0;
            Deaths = 0;


            pickupSystem.Reset();

            PlayerPrefs.DeleteAll();

            health.Reset();
        }

        void Awake()
        {

            canLookAround = true;
            vrPlayer = GetComponent<VRPlayer>();

            if (vrPlayer)
                movementVR = true;
            // set afk variables
            // used to calculate hoy many time they have been in afk mode before kick them

            afklastPos = transform.position;
            afklastRot = transform.rotation;
            afkTime = 0f;

            downThreshold = -0.4f;
            climbDirection = Vector3.up;



            if (Application.isMobilePlatform && GameManager.Instance.MobileUI)
                GameManager.Instance.MobileUI.gameObject.SetActive(true);
            if (PlayerPrefs.GetString("firstPlay") == "true")
            {
                // pickup the start weapons
                for (int i = 0; i < pickupSystem.StartItems.Length; i++)
                {
                    InteractiveObject itemSpawned = Instantiate(pickupSystem.StartItems[i], Vector3.zero, Quaternion.identity);
                    //   Weapon weaponScpawned = weaponSpawned.GetComponent<Weapon>();
                    pickupSystem.StartItemsSpawned.Add(itemSpawned);
                    //  photonView.RPC("PickupItem", PhotonTargets.AllBuffered, weaponScpawned.photonView.viewID);
                    pickupSystem.PickupItem(itemSpawned);
                }

                if (pickupSystem.StartItemsSpawned.Count > 0)
                    // use the first start weapon 
                    pickupSystem.UseItem(pickupSystem.StartItemsSpawned[0]);
            }
            // get references
            startFov = Camera.main.fieldOfView;
            motor = GetComponent<PlayerMotor>();
            rigidBody = GetComponent<Rigidbody>();

            capsuleCollider = GetComponent<CapsuleCollider>();
            health = GetComponent<Health>();

            characterController = GetComponent<CharacterController>();
            startHeight = characterController.height;
            lastPos = transform.localPosition;

            // calculate distance to ground, used to check if player is grounuded
            if (capsuleCollider)
                distToGround = capsuleCollider.bounds.extents.y;
        }

        private void Start()
        {
            GameManager.Instance.Player = this;
        }



        void Update()
        {


            if (health.isDead)
                return;
            LookSensitivity = GameOptions.Instance.LookSensitivity.CurrentSliderValue;

            if (GetIsIdle() || pickupSystem.CurrentItem != null && pickupSystem.CurrentItem.script != null && pickupSystem.CurrentItem.script.canRun)
                runKeyDown = false;

            if (Camera.main != null && pickupSystem.CurrentItem != null  && pickupSystem.CurrentItem.item != null && !pickupSystem.CurrentItem.item.GetComponent<CameraSystem>() || pickupSystem.CurrentItem == null)
            {

                if (InputManager.inputManager.GetButton(InputManager.inputManager.ZoomInput))
                {
                    if (pickupSystem.CurrentItem.weapon && !pickupSystem.CurrentItem.weapon.isAimingDown || !pickupSystem.CurrentItem.weapon)

                        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, CameraZoomFov, ZoomCameraSpeed * Time.deltaTime);
                    if (GameManager.Instance.ZoomCameraIcon)
                        GameManager.Instance.ZoomCameraIcon.DoFadeIn();
                }
                else
                {
                    if (pickupSystem.CurrentItem != null && pickupSystem.CurrentItem.weapon && !pickupSystem.CurrentItem.weapon.isAimingDown || pickupSystem.CurrentItem == null)
                        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, startFov, ZoomCameraSpeed * Time.deltaTime);
                    if (GameManager.Instance.ZoomCameraIcon)
                        GameManager.Instance.ZoomCameraIcon.DoFadeOut();

                }
            }

            Ray ray = new Ray(Vector3.zero, Vector3.one);
            if (Camera.main != null)
                ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            // check if player left the ladder
            if (ladderExit)
            {


                ladderExitTimer += Time.deltaTime;

                if (OnLadder)
                {
                    ladderExitTimer = 0f;
                    ladderExit = false;
                }
                if (ladderExitTimer >= 1f)
                {
                    ladderExitTimer = 0f;
                    ladderExit = false;

                    OnLadder = false;

                    if (pickupSystem.CurrentItem.item != null)
                    {
                        //     photonView.RPC("UseItem", PhotonTargets.All, CurrentWeapon.photonView.viewID, index);
                        pickupSystem.UseItem(pickupSystem.CurrentItem.script);
                    }

                }
            }
            // tilt 
            if (Input.GetKey(KeyCode.E))
            {
                flag = false;
                FPSView.localRotation = Quaternion.Lerp(FPSView.transform.localRotation, new Quaternion(FPSView.transform.localRotation.x, FPSView.transform.localRotation.y, TiltRightAngle, FPSView.transform.localRotation.w), Time.deltaTime * 7f);
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                flag = false;

                FPSView.transform.localRotation = Quaternion.Lerp(FPSView.transform.localRotation, new Quaternion(FPSView.transform.localRotation.x, FPSView.transform.localRotation.y, TiltLeftAngle, FPSView.transform.localRotation.w), Time.deltaTime * 7f);
            }
            else if (Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.Q))// (pickupSystem.CurrentItem.weapon != null && !pickupSystem.CurrentItem.weapon.isAimingDown || !pickupSystem.CurrentItem.weapon)
            {

                flag = true;
            }
            if (flag)
            {
                FPSView.transform.localRotation = Quaternion.Lerp(FPSView.transform.localRotation, new Quaternion(FPSView.transform.localRotation.x, FPSView.transform.localRotation.y, 0, FPSView.transform.localRotation.w), Time.deltaTime * 7f);

                if (Quaternion.Angle(FPSView.transform.localRotation, new Quaternion(FPSView.transform.localRotation.x, FPSView.transform.localRotation.y, 0, FPSView.transform.localRotation.w)) < 0.1f)
                    flag = false;
            }
            // still grab system doesn't work in online mode, soon releasing
            if (GrabOffset)
            {

                RaycastHit grabHit;
                Physics.Raycast(transform.position, transform.up, out grabHit);

                RaycastHit[] grabhits = Physics.RaycastAll(ray, GrabFromDistance);
                //     RaycastHit hit = hits[0];
                bool grabHitted = false;
                if (grabhits.Length > 0)
                {
                    for (int x = 0; x < grabhits.Length; x++)
                    {
                        if (grabhits[x].transform != transform && grabhits[x].transform.root != transform && grabhits[x].transform.parent != transform)
                        {

                            grabHit = grabhits[x];
                            grabHitted = true;
                        }

                    }

                    if (grabHitted)
                    {
                        // check if there's something front to me
                        //    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, GrabFromDistance))
                        //   {
                        // show grab icon
                        if (GrabIcon)
                            GrabIcon.DoFadeIn();

                        if (InputManager.inputManager.GetButtonDown(InputManager.inputManager.GrabKey.ToString()))
                        {
                            // grab item
                            grabbedItem = grabHit.transform.GetComponent<Item>();
                            // check if this item can be dragged
                            if (grabbedItem && grabbedItem.Dragable)
                            {
                                Rigidbody currentDraggingItemRg = grabbedItem.GetComponent<Rigidbody>();


                                if (currentDraggingItemRg)
                                    currentDraggingItemRg.isKinematic = true;
                                itemLastParent = grabbedItem.transform.parent;


                                grabbedItem.transform.SetParent(GrabOffset);
                                if (MoveToOffset)
                                {
                                    grabbedItem.transform.localPosition = Vector3.zero;

                                    grabbedItem.transform.localRotation = Quaternion.identity;
                                }

                            }
                            else
                            {
                                grabbedItem = null;
                            }
                        }
                    }
                    // if there isn't something fron to me
                    else
                    {

                        if (GrabIcon)
                            GrabIcon.DoFadeOut();
                    }
                    if (InputManager.inputManager.GetButton(InputManager.inputManager.GrabKey.ToString()))
                    {
                        // make lerp rotation effect
                        if (grabbedItem)
                            grabbedItem.transform.localRotation = Quaternion.Lerp(grabbedItem.transform.localRotation, new Quaternion(Camera.main.transform.localRotation.x, transform.localRotation.y, grabbedItem.transform.localRotation.z, grabbedItem.transform.localRotation.w), Time.deltaTime * 7f);
                    }
                    // drop item
                    if (InputManager.inputManager.GetButtonUp(InputManager.inputManager.GrabKey.ToString()))
                    {
                        if (grabbedItem)
                        {


                            grabbedItem.transform.SetParent(itemLastParent);

                            Rigidbody currentDraggingItemRg = grabbedItem.GetComponent<Rigidbody>();
                            if (currentDraggingItemRg)
                                currentDraggingItemRg.isKinematic = false;

                            grabbedItem = null;
                        }
                    }
                }


            }


            if (pickupSystem.CurrentItem != null && pickupSystem.CurrentItem.weapon != null)
            {
                // return kick back weapon and camera to original pos
                camKickBack.localRotation = Quaternion.Lerp(camKickBack.localRotation, Quaternion.identity, Time.deltaTime * pickupSystem.CurrentItem.weapon.returnSpeed);
                weaponKickBack.localRotation = Quaternion.Lerp(weaponKickBack.localRotation, Quaternion.identity, Time.deltaTime * pickupSystem.CurrentItem.weapon.returnSpeed);
            }
            // head bob effect
            if (HeadBob)
            {
                if (IsGrounded())

                    headbobStepCounter += Vector3.Distance(parentLastPos, transform.position) * headbobSpeed;

                Vector3 newCameraPos = Camera.main.transform.localPosition;
                newCameraPos.x = Mathf.Sin(headbobStepCounter) * headbobAmountX;
                newCameraPos.y = (Mathf.Cos(headbobStepCounter * 2) * headbobAmountY * -1) + (Camera.main.transform.localScale.y * eyeHeightRacio) - (Camera.main.transform.localScale.y / 2);

                Camera.main.transform.localPosition = newCameraPos;

                parentLastPos = transform.position;

            }


            // play player effects sounds
            // Run, walk, jump,land
            if (AudioSource && !AudioSource.isPlaying)
            {

                if (IsGrounded())
                {

                    if (FootstepsSounds.Length > 0)
                    {
                        if (GetIsRun())
                        {
                            if (!pickupSystem.CurrentItem.item || pickupSystem.CurrentItem.item)
                            {
                                if (pickupSystem.CurrentItem.script && pickupSystem.CurrentItem.script.animationC && !pickupSystem.CurrentItem.script.animationC.IsPlaying("Land") || pickupSystem.CurrentItem.script && !pickupSystem.CurrentItem.script.animationC)
                                    // when running add more pitch, it sounds faster
                                    AudioSource.pitch = 1.5f;


                            }

                        }
                        else
                        {
                            AudioSource.pitch = 1f;
                        }

                        // check if walked distance to play footStep sound
                        if (GetIsWalk() || GetIsRun() && Vector3.Distance(transform.localPosition, lastPos) > WalkDistanceToPlayStep)
                        {
                            lastPos = transform.localPosition;
                            RaycastHit hit;
                            if (Physics.Raycast(transform.position, -Vector3.up, out hit, distToGround + 0.1f))
                            {
                                if (FootstepsSounds.Length > 0)
                                {
                                    foreach (var footStep in FootstepsSounds)
                                    {
                                        if (footStep.LayerMask == hit.transform.gameObject.layer)
                                        {
                                            AudioSource.clip = footStep.Sounds[footstepSoundIndex];
                                            break;
                                        }
                                    }
                                }
                                AudioSource.Play();
                            }
                            if (footstepSoundIndex < FootstepsSounds.Length)
                            {

                                footstepSoundIndex++;

                            }
                            if (footstepSoundIndex > FootstepsSounds.Length - 1)
                            {
                                footstepSoundIndex = 0;
                            }
                        }
                    }
                    if (!previousGrounded)
                    {
                        if (pickupSystem.CurrentItem != null && pickupSystem.CurrentItem.script && pickupSystem.CurrentItem.script.animationC)
                        {
                            if (pickupSystem.CurrentItem.script.animationC.GetClip("Land") != null)
                                pickupSystem.CurrentItem.script.animationC.Play("Land");

                        }

                        if (LandSounds.Length > 0)
                        {
                            // play land effect
                            RaycastHit hit;
                            if (Physics.Raycast(transform.position, -Vector3.up, out hit, distToGround + 0.1f))
                            {
                                if (LandSounds.Length > 0)
                                {
                                    foreach (var landSound in LandSounds)
                                    {
                                        if (landSound.LayerMask == hit.transform.gameObject.layer)
                                        {

                                            AudioSource.PlayOneShot(landSound.Sounds[0]);
                                            break;
                                        }
                                    }
                                }
                            }
                        }


                    }
                    else
                    {
                        AudioSource.pitch = 1f;
                    }

                }
            }
            previousGrounded = IsGrounded();


            timer += Time.deltaTime;

            if (canLookAround && CanMove)
            {
                //Calculate rotation as a 3D vector (turning around)
                _yRot = InputManager.inputManager.GetAxisRaw("Mouse X");

                float _lookSensitivity = LookSensitivity;
                if (pickupSystem.CurrentItem != null && pickupSystem.CurrentItem.item && pickupSystem.CurrentItem.item.GetComponent<Weapon>() && pickupSystem.CurrentItem.item.GetComponent<Weapon>().isAimingDown)
                    _lookSensitivity = AimingDown_LookSensitivity;

                if (pickupSystem.CurrentItem != null && pickupSystem.CurrentItem.item && pickupSystem.CurrentItem.item.GetComponent<Weapon>() && pickupSystem.CurrentItem.item.GetComponent<Weapon>().isAimingDown && pickupSystem.CurrentItem.item.GetComponent<Weapon>().Sight2D)

                    _lookSensitivity = pickupSystem.CurrentItem.item.GetComponent<Weapon>().Sight2DLookSensitivity;

                Vector3 _rotation = new Vector3(0f, _yRot, 0f) * _lookSensitivity;

                //Apply rotation
                motor.Rotate(_rotation);
                //Calculate camera rotation as a 3D vector (turning around)
                float _xRot = InputManager.inputManager.GetAxisRaw("Mouse Y");

                _cameraRotationX = _xRot * _lookSensitivity;

                //Apply camera rotation
                motor.RotateCamera(_cameraRotationX);

            }
            // Calculate the thrusterforce based on player InputManager.inputManager
            Vector3 _thrusterForce = Vector3.zero;


            if (InputManager.inputManager.GetButtonDown(InputManager.inputManager.Crouch) && IsGrounded())
            {
                // crouch
                isCrouched = !isCrouched;
            }



            // inventory

            if (pickupSystem.Items.Count > 1 && !OnLadder)
            {
                if (pickupSystem.CurrentItem != null && pickupSystem.CurrentItem.item && pickupSystem.CurrentItem.item.GetComponent<Weapon>() && pickupSystem.CurrentItem.item.GetComponent<Weapon>().Sight2D && !pickupSystem.CurrentItem.item.GetComponent<Weapon>().isAimingDown || pickupSystem.CurrentItem != null && pickupSystem.CurrentItem.item.GetComponent<Weapon>() && !pickupSystem.CurrentItem.item.GetComponent<Weapon>().Sight2D || pickupSystem.CurrentItem == null)
                {
                    // change items with scroll wheel or ps4 mapping
                    var d = InputManager.inputManager.GetAxisRaw("Mouse ScrollWheel");
                    if (InputManager.inputManager.GetButtonDown(InputManager.inputManager.ChangeItemAxis))
                        d = 1f;

                    if (d > 0f)
                    {

                        // scroll up
                        if (index == pickupSystem.Items.ToArray().Length - 1)
                            index = 0;
                        else
                        {
                            index++;
                        }
                        pickupSystem.UseItem(pickupSystem.Items[index].script);

                    }
                    else if (d < 0f)
                    {

                        // scroll down
                        if (index <= 0)
                            index = pickupSystem.Items.ToArray().Length - 1;
                        else
                        {
                            index--;
                        }
                        //   photonView.RPC("UseItem", PhotonTargets.AllBuffered, pickupSystem.Items[index].item.GetComponent<PhotonView>().viewID, index);
                        pickupSystem.UseItem(pickupSystem.Items[index].script);
                    }
                }
            }
            //--
            if (pickupSystem.CurrentItem != null && pickupSystem.CurrentItem.weapon && pickupSystem.CurrentItem.weapon.hasReloaded)
            {

                /*
                if (pickupSystem.CurrentItem.script.Sway)
                {
             
                }
                */
                if (InputManager.inputManager.GetButtonDown("Fire2"))
                {
                    // when aim down reset position

                    pickupSystem.CurrentItem.script.animationC.Stop();
                    pickupSystem.CurrentItem.script.transform.localPosition = Vector3.zero;
                    pickupSystem.CurrentItem.script.transform.parent.parent.localPosition = Vector3.zero;

                }

                if (InputManager.inputManager.GetButton("Fire2"))
                    AimingDownSights();
                else
                    AimingUpSights();

            }

            if (pickupSystem.CurrentItem != null && pickupSystem.CurrentItem.weapon != null)
            {

                if (!GetIsRun() && !pickupSystem.objectLooking && !pickupSystem.IsExamining)
                {

                    if (InputManager.inputManager.GetButton("Fire1") && timer > pickupSystem.CurrentItem.weapon.ShootInterval)
                    {
                        // shoot weapon
                        if (pickupSystem.CurrentItem.weapon.hasReloaded)
                        {
                            timer = 0f;
                            isShooting = true;

                            //photonView.RPC("Shoot", PhotonTargets.All, null);
                            Shoot();
                        }

                        else if (pickupSystem.CurrentItem.weapon.ammunition <= 0)
                        {
                            timer = 0f;
                            if (pickupSystem.CurrentItem.weapon.AudioSource && pickupSystem.CurrentItem.weapon.NoAmmoSound)
                                pickupSystem.CurrentItem.weapon.AudioSource.PlayOneShot(pickupSystem.CurrentItem.weapon.NoAmmoSound);
                        }
                    }
                    else
                    {
                        isShooting = false;
                    }

                }
                if (!pickupSystem.CurrentItem.weapon.hasReloaded)
                {
                    AimingUpSights();

                }
            }

        }
        float LoopPos(float current, float targetA, float targetB, float speed, ref bool _flag)

        {

            if (!_flag)
            {

                current = Mathf.LerpUnclamped(current, targetA, Time.deltaTime * speed);
                if (Mathf.Abs(current - targetA) < 0.01f)
                    _flag = !_flag;
            }
            else
            {

                current = Mathf.LerpUnclamped(current, targetB, Time.deltaTime * speed);
                if (Mathf.Abs(current - targetB) < 0.01f)
                    _flag = !_flag;
            }
            return current;

        }


        Vector3 LoopRot(Vector3 current, Vector3 targetA, Vector3 targetB, float speed, ref bool _flag)
        {

            current.x = (current.x > 180) ? current.x - 360 : current.x;
            current.y = (current.y > 180) ? current.y - 360 : current.y;
            current.z = (current.z > 180) ? current.z - 360 : current.z;


            if (!_flag)
            {

                current = Vector3.MoveTowards(current, targetA, Time.deltaTime * speed);
                if (Vector3.Distance(current, targetA) < 0.01f)
                    _flag = !_flag;
            }
            else
            {

                current = Vector3.MoveTowards(current, targetB, Time.deltaTime * speed);
                if (Vector3.Distance(current, targetB) < 0.01f)
                    _flag = !_flag;
            }
            return current;
        }
        Vector3 LoopPos(Vector3 current, Vector3 targetA, Vector3 targetB, float speed, ref bool _flag)

        {


            if (!_flag)
            {

                current = Vector3.MoveTowards(current, targetA, Time.deltaTime * speed);
                if (Vector3.Distance(current, targetA) < 0.01f)
                    _flag = !_flag;
            }
            else
            {

                current = Vector3.MoveTowards(current, targetB, Time.deltaTime * speed);
                if (Vector3.Distance(current, targetB) < 0.01f)
                    _flag = !_flag;
            }
            return current;

        }

        public float GetCurrentCrosshairState()
        {
            if (pickupSystem.CurrentItem == null)// && pickupSystem.CurrentItem.weapon)
                return 0f;
            else if (!pickupSystem.CurrentItem.weapon)
                return 0f;
            if (isCrouched)
                return pickupSystem.CurrentItem.weapon.CrosshairCrouchPrecision;
            if (GetIsIdle())
                return pickupSystem.CurrentItem.weapon.CrosshairIdlePrecision;
            if (GetIsWalk())
                return pickupSystem.CurrentItem.weapon.CrosshairWalkPrecision;
            if (GetIsRun())
                return pickupSystem.CurrentItem.weapon.CrosshairRunPrecision;

            return pickupSystem.CurrentItem.weapon.CrosshairIdlePrecision;
        }
        private void FixedUpdate()
        {

            // if controlled by vr player cant move here
            float h = characterController.height;
            float speed = WalkSpeed;

            if (pickupSystem.CurrentItem != null && pickupSystem.CurrentItem.weapon && pickupSystem.CurrentItem.weapon.isAimingDown)
            {
                speed = AimingDown_WalkSpeed;
            }
            if (isCrouched)
            {
                speed = CrouchWalkSpeed;


            }
            float fpHeight = startHeight;
            if (isCrouched)
                // crouch
                fpHeight = characterController.height * 0.5f;


            float lastFPHeight = characterController.height;
            // set character controller height
            characterController.height = Mathf.Lerp(characterController.height, fpHeight, 10f * Time.deltaTime);
            float fixedVerticalPosition = transform.position.y + (characterController.height - lastFPHeight) / 2;
            // and fixed position to make the crouch smooth
            transform.position = new Vector3(transform.position.x, fixedVerticalPosition, transform.position.z);
            // set the speed respect to the state of the player (crouched , slower speed, run, faster speed)
            if (GetIsRun() && canRun)
            {

                timerRunning += Time.deltaTime;
                if (timerRunning >= MaxTimeRunning)
                {
                    timerRunning = 0f;
                    canRun = false;
                }
                else
                    speed = RunSpeed;
            }
            if (!canRun)
            {
                timerRunning += Time.deltaTime;
                if (timerRunning >= 3f)
                {
                    timerRunning = 0f;
                    canRun = true;
                }
            }

            GameManager.Instance.ExpandCrosshair(GetCurrentCrosshairState());

           
            if (!vrPlayer || vrPlayer && vrPlayer.MovementType == MovementTypes.BluetoothController)
            {
                canMove = true;

            }
            else
            {

                canMove = false;
            }


            if (pickupSystem.CurrentItem != null && pickupSystem.CurrentItem.weapon)
            {
                Recoil();
                if (pickupSystem.CurrentItem.weapon.recoil > 0)
                {
                    GameManager.Instance.RecoilCrosshair();
                }
                else
                {
                    GameManager.Instance.UnRecoilCrosshair();
                }
            }
            if (!OnLadder)
            {

                Vector3 input = new Vector3(InputManager.inputManager.GetAxis(InputManager.inputManager.MoveHorizontal), InputManager.inputManager.GetAxis(InputManager.inputManager.MoveVertical), 0);
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = transform.forward * input.y + transform.right * input.x;

                // get a normal for the surface that is being touched to move along it
                RaycastHit hitInfo;
                Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out hitInfo,
                                   characterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
                desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;


                moveDirection.x = desiredMove.x * speed;
                moveDirection.z = desiredMove.z * speed;
                if (characterController.isGrounded)
                {
                    moveDirection.y = -StickToGroundForce;

                }
                else
                {
                    moveDirection += Physics.gravity * GravityMultiplier * Time.fixedDeltaTime;

                }
            }
            else
            {
                var cameraRotation = Camera.main.transform.forward.y;
                if (OnLadder)
                {
                    Vector3 verticalMove;
                    verticalMove = climbDirection.normalized;
                    verticalMove *= InputManager.inputManager.GetAxis(InputManager.inputManager.MoveVertical);
                    verticalMove *= (cameraRotation > downThreshold) ? 1 : -1;
                    lateralMove = new Vector3(InputManager.inputManager.GetAxis(InputManager.inputManager.MoveHorizontal), 0, InputManager.inputManager.GetAxis(InputManager.inputManager.MoveVertical));
                    lateralMove = transform.TransformDirection(lateralMove);
                    ladderMovement = verticalMove + lateralMove;
                    characterController.Move(ladderMovement * ClimbSpeed * Time.deltaTime);
                    if (InputManager.inputManager.GetButtonDown(InputManager.inputManager.Jump))
                    {
                        OnLadder = false;
                    }
                }

            }
            if (InputManager.inputManager.GetButtonDown(InputManager.inputManager.Jump) && IsGrounded() && !OnLadder && !isCrouched)
            {
                if (pickupSystem.CurrentItem != null && pickupSystem.CurrentItem.script == null || pickupSystem.CurrentItem != null && pickupSystem.CurrentItem.script && pickupSystem.CurrentItem.weapon == null || pickupSystem.CurrentItem != null && pickupSystem.CurrentItem.weapon && !pickupSystem.CurrentItem.weapon.isAimingDown)
                {

                    moveDirection.y = JumpForce;

                    jumpFlag = true;
                    if (pickupSystem.CurrentItem.script && pickupSystem.CurrentItem.script.animationC)
                        pickupSystem.CurrentItem.script.animationC.Stop();
                    if (pickupSystem.CurrentItem.script && pickupSystem.CurrentItem.script.animationC)
                    {
                        if (pickupSystem.CurrentItem.script.animationC.GetClip("Jump") != null)
                            pickupSystem.CurrentItem.script.animationC.Play("Jump");
                    }
                }
            }
            if (!OnLadder && canMove && CanMove)
                characterController.Move(moveDirection * Time.fixedDeltaTime);

            if (!IsGrounded() && jumpFlag)
            {
                // if jumped and its not anymore in the ground
                jumpFlag = false;
            }

        }

        public void ApplyJumpForce()
        {
            if (AudioSource && JumpSound)
                AudioSource.PlayOneShot(JumpSound);
            rigidBody.AddForce(new Vector3(0, JumpForce, 0), ForceMode.Impulse);
        }


        #region Weapon 
        public void Shoot()
        {

            pickupSystem.CurrentItem.weapon.recoil += pickupSystem.CurrentItem.weapon.RecoilForce;
            pickupSystem.CurrentItem.weapon.WeaponKick();

            if (pickupSystem.CurrentItem.weapon.AudioSource && pickupSystem.CurrentItem.weapon.ShootSound)
                pickupSystem.CurrentItem.weapon.AudioSource.PlayOneShot(pickupSystem.CurrentItem.weapon.ShootSound);

            if (pickupSystem.CurrentItem.weapon.MuzzleEffect)
            {
                if (pickupSystem.CurrentItem.weapon && pickupSystem.CurrentItem.weapon.Sight2D && !pickupSystem.CurrentItem.weapon.isAimingDown || pickupSystem.CurrentItem.weapon && !pickupSystem.CurrentItem.weapon.Sight2D || !pickupSystem.CurrentItem.weapon)
                {
                    pickupSystem.CurrentItem.weapon.MuzzleEffect.transform.SetParent(pickupSystem.CurrentItem.weapon.ShootBase.transform);
                    pickupSystem.CurrentItem.weapon.MuzzleEffect.transform.localPosition = Vector3.zero;
                    pickupSystem.CurrentItem.weapon.MuzzleEffect.gameObject.SetActive(true);
                }
            }

            if (pickupSystem.CurrentItem.weapon.ShellPrefab)
            {
                // spawn shells
                GameObject _shell = Instantiate(pickupSystem.CurrentItem.weapon.ShellPrefab);
                _shell.GetComponent<Rigidbody>().isKinematic = false;


                _shell.transform.parent = null;
                _shell.transform.position = pickupSystem.CurrentItem.weapon.ShellPosBase.position;

                _shell.SetActive(true);

                _shell.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(pickupSystem.CurrentItem.weapon.shellForce + Random.Range(0, pickupSystem.CurrentItem.weapon.shellRandomForce), 0, 0), ForceMode.Impulse);
                _shell.GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(pickupSystem.CurrentItem.weapon.shellTorqueX + Random.Range(-pickupSystem.CurrentItem.weapon.shellRandomTorque, pickupSystem.CurrentItem.weapon.shellRandomTorque), pickupSystem.CurrentItem.weapon.shellTorqueY + Random.Range(-pickupSystem.CurrentItem.weapon.shellRandomTorque, pickupSystem.CurrentItem.weapon.shellRandomTorque), 0), ForceMode.Impulse);
            }
            for (int i = 0; i < pickupSystem.CurrentItem.weapon.WeaponShoots; i++)
            {
                float recoilX = 0f;
                float recoilY = 0f;
                if (pickupSystem.CurrentItem.weapon.WeaponShoots == 1 && !pickupSystem.CurrentItem.weapon.isAimingDown || pickupSystem.CurrentItem.weapon.WeaponShoots > 1 && i != 0)
                {

                    recoilX = Random.Range(-pickupSystem.CurrentItem.weapon.MaxAimRecoilX, pickupSystem.CurrentItem.weapon.MaxAimRecoilX);
                    recoilY = Random.Range(-pickupSystem.CurrentItem.weapon.MaxAimRecoilY, pickupSystem.CurrentItem.weapon.MaxAimRecoilY);

                }

                Ray ray = new Ray(Camera.main.transform.position, new Vector3(Camera.main.transform.forward.x + recoilX, Camera.main.transform.forward.y + recoilY, Camera.main.transform.forward.z));
                Debug.DrawRay(ray.origin, ray.direction);
                //   Debug.Log(LayerMask.LayerToName(8));
                RaycastHit[] hits = Physics.RaycastAll(ray, pickupSystem.CurrentItem.weapon.DistanceShoot);
                //     RaycastHit hit = hits[0];
                bool hitted = false;
                if (hits.Length > 0)
                {
                    for (int x = 0; x < hits.Length; x++)
                    {
                        if (hits[x].transform != transform && hits[x].transform.root != transform && hits[x].transform.parent != transform)
                        {

                            hit = hits[x];
                            hitted = true;
                        }
                        
                    }

                    if (hitted)
                    {
                        foreach (var effect in pickupSystem.CurrentItem.weapon.EffectsInHit)
                        {


                            if (hit.transform.gameObject.layer == effect.LayerMask)
                            {
                                if (pickupSystem.CurrentItem.weapon.AudioSource && effect.Sound)
                                    pickupSystem.CurrentItem.weapon.AudioSource.PlayOneShot(effect.Sound);


                                GameObject objSpawned = Instantiate(effect.Go, hit.point, Quaternion.Euler(hit.normal));
                                objSpawned.transform.localPosition = new Vector3(objSpawned.transform.localPosition.x, objSpawned.transform.localPosition.y + effect.OffsetY, objSpawned.transform.localPosition.z);

                                // photonView.RPC("InstantiateEffectWithOffset", PhotonTargets.Others, effect.Go.name, hit.point, hit.normal, effect.OffsetY);
                            }
                        }


                        Health health = hit.transform.GetComponent<Health>();
                        Table table = null;

                        // if target has health, damage him
                        if (health)
                        {
                            GameManager.Instance.WeaponHit();

                            health.TakeDamage(pickupSystem.CurrentItem.weapon.DamageBody, hit.point, gameObject);

                        }
                        else
                        {
                            table = hit.transform.GetComponent<Table>();
                            if (table)
                                table.Break();
                        }

                        if (!health && !table)
                        {
                            HealthBody healthBody = hit.transform.GetComponent<HealthBody>();
                            if (healthBody)
                            {
                                GameManager.Instance.WeaponHit();

                                if (healthBody.BodyPart == HealthBody.BodyParts.Head)
                                    healthBody.healthScript.TakeDamage(pickupSystem.CurrentItem.weapon.DamageHeadShot, hit.point, gameObject);
                                if (healthBody.BodyPart == HealthBody.BodyParts.Arms)
                                    healthBody.healthScript.TakeDamage(pickupSystem.CurrentItem.weapon.DamageArms, hit.point, gameObject);
                                if (healthBody.BodyPart == HealthBody.BodyParts.Legs)
                                    healthBody.healthScript.TakeDamage(pickupSystem.CurrentItem.weapon.DamageLegs, hit.point, gameObject);
                                if (healthBody.BodyPart == HealthBody.BodyParts.Torso)
                                    healthBody.healthScript.TakeDamage(pickupSystem.CurrentItem.weapon.DamageTorso, hit.point, gameObject);
                                if (healthBody.BodyPart == HealthBody.BodyParts.Hips)
                                    healthBody.healthScript.TakeDamage(pickupSystem.CurrentItem.weapon.DamageHips, hit.point, gameObject);
                            }
                        }
                    }

                }
            }
            pickupSystem.CurrentItem.weapon.cartridgeAmmo--;
            GameManager.Instance.UpdateAmmoUI();
        }

        //[PunRPC]
        // aiming down weapon
        void AimingDownSights()
        {


            //       if (photonView.isMine)
            //       {
            pickupSystem.CurrentItem.weapon.isAimingDown = true;
            GameManager.Instance.FadeOutCrosshair();
            //       }
            pickupSystem.CurrentItem.weapon.transform.parent.localPosition = Vector3.MoveTowards(pickupSystem.CurrentItem.weapon.transform.parent.localPosition, pickupSystem.CurrentItem.weapon.aimPosition, Time.deltaTime * pickupSystem.CurrentItem.weapon.adsSpeed);

            if (pickupSystem.CurrentItem.weapon.Sight2D == null)
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, pickupSystem.CurrentItem.weapon.FovAds, pickupSystem.CurrentItem.weapon.FovAdsSpeed * Time.deltaTime);
            if (Vector3.Distance(pickupSystem.CurrentItem.weapon.transform.parent.localPosition, pickupSystem.CurrentItem.weapon.aimPosition) < pickupSystem.CurrentItem.weapon.Sight2DMinDistanceToShow)
            {
                if (pickupSystem.CurrentItem.weapon.Sight2D != null && !isSighting2D)
                {
                    pickupSystem.CurrentItem.weapon.Sight2D.DoFadeIn();
                    isSighting2D = true;
                    CameraAnim.enabled = true;
                    pickupSystem.CurrentItem.weapon.DesactivateModel();
                    GameManager.Instance.FadeWhenSight2D.SetActive(false);
                    Camera.main.fieldOfView = pickupSystem.CurrentItem.weapon.Sight2DCameraFov;

                }

            }
        }
        // we check if is grounded with on collision enter because it's 100% accurate more than a raycast
        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(GroundTag))
            {
                isGrounded = true;
            }

        }

        void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag(GroundTag))
            {

                isGrounded = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // check if player is triggering with a ladder
            if (other.gameObject.CompareTag("Ladder"))
            {

                if (pickupSystem.CurrentItem.script && !ladderExit)
                {
                    pickupSystem.CurrentItem.script.gameObject.SetActive(true);
                    pickupSystem.StopUsing();

                }
                OnLadder = true;

            }
            if (other.gameObject.GetComponent<Weapon>())
            {
                onTriggerWithWeapon = true;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            // exit from a ladder
            if (other.gameObject.CompareTag("Ladder"))
            {

                OnLadder = false;

                ladderExit = true;

            }
            if (other.gameObject.GetComponent<Weapon>())
            {
                onTriggerWithWeapon = false;
            }
        }
        // [PunRPC]
        // weapon aiming up
        void AimingUpSights()
        {

            if (pickupSystem.CurrentItem.weapon == null)
                return;
            if (!pickupSystem.CurrentItem.weapon.animationC || pickupSystem.CurrentItem.weapon.animationC && pickupSystem.CurrentItem.weapon.animationC.IsPlaying("SwitchIn") || pickupSystem.CurrentItem.weapon.animationC && pickupSystem.CurrentItem.weapon.animationC.IsPlaying("SwitchOut"))
                return;

            //    if (photonView.isMine)
            //    {
            pickupSystem.CurrentItem.weapon.isAimingDown = false;
            GameManager.Instance.FadeInCrosshair();
            //    }

            if (!GetIsRun())
            {
                pickupSystem.CurrentItem.weapon.transform.parent.localPosition = Vector3.MoveTowards(pickupSystem.CurrentItem.weapon.transform.parent.localPosition, pickupSystem.CurrentItem.weapon.transform.parent.GetComponent<WeaponBaseData>().weaponBaseInitialPosition, Time.deltaTime * pickupSystem.CurrentItem.weapon.adsSpeed);

            }

            if (pickupSystem.CurrentItem.weapon.Sight2D == null)
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, startFov, pickupSystem.CurrentItem.weapon.FovAdsSpeed * Time.deltaTime);

            if (pickupSystem.CurrentItem.weapon.Sight2D != null && isSighting2D)
            {
                pickupSystem.CurrentItem.weapon.Sight2D.DoFadeOut();


                CameraAnim.enabled = false;

                isSighting2D = false;


                pickupSystem.CurrentItem.weapon.ActivateModel();
                GameManager.Instance.FadeWhenSight2D.SetActive(true);
                Camera.main.fieldOfView = startFov;


            }




        }

        // recoil weapon by script
        void Recoil()
        {

            if (pickupSystem.CurrentItem.weapon == null)
                return;

            if (pickupSystem.CurrentItem.weapon.recoil > 0f)
            {
                //                if (photonView.isMine)
                GameManager.Instance.RecoilCrosshair();
                Quaternion maxRecoil = Quaternion.Euler(pickupSystem.CurrentItem.weapon.maxRecoil_x, pickupSystem.CurrentItem.weapon.maxRecoil_y, 0f);

                // Dampen towards the target rotation
                pickupSystem.CurrentItem.weapon.transform.localRotation = Quaternion.Slerp(pickupSystem.CurrentItem.weapon.transform.localRotation, maxRecoil, Time.deltaTime * pickupSystem.CurrentItem.weapon.recoilSpeed);

                pickupSystem.CurrentItem.weapon.recoil -= Time.deltaTime;
            }
            else
            {
                //       if (photonView.isMine)
                GameManager.Instance.UnRecoilCrosshair();

                pickupSystem.CurrentItem.weapon.recoil = 0f;
                // Dampen towards the target rotation
                pickupSystem.CurrentItem.weapon.transform.localRotation = Quaternion.Slerp(pickupSystem.CurrentItem.weapon.transform.localRotation, Quaternion.identity, Time.deltaTime * pickupSystem.CurrentItem.weapon.recoilSpeed / 2);
            }

        }
        // called by the trigger of the locker for example, the bool variable Hidden is used by the enemies to know if this object is hidden.
        public void SetHiddenValue(bool _value)
        {
            Hidden = _value;
        }
        #endregion

        // it's made so because if you want to modify is grounded method you only have to modify the return value 
        public bool IsGrounded()
        {
            if (characterController)
                return characterController.isGrounded;
            else
                return isGrounded;

        }

        #region WeaponAnimator
        public bool GetIsIdle()
        {
            if (!IsGrounded())
                return false;
            if (InputManager.inputManager.GetAxisRaw(InputManager.inputManager.MoveHorizontal) != 0 || InputManager.inputManager.GetAxisRaw(InputManager.inputManager.MoveVertical) != 0)
                return false;
            else
                return true;

        }
        public bool GetIsWalk()
        {
            if (!IsGrounded())
                return false;
            if (InputManager.inputManager.GetAxisRaw(InputManager.inputManager.MoveHorizontal) != 0 || InputManager.inputManager.GetAxisRaw(InputManager.inputManager.MoveVertical) != 0)
            {
                if (!GetIsRun())
                    return true;

            }

            return false;

        }
        public bool GetIsRun()
        {
            if (!IsGrounded())
                return false;
            if (pickupSystem.CurrentItem != null && pickupSystem.CurrentItem.weapon && !pickupSystem.CurrentItem.weapon.hasReloaded)
                return false;

            if (pickupSystem.CurrentItem != null && pickupSystem.CurrentItem.script != null && pickupSystem.CurrentItem.script.animationC)
            {
                if (pickupSystem.CurrentItem.script.animationC.IsPlaying("SwitchIn") || pickupSystem.CurrentItem.script.animationC.IsPlaying("SwitchOut"))
                    return false;
            }
            if (InputManager.inputManager.GetAxisRaw(InputManager.inputManager.MoveHorizontal) != 0 || InputManager.inputManager.GetAxisRaw(InputManager.inputManager.MoveVertical) != 0)
            {
                if (Application.isMobilePlatform && runKeyDown || !Application.isMobilePlatform && InputManager.inputManager.GetButton(InputManager.inputManager.RunAxis))
                {
                    if (pickupSystem.CurrentItem != null && pickupSystem.CurrentItem.weapon == null || pickupSystem.CurrentItem == null || pickupSystem.CurrentItem != null && pickupSystem.CurrentItem.weapon != null && !pickupSystem.CurrentItem.weapon.isAimingDown)
                        return true;
                }
            }
            return false;
        }

        public bool GetIsShoot()
        {
            if (GetIsRun())
                return false;
            if (isShooting)
            {

                return true;
            }
            return false;

        }
        #endregion
    }
}