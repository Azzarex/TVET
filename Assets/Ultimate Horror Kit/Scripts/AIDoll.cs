using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AxlPlay
{

    [RequireComponent(typeof(NavMeshAgent))]

    public class AIDoll : MonoBehaviour
    {
        [SerializeField]
        public class ScareSound
        {
            public float AfterSecondsSpawned = 2.5f;
            public AudioClip Sound;
        }

        public enum DollBehaviours
        {
            Kiki,
            Chester
        }
        public Renderer modelRenderer;
        public DollBehaviours DollBehaviour = DollBehaviours.Kiki;
        public float DeadTimeToKill = 1.5f;

        public GameObject JumpScare3D;
        public AudioSource AudioSource;
        public AudioClip KillSound;

        public AudioClip DissapearSound;

        public AudioClip SpawnSound;
        public float MinRandomSpawn = 10f;
        public float MaxRandomSpawn = 20f;
        public float TimeSpawned = 4f;
        public int MaxSpawns = 0;
        public ScareSound ScareSoundWhenActive;
        public float SpawnDistanceOfPlayer = 5f;

        public float RespawnTime = 2f;

         [HideInInspector]
        public bool CanSpawn;

        // all of these private ones
        private float timerToSpawn;
        private float timerSpawned;
        private int timesSpawned;
        private float randomTimeToSpawn;
        private bool spawned;

        private bool scareSoundPlayed;
        private bool killed;
        private float timerToKill;
        private NavMeshAgent agent;
        private bool startedDestination;
        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            transform.localScale = Vector3.zero;

            CanSpawn = true;
        }
        void Start()
        {
            if (DollBehaviour == DollBehaviours.Chester)
            {
                NotificationCenter.DefaultCenter.AddObserver(this, "PlayerChangedAmbient");

            }
        }

        void Update()
        {
            // dont spawn if player is dead
            if (GameManager.Instance.PlayerHealth.isDead)
                return;

            // if can spawn (if I'm allowed from the game manager to spawn) and I'm not spawned because this gameobject is always activated only with mesh renderer invisible
            if (CanSpawn && !spawned)
            {
                // check in how much time will spawn
                if (randomTimeToSpawn == 0f)
                    randomTimeToSpawn = Random.Range(MinRandomSpawn, MaxRandomSpawn);

                timerToSpawn += Time.deltaTime;
                if (timerToSpawn >= randomTimeToSpawn)
                {
                    // reset some variables and do spawn
                    Spawn();

                    randomTimeToSpawn = 0f;
                    timerToSpawn = 0f;

                    timesSpawned++;
                    spawned = true;
                }

            }
            // If I'm spawned and I've not killed the plase 
            if (spawned && !killed)
            {
                // look always at player
                transform.LookAt(GameManager.Instance.Player.transform);
                // handle all differents doll behaviours
                if (DollBehaviour == DollBehaviours.Kiki)

                    CheckIfPlayerDies();
                else if (startedDestination)
                {

                    agent.SetDestination(GameManager.Instance.Player.transform.position);
                    agent.isStopped = false;
                    CheckIfPlayerDies();
                }
                // play some intermediate sounds
                if (AudioSource && ScareSoundWhenActive.Sound && !scareSoundPlayed)
                {
                    if (timerSpawned >= ScareSoundWhenActive.AfterSecondsSpawned)
                    {
                        AudioSource.PlayOneShot(ScareSoundWhenActive.Sound);
                        scareSoundPlayed = true;
                    }

                }
                // dissapear after certain time
                if (DollBehaviour == DollBehaviours.Kiki)
                {
                    timerSpawned += Time.deltaTime;

                    if (timerSpawned >= TimeSpawned)
                    {
                        Dissapear();
                    }
                }
            }
        }
        public void Dissapear()
        {
            // restart and make invisible gameobject to spawn later

            startedDestination = false;
            timerSpawned = 0f;
            spawned = false;
            transform.localScale = Vector3.zero;
            scareSoundPlayed = false;

            if (AudioSource && DissapearSound)
                AudioSource.PlayOneShot(DissapearSound);

            if (MaxSpawns > 0 && timesSpawned >= MaxSpawns)
                CanSpawn = false;

        }
        // for certain doll behaviour that dissapears when player runs away from the room 
        public void PlayerChangedAmbient()
        {
            Dissapear();
        }
        public void Spawn()
        {

            if (AudioSource && SpawnSound)
                AudioSource.PlayOneShot(SpawnSound);

            // the doll is made invisible putting scale to 0 so to spawn needss to be restarted to one
            transform.localScale = Vector3.one;
            Vector3 targetPos = GameManager.Instance.Player.transform.position;
            // spawn near player

            Vector3 randomPoint = targetPos + Random.insideUnitSphere * SpawnDistanceOfPlayer;
            NavMeshHit hit;
            // get a position within the nav mesh to spawn

            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
            }
            if (DollBehaviour == DollBehaviours.Chester)
            {
                StartCoroutine(WaitToRun());
            }

        }
        IEnumerator WaitToRun()
        {
            yield return new WaitForSeconds(1.5f);

            startedDestination = true;
        }
        public void CheckIfPlayerDies()
        {
            // check by doll kill mode

            if (DollBehaviour == DollBehaviours.Kiki)
            { 
                // if player dont look at the doll
                if (!modelRenderer.isVisible)
                {
                    timerToKill += Time.deltaTime;

                }
            }
            else if (DollBehaviour == DollBehaviours.Chester)
            {
                // if doll arrives to the player
                if (HasArrived())
                {
                    timerToKill = DeadTimeToKill;
                }
            }
            // if player dont look at the doll this time kill
           
            if (timerToKill >= DeadTimeToKill)
                PlayerDied();
        }
        public void PlayerDied()
        {
            // kill player

            killed = true;
            transform.localScale = Vector3.zero;
            timerToKill = 0f;

            // for chester
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            startedDestination = false;

            // kill sound
            if (AudioSource && KillSound)

                AudioSource.PlayOneShot(KillSound);

            GameManager.Instance.PlayerHealth.TakeDamage(1000f, Vector3.zero, gameObject);

            // show jumpscare 3d 
            if (JumpScare3D)
                JumpScare3D.gameObject.SetActive(true);

            // reset after time
            StartCoroutine(Respawn());

        }

        public IEnumerator Respawn()
        {
            yield return new WaitForSeconds(RespawnTime);
            if (JumpScare3D)
                JumpScare3D.gameObject.SetActive(false);


            killed = false;
            timerSpawned = 0f;
            spawned = false;
            scareSoundPlayed = false;

        }
        // check if doll arrives the nav messh destination
        private bool HasArrived()
        {
            return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.001f;
        }
        // when you want to start spawning doll call this and set it to true
        public void SetCanSpawnValue(bool _value)
        {
            CanSpawn = _value;
        }
    }
}