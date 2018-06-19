using UnityEngine;
using System.Collections;
using AxlPlay;
#if UNITY_5_5_OR_NEWER
using UnityEngine.AI;

#endif
namespace AxlPlay {
[RequireComponent(typeof(NavMeshAgent))]
[AddComponentMenu("Easy AI/Go To Destination")]
public class SetDestination : MonoBehaviour {

    public GameObject target;
	public float arrivedDistance = 0.1f;
	[Tooltip("The Agent speed.")]
	public float AgentSpeed = 3.5f;
	
	//[HideInInspector]
    public NavMeshAgent agent;
    private Vector3 localitation;

	private GameObject _gameObject;
	// used in Easy AI Base
	[HideInInspector]
	public bool hasArrived;
    // init FSM
    public enum States
    {
        goDestination,
        ArrivedEvent
    }
    public StateMachine<States> fsm;
    void Awake() {
	    agent = GetComponent<NavMeshAgent>();

	    //Initialize State Machine Engine		
	    fsm = StateMachine<States>.Initialize(this);
	    
    }
	void Start(){
		hasArrived = false;
		
	}

	void goDestination_Enter() {
			agent.speed = AgentSpeed;
			agent.stoppingDistance = arrivedDistance;
			agent.isStopped = false;
		 agent.SetDestination(target.transform.position);
	}
    void goDestination_Update() {

        if (HasArrived())
        {
            fsm.ChangeState(States.ArrivedEvent);
        }

    }
	void ArrivedEvent_Enter() {
            agent.isStopped = true;
	    hasArrived = true;
    }
	
    private bool HasArrived()
    {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.001f;
    }
}
}
