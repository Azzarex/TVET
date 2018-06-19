using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
namespace AxlPlay
{
    public class Health : MonoBehaviour
    {

        public GameObject BloodEffect;
        public bool FallDamage;
        public float FallDamageDistance = 5f;
        public bool AutoHealing;
        public float HealingSpeed = 1f;
        public float currentHealth = 100f;
        public float RespawnDelay = 3f;

        [HideInInspector]

        public float startCurrentHealth;

        private bool player;
        [HideInInspector]
        public bool isDead;
        private GameManager gameManager;
        private PlayerController playerController;

        private Vector3 spawnedPosition;

        private float timer;
        private bool damaged;
        private Transform damager;


        private float lastPositionY = 0f;
        private float fallDistance = 0f;

        public float CurrentHealth
        {
            get
            {
                return currentHealth;
            }

            set
            {
                currentHealth = value;

            }
        }

        public void Reset()
        {

            startCurrentHealth = CurrentHealth;
            isDead = false;
            transform.position = spawnedPosition;

        }
        void Awake()
        {
            // get references


            playerController = GetComponent<PlayerController>();
            player = playerController != null;
            spawnedPosition = transform.position;
        }
        void Start()
        {
            gameManager = GameManager.Instance;
            gameManager.GetPlayerHealth(this);
            // set health
            startCurrentHealth = CurrentHealth;
            if (gameManager.BloodSplash)
                gameManager.BloodSplash.DoFadeIn();

        }

        void Update()
        {
            if (player)
            {
                // fall damage
                if (lastPositionY > playerController.transform.position.y)
                {
                    fallDistance += lastPositionY - playerController.transform.position.y;

                }
                lastPositionY = playerController.transform.position.y;

                if (fallDistance >= FallDamageDistance && playerController.IsGrounded())
                {
                    TakeDamage(fallDistance * 5f, Vector3.zero, null);
                    ApplyNormal();
                }

                if (fallDistance <= FallDamageDistance && playerController.IsGrounded())
                {
                    ApplyNormal();
                }

            }
            // put damage indicator arrow
            if (damaged && damager)
            {
                timer += Time.deltaTime;
                DamageIndicatorMove();
                if (timer >= 5f)
                {
                    if (gameManager.DamageIndicator)
                        gameManager.DamageIndicator.DoFadeOut();
                    damager = null;
                    timer = 0f;
                    damaged = false;
                }
            }



            // text health amount
            if (playerController && gameManager.HealthUI)
            {
                gameManager.HealthUI.text = ((int)CurrentHealth).ToString();
            }

            if (!isDead)
            {
                if (CurrentHealth < startCurrentHealth)
                {
                    if (gameManager.BloodSplash)
                    {
                        // auto healing

                        CurrentHealth += HealingSpeed * Time.deltaTime;


                    }
                }
                else if (CurrentHealth > startCurrentHealth)

                {
                    // health to start health
                    CurrentHealth = startCurrentHealth;

                }

            }



        }
        void ApplyNormal()
        {
            fallDistance = 0;
            lastPositionY = 0;
        }
  
        // damage arrow
        void DamageIndicatorMove()
        {
            if (!gameManager.DamageIndicator)
                return;
            Vector3 damagerPos = damager.transform.position;
            Vector3 dir = Vector3.zero;
            if (Camera.main != null)
                dir = Camera.main.WorldToScreenPoint(damagerPos);
            Vector3 pointing = Vector3.zero;
            pointing.z = Mathf.Atan2((gameManager.DamageIndicator.transform.position.y - dir.y), (gameManager.DamageIndicator.transform.position.x - dir.x)) * Mathf.Rad2Deg - 90;
            pointing.z = -pointing.z;
            var targetDir = Quaternion.Euler(pointing);
            gameManager.DamageIndicator.transform.rotation = targetDir;

        }

        public void TakeDamage(float damage, Vector3 hitPoint, GameObject _damager)//, int viewIdDamager, bool comeInRpc = true)
        {

            if (!player)
            {
                if (BloodEffect)
                {
                    // put blood effect particle in player model
                    BloodEffect.transform.position = hitPoint;
                    BloodEffect.SetActive(true);
                }
                GetComponent<AIPlayer>().GotHitBy(_damager);

            }
            if (_damager)
                damager = _damager.transform;


            if (CurrentHealth > 0)
            {
                CurrentHealth -= damage;


                if (playerController)
                {
                    // someone damaged me and it is not fall damage 
                    if (damager)
                    {

                        if (gameManager.DamageIndicator)
                            gameManager.DamageIndicator.DoFadeIn();
                    }
                    else
                    {
                        damager = null;
                    }
                    if (gameManager.BloodSplash)
                    {
                        // fade in blood overlay screen
                        Color imageColor = gameManager.BloodSplash.GetComponent<Image>().color;
                        imageColor.a = 255 - CurrentHealth;
                        gameManager.BloodSplash.GetComponent<Image>().color = imageColor;
                        damaged = true;
                    }

                }

            }
            if (playerController && !isDead && gameManager.BloodSplash)

                gameManager.BloodSplash.DoFadeIn();



            if (CurrentHealth <= 0 && !isDead)
            {
                // die
                //    photonView.RPC("Die", PhotonTargets.All);
                StartCoroutine(Die());
            }

        }

        // use pun rpc when called by fall damage

