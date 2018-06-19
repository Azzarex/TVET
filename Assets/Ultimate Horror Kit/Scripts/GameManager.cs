using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AxlPlay;
using UnityEngine.SceneManagement;

namespace AxlPlay
{
    [System.Serializable]
    public class Jumpscare2D
    {
        public GameObject UI;
        public float TimeActive = 2.5f;
        public AudioClip Scream;
        public float Volume = 1f;

    }
    public class GameManager : MonoBehaviour
    {

        public Slider LoadingSliderBar;
        public Text LoadingText;
        
        public string MainScene;
        public Text InfoText;
        public Jumpscare2D[] Jumpscares2D;
        public AudioSource Jumpscares2DAudioSource;

        public UIEffects BatteryLevelUI;
        public UIEffects BatteryLevel1;
        public UIEffects BatteryLevel2;
        public UIEffects BatteryLevel3;
        public UIEffects BatteryLevel4;

        public UIEffects ZoomCameraIcon;

        public InteractiveObject[] ItemsPrefabs;
        [HideInInspector]
        public PlayerController Player;
        public Transform ExaminableObjectTransform;
        public Transform[] SpawnPoints;


        public UIEffects InventoryUI;

        public UIEffects PauseMenuUI;
        public AudioClip PickUpAmmoSound;
        public GameObject MobileUI;
        public float PickUpDistance = 3f;
        public UIEffects InteractIcon;
        //  public KeyCode PickupKey;

        public AudioClip HitMarkerSound;
        public AudioSource GameAudioSource;

        public GameObject KillPopUp;
        public UIEffects DamageIndicator;
        public UIEffects BloodSplash;

        public GameObject FadeWhenSight2D;

        public static GameManager Instance;
        public GameObject HealthPanel;
        public Text HealthUI;
        public float TimeToUnrecoil = 0.2f;
        public float SpeedCrosshairExpand = 5f;
        public float HitCrosshairTime = 1f;
        public RectTransform CrosshairLeft;
        public RectTransform CrosshairRight;
        public RectTransform CrosshairUp;
        public RectTransform CrosshairDown;

        [HideInInspector]
        public Health PlayerHealth;
        [HideInInspector]
        public Weapon PlayerWeapon;


        public GameObject UIWeapon;
        public UIEffects Crosshair;
        public UIEffects HitCrosshair;
        public Text AmmunitionMagazine;
        public Text Ammunition;

        public UIEffects Sight2DSniper;


        private bool didRecoil;

        // private bool inRecoil;
        private float timer;


        private void Awake()
        {
            Instance = this;
        }

        void Update()
        {


            if (/*MultiplayerGameManager.Instance.LocalPlayer != null && */!Application.isMobilePlatform)
            {
                if (InputManager.inputManager.GetButtonDown(InputManager.inputManager.PauseBt))
                {
                    // fade in pause menu 
                    if (PauseMenuUI)
                    {
                        if (PauseMenuUI.canvasGroup.alpha > 0.9f)
                        {
                            PauseMenuUI.DoFadeOut();
                            ResumeGame();
                        }
                        else
                        {
                            PauseMenuUI.DoFadeIn();
                            PauseGame();
                        }
                    }
                }
            }
            if (!Application.isMobilePlatform)
            {
                if (InputManager.inputManager.GetButtonDown(InputManager.inputManager.ShowInventory))
                {
                    // fade in pause menu 
                    if (InventoryUI)
                    {
                        if (InventoryUI.canvasGroup.alpha > 0.9f)
                        {
                            InventoryUI.DoFadeOut();
                            Cursor.visible = false;
                        }
                        else
                        {
                            InventoryUI.DoFadeIn();
                            Cursor.visible = true;

                        }
                    }
                }
            }
        }

        // you want to show a jumpscare 2d with sound also call this method from the inspector with Unity Event, (using Send Event on Trigger script)
        public void ShowJumpscare2D(int jumpscareIndex)
        {
            Jumpscares2D[jumpscareIndex].UI.SetActive(true);
            if (Jumpscares2DAudioSource && Jumpscares2D[jumpscareIndex].Scream)
                Jumpscares2DAudioSource.PlayOneShot(Jumpscares2D[jumpscareIndex].Scream, Jumpscares2D[jumpscareIndex].Volume);

        }
        IEnumerator DesactivateJumpScare2D(int index)
        {
            yield return new WaitForSeconds(index);
            Jumpscares2D[index].UI.SetActive(false);
        }
        public void ResumeGame()
        {
            Time.timeScale = 1f;
            if (Application.isMobilePlatform)
                MobileUI.SetActive(true);

        }
        public void PauseGame()
        {
            Cursor.visible = true;
            Cursor.lockState = true ? CursorLockMode.Locked : CursorLockMode.None;

            Time.timeScale = 0f;

        }
        IEnumerator LoadNewScene(string sceneName)
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);

