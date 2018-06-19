using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AxlPlay;

namespace AxlPlay
{
	public class AnimatePlayer : MonoBehaviour
	{

		public float ForwardSpeed = 15f;
		public float TurnSpeed = 15f;

		private Animator animator;

		private bool hasForwardParameter;
		private bool hasTurnParameter;
		private CharacterController playerController;

		void Awake ()
		{
			animator = GetComponent<Animator> ();
			playerController = transform.parent.GetComponent<CharacterController> ();
		
		}

		// Use this for initialization
		void Start ()
		{
			NotificationCenter.DefaultCenter.AddObserver (this, "IsDead");
			hasForwardParameter = HasParameter ("Forward", animator);
			hasTurnParameter = HasParameter ("Turn", animator);

		}
	
		// Update is called once per frame
		void Update ()
		{
			if (animator != null) {
				Vector3 velocity = Vector3.zero;

				if(playerController != null)
					velocity = new Vector3 (playerController.velocity.x, 0, playerController.velocity.z); 
				
				float vertical = Mathf.Abs (Input.GetAxis ("Vertical") * Time.deltaTime * velocity.magnitude * ForwardSpeed);
				float horizontal = Input.GetAxis ("Horizontal") * Time.deltaTime * velocity.magnitude * TurnSpeed;


				if(hasForwardParameter)
				animator.SetFloat ("Forward", vertical, 0.1f, Time.deltaTime);
				if(hasTurnParameter)
				animator.SetFloat ("Turn", horizontal, 0.1f, Time.deltaTime);
			}
		}

		bool HasParameter (string paramName, Animator animator)
		{
			foreach (AnimatorControllerParameter param in animator.parameters) {
				if (param.name == paramName)
					return true;
			}
			return false;
		}

	}

}