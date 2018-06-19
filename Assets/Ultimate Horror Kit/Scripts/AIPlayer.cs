using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using AxlPlay;

namespace AxlPlay
{
    [RequireComponent(typeof(NavMeshAgent))]
   [RequireComponent(typeof(FieldOfView))]
    [RequireComponent(typeof(SetDestination))]
    [RequireComponent(typeof(Patrol))]

    public class AIPlayer : MonoBehaviour
    {


        public AIDamage AIDamager;
        public Animator Model;

        public float AttackInterval = 0.8f;

        public float MinDistanceToAttack = 1f;

        public enum States
        {
            Idle,
            Attack,
            ChaseEnemy,
            Patrol
        }
        public StateMachine<States> fsm;

        private SetDestination setDestination;
        private FieldOfView fieldOfView;
        private Patrol patrol;
        [HideInInspector]
        public NavMeshAgent agent;

        [HideInInspector]
        public float startArrivedDistance;

        private float timer;
        [HideInInspector]
        public Rigidbody rigidBody;

        RaycastHit hit;
        private bool previousHidden;
        private void Awake()
        {
            // get references
            // initialize state machine
            fsm = StateMachine<States>.Initialize(this);
            rigidBody = GetComponent<Rigidbody>();
            setDestination = GetComponent<SetDestination>();
            startArrivedDistance = setDestination.arrivedDistance;
            fieldOfView = GetComponent<FieldOfView>();
            patrol = GetComponent<Patrol>();
            agent = GetComponent<NavMeshAgent>();

        }
        void Start()
        {
            // start state patrolling
            fsm.ChangeState(States.Patrol);
        }
    
        void Idle_Enter()
        {
            // set the gameobject to a idle state, without move and attack.

            Model.SetFloat("Movement", 0f);
            Model.SetBool("Die",true);
            Model.ResetTrigger("Attack");

            agent.stoppingDistance = startArrivedDistance;
            setDestination.fsm.ChangeState(SetDestination.States.ArrivedEvent,StateTransition.Overwrite);
            agent.isStopped = true;
          //  setDestination.agent = null;
            agent.velocity = Vector3.zero;

            patrol.fsm.ChangeState(Patrol.States.Finish);
        }
        void Patrol_Enter()
        {
            // patrol trough waypointss
            Model.SetFloat("Movement", 0.5f);
            setDestination.fsm.ChangeState(SetDestination.States.ArrivedEvent,StateTransition.Overwrite);

            patrol.fsm.ChangeState(Patrol.States.Patrol);
            agent.isStopped = false;

        }
        void Patrol_Update()
        {
            // if see player, chase him
            if (CanSeeEnemy())
            {

                patrol.fsm.ChangeState(Patrol.States.Finish, StateTransition.Overwrite);
                setDestination.target = CanSeeEnemy();
                fsm.ChangeState(States.ChaseEnemy, StateTransition.Safe);
            }
        }

        // if someone hits me turn to him, chase and  attack
        public void GotHitBy(GameObject _damager)
        {
            if (fsm.State != States.Attack && fsm.State != States.Idle)
            {
                // chase enemy
                setDestination.target = _damager;

                fsm.ChangeState(States.ChaseEnemy);

            }
        }
        void Attack_Enter()
        {
            // attack enemy seen
            setDestination.target = CanSeeEnemy();
            agent.stoppingDistance = MinDistanceToAttack;

            setDestination.fsm.ChangeState(SetDestination.States.goDestination, StateTransition.Overwrite);

        }
        void Attack_Update()
        {
            // go to enemy and if is near enough attack
            timer += Time.deltaTime;

            if (CanSeeEnemy())
            {
                var targetRotation = Quaternion.LookRotation(CanSeeEnemy().transform.position - transform.position);

                // Smoothly rotate towards the target point.
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.deltaTime);


                if (setDestination.hasArrived && CanSeeEnemy())
                {

                    // player got far, get near
                    if (Vector3.Distance(transform.position, CanSeeEnemy().transform.position) > MinDistanceToAttack)
                    {

                        Model.ResetTrigger("Attack");

                        agent.stoppingDistance = MinDistanceToAttack;
                        Model.SetFloat("Movement", 1f);

                        setDestination.fsm.ChangeState(SetDestination.States.goDestination, StateTransition.Overwrite);
                        agent.isStopped = false;

                    }
                    else
                    {
                        setDestination.fsm.ChangeState(SetDestination.States.ArrivedEvent, StateTransition.Overwrite);
                        agent.velocity = Vector3.zero;
                        agent.isStopped = true;

                        Model.SetFloat("Movement", 0f);

                        // attack
                        if (timer > AttackInterval)
                        {
                            timer = 0f;

                            MeleeAttack();

                        }
                    }
                }



            }
            // if cant see enemy, search again
            else if(!previousHidden)
            {
                fsm.ChangeState(States.Patrol, StateTransition.Overwrite);

            }


        }
        // turn to enemy that hit me
        void ChaseEnemy_Enter()
        {
            agent.velocity = Vector3.zero;
            agent.stoppingDistance = startArrivedDistance;
            agent.isStopped = false;
            setDestination.fsm.ChangeState(SetDestination.States.goDestination);

        }
        void ChaseEnemy_Update()
        {
            var targetRotation = Quaternion.LookRotation(setDestination.target.transform.position - transform.position);

            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.deltaTime);
            if (CanSeeEnemy())
            {
                fsm.ChangeState(States.Attack);

            }
            if (setDestination.fsm.State == SetDestination.States.ArrivedEvent)
            {
                setDestination.fsm.ChangeState(SetDestination.States.goDestination);
                agent.isStopped = false;

            }

        }


        // check if is seeing a enemy to attack 
        GameObject CanSeeEnemy()
        {
            if (fieldOfView.visibleTargets.Count == 0)
                return null;


            PlayerController pC = fieldOfView.visibleTargets[0].GetComponent<PlayerController>();

            if (pC != null)
            {
                // if the player hided in a locker 
                if(pC.Hidden && !previousHidden)
                {
                    PlayerHidden();
                    
                }

                previousHidden = pC.Hidden;
                if (pC.Hidden)
                    return null;
                return pC.gameObject;
            }

            return null;

        }
        void MeleeAttack()
        {
            Model.SetTrigger("Attack");
        }
        void PlayerHidden()
        {
            Model.SetBool("Look",true);
            fsm.ChangeState(States.Idle, StateTransition.Overwrite);
            StartCoroutine(StopSearchHidden());
        }
        IEnumerator StopSearchHidden()
        {
            yield return new WaitForSeconds(5f);
            Model.SetBool("Look", false);
            fsm.ChangeState(States.Patrol, StateTransition.Overwrite);

        }

        // called by melee attack animation event 
        public void CantDamage()
        {
            AIDamager.canDamage = false;

        }
        public void CanDamage()
        {
            AIDamager.canDamage = true;

        }

        Transform GetClosestObject(Transform[] objects)
        {

            Transform closestObject = null;
            foreach (Transform obj in objects)
            {
                if (!closestObject)
                {
                    closestObject = obj;
                }
                //compares distances
                if (Vector3.Distance(transform.position, obj.transform.position) <= Vector3.Distance(transform.position, closestObject.transform.position))
                {
                    closestObject = obj;
                }
            }
            return closestObject;

        }
    }
}