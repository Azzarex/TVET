using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AxlPlay;

namespace AxlPlay
{
	public class AnimateEnemy : MonoBehaviour
	{
		private Animator animator;




		void Awake() {
			animator = GetComponent<Animator> ();
		}

		void Start() {
			NotificationCenter.DefaultCenter.AddObserver (this, "Attack");
			NotificationCenter.DefaultCenter.AddObserver (this, "Patrol");
			NotificationCenter.DefaultCenter.AddObserver (this, "Pursue");
			NotificationCenter.DefaultCenter.AddObserver (this, "Wait");

		}


		public void Patrol() {
			animator.Play ("Movement");
			animator.SetFloat ("State", 0f);

		}
		public void Pursue() {
			animator.SetFloat ("State", 1f);

		}
		public void Attack() {
			animator.SetFloat ("State", 2f);

			animator.Play("Attack 1");
		}

		public void Wait() {
			animator.SetFloat ("State", 2f);

		}
	}
}