            while (!async.isDone)
            {
                float progress = Mathf.Clamp01(async.progress / 0.9f);
                LoadingSliderBar.value = progress;
                LoadingText.text = "Loading... (" + progress * 100f + "%)";
                yield return null;
            }
        }

        public Transform GetSpawnPoint(bool _team1)
        {
            return SpawnPoints[UnityEngine.Random.Range(0, SpawnPoints.Length)].transform;
        }
        public void PlayerChangedAmbient()
        {
            NotificationCenter.DefaultCenter.PostNotification(this, "PlayerChangedAmbient");
        }
        #region Info
        public void NeedAKey()
        {
            if (!InfoText)
                return;
            InfoText.text = "You need a key to open this door";
            StartCoroutine(FadeOutInfoText());

        }
        public void KeyDontMatch()
        {
            if (!InfoText)
                return;
            InfoText.text = "This key can't open this door";
            StartCoroutine(FadeOutInfoText());
        }
        IEnumerator FadeOutInfoText()
        {
            yield return new WaitForSeconds(3f);
            InfoText.text = "";

        }

        #endregion
        // draw the recoil in the crosshair
        public void RecoilCrosshair()
        {
            if (PlayerWeapon == null)
                return;

            if (!CrosshairLeft)
                return;
            // Left -X
            Vector2 newCrossLeftPos = CrosshairLeft.anchoredPosition;
            newCrossLeftPos.x = -PlayerWeapon.CrosshairShootingPrecision;
            CrosshairLeft.anchoredPosition = Vector2.Lerp(CrosshairLeft.anchoredPosition, newCrossLeftPos, Time.deltaTime * SpeedCrosshairExpand);
            // Down -Y
            Vector2 newCrossDownPos = CrosshairDown.anchoredPosition;
            newCrossDownPos.y = -PlayerWeapon.CrosshairShootingPrecision;
            CrosshairDown.anchoredPosition = Vector2.Lerp(CrosshairDown.anchoredPosition, newCrossDownPos, Time.deltaTime * SpeedCrosshairExpand);
            // Up Y
            Vector2 newCrossUpPos = CrosshairUp.anchoredPosition;
            newCrossUpPos.y = PlayerWeapon.CrosshairShootingPrecision;
            CrosshairUp.anchoredPosition = Vector2.Lerp(CrosshairUp.anchoredPosition, newCrossUpPos, Time.deltaTime * SpeedCrosshairExpand);
            // Right Y
            Vector2 newCrossRightPos = CrosshairRight.anchoredPosition;
            newCrossRightPos.x = PlayerWeapon.CrosshairShootingPrecision;
            CrosshairRight.anchoredPosition = Vector2.Lerp(CrosshairRight.anchoredPosition, newCrossRightPos, Time.deltaTime * SpeedCrosshairExpand);


        }
        // return the crosshair to original pos

        public void UnRecoilCrosshair()
        {
            if (PlayerWeapon != null)
            {
                if (!CrosshairLeft)
                    return;

                // Left -X
                Vector2 newCrossLeftPos = CrosshairLeft.anchoredPosition;
                newCrossLeftPos.x = -PlayerWeapon.Owner.GetCurrentCrosshairState();

                CrosshairLeft.anchoredPosition = Vector2.Lerp(CrosshairLeft.anchoredPosition, newCrossLeftPos, Time.deltaTime * (SpeedCrosshairExpand / 2));
                // Down -Y
                Vector2 newCrossDownPos = CrosshairDown.anchoredPosition;
                newCrossDownPos.y = -PlayerWeapon.Owner.GetCurrentCrosshairState();
                CrosshairDown.anchoredPosition = Vector2.Lerp(CrosshairDown.anchoredPosition, newCrossDownPos, Time.deltaTime * (SpeedCrosshairExpand / 2));
                // Right X
                Vector2 newCrossRightPos = CrosshairRight.anchoredPosition;
                newCrossRightPos.x = PlayerWeapon.Owner.GetCurrentCrosshairState();
                CrosshairRight.anchoredPosition = Vector2.Lerp(CrosshairRight.anchoredPosition, newCrossRightPos, Time.deltaTime * (SpeedCrosshairExpand / 2));
                // Up Y
                Vector2 newCrossUpPos = CrosshairUp.anchoredPosition;
                newCrossUpPos.y = PlayerWeapon.Owner.GetCurrentCrosshairState();
                CrosshairUp.anchoredPosition = Vector2.Lerp(CrosshairUp.anchoredPosition, newCrossUpPos, Time.deltaTime * (SpeedCrosshairExpand / 2));

                if (CrosshairLeft.anchoredPosition.x < -(PlayerWeapon.CrosshairShootingPrecision - 0.1f) && CrosshairLeft.anchoredPosition.x > -(PlayerWeapon.Owner.GetCurrentCrosshairState() - 0.1f) && didRecoil)
                {
                    didRecoil = false;
                }

            }
        }
        // show how many ammo
        public void UpdateAmmoUI()
        {
            if (PlayerWeapon)
            {

                Ammunition.text = ("/ " + PlayerWeapon.ammunition).ToString();
                AmmunitionMagazine.text = PlayerWeapon.cartridgeAmmo.ToString();

            }
        }

        // get reference from the health script
        public void GetPlayerHealth(Health _health)
        {

            if (HealthPanel)
                HealthPanel.gameObject.SetActive(true);
            PlayerHealth = _health;
        }
        // fade out Crosshair UI
        public void FadeOutCrosshair()
        {
            if (Crosshair)
                Crosshair.DoFadeOut();
        }
        public void ReturnMainScene()
        {
            StartCoroutine(LoadNewScene(MainScene));
        }
        // fade in Crosshair UI
        public void FadeInCrosshair()
        {
            if (Crosshair)
                Crosshair.DoFadeIn();
        }
        // show hit marker when the weapon hits something
        public void WeaponHit()
        {
            Crosshair.DoFadeOut();
            HitCrosshair.DoFadeIn();
            if (GameAudioSource && HitMarkerSound)
                GameAudioSource.PlayOneShot(HitMarkerSound);

            StartCoroutine(HitCrosshairActive());
        }
        IEnumerator HitCrosshairActive()
        {
            yield return new WaitForSeconds(HitCrosshairTime);

            if (!PlayerWeapon.isAimingDown)
                Crosshair.DoFadeIn();
            HitCrosshair.DoFadeOut();
        }
        // when start using weapon put the crosshair with distance
        public void UsingWeapon()
        {
            if (!UIWeapon)
                return;
            UIWeapon.gameObject.SetActive(true);
            Vector2 crossLeftPos = CrosshairLeft.anchoredPosition;
            crossLeftPos.x = -PlayerWeapon.Owner.GetCurrentCrosshairState();
            CrosshairLeft.anchoredPosition = crossLeftPos;

            Vector2 crossDownPos = CrosshairDown.anchoredPosition;
            crossDownPos.y = -PlayerWeapon.Owner.GetCurrentCrosshairState();
            CrosshairDown.anchoredPosition = crossDownPos;

            Vector2 crossRightPos = CrosshairRight.anchoredPosition;
            crossRightPos.x = PlayerWeapon.Owner.GetCurrentCrosshairState();
            CrosshairRight.anchoredPosition = crossRightPos;

            Vector2 crossUpPos = CrosshairUp.anchoredPosition;
            crossUpPos.y = PlayerWeapon.Owner.GetCurrentCrosshairState();
            CrosshairUp.anchoredPosition = crossUpPos;


            UpdateAmmoUI();
        }
        //  put the crosshair with specific distance

        public void ExpandCrosshair(float amount)
        {

            if (!CrosshairLeft)
                return;
            // Left -X
            Vector2 newCrossLeftPos = CrosshairLeft.anchoredPosition;
            newCrossLeftPos.x = -amount;
            if (Vector2.Distance(CrosshairLeft.anchoredPosition, newCrossLeftPos) < 0.01f)
                return;

            CrosshairLeft.anchoredPosition = Vector2.Lerp(CrosshairLeft.anchoredPosition, newCrossLeftPos, Time.deltaTime * SpeedCrosshairExpand);
            // Down -Y
            Vector2 newCrossDownPos = CrosshairDown.anchoredPosition;
            newCrossDownPos.y = -amount;
            CrosshairDown.anchoredPosition = Vector2.Lerp(CrosshairDown.anchoredPosition, newCrossDownPos, Time.deltaTime * SpeedCrosshairExpand);
            // Up Y
            Vector2 newCrossUpPos = CrosshairUp.anchoredPosition;
            newCrossUpPos.y = amount;
            CrosshairUp.anchoredPosition = Vector2.Lerp(CrosshairUp.anchoredPosition, newCrossUpPos, Time.deltaTime * SpeedCrosshairExpand);
            // Right Y
            Vector2 newCrossRightPos = CrosshairRight.anchoredPosition;
            newCrossRightPos.x = amount;
            CrosshairRight.anchoredPosition = Vector2.Lerp(CrosshairRight.anchoredPosition, newCrossRightPos, Time.deltaTime * SpeedCrosshairExpand);


        }
        // desactivate UI Weapon ( ammo ammount)
        public void StopUsingWeapon()
        {
            if (UIWeapon)
                UIWeapon.gameObject.SetActive(false);
            if (Ammunition)

                Ammunition.text = "";


        }
    }
}