        IEnumerator Die()
        {



            isDead = true;
            if (playerController)
            {
                // add deaths and kills in players
                playerController.Deaths++;

                if (playerController.grabbedItem)
                {


                    playerController.grabbedItem.transform.SetParent(playerController.itemLastParent);

                    Rigidbody currentDraggingItemRg = playerController.grabbedItem.GetComponent<Rigidbody>();
                    if (currentDraggingItemRg)
                        currentDraggingItemRg.isKinematic = false;

                    playerController.grabbedItem = null;
                }

                if (damager && damager.GetComponent<PlayerController>())
                {


                    damager.GetComponent<PlayerController>().Kills++;

                    if (damager && damager.GetComponent<AIPlayer>())
                    {
                        damager.GetComponent<AIPlayer>().fsm.ChangeState(AIPlayer.States.Patrol);
                    }
                }
                // KILL CAM
                // activate my model for kill cam
                bool killedDoll = false;
                if (damager)
                {

                    killedDoll = (damager.GetComponent<AIDoll>()) ? true : false;

                    if (killedDoll)
                    {
                        if (playerController.DollDieCamera)
                            playerController.DollDieCamera.transform.SetParent(null);
                    }
                }


                if (GameManager.Instance.FadeWhenSight2D)
                    GameManager.Instance.FadeWhenSight2D.SetActive(false);
                if (playerController.pickupSystem.CurrentItem.weapon && playerController.pickupSystem.CurrentItem.weapon.Sight2D)
                {
                    playerController.pickupSystem.CurrentItem.weapon.Sight2D.DoFadeOutInmmediately();
                    playerController.pickupSystem.CurrentItem.weapon.Sight2D.gameObject.SetActive(false);


                }
                if (gameManager.DamageIndicator)
                    // fade out damage indicator
                    gameManager.DamageIndicator.DoFadeOut();

                playerController.rigidBody.isKinematic = true;

                if (playerController.DollDieCamera)
                {
                    playerController.DollDieCamera.gameObject.SetActive(true);
                    if (!killedDoll)
                        playerController.DollDieCamera.gameObject.SendMessage("DesactivateHolder");
                    else
                        playerController.DollDieCamera.gameObject.SendMessage("ActivateHolder");

                }

                yield return new WaitForSeconds(0.2f);
            }
            else
            {

                AIPlayer aiPlayer = GetComponent<AIPlayer>();
                if (aiPlayer.rigidBody)
                    aiPlayer.rigidBody.isKinematic = true;
                aiPlayer.fsm.ChangeState(AIPlayer.States.Idle, AxlPlay.StateTransition.Overwrite);
                yield return new WaitForSeconds(0.01f);

                aiPlayer.Model.enabled = false;

            }

            StartCoroutine(Respawn());
        }


        // respawn player 
        public IEnumerator Respawn()
        {
            if (gameManager.BloodSplash)

                GameManager.Instance.BloodSplash.DoFadeOutAtSpeed(0.09f);

            yield return new WaitForSeconds(RespawnDelay);

            isDead = false;
            CurrentHealth = startCurrentHealth;
            if (playerController)
            {


                if (playerController.pickupSystem.CurrentItem.weapon)
                {
                    playerController.pickupSystem.CurrentItem.weapon.ammunition = playerController.pickupSystem.CurrentItem.weapon.StartAmmunition;
                    playerController.pickupSystem.CurrentItem.weapon.cartridgeAmmo = playerController.pickupSystem.CurrentItem.weapon.CartridgeAmmo;

                    GameManager.Instance.UpdateAmmoUI();
                }


                if (GameManager.Instance.FadeWhenSight2D)
                    GameManager.Instance.FadeWhenSight2D.SetActive(true);
                if (playerController.pickupSystem.CurrentItem.weapon && playerController.pickupSystem.CurrentItem.weapon.Sight2D)
                {
                    playerController.pickupSystem.CurrentItem.weapon.Sight2D.DoFadeOutInmmediately();
                    playerController.pickupSystem.CurrentItem.weapon.Sight2D.gameObject.SetActive(true);



                }
                playerController.rigidBody.isKinematic = false;
                if (gameManager.BloodSplash)
                {
                    GameManager.Instance.BloodSplash.DoFadeOutInmmediately();

                    GameManager.Instance.BloodSplash.gameObject.SetActive(true);
                }

                if (playerController.DollDieCamera)
                    playerController.DollDieCamera.transform.SetParent(Camera.main.transform);

                if (playerController.DollDieCamera)
                    playerController.DollDieCamera.gameObject.SetActive(false);


                // delete items
                List<ItemInInventoryOld> deleteThings = new List<ItemInInventoryOld>();
                foreach (var item in playerController.pickupSystem.Items)
                {
                    if (!playerController.pickupSystem.StartItemsSpawned.Contains(item.script))
                    {
                        deleteThings.Add(item);
                    }

                }
                foreach (var itemToDelete in deleteThings)
                {
                    playerController.pickupSystem.DropItem(itemToDelete);

                }
                // use start item

                if (playerController.pickupSystem.StartItemsSpawned.Count > 0)
                    playerController.pickupSystem.UseItem(playerController.pickupSystem.StartItemsSpawned[0]);


                // set position to spawn point
                transform.position = GameManager.Instance.GetSpawnPoint(playerController.Team1).position;

            }
            // AI Player
            else
            {
                // set position to spawn point

                transform.position = AIManager.Instance.GetSpawnPoint(true).position;

                AIPlayer aiPlayer = GetComponent<AIPlayer>();



                aiPlayer.Model.enabled = true;
                if(aiPlayer.rigidBody)
                aiPlayer.rigidBody.isKinematic = false;

                aiPlayer.agent.stoppingDistance = aiPlayer.startArrivedDistance;
                aiPlayer.fsm.ChangeState(AIPlayer.States.Patrol, AxlPlay.StateTransition.Overwrite);
                aiPlayer.Model.SetBool("Die", false);



            }

        }

    }
}