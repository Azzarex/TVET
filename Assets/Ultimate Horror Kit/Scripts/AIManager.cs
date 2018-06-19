using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AxlPlay
{

    [System.Serializable]
    public class AISpawnSettings
    {
        public AIPlayer AIPrefab;
        public AIDoll AIDoll;

    }

    // Instantiate AIs
    public class AIManager : MonoBehaviour
    {

        //     public AIPlayer[] AIPrefabs;

        [HideInInspector]
        public bool CanStartSpawningDolls = false;

        public AISpawnSettings[] AIEnemiesToSpawn;

        public Transform[] PatrolPoints;
        public Transform[] SpawnPoints;


        private List<AIPlayer> EnemiesSpawned = new List<AIPlayer>();
        private List<AIDoll> DollsSpawned = new List<AIDoll>();


        private bool gameStarted;
        public static AIManager Instance;
        private Queue<AISpawnSettings> aiPrefabsQueue = new Queue<AISpawnSettings>();

        // for offline mode destroy ai
        public void Reset()
        {
            gameStarted = false;

            foreach (var enemy in EnemiesSpawned)
            {
                Destroy(enemy);
            }

            foreach (var doll in DollsSpawned)
            {
                Destroy(doll);
            }
            
            EnemiesSpawned = new List<AIPlayer>();

        }
        void Awake()
        {
            Instance = this;
            aiPrefabsQueue = new Queue<AISpawnSettings>(AIEnemiesToSpawn);
            CanStartSpawningDolls = true; // solo por ahora
        }


        void Update()
        {

            // flag

            if (!gameStarted)
            {
                gameStarted = true;
                // instantiate AI 
                for (int i = 0; i < AIEnemiesToSpawn.Length; i++)
                {
                    GameObject player = null;
                    GameObject instantiateObj = null;
                    AIDoll _aiDoll = null;
                    var _aiPrefab = aiPrefabsQueue.Peek().AIPrefab;
                    if (_aiPrefab == null)
                        _aiDoll = aiPrefabsQueue.Peek().AIDoll;

                    instantiateObj = (_aiPrefab == null) ? _aiDoll.gameObject : _aiPrefab.gameObject;

                    player = Instantiate(instantiateObj, SpawnPoints[Random.Range(0, SpawnPoints.Length)].transform.position, Quaternion.identity);
                    if (_aiPrefab != null)
                    {
                        AIPlayer playerScript = player.GetComponent<AIPlayer>();
                        EnemiesSpawned.Add(playerScript);

                    }
                    else
                    {
                        AIDoll dollScript = player.GetComponent<AIDoll>();
                        DollsSpawned.Add(dollScript);
                    }
                }
            }



        }
        public Transform GetSpawnPoint(bool _team1)
        {
            return SpawnPoints[UnityEngine.Random.Range(0, SpawnPoints.Length)].transform;
        }

    }